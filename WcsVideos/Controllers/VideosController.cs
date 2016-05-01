using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Mvc;
using WcsVideos.Models;
using WcsVideos.Models.Population;
using WcsVideos.Contracts;
using WcsVideos.Providers;
using WcsVideos.Providers.AutoPopulation;
using Microsoft.AspNet.Http;

namespace WcsVideos.Controllers
{
    public class VideosController : Controller
    {
        private const int ResultsPerPage = 5;
        
        private IDataAccess dataAccess;
        private IUserSessionHandler userSessionHandler;
        
        public VideosController(IDataAccess dataAccess, IUserSessionHandler userSessionHandler)
        {
            this.dataAccess = dataAccess;
            this.userSessionHandler = userSessionHandler;
        }
        
        public IActionResult Watch(string id)
        {
            bool loggedIn = this.userSessionHandler.GetUserLoginState(
                this.HttpContext.Request.Cookies,
                this.HttpContext.Response.Cookies);
                
            Video video = this.dataAccess.GetVideoById(id);
            
            if (video == null)
            {
                return this.HttpNotFound();    
            }
            
            WatchViewModel model = this.PopulateWatchViewModel(video);
            ViewModelHelper.PopulateUserInfo(model, loggedIn);
            
            if (loggedIn)
            {
                model.EditUrl = this.Url.Link(
                    "default",
                    new { controller = "Videos", action = "Edit", id = id });
            }
            else
            {
                model.EditUrl = this.Url.Link(
                    "default",
                    new { controller = "Videos", action = "Flag", id = id });
            }
            
            return this.View(model);
        }
        
        private WatchViewModel PopulateWatchViewModel(Video video)
        {
            WatchViewModel model = new WatchViewModel();
            ViewModelHelper.PopulateUserInfo(
                model,
                this.userSessionHandler.GetUserLoginState(
                    this.HttpContext.Request.Cookies,
                    this.HttpContext.Response.Cookies));
            
            IVideoViewModelPopulator populator = VideoViewModelPopulatorFactory.GetPopulator(video);
            populator.Populate(model);
            
            model.Dancers = new List<DancerLinkViewModel>();
            
            if (video.DancerIdList == null || video.DancerIdList.Length == 0)
            {
                model.ShowAddDancerLink = true;
            }
            else
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
            if (string.IsNullOrEmpty(video.EventId))
            {
                model.ShowAddEventLink = true;
            }
            else
            {
                contractEvent = this.dataAccess.GetEvent(video.EventId);
            }
            
            if (contractEvent == null)
            {
                model.EventName = "(None)";
                model.ShowAddEventLink = true;
            }
            else
            {
                model.EventName = contractEvent.Name + " " + contractEvent.EventDate.Year;
                model.EventUrl = this.Url.Link(
                    "default",
                    new { controller = "Events", action = "Event", id = contractEvent.EventId });
            } 
            
            model.SkillLevel = SkillLevel.GetSkillLevelDisplayName(video.SkillLevel);
            if (string.IsNullOrEmpty(video.SkillLevel))
            {
                model.ShowAddSkillLevelLink = true;
            }
            
            model.DanceCategory = DanceCategory.GetDanceCategoryDisplayName(video.DanceCategory);
            if (string.IsNullOrEmpty(video.DanceCategory))
            {
                model.ShowAddDanceCategoryLink = true;
            }
            
            return model;
        }
        
        public IActionResult AddUrl()
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
            model.DancerIdList = (requestCookies["DancerIdList"].FirstOrDefault() ?? string.Empty).Replace('+', ';');
            model.DancerIdListValidationError = !bool.Parse(
                requestCookies["DancerIdListValid"].FirstOrDefault() ?? "True");
            model.EventId = requestCookies["EventId"].FirstOrDefault();
            model.EventIdValidationError = !bool.Parse(requestCookies["EventIdValid"].FirstOrDefault() ?? "True");
            model.SkillLevelId = SkillLevel.GetValidatedSkillLevel(requestCookies["SkillLevelId"].FirstOrDefault());
            model.DanceCategoryId = DanceCategory.GetValidatedDanceCategory(
                requestCookies["danceCategoryId"].FirstOrDefault());
            
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
            responseCookies.Delete("DanceCategoryId", cookieOptions);

            if (!string.IsNullOrEmpty(title))
            {
                model.Title = title;
            }
            
            if (providerId != 0)
            {
                model.ProviderId = providerId;
            }
            
            if (!string.IsNullOrEmpty(providerVideoId))
            {
                model.ProviderVideoId = providerVideoId;
            }

            if (!string.IsNullOrEmpty(skillLevel))
            {
                model.SkillLevelId = SkillLevel.GetValidatedSkillLevel(skillLevel);
            }
            
            if (!string.IsNullOrEmpty(danceCategory))
            {
                model.DanceCategoryId = DanceCategory.GetValidatedDanceCategory(danceCategory);
            }

            if (!string.IsNullOrEmpty(dancerIdList))
            {
                model.DancerIdList = dancerIdList;
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
                
                string videoId = this.dataAccess.AddVideo(video);
                
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
                responseCookies.Append("SkillLevelId", SkillLevel.GetValidatedSkillLevel(skillLevelId) ?? string.Empty, cookieOptions);
                responseCookies.Append(
                    "DanceCategoryId",
                    DanceCategory.GetValidatedDanceCategory(danceCategoryId) ?? string.Empty,
                    cookieOptions);

                return this.RedirectToAction("Add");
            }
        }
        
        public IActionResult AddSuccess(string id)
        {
            VideosAddSuccessViewModel model = new VideosAddSuccessViewModel();
            
            Video video = this.dataAccess.GetVideoById(id);
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
            model.VideoUrl = this.Url.Link(
                "default",
                new { controller = "Videos", action = "Watch", id = id });
            model.AddVideoUrl = this.Url.Link(
                "default",
                new { controller = "Videos", action = "Add" });

            model.SkillLevel = SkillLevel.GetSkillLevelDisplayName(video.SkillLevel);
            model.DanceCategory = DanceCategory.GetDanceCategoryDisplayName(video.DanceCategory);

            return this.View(model);
        }
        
        public IActionResult Edit(string id)
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
            
            VideoModifyViewModel model = new VideoModifyViewModel();

            Video video = this.dataAccess.GetVideoById(id);
            
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
            model.DancerIdList = (requestCookies["DancerIdList"].FirstOrDefault() ?? string.Empty).Replace('+', ';');
            model.DancerIdListValidationError = !bool.Parse(
                requestCookies["DancerIdListValid"].FirstOrDefault() ?? "True");
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
                
                VideoDetails details = new VideoDetails { Title = model.Title };
                if (string.IsNullOrEmpty(model.SkillLevelId))
                {
                    
                    model.SkillLevelId = SkillLevelPopulator.GetSkillLevel(details);
                }
                
                if (string.IsNullOrEmpty(model.DanceCategoryId))
                {
                    model.DanceCategoryId = DanceCategoryPopulator.GetDanceCategory(details);
                }
                
                if (string.IsNullOrEmpty(model.DancerIdList))
                {
                    DancerPopulator dancerPopulator = new DancerPopulator(this.dataAccess);
                    model.DancerIdList = dancerPopulator.GetDancers(details);
                }
            }

            string[] dancerIds = (model.DancerIdList ?? string.Empty).Split(
                new char[] { ';' },
                20,
                StringSplitOptions.RemoveEmptyEntries);

            model.DancerNameList = ViewModelHelper.GetDancerNames(this.dataAccess, dancerIds);
            model.EventName = ViewModelHelper.GetEventName(this.dataAccess, model.EventId);
            model.PostbackUrl = this.Url.Link("default", new { controller = "Videos", action = "SubmitEdit" });

            return this.View(model);
        }
        
        public IActionResult SubmitEdit(
            string videoId,
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
            
            bool titleValid = true;
            bool dancerIdListValid = true;
            bool eventIdValid = true;
            
            if (string.IsNullOrEmpty(videoId))
            {
                return this.HttpNotFound();
            }

            Video existingVideo = this.dataAccess.GetVideoById(videoId);
            
            if (existingVideo == null)
            {
                return this.HttpNotFound();    
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
            
            if (titleValid && dancerIdListValid && eventIdValid)
            {
                Video video = existingVideo.Clone();
                video.Title = title;
                video.DancerIdList = dancerIds;
                video.EventId = eventId;
                video.SkillLevel = SkillLevel.GetValidatedSkillLevel(skillLevelId);
                video.DanceCategory = DanceCategory.GetValidatedDanceCategory(danceCategoryId);
                this.dataAccess.UpdateVideo(video);
                
                return this.RedirectToAction("Watch", new { id = video.Id });
            }
            else
            {
                CookieOptions cookieOptions = new CookieOptions();
                cookieOptions.Expires = DateTime.UtcNow.AddDays(1);
                
                // Populate cookies with form data so that we can repopulate the form after the redirect.
                // Traditionally this would be done using session variables, but we are using cookies here so
                // that we don't need to worry about session management.
                IResponseCookies responseCookies = this.HttpContext.Response.Cookies;
                responseCookies.Append("Title", title ?? string.Empty, cookieOptions);
                responseCookies.Append("TitleValid", titleValid.ToString(), cookieOptions);
                responseCookies.Append("DancerIdList", (dancerIdList ?? string.Empty).Replace(';', '+'), cookieOptions);
                responseCookies.Append("DancerIdListValid", dancerIdListValid.ToString(), cookieOptions);
                responseCookies.Append("EventId", eventId ?? string.Empty, cookieOptions);
                responseCookies.Append("EventIdValid", eventIdValid.ToString(), cookieOptions);
                responseCookies.Append("SkillLevelId", SkillLevel.GetValidatedSkillLevel(skillLevelId) ?? string.Empty);
                responseCookies.Append("DanceCategoryId", DanceCategory.GetValidatedDanceCategory(danceCategoryId) ?? string.Empty);
                
                return this.RedirectToAction("Edit", new { id = videoId });
            }
        }
        
        public IActionResult Flag(string id)
        {
            bool loggedIn = this.userSessionHandler.GetUserLoginState(
                this.HttpContext.Request.Cookies,
                this.HttpContext.Response.Cookies);
            
            VideoModifyViewModel model = new VideoModifyViewModel();

            Video video = this.dataAccess.GetVideoById(id);
            
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
            model.DancerIdList = (requestCookies["DancerIdList"].FirstOrDefault() ?? string.Empty).Replace('+', ';');
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
            }
            
            string[] dancerIds = (model.DancerIdList ?? string.Empty).Split(
                new char[] { ';' },
                20,
                StringSplitOptions.RemoveEmptyEntries);

            model.DancerNameList = ViewModelHelper.GetDancerNames(this.dataAccess, dancerIds);
            model.EventName = ViewModelHelper.GetEventName(this.dataAccess, model.EventId);
            model.PostbackUrl = this.Url.Link("default", new { controller = "Videos", action = "SubmitFlag" });

            return this.View(model);
        }
        
        public IActionResult SubmitFlag(
            string videoId,
            string title,
            string dancerIdList,
            string eventId,
            string skillLevelId,
            string danceCategoryId)
        {           
            bool titleValid = true;
            bool dancerIdListValid = true;
            bool eventIdValid = true;
            
            if (string.IsNullOrEmpty(videoId))
            {
                return this.HttpNotFound();
            }

            Video existingVideo = this.dataAccess.GetVideoById(videoId);
            
            if (existingVideo == null)
            {
                return this.HttpNotFound();    
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
            
            if (titleValid && dancerIdListValid && eventIdValid)
            {
                FlaggedVideo flaggedVideo = FlaggedVideo.FromVideo(existingVideo);
                flaggedVideo.Title = title;
                flaggedVideo.DancerIdList = dancerIds;
                flaggedVideo.EventId = eventId;
                flaggedVideo.SkillLevel = SkillLevel.GetValidatedSkillLevel(skillLevelId);
                flaggedVideo.DanceCategory = DanceCategory.GetValidatedDanceCategory(danceCategoryId);
                this.dataAccess.AddFlaggedVideo(flaggedVideo);
                
                return this.RedirectToAction("FlagSuccess", new { id = existingVideo.Id });
            }
            else
            {
                CookieOptions cookieOptions = new CookieOptions();
                cookieOptions.Expires = DateTime.UtcNow.AddDays(1);
                
                // Populate cookies with form data so that we can repopulate the form after the redirect.
                // Traditionally this would be done using session variables, but we are using cookies here so
                // that we don't need to worry about session management.
                IResponseCookies responseCookies = this.HttpContext.Response.Cookies;
                responseCookies.Append("Title", title ?? string.Empty, cookieOptions);
                responseCookies.Append("TitleValid", titleValid.ToString(), cookieOptions);
                responseCookies.Append("DancerIdList", (dancerIdList ?? string.Empty).Replace(';', '+'), cookieOptions);
                responseCookies.Append("DancerIdListValid", dancerIdListValid.ToString(), cookieOptions);
                responseCookies.Append("EventId", eventId ?? string.Empty, cookieOptions);
                responseCookies.Append("EventIdValid", eventIdValid.ToString(), cookieOptions);
                responseCookies.Append("SkillLevelId", SkillLevel.GetValidatedSkillLevel(skillLevelId) ?? string.Empty, cookieOptions);
                responseCookies.Append(
                    "DanceCategoryId",
                    DanceCategory.GetValidatedDanceCategory(danceCategoryId) ?? string.Empty,
                    cookieOptions);
                
                return this.RedirectToAction("Flag", new { id = videoId });
            }
        }
        
        public IActionResult DancerSearchResults(string query, int start)
        {
            DancerSearchResultsViewModel model = new DancerSearchResultsViewModel();
            model.Query = query;
            
            if (!string.IsNullOrEmpty(query))
            {
                List<Dancer> fullResults = this.dataAccess.SearchForDancer(query);
                ViewModelHelper.PopulateSearchResults(
                    model,
                    fullResults,
                    start,
                    VideosController.ResultsPerPage, 
                    (dancer) =>
                        {
                            DancerListItemViewModel listItem = new DancerListItemViewModel();
                            listItem.Name = dancer.Name;
                            listItem.VideoCount = dancer.VideoIdList == null ? 0 : dancer.VideoIdList.Length;
                            listItem.WsdcId = dancer.WsdcId;
                            listItem.Url = "javascript:void(0)";
                            return listItem;
                        },
                    (s) => "javascript:javascript:void(0)");
            }
            
            return View(model);
        }
        
        public IActionResult FlagSuccess(string id)
        {
            bool loggedIn = this.userSessionHandler.GetUserLoginState(
                this.HttpContext.Request.Cookies,
                this.HttpContext.Response.Cookies);
            
            VideoFlagSuccessViewModel model = new VideoFlagSuccessViewModel();            
            ViewModelHelper.PopulateUserInfo(model, loggedIn);
            model.VideoUrl = this.Url.Link(
                "default",
                new { controller = "Videos", action = "Watch", id = id });
                
            return View(model);
        }
        
        public IActionResult ReviewFlag(string id)
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
            
            ReviewFlagViewModel model = new ReviewFlagViewModel();

            FlaggedVideo flaggedVideo = this.dataAccess.GetFlaggedVideo(id);
            
            if (flaggedVideo == null)
            {
                return this.HttpNotFound();    
            }
            
            Video originalVideo = this.dataAccess.GetVideoById(flaggedVideo.FlaggedVideoId);
            
            if (originalVideo == null)
            {
                return this.HttpNotFound();
            }
            
            ViewModelHelper.PopulateUserInfo(model, loggedIn);
            model.Existing = this.PopulateWatchViewModel(originalVideo);
            model.VideoId = id;
            model.FlagId = flaggedVideo.FlagId;
            
            // Populate the page based on Cookies.  This will populate the page in the case of an error during submit
            // which will redirect to this page with all of the necessary cookies populated.
            IReadableStringCollection requestCookies = this.HttpContext.Request.Cookies;
            model.Title = requestCookies["Title"].FirstOrDefault();            
            model.TitleValidationError = !bool.Parse(requestCookies["TitleValid"].FirstOrDefault() ?? "True");
            model.DancerIdList = (requestCookies["DancerIdList"].FirstOrDefault() ?? string.Empty).Replace('+', ';');
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
                // if there were no errors, this must be the initial load, so pull the data from the flag
                model.Title = flaggedVideo.Title;
                model.DancerIdList = string.Join(";", flaggedVideo.DancerIdList ?? new string[] {});
                model.EventId = flaggedVideo.EventId;
                model.SkillLevelId = SkillLevel.GetValidatedSkillLevel(flaggedVideo.SkillLevel);
                model.DanceCategoryId = DanceCategory.GetValidatedDanceCategory(flaggedVideo.DanceCategory);
            }
            
            string[] dancerIds = (model.DancerIdList ?? string.Empty).Split(
                new char[] { ';' },
                20,
                StringSplitOptions.RemoveEmptyEntries);

            model.DancerNameList = ViewModelHelper.GetDancerNames(this.dataAccess, dancerIds);
            model.EventName = ViewModelHelper.GetEventName(this.dataAccess, model.EventId);
            model.PostbackUrl = this.Url.Link("default", new { controller = "Videos", action = "SubmitReviewFlag" });
            model.DeleteFlagUrl = this.Url.Link(
                "default",
                new
                {
                    controller = "Videos",
                    action = "SubmitDeleteFlag",
                    id = flaggedVideo.FlagId,
                });

            return this.View(model);
        }
        
        public IActionResult SubmitDeleteFlag(string flagId)
        {
            bool loggedIn = this.userSessionHandler.GetUserLoginState(
                this.HttpContext.Request.Cookies,
                this.HttpContext.Response.Cookies);
            
            if (!loggedIn)
            {
                return this.Error();
            }
            
            FlaggedVideo flaggedVideo = this.dataAccess.GetFlaggedVideo(flagId);
            
            if (flaggedVideo == null)
            {
                return this.HttpNotFound();
            }
                        
            this.dataAccess.DeleteFlaggedVideo(flagId);
            
            return this.RedirectToRoute("admin", new { controller = "Admin", action = "FlaggedVideoList" });
        }
        
        public IActionResult SubmitReviewFlag(
            string flagId,
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
                
            bool titleValid = true;
            bool dancerIdListValid = true;
            bool eventIdValid = true;
            
            if (string.IsNullOrEmpty(flagId))
            {
                return this.HttpNotFound();
            }

            FlaggedVideo flaggedVideo = this.dataAccess.GetFlaggedVideo(flagId);
            
            if (flaggedVideo == null)
            {
                return this.HttpNotFound();    
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
            
            if (titleValid && dancerIdListValid && eventIdValid)
            {
                Video video = flaggedVideo.ToVideo();
                video.Title = title;
                video.DancerIdList = dancerIds;
                video.EventId = eventId;
                video.SkillLevel = SkillLevel.GetValidatedSkillLevel(skillLevelId);
                video.DanceCategory = DanceCategory.GetValidatedDanceCategory(danceCategoryId);
                this.dataAccess.UpdateVideo(video);
                this.dataAccess.DeleteFlaggedVideo(flagId);
                
                return this.RedirectToRoute("admin", new { controller = "Admin", action = "FlaggedVideoList" });
            }
            else
            {
                CookieOptions cookieOptions = new CookieOptions();
                cookieOptions.Expires = DateTime.UtcNow.AddDays(1);
                
                // Populate cookies with form data so that we can repopulate the form after the redirect.
                // Traditionally this would be done using session variables, but we are using cookies here so
                // that we don't need to worry about session management.
                IResponseCookies responseCookies = this.HttpContext.Response.Cookies;
                responseCookies.Append("Title", title ?? string.Empty, cookieOptions);
                responseCookies.Append("TitleValid", titleValid.ToString(), cookieOptions);
                responseCookies.Append("DancerIdList", (dancerIdList ?? string.Empty).Replace(';', '+'), cookieOptions);
                responseCookies.Append("DancerIdListValid", dancerIdListValid.ToString(), cookieOptions);
                responseCookies.Append("EventId", eventId ?? string.Empty, cookieOptions);
                responseCookies.Append("EventIdValid", eventIdValid.ToString(), cookieOptions);
                responseCookies.Append("SkillLevelId", SkillLevel.GetValidatedSkillLevel(skillLevelId) ?? string.Empty, cookieOptions);
                responseCookies.Append(
                    "DanceCategoryId",
                    DanceCategory.GetValidatedDanceCategory(danceCategoryId) ?? string.Empty,
                    cookieOptions);

                return this.RedirectToAction("Flag", new { id = flagId });
            }
        }
        
        public IActionResult EventSearchResults(string query, int start)
        {
            EventSearchResultsViewModel viewModel = new EventSearchResultsViewModel();
            viewModel.Query = query;
            
            if (!string.IsNullOrEmpty(query))
            {
                List<Event> fullResults;
                fullResults = this.dataAccess.SearchForEvent(query).OrderByDescending(e => e.EventDate).ToList();
                ViewModelHelper.PopulateSearchResults(
                    viewModel,
                    fullResults,
                    start,
                    VideosController.ResultsPerPage, 
                    (contractEvent) =>
                        {
                            EventListItemViewModel listItem = new EventListItemViewModel();
                            listItem.Name = contractEvent.Name + " " + contractEvent.EventDate.Year;
                            listItem.Id = contractEvent.EventId;
                            listItem.Url = "javascript:void(0)";
                            return listItem;
                        },
                    (s) => "javascript:void(0)");
            }
            
            return View(viewModel);
        }
        
        public IActionResult CheckVideo(string providerId, string providerVideoId)
        {
            bool exists = this.dataAccess.ProviderVideoIdExists(providerId, providerVideoId);
            return this.Json(new { exists = exists });
        }
        
        public IActionResult Error()
        {
            return View("~/Views/Shared/Error.cshtml", new BasePageViewModel());
        }
    }
}