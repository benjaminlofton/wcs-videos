using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Mvc;
using WcsVideos.Models;
using WcsVideos.Models.Population;
using WcsVideos.Contracts;
using Microsoft.AspNet.Http;
using WcsVideos.Providers;
using WcsVideos.Providers.AutoPopulation;

namespace WcsVideos.Controllers
{
    public class SuggestedVideosController : Controller
    {
        private IDataAccess dataAccess;
        private IUserSessionHandler userSessionHandler;
        
        public SuggestedVideosController(IDataAccess dataAccess, IUserSessionHandler userSessionHandler)
        {
            this.dataAccess = dataAccess;
            this.userSessionHandler = userSessionHandler;
        }
        
        public IActionResult AddUrl()
        {
            bool loggedIn = this.userSessionHandler.GetUserLoginState(
                this.HttpContext.Request.Cookies,
                this.HttpContext.Response.Cookies);
                
            AddVideoUrlViewModel model = new AddVideoUrlViewModel();
            ViewModelHelper.PopulateUserInfo(model, loggedIn);
            
            // Populate the page based on Cookies.  This will populate the page in the case of an error during submit
            // which will redirect to this page with all of the necessary cookies populated.
            IReadableStringCollection requestCookies = this.HttpContext.Request.Cookies;            
            model.ValidationError = bool.Parse(requestCookies["ValidationError"].FirstOrDefault() ?? "False");
            model.ValidationErrorMessage = requestCookies["ValidationErrorMessage"].FirstOrDefault();
            model.Url = requestCookies["Url"].FirstOrDefault();
            
            CookieOptions cookieOptions = new CookieOptions();
            IResponseCookies responseCookies = this.HttpContext.Response.Cookies;
            responseCookies.Delete("ValidationError", cookieOptions);
            responseCookies.Delete("ValidationErrorMessage", cookieOptions);
            responseCookies.Delete("Url", cookieOptions);
            
            return this.View(model);
        }
        
        public IActionResult SubmitUrl(string url)
        {
            string validationErrorMessage = null;
            Uri parsedUrl;
            IVideoDetailsProvider provider;
            
            if (string.IsNullOrEmpty(url))
            {
                validationErrorMessage = "A URL must be provided";
            }
            else if (!Uri.TryCreate(url, UriKind.Absolute, out parsedUrl))
            {
                validationErrorMessage = "That doesn't appear to be a valid URL";
            }
            else if (VideoDetailsProviderFactory.TryGetProvider(parsedUrl, out provider))
            {
                VideoDetails videoDetails = provider.GetVideoDetails();
                
                if (videoDetails == null)
                {
                    validationErrorMessage = "There doesn't appear to be a video at that URL";
                }
                else if (this.dataAccess.ProviderVideoIdExists(
                    videoDetails.ProviderId.ToString(),
                    videoDetails.ProviderVideoId))
                {
                    validationErrorMessage = "The selected video already exists";
                }
                else
                {
                    string skillLevel = SkillLevelPopulator.GetSkillLevel(videoDetails);
                    string danceCategory = DanceCategoryPopulator.GetDanceCategory(videoDetails);
                    string dancerIdList = new DancerPopulator(this.dataAccess).GetDancers(videoDetails);
                    return this.RedirectToAction(
                        "Add",
                        new
                        {
                            providerId = videoDetails.ProviderId,
                            providerVideoId = videoDetails.ProviderVideoId,
                            title = videoDetails.Title,
                            skillLevel = skillLevel,
                            danceCategory = danceCategory,
                            dancerIdList = dancerIdList,
                        });
                }
            }
            else
            {
                validationErrorMessage = "Videos from the provided site are not supported";
            }
            
            CookieOptions cookieOptions = new CookieOptions();
            cookieOptions.Expires = DateTime.UtcNow.AddDays(1);
            
            // Populate cookies with form data so that we can repopulate the form after the redirect.
            // Traditionally this would be done using session variables, but we are using cookies here so
            // that we don't need to worry about session management.
            IResponseCookies responseCookies = this.HttpContext.Response.Cookies;
            responseCookies.Append("Url", url, cookieOptions);
            responseCookies.Append("ValidationError", true.ToString(), cookieOptions);
            responseCookies.Append("ValidationErrorMessage", validationErrorMessage, cookieOptions);
            
            return this.RedirectToAction("AddUrl");
        }
        
        public IActionResult Add(
            string title,
            int providerId,
            string providerVideoId,
            string skillLevel,
            string danceCategory,
            string dancerIdList)
        {
            bool loggedIn = this.userSessionHandler.GetUserLoginState(
                this.HttpContext.Request.Cookies,
                this.HttpContext.Response.Cookies);
            
            VideosAddViewModel model = new VideosAddViewModel();
            ViewModelHelper.PopulateUserInfo(model, loggedIn);
                        
            // Populate the page based on Cookies.  This will populate the page in the case of an error during submit
            // which will redirect to this page with all of the necessary cookies populated.
            IReadableStringCollection requestCookies = this.HttpContext.Request.Cookies;
            string rawProviderId = requestCookies["ProviderId"].FirstOrDefault();
            int parsedProviderId;
            if (string.IsNullOrEmpty(rawProviderId) || !int.TryParse(rawProviderId, out parsedProviderId))
            {
                parsedProviderId = 1;    
            }
            
            model.ProviderId = parsedProviderId;
            model.ProviderVideoIdValidationError = !bool.Parse(
                requestCookies["ProviderIdValid"].FirstOrDefault() ?? "True");
            model.ProviderVideoId = requestCookies["ProviderVideoId"].FirstOrDefault();
            model.ProviderVideoIdValidationError = !bool.Parse(
                requestCookies["ProviderVideoIdValid"].FirstOrDefault() ?? "True");
            model.Title = requestCookies["Title"].FirstOrDefault();
            model.TitleValidationError = !bool.Parse(requestCookies["TitleValid"].FirstOrDefault() ?? "True");
            model.DancerIdList = (requestCookies["DancerIdList"].FirstOrDefault() ?? string.Empty).Replace('+',';');
            model.DancerIdListValidationError = !bool.Parse(
                requestCookies["DancerIdListValid"].FirstOrDefault() ?? "True");
            model.EventId = requestCookies["EventId"].FirstOrDefault();
            model.EventIdValidationError = !bool.Parse(requestCookies["EventIdValid"].FirstOrDefault() ?? "True");
            model.SkillLevelId = SkillLevel.GetValidatedSkillLevel(requestCookies["SkillLevelId"].FirstOrDefault());
            model.DanceCategoryId = DanceCategory.GetValidatedDanceCategory(
                requestCookies["DanceCategoryId"].FirstOrDefault());
            
            CookieOptions cookieOptions = new CookieOptions();
            IResponseCookies responseCookies = this.HttpContext.Response.Cookies;
            responseCookies.Delete("ProviderId", cookieOptions);
            responseCookies.Delete("ProviderIdValid", cookieOptions);
            responseCookies.Delete("ProviderVideoId", cookieOptions);
            responseCookies.Delete("ProviderVideoIdValid", cookieOptions);
            responseCookies.Delete("Title", cookieOptions);
            responseCookies.Delete("TitleValid", cookieOptions);
            responseCookies.Delete("DancerIdList", cookieOptions);
            responseCookies.Delete("DancerIdListValid", cookieOptions);
            responseCookies.Delete("EventId", cookieOptions);
            responseCookies.Delete("EventIdValid", cookieOptions);
            responseCookies.Delete("SkillLevelId", cookieOptions);

            if (!string.IsNullOrEmpty(title))
            {
                model.Title = title;
            }
            
            if (!string.IsNullOrEmpty(providerVideoId))
            {
                model.ProviderVideoId = providerVideoId;
            }

            if (!string.IsNullOrEmpty(danceCategory))
            {
                model.DanceCategoryId = DanceCategory.GetValidatedDanceCategory(danceCategory);
            }

            if (!string.IsNullOrEmpty(skillLevel))
            {
                model.SkillLevelId = SkillLevel.GetValidatedSkillLevel(skillLevel);
            }

            if (!string.IsNullOrEmpty(dancerIdList))
            {
                model.DancerIdList = dancerIdList;
            }

            if (providerId != 0)
            {
                model.ProviderId = providerId;
            }

            Event contractEvent = null;
            if (!string.IsNullOrEmpty(model.EventId))
            {
                contractEvent = this.dataAccess.GetEvent(model.EventId);
            }
            
            if (string.IsNullOrEmpty(model.DancerIdList))
            {
                model.DancerNameList = "(None)";
            }
            else
            {
                string[] dancerIds = model.DancerIdList.Split(
                    new char[] { ';' },
                    20,
                    StringSplitOptions.RemoveEmptyEntries);
                List<string> dancerNameList = new List<string>();
                foreach (string dancerId in dancerIds)
                {
                    Dancer dancer = this.dataAccess.GetDancerById(dancerId);
                    if (dancer != null)
                    {
                        dancerNameList.Add(dancer.Name + " (" + dancerId + ")");
                    }
                }
                
                model.DancerNameList = string.Join("; ", dancerNameList);
            }
            
            if (contractEvent == null)
            {
                model.EventName = "(None)";
            }
            else
            {
                model.EventName = contractEvent.Name + " " + contractEvent.EventDate.Year;
            } 

            return this.View(model);
        }

        public IActionResult SubmitAdd(
            string providerId,
            string providerVideoId,
            string title,
            string dancerIdList,
            string eventId,
            string skillLevelId,
            string danceCategoryId)
        {
            bool providerIdValid = true;
            bool providerVideoIdValid = true;
            bool titleValid = true;
            bool dancerIdListValid = true;
            bool eventIdValid = true;
            int parsedProviderId = 0;
            if (string.IsNullOrEmpty(providerId) ||
                !int.TryParse(providerId, out parsedProviderId) ||
                (parsedProviderId != 1 && parsedProviderId != 2))
            {
                providerIdValid = false;
            }
            
            if (string.IsNullOrEmpty(providerVideoId))
            {
                providerVideoIdValid = false;
            }
            else if (this.dataAccess.ProviderVideoIdExists(providerId, providerVideoId))
            {
                providerVideoIdValid = false;
            }
            
            if (string.IsNullOrEmpty(title))
            {
                titleValid = false;
            }
            
            string[] dancerIds = null;
            if (string.IsNullOrEmpty(dancerIdList))
            {
                dancerIds = new string[0];
            }
            else
            {
                dancerIds = dancerIdList.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string dancerId in dancerIds)
                {
                    if (this.dataAccess.GetDancerById(dancerId) == null)
                    {
                        dancerIdListValid = false;
                    }
                }
            }
            
            if (string.IsNullOrEmpty(eventId))
            {
                eventId = null;
            }
            else
            {
                Event contractEvent = this.dataAccess.GetEvent(eventId);
                if (contractEvent == null)
                {
                    eventIdValid = false;
                }
            }
            
            if (providerIdValid && providerVideoIdValid && titleValid && dancerIdListValid && eventIdValid)
            {
                Video video = new Video();
                video.ProviderId = parsedProviderId;
                video.ProviderVideoId = providerVideoId;
                video.Title = title;
                video.DancerIdList = dancerIds;
                video.EventId = eventId;
                video.SkillLevel = SkillLevel.GetValidatedSkillLevel(skillLevelId);
                video.DanceCategory = DanceCategory.GetValidatedDanceCategory(danceCategoryId);
                string videoId = this.dataAccess.AddSuggestedVideo(video);
                
                return this.RedirectToAction("AddSuccess", new { id = videoId });
            }
            else
            {
                CookieOptions cookieOptions = new CookieOptions();
                cookieOptions.Expires = DateTime.UtcNow.AddDays(1);
                
                // Populate cookies with form data so that we can repopulate the form after the redirect.
                // Traditionally this would be done using session variables, but we are using cookies here so
                // that we don't need to worry about session management.
                IResponseCookies responseCookies = this.HttpContext.Response.Cookies;
                responseCookies.Append("ProviderId", providerId ?? string.Empty, cookieOptions);
                responseCookies.Append("ProviderIdValid", providerIdValid.ToString(), cookieOptions);
                responseCookies.Append("ProviderVideoId", providerVideoId ?? string.Empty, cookieOptions);
                responseCookies.Append("ProviderVideoIdValid", providerVideoIdValid.ToString(), cookieOptions);
                responseCookies.Append("Title", title ?? string.Empty, cookieOptions);
                responseCookies.Append("TitleValid", titleValid.ToString(), cookieOptions);
                responseCookies.Append("DancerIdList", (dancerIdList ?? string.Empty).Replace(';', '+'), cookieOptions);
                responseCookies.Append("DancerIdListValid", dancerIdListValid.ToString(), cookieOptions);
                responseCookies.Append("EventId", eventId ?? string.Empty, cookieOptions);
                responseCookies.Append("EventIdValid", eventIdValid.ToString(), cookieOptions);
                responseCookies.Append("SkillLevelId", skillLevelId ?? string.Empty, cookieOptions);
                responseCookies.Append("DanceCategoryId", danceCategoryId ?? string.Empty, cookieOptions);
                
                return this.RedirectToAction("Add");
            }
        }
        
        public IActionResult AddSuccess(string id)
        {
            bool loggedIn = this.userSessionHandler.GetUserLoginState(
                this.HttpContext.Request.Cookies,
                this.HttpContext.Response.Cookies);
            
            VideosAddSuccessViewModel model = new VideosAddSuccessViewModel();
            ViewModelHelper.PopulateUserInfo(model, loggedIn);
            
            Video video = this.dataAccess.GetSuggestedVideo(id);
            string eventName = null;
            if (!string.IsNullOrEmpty(video.EventId))
            {
                Event contractEvent = this.dataAccess.GetEvent(video.EventId);
                if (contractEvent != null)
                {
                    eventName = contractEvent.Name + " " + contractEvent.EventDate.Year;
                }
            }
            
            if (string.IsNullOrEmpty(eventName))
            {
                eventName = "(None)";
            }
            
            model.ProviderId = video.ProviderId;
            model.ProviderVideoId = video.ProviderVideoId;
            model.Title = video.Title;
            List<string> dancerNameList = new List<string>();
            foreach (string dancerId in video.DancerIdList)
            {
                Dancer dancer = this.dataAccess.GetDancerById(dancerId);
                if (dancer != null)
                {
                    dancerNameList.Add(dancer.Name + " (" + dancerId + ")");
                }
            }
            
            model.DancerNameList = string.Join("; ", dancerNameList);
            model.EventName = eventName;
            model.AddVideoUrl = this.Url.Link(
                "default",
                new { controller = "SuggestedVideos", action = "Add" });

            model.SkillLevel = SkillLevel.GetSkillLevelDisplayName(video.SkillLevel);
            model.DanceCategory = DanceCategory.GetDanceCategoryDisplayName(video.DanceCategory);

            return this.View(model);
        }

        public IActionResult Review(string id)
        {
            bool loggedIn = this.userSessionHandler.GetUserLoginState(
                this.HttpContext.Request.Cookies,
                this.HttpContext.Response.Cookies);

            if (!loggedIn)
            {
                CookieOptions loginCookieOptions = new CookieOptions();
                loginCookieOptions.Expires = DateTime.UtcNow.AddDays(1);
                this.HttpContext.Response.Cookies.Append(
                    "LoginRedirect",
                    this.Request.Path.ToUriComponent() + this.Request.QueryString,
                    loginCookieOptions);
                return this.RedirectToRoute("default", new { controller = "User", action = "Login" });
            }
            
            ReviewSuggestedVideoViewModel model = new ReviewSuggestedVideoViewModel();

            Video video = this.dataAccess.GetSuggestedVideo(id);
            
            if (video == null)
            {
                return this.HttpNotFound();    
            }
            
            ViewModelHelper.PopulateUserInfo(model, loggedIn);
            model.Existing = this.PopulateWatchViewModel(video);
            model.VideoId = id;
            
            // Populate the page based on Cookies.  This will populate the page in the case of an error during submit
            // which will redirect to this page with all of the necessary cookies populated.
            IReadableStringCollection requestCookies = this.HttpContext.Request.Cookies;
            model.Title = requestCookies["Title"].FirstOrDefault();            
            model.TitleValidationError = !bool.Parse(requestCookies["TitleValid"].FirstOrDefault() ?? "True");
            model.DancerIdList = (requestCookies["DancerIdList"].FirstOrDefault() ?? string.Empty).Replace('+',';');
            model.DancerIdListValidationError = !bool.Parse(requestCookies["DancerIdListValid"].FirstOrDefault() ?? "True");
            model.EventId = requestCookies["EventId"].FirstOrDefault();
            model.EventIdValidationError = !bool.Parse(requestCookies["EventIdValid"].FirstOrDefault() ?? "True");
            model.SkillLevelId = SkillLevel.GetValidatedSkillLevel(requestCookies["SkillLevelId"].FirstOrDefault());
            model.DanceCategoryId = DanceCategory.GetValidatedDanceCategory(
                requestCookies["DanceCategoryId"].FirstOrDefault());
            
            CookieOptions cookieOptions = new CookieOptions();
            IResponseCookies responseCookies = this.HttpContext.Response.Cookies;
            responseCookies.Delete("Title", cookieOptions);
            responseCookies.Delete("TitleValid", cookieOptions);
            responseCookies.Delete("DancerIdList", cookieOptions);
            responseCookies.Delete("DancerIdListValid", cookieOptions);
            responseCookies.Delete("EventId", cookieOptions);
            responseCookies.Delete("EventIdValid", cookieOptions);
            responseCookies.Delete("SkillLevelId", cookieOptions);
            responseCookies.Delete("DanceCategoryId", cookieOptions);

            if (!model.TitleValidationError &&
                !model.DancerIdListValidationError &&
                !model.EventIdValidationError)
            {
                // if there were no errors, this must be the initial load, so pull the data from the video
                model.Title = video.Title;
                model.DancerIdList = string.Join(";", video.DancerIdList ?? new string[] {});
                model.EventId = video.EventId;
                model.SkillLevelId = SkillLevel.GetValidatedSkillLevel(video.SkillLevel);
                model.DanceCategoryId = DanceCategory.GetValidatedDanceCategory(video.DanceCategory);
                
                IVideoDetailsProvider provider;
                if (VideoDetailsProviderFactory.TryGetProvider(video, out provider))
                {
                    VideoDetails videoDetails = provider.GetVideoDetails();
                    if (string.IsNullOrEmpty(model.SkillLevelId))
                    {
                        model.SkillLevelId = SkillLevel.GetValidatedSkillLevel(
                            SkillLevelPopulator.GetSkillLevel(videoDetails));
                    }
                    
                    if (string.IsNullOrEmpty(model.DanceCategoryId))
                    {
                        model.DanceCategoryId = DanceCategory.GetValidatedDanceCategory(
                            DanceCategoryPopulator.GetDanceCategory(videoDetails));
                    }
                    
                    if (model.DancerIdList.Length == 0)
                    {
                        model.DancerIdList = new DancerPopulator(this.dataAccess).GetDancers(videoDetails);    
                    }
                }
            }
            
            string[] dancerIds = (model.DancerIdList ?? string.Empty).Split(
                new char[] { ';' },
                20,
                StringSplitOptions.RemoveEmptyEntries);

            model.DancerNameList = ViewModelHelper.GetDancerNames(this.dataAccess, dancerIds);
            model.EventName = ViewModelHelper.GetEventName(this.dataAccess, model.EventId);
            model.PostbackUrl = this.Url.Action("Accept");
            model.DeleteSuggestedVideoUrl = this.Url.Action("Delete");
            model.SuggestedVideoId = id;
            model.ProviderId = video.ProviderId;
            model.ProviderVideoId = video.ProviderVideoId;

            return this.View(model);
        }
        
        public IActionResult Delete(string suggestedVideoId)
        {
            bool loggedIn = this.userSessionHandler.GetUserLoginState(
                this.HttpContext.Request.Cookies,
                this.HttpContext.Response.Cookies);
            
            if (!loggedIn)
            {
                return this.Error();
            }
            
            this.dataAccess.DeleteSuggestedVideo(suggestedVideoId);
            
            return this.RedirectToRoute("default", new { Controller = "Admin", action = "SuggestedVideoList" });
        }
        
        public IActionResult Accept(
            string suggestedVideoId,
            string providerId,
            string providerVideoId,
            string title,
            string dancerIdList,
            string eventId,
            string skillLevelId,
            string danceCategoryId)
        {
            bool loggedIn = this.userSessionHandler.GetUserLoginState(
                this.HttpContext.Request.Cookies,
                this.HttpContext.Response.Cookies);
            
            if (!loggedIn)
            {
                return this.Error();
            }
            
            bool providerIdValid = true;
            bool providerVideoIdValid = true;
            bool titleValid = true;
            bool dancerIdListValid = true;
            bool eventIdValid = true;
            int parsedProviderId = 0;
            if (string.IsNullOrEmpty(providerId) ||
                !int.TryParse(providerId, out parsedProviderId) ||
                (parsedProviderId != 1 && parsedProviderId != 2))
            {
                providerIdValid = false;
            }
            
            if (string.IsNullOrEmpty(providerVideoId))
            {
                providerVideoIdValid = false;
            }
            else if (this.dataAccess.ProviderVideoIdExists(providerId, providerVideoId))
            {
                providerVideoIdValid = false;
            }
            
            if (string.IsNullOrEmpty(title))
            {
                titleValid = false;
            }
            
            string[] dancerIds = null;
            if (string.IsNullOrEmpty(dancerIdList))
            {
                dancerIds = new string[0];
            }
            else
            {
                dancerIds = dancerIdList.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string dancerId in dancerIds)
                {
                    if (this.dataAccess.GetDancerById(dancerId) == null)
                    {
                        dancerIdListValid = false;
                    }
                }
            }
            
            if (string.IsNullOrEmpty(eventId))
            {
                eventId = null;
            }
            else
            {
                Event contractEvent = this.dataAccess.GetEvent(eventId);
                if (contractEvent == null)
                {
                    eventIdValid = false;
                }
            }
            
            if (providerIdValid && providerVideoIdValid && titleValid && dancerIdListValid && eventIdValid)
            {
                Video video = new Video();
                video.ProviderId = parsedProviderId;
                video.ProviderVideoId = providerVideoId;
                video.Title = title;
                video.DancerIdList = dancerIds;
                video.EventId = eventId;
                video.SkillLevel = SkillLevel.GetValidatedSkillLevel(skillLevelId);
                video.DanceCategory = DanceCategory.GetValidatedDanceCategory(danceCategoryId);
                this.dataAccess.AddVideo(video);
                this.dataAccess.DeleteSuggestedVideo(suggestedVideoId);
                
                return this.RedirectToRoute("default", new { Controller = "Admin", Action = "SuggestedVideoList" });
            }
            else
            {
                CookieOptions cookieOptions = new CookieOptions();
                cookieOptions.Expires = DateTime.UtcNow.AddDays(1);
                
                // Populate cookies with form data so that we can repopulate the form after the redirect.
                // Traditionally this would be done using session variables, but we are using cookies here so
                // that we don't need to worry about session management.
                IResponseCookies responseCookies = this.HttpContext.Response.Cookies;
                responseCookies.Append("ProviderId", providerId ?? string.Empty, cookieOptions);
                responseCookies.Append("ProviderIdValid", providerIdValid.ToString(), cookieOptions);
                responseCookies.Append("ProviderVideoId", providerVideoId ?? string.Empty, cookieOptions);
                responseCookies.Append("ProviderVideoIdValid", providerVideoIdValid.ToString(), cookieOptions);
                responseCookies.Append("Title", title ?? string.Empty, cookieOptions);
                responseCookies.Append("TitleValid", titleValid.ToString(), cookieOptions);
                responseCookies.Append("DancerIdList", (dancerIdList ?? string.Empty).Replace(';','+'), cookieOptions);
                responseCookies.Append("DancerIdListValid", dancerIdListValid.ToString(), cookieOptions);
                responseCookies.Append("EventId", eventId ?? string.Empty, cookieOptions);
                responseCookies.Append("EventIdValid", eventIdValid.ToString(), cookieOptions);
                responseCookies.Append("SkillLevelId", SkillLevel.GetValidatedSkillLevel(skillLevelId) ?? string.Empty, cookieOptions);
                responseCookies.Append(
                    "DanceCategoryId",
                    DanceCategory.GetValidatedDanceCategory(danceCategoryId) ?? string.Empty,
                    cookieOptions);
                
                return this.RedirectToAction("Review", new { id = suggestedVideoId });
            }
        }
        
        // TODO: Copied from VideosController, please refactor me
        private WatchViewModel PopulateWatchViewModel(Video video)
        {
            WatchViewModel model = new WatchViewModel();
            ViewModelHelper.PopulateUserInfo(
                model,
                this.userSessionHandler.GetUserLoginState(this.HttpContext.Request.Cookies, this.HttpContext.Response.Cookies));
            
            IVideoViewModelPopulator populator = VideoViewModelPopulatorFactory.GetPopulator(video);
            populator.Populate(model);
            
            model.Dancers = new List<DancerLinkViewModel>();
            
            if (video.DancerIdList != null)
            {
                foreach (string dancerId in video.DancerIdList)
                {
                    Dancer dancer = this.dataAccess.GetDancerById(dancerId);
                    if (dancer != null)
                    {
                        DancerLinkViewModel dancerModel = new DancerLinkViewModel();
                        dancerModel.DisplayName = dancer.Name;
                        dancerModel.Url = this.Url.Link(
                            "default",
                            new { controller = "Home", action = "Dancer", id = dancer.WsdcId });
                        model.Dancers.Add(dancerModel);
                    }    
                }
            }
            
            Event contractEvent = null;
            if (!string.IsNullOrEmpty(video.EventId))
            {
                contractEvent = this.dataAccess.GetEvent(video.EventId);
            }
            
            if (contractEvent == null)
            {
                model.EventName = "(None)";
            }
            else
            {
                model.EventName = contractEvent.Name + " " + contractEvent.EventDate.Year;
                model.EventUrl = this.Url.Link(
                    "default",
                    new { controller = "Events", action = "Event", id = contractEvent.EventId });
            } 
            
            model.SkillLevel = SkillLevel.GetSkillLevelDisplayName(video.SkillLevel);
            model.DanceCategory = DanceCategory.GetDanceCategoryDisplayName(video.DanceCategory);
            
            return model;
        }
        
        public IActionResult Error()
        {
            return View("~/Views/Shared/Error.cshtml", new BasePageViewModel());
        }
    }
}