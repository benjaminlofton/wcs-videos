using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Mvc;
using WcsVideos.Models;
using WcsVideos.Contracts;
using Microsoft.AspNet.Http;

namespace WcsVideos.Controllers
{
    public class VideosController : Controller
    {
        private const int ResultsPerPage = 20;
        
        private IDataAccess dataAccess;
        
        public VideosController(IDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
        }
        
        public IActionResult Add()
        {
            VideosAddViewModel model = new VideosAddViewModel();
            model.ProviderId = this.Context.Request.Cookies.Get("ProviderId");
            model.ProviderVideoIdValidationError = !bool.Parse(this.Context.Request.Cookies.Get("ProviderIdValid") ?? "True");
            model.ProviderVideoId = this.Context.Request.Cookies.Get("ProviderVideoId");
            model.ProviderVideoIdValidationError = !bool.Parse(this.Context.Request.Cookies.Get("ProviderVideoIdValid") ?? "True");
            model.Title = this.Context.Request.Cookies.Get("Title");
            model.TitleValidationError = !bool.Parse(this.Context.Request.Cookies.Get("TitleValid") ?? "True");
            model.DancerIdList = this.Context.Request.Cookies.Get("DancerIdList");
            model.DancerIdListValidationError = !bool.Parse(this.Context.Request.Cookies.Get("DancerIdListValid") ?? "True");
            model.EventId = this.Context.Request.Cookies.Get("EventId");
            model.EventIdValidationError = !bool.Parse(this.Context.Request.Cookies.Get("EventIdValid") ?? "True");
            
            CookieOptions cookieOptions = new CookieOptions();
            this.Context.Response.Cookies.Delete("ProviderId", cookieOptions);
            this.Context.Response.Cookies.Delete("ProviderIdValid", cookieOptions);
            this.Context.Response.Cookies.Delete("ProviderVideoId", cookieOptions);
            this.Context.Response.Cookies.Delete("ProviderVideoIdValid", cookieOptions);
            this.Context.Response.Cookies.Delete("Title", cookieOptions);
            this.Context.Response.Cookies.Delete("TitleValid", cookieOptions);
            this.Context.Response.Cookies.Delete("DancerIdList", cookieOptions);
            this.Context.Response.Cookies.Delete("DancerIdListValid", cookieOptions);
            this.Context.Response.Cookies.Delete("EventId", cookieOptions);
            this.Context.Response.Cookies.Delete("EventIdValid", cookieOptions);

            Event contractEvent = null;
            if (!string.IsNullOrEmpty(model.EventId))
            {
                contractEvent = this.dataAccess.GetEvent(model.EventId);
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
            bool providerIdValid = true;
            bool providerVideoIdValid = true;
            bool titleValid = true;
            bool dancerIdListValid = true;
            bool eventIdValid = true;
            if (string.IsNullOrEmpty(providerId) ||
                !string.Equals(providerId, "youtube", StringComparison.Ordinal))
            {
                providerIdValid = false;
            }
            
            if (string.IsNullOrEmpty(providerVideoId))
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
                video.ProviderId = "1";
                video.ProviderVideoId = providerVideoId;
                video.Title = title;
                video.DancerIdList = dancerIds;
                video.EventId = eventId;
                string videoId = this.dataAccess.AddVideo(video);
                
                return this.RedirectToAction(
                    "AddSuccess",
                    new { id = videoId });
            }
            else
            {
                CookieOptions cookieOptions = new CookieOptions();
                cookieOptions.Expires = DateTime.UtcNow.AddDays(1);
                this.Context.Response.Cookies.Append("ProviderId", providerId, cookieOptions);
                this.Context.Response.Cookies.Append("ProviderIdValid", providerIdValid.ToString(), cookieOptions);
                this.Context.Response.Cookies.Append("ProviderVideoId", providerVideoId, cookieOptions);
                this.Context.Response.Cookies.Append("ProviderVideoIdValid", providerVideoIdValid.ToString(), cookieOptions);
                this.Context.Response.Cookies.Append("Title", title, cookieOptions);
                this.Context.Response.Cookies.Append("TitleValid", titleValid.ToString(), cookieOptions);
                this.Context.Response.Cookies.Append("DancerIdList", dancerIdList, cookieOptions);
                this.Context.Response.Cookies.Append("DancerIdListValid", dancerIdListValid.ToString(), cookieOptions);
                this.Context.Response.Cookies.Append("EventId", eventId, cookieOptions);
                this.Context.Response.Cookies.Append("EventIdValid", eventIdValid.ToString(), cookieOptions);
                
                return this.RedirectToAction(
                    "Add");
            }
        }
        
        public IActionResult AddSuccess(string id)
        {
            VideosAddSuccessViewModel model = new VideosAddSuccessViewModel();
            
            Video video = this.dataAccess.GetVideoById(id);
            model.ProviderId = video.ProviderId;
            model.ProviderVideoId = video.ProviderVideoId;
            model.Title = video.Title;
            model.DancerIdList = string.Join(";", video.DancerIdList);
            model.VideoUrl = this.Url.Link(
                "default",
                new { controller = "Home", action = "Watch", id = id });
            model.AddVideoUrl = this.Url.Link(
                "default",
                new { controller = "Videos", action = "Add" });

            return this.View(model);
        }
        
        public IActionResult DancerSearchResults(string query, int start)
        {
            DancerSearchResultsViewModel viewModel = new DancerSearchResultsViewModel();
            List<DancerListItemViewModel> results = new List<DancerListItemViewModel>();
            viewModel.Query = query;
            viewModel.Entries = results;
            
            if (start <= 0)
            {
                start = 1;
            }
            
            if (!string.IsNullOrEmpty(query))
            {
                viewModel.ShowResults = true;
                
                List<Dancer> fullResults = this.dataAccess.SearchForDancer(query);
                viewModel.ResultsTotal = fullResults.Count;
                
                foreach(Dancer dancer in fullResults.Skip(start - 1).Take(VideosController.ResultsPerPage))
                {
                    DancerListItemViewModel listItem = new DancerListItemViewModel();
                    listItem.Name = dancer.Name;
                    listItem.VideoCount = dancer.VideoIdList == null ? 0 : dancer.VideoIdList.Length;
                    listItem.WsdcId = dancer.WsdcId;
                    listItem.Url = string.Format("javascript:addDancer('{0}');", dancer.WsdcId);
                    results.Add(listItem);
                }
                
                viewModel.ResultsStart = start;
                viewModel.ResultsEnd = start + results.Count - 1;
                viewModel.ResultsTotal = fullResults.Count;
                
                if (viewModel.ResultsEnd < fullResults.Count)
                {
                    viewModel.ShowNextLink = true;
                    viewModel.NextLinkUrl = string.Format(
                        "javascript:searchForDancer('{0}', {1});",
                        query,
                        viewModel.ResultsEnd + 1);
                }
                
                if (start > 1)
                {
                    viewModel.ShowPreviousLink = true;
                    viewModel.PreviousLinkUrl = string.Format(
                        "javascript:searchForDancer('{0}', {1});",
                        query,
                        viewModel.ResultsStart - VideosController.ResultsPerPage);
                }
            }
            
            return View(viewModel);
        }
        
        public IActionResult EventSearchResults(string query, int start)
        {
            EventSearchResultsViewModel viewModel = new EventSearchResultsViewModel();
            List<EventListItemViewModel> results = new List<EventListItemViewModel>();
            viewModel.Query = query;
            viewModel.Entries = results;
            
            if (start <= 0)
            {
                start = 1;
            }
            
            if (!string.IsNullOrEmpty(query) && query.Trim().Length > 2)
            {
                viewModel.ShowResults = true;
                
                List<Event> fullResults = this.dataAccess.SearchForEvent(query);
                viewModel.ResultsTotal = fullResults.Count;
                
                foreach(Event contractEvent in fullResults.Skip(start - 1).Take(VideosController.ResultsPerPage))
                {
                    EventListItemViewModel listItem = new EventListItemViewModel();
                    listItem.Name = contractEvent.Name + " " + contractEvent.EventDate.Year;
                    listItem.Url = string.Format(
                        "javascript:setEvent('{0}', '{1}');",
                        contractEvent.EventId,
                        listItem.Name);
                    results.Add(listItem);
                }
                
                viewModel.ResultsStart = start;
                viewModel.ResultsEnd = start + results.Count - 1;
                viewModel.ResultsTotal = fullResults.Count;
                
                if (viewModel.ResultsEnd < fullResults.Count)
                {
                    viewModel.ShowNextLink = true;
                    viewModel.NextLinkUrl = string.Format(
                        "javascript:searchForEvent('{0}', {1});",
                        query,
                        viewModel.ResultsEnd + 1);
                }
                
                if (start > 1)
                {
                    viewModel.ShowPreviousLink = true;
                    viewModel.PreviousLinkUrl = string.Format(
                        "javascript:searchForEvent('{0}', {1});",
                        query,
                        viewModel.ResultsStart - VideosController.ResultsPerPage);
                }
            }
            
            return View(viewModel);
        }
        
        public IActionResult Error()
        {
            return View("~/Views/Shared/Error.cshtml");
        }
    }
}