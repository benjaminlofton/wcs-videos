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
        
        public SearchController(IDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
        }      
        
        public IActionResult Events()
        {
            return View();
        }
        
        public IActionResult DancerSearchResults(string query)
        {
            return this.Dancers(query, 1);
        }
        
        public IActionResult Videos(string query, string events, string dancers, int start)
        {
            VideoSearchViewModel viewModel = new VideoSearchViewModel();
            viewModel.Title = "Video Search";
            viewModel.Query = query;
            
            if (!string.IsNullOrEmpty(query) || !string.IsNullOrEmpty(events) || !string.IsNullOrEmpty(dancers))
            {
                string[] titleFragments = (query ?? string.Empty).Split(
                    new char[] { ' ' },
                    10,
                    StringSplitOptions.RemoveEmptyEntries);
                
                string[] eventIds = (events ?? string.Empty).Split(
                    new char[] { ';' },
                    10,
                    StringSplitOptions.RemoveEmptyEntries);
                
                string[] dancerIds = (dancers ?? string.Empty).Split(
                    new char[] { ';' },
                    10,
                    StringSplitOptions.RemoveEmptyEntries);
                
                List<Video> fullResults = this.dataAccess.SearchForVideo(titleFragments, dancerIds, eventIds);
                ViewModelHelper.PopulateSearchResults(
                    viewModel,
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
            
            return this.View(viewModel);
        }
        
        public IActionResult Dancers(string query, int start)
        {
            DancerSearchResultsViewModel viewModel = new DancerSearchResultsViewModel();
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