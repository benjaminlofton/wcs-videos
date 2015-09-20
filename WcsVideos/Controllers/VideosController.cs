using System;
using System.Collections.Generic;
using Microsoft.AspNet.Mvc;
using WcsVideos.Models;
using WcsVideos.Models.Population;
using WcsVideos.Contracts;
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
                this.Context.Request.Cookies,
                this.Context.Response.Cookies);
                
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
                this.userSessionHandler.GetUserLoginState(this.Context.Request.Cookies, this.Context.Response.Cookies));
            
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
            
            return model;
        }
        
        public IActionResult Add(string title, string providerVideoId)
        {
            bool loggedIn = this.userSessionHandler.GetUserLoginState(
                this.Context.Request.Cookies,
                this.Context.Response.Cookies);
            
            if (!loggedIn)
            {
                CookieOptions loginCookieOptions = new CookieOptions();
                loginCookieOptions.Expires = DateTime.UtcNow.AddDays(1);
                this.Context.Response.Cookies.Append(
                    "LoginRedirect",
                    this.Request.Path.ToUriComponent() + this.Request.QueryString,
                    loginCookieOptions);
                return this.RedirectToRoute("default", new { controller = "User", action = "Login" });
            }
            
            VideosAddViewModel model = new VideosAddViewModel();
            ViewModelHelper.PopulateUserInfo(model, loggedIn);
                        
            // Populate the page based on Cookies.  This will populate the page in the case of an error during submit
            // which will redirect to this page with all of the necessary cookies populated.
            IReadableStringCollection requestCookies = this.Context.Request.Cookies;
            model.ProviderId = 1;  //requestCookies.Get("ProviderId");
            model.ProviderVideoIdValidationError = !bool.Parse(requestCookies.Get("ProviderIdValid") ?? "True");
            model.ProviderVideoId = requestCookies.Get("ProviderVideoId");
            model.ProviderVideoIdValidationError = !bool.Parse(requestCookies.Get("ProviderVideoIdValid") ?? "True");
            model.Title = requestCookies.Get("Title");
            model.TitleValidationError = !bool.Parse(requestCookies.Get("TitleValid") ?? "True");
            model.DancerIdList = requestCookies.Get("DancerIdList");
            model.DancerIdListValidationError = !bool.Parse(requestCookies.Get("DancerIdListValid") ?? "True");
            model.EventId = requestCookies.Get("EventId");
            model.EventIdValidationError = !bool.Parse(requestCookies.Get("EventIdValid") ?? "True");
            
            CookieOptions cookieOptions = new CookieOptions();
            IResponseCookies responseCookies = this.Context.Response.Cookies;
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

            if (!string.IsNullOrEmpty(title))
            {
                model.Title = title;
            }
            
            if (!string.IsNullOrEmpty(providerVideoId))
            {
                model.ProviderVideoId = providerVideoId;
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
            string eventId)
        {
            bool loggedIn = this.userSessionHandler.GetUserLoginState(
                this.Context.Request.Cookies,
                this.Context.Response.Cookies);
            
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
                parsedProviderId != 1)
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
                IResponseCookies responseCookies = this.Context.Response.Cookies;
                responseCookies.Append("ProviderId", providerId, cookieOptions);
                responseCookies.Append("ProviderIdValid", providerIdValid.ToString(), cookieOptions);
                responseCookies.Append("ProviderVideoId", providerVideoId, cookieOptions);
                responseCookies.Append("ProviderVideoIdValid", providerVideoIdValid.ToString(), cookieOptions);
                responseCookies.Append("Title", title, cookieOptions);
                responseCookies.Append("TitleValid", titleValid.ToString(), cookieOptions);
                responseCookies.Append("DancerIdList", dancerIdList, cookieOptions);
                responseCookies.Append("DancerIdListValid", dancerIdListValid.ToString(), cookieOptions);
                responseCookies.Append("EventId", eventId, cookieOptions);
                responseCookies.Append("EventIdValid", eventIdValid.ToString(), cookieOptions);
                
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

            return this.View(model);
        }
        
        public IActionResult Edit(string id)
        {
            bool loggedIn = this.userSessionHandler.GetUserLoginState(
                this.Context.Request.Cookies,
                this.Context.Response.Cookies);
            
            if (!loggedIn)
            {
                CookieOptions loginCookieOptions = new CookieOptions();
                loginCookieOptions.Expires = DateTime.UtcNow.AddDays(1);
                this.Context.Response.Cookies.Append(
                    "LoginRedirect",
                    this.Request.Path.ToUriComponent() + this.Request.QueryString,
                    loginCookieOptions);
                return this.RedirectToRoute("default", new { controller = "User", action = "Login" });
            }
            
            VideoEditViewModel model = new VideoEditViewModel();

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
            IReadableStringCollection requestCookies = this.Context.Request.Cookies;
            model.Title = requestCookies.Get("Title");            
            model.TitleValidationError = !bool.Parse(requestCookies.Get("TitleValid") ?? "True");
            model.DancerIdList = requestCookies.Get("DancerIdList");
            model.DancerIdListValidationError = !bool.Parse(requestCookies.Get("DancerIdListValid") ?? "True");
            model.EventId = requestCookies.Get("EventId");
            model.EventIdValidationError = !bool.Parse(requestCookies.Get("EventIdValid") ?? "True");
            
            CookieOptions cookieOptions = new CookieOptions();
            IResponseCookies responseCookies = this.Context.Response.Cookies;
            responseCookies.Delete("Title", cookieOptions);
            responseCookies.Delete("TitleValid", cookieOptions);
            responseCookies.Delete("DancerIdList", cookieOptions);
            responseCookies.Delete("DancerIdListValid", cookieOptions);
            responseCookies.Delete("EventId", cookieOptions);
            responseCookies.Delete("EventIdValid", cookieOptions);

            if (!model.TitleValidationError &&
                !model.DancerIdListValidationError &&
                !model.EventIdValidationError)
            {
                // if there were no errors, this must be the initial load, so pull the data from the video
                model.Title = video.Title;
                model.DancerIdList = string.Join(";", video.DancerIdList ?? new string[] {});
                model.EventId = video.EventId;
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
                string[] dancerIds = (model.DancerIdList ?? string.Empty).Split(
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
        
        public IActionResult SubmitEdit(
            string videoId,
            string title,
            string dancerIdList,
            string eventId)
        {
            bool loggedIn = this.userSessionHandler.GetUserLoginState(
                this.Context.Request.Cookies,
                this.Context.Response.Cookies);
            
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
                IResponseCookies responseCookies = this.Context.Response.Cookies;
                responseCookies.Append("Title", title, cookieOptions);
                responseCookies.Append("TitleValid", titleValid.ToString(), cookieOptions);
                responseCookies.Append("DancerIdList", dancerIdList, cookieOptions);
                responseCookies.Append("DancerIdListValid", dancerIdListValid.ToString(), cookieOptions);
                responseCookies.Append("EventId", eventId, cookieOptions);
                responseCookies.Append("EventIdValid", eventIdValid.ToString(), cookieOptions);
                
                return this.RedirectToAction("Edit");
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
        
        public IActionResult EventSearchResults(string query, int start)
        {
            EventSearchResultsViewModel viewModel = new EventSearchResultsViewModel();
            viewModel.Query = query;
            
            if (!string.IsNullOrEmpty(query))
            {
                List<Event> fullResults = this.dataAccess.SearchForEvent(query);
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