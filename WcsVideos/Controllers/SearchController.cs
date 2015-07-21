using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        
        public IActionResult Dancers(string query, int start)
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
                
                List<Dancer> fullResults = this.dataAccess.SearchForDancer(query)
                    .Where(d => d.VideoIdList != null && d.VideoIdList.Length > 0)
                    .ToList();
                viewModel.ResultsTotal = fullResults.Count;
                
                foreach(Dancer dancer in fullResults.Skip(start - 1).Take(SearchController.ResultsPerPage))
                {
                    DancerListItemViewModel listItem = new DancerListItemViewModel();
                    listItem.Name = dancer.Name;
                    listItem.VideoCount = dancer.VideoIdList == null ? 0 : dancer.VideoIdList.Length;
                    listItem.WsdcId = dancer.WsdcId;
                    listItem.Url = this.Url.Link(
                        "default",
                        new { controller = "Home", action = "Dancer", id = dancer.WsdcId });
                    results.Add(listItem);
                }
                
                viewModel.ResultsStart = start;
                viewModel.ResultsEnd = start + results.Count - 1;
                viewModel.ResultsTotal = fullResults.Count;
                
                if (viewModel.ResultsEnd < fullResults.Count)
                {
                    viewModel.ShowNextLink = true;
                    viewModel.NextLinkUrl = this.Url.Link(
                        "default",
                        new
                        {
                            controller = "Search",
                            action = "Dancers",
                            query = query,
                            start = viewModel.ResultsEnd
                        });
                }
                
                if (start > 1)
                {
                    viewModel.ShowPreviousLink = true;
                    viewModel.PreviousLinkUrl = this.Url.Link(
                        "default",
                        new
                        {
                            controller = "Search",
                            action = "Dancers",
                            query = query,
                            start = start - 20
                        });
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