using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNet.Mvc;
using WcsVideos.Models;
using WcsVideos.Contracts;

namespace WcsVideos.Controllers
{
    public class SearchController : Controller
    {
        private const int ResultsPerPage = 20;
        
        private IDataAccess dataAccess;
        private IUserSessionHandler userSessionHandler;
        
        public SearchController(IDataAccess dataAccess, IUserSessionHandler userSessionHandler)
        {
            this.dataAccess = dataAccess;
            this.userSessionHandler = userSessionHandler;
        }      
        
        public IActionResult Events()
        {
            return View();
        }
        
        public IActionResult DancerSearchResults(string query)
        {
            return this.Dancers(query, 1);
        }
        
        public IActionResult Videos(string query, bool advancedSearch, string eventId, string dancerIdList, int start)
        {
            VideoSearchViewModel model = new VideoSearchViewModel();
            ViewModelHelper.PopulateUserInfo(
                model,
                this.userSessionHandler.GetUserLoginState(this.Context.Request.Cookies, this.Context.Response.Cookies));
                
            model.Title = "Video Search";
            model.Query = query;
            model.EventId = eventId;
            model.DancerIdList = dancerIdList;
            model.AdvancedSearch = advancedSearch;
            
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
            
            if (!string.IsNullOrEmpty(query) || !string.IsNullOrEmpty(eventId) || !string.IsNullOrEmpty(dancerIdList))
            {
                string[] titleFragments = (query ?? string.Empty).Split(
                    new char[] { ' ' },
                    10,
                    StringSplitOptions.RemoveEmptyEntries);
                
                string[] eventIds = (eventId ?? string.Empty).Split(
                    new char[] { ';' },
                    10,
                    StringSplitOptions.RemoveEmptyEntries);
                
                string[] dancerIds = (dancerIdList ?? string.Empty).Split(
                    new char[] { ';' },
                    10,
                    StringSplitOptions.RemoveEmptyEntries);
                
                List<Video> fullResults = this.dataAccess.SearchForVideo(titleFragments, dancerIds, eventIds);
                ViewModelHelper.PopulateSearchResults(
                    model,
                    fullResults,
                    start,
                    SearchController.ResultsPerPage,
                    (video) => ViewModelHelper.PopulateVideoListItem(video, this.Url),
                    (s) => this.Url.Link(
                        "default",
                        new
                        {
                            controller = "Search",
                            action = "Videos",
                            query = query,
                            start = s
                        }));
            }
            
            return this.View(model);
        }
        
        public IActionResult Dancers(string query, int start)
        {
            DancerSearchResultsViewModel viewModel = new DancerSearchResultsViewModel();
            ViewModelHelper.PopulateUserInfo(
                viewModel,
                this.userSessionHandler.GetUserLoginState(this.Context.Request.Cookies, this.Context.Response.Cookies));
                
            viewModel.Query = query;
            viewModel.Title = "Dancer Search";
            
            if (!string.IsNullOrEmpty(query))
            {
                List<Dancer> fullResults = this.dataAccess.SearchForDancer(query);
                ViewModelHelper.PopulateSearchResults(
                    viewModel,
                    fullResults,
                    start,
                    SearchController.ResultsPerPage, 
                    (dancer) =>
                        {
                            DancerListItemViewModel listItem = new DancerListItemViewModel();
                            listItem.Name = dancer.Name;
                            listItem.VideoCount = dancer.VideoIdList == null ? 0 : dancer.VideoIdList.Length;
                            listItem.WsdcId = dancer.WsdcId;
                            listItem.Url = this.Url.Link(
                                "default",
                                new { controller = "Home", action = "Dancer", id = dancer.WsdcId });
                            return listItem;
                        },
                    (s) => this.Url.Link(
                        "default",
                        new
                        {
                            controller = "Search",
                            action = "Dancers",
                            query = query,
                            start = s
                        }));
            }
            
            return View(viewModel);
        }
        
        public IActionResult Error()
        {
            return View("~/Views/Shared/Error.cshtml");
        }
    }
}