
namespace WcsVideos.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNet.Mvc;
    using WcsVideos.Contracts;
    
    public static class ViewModelHelper
    {
        public static void PopulateSearchResults<T, K>(
            PagingViewModelBase<K> viewModel,
            List<T> fullResults,
            int start,
            int resultsPerPage,
            Func<T, K> populateListItem,
            Func<int, string> createPageLinkUrl)    
        {
            if (start <= 0)
            {
                start = 1;
            }
            
            List<K> results = new List<K>();
            viewModel.Entries = results;

            viewModel.ShowResults = true;                
            viewModel.ResultsTotal = fullResults.Count;
            
            foreach(T contractItem in fullResults.Skip(start - 1).Take(resultsPerPage))
            {
                K listItem = populateListItem(contractItem);
                results.Add(listItem);
            }
            
            viewModel.ResultsStart = fullResults.Count > 0 ? start : 0;
            viewModel.ResultsEnd = fullResults.Count > 0 ? start + results.Count - 1 : 0;
            viewModel.ResultsTotal = fullResults.Count;
            
            if (viewModel.ResultsEnd < fullResults.Count)
            {
                viewModel.ShowNextLink = true;
                viewModel.NextLinkUrl = createPageLinkUrl(viewModel.ResultsEnd + 1);
            }
            
            if (start > 1)
            {
                viewModel.ShowPreviousLink = true;
                viewModel.PreviousLinkUrl = createPageLinkUrl(start - resultsPerPage);
            }
        }
        
        public static VideoListItemViewModel PopulateVideoListItem(Video video, IUrlHelper urlHelper)
        {
            VideoListItemViewModel listItem = new VideoListItemViewModel();
            listItem.Title = video.Title;
            listItem.Url = urlHelper.Link(
                "default",
                new { controller = "Videos", action = "Watch", id = video.Id });
            listItem.ThumbnailUrl = string.Format(
                "http://img.youtube.com/vi/{0}/default.jpg",
                video.ProviderVideoId);
            
            return listItem;
        }
        
        public static void PopulateUserInfo(BasePageViewModel model, bool isLoggedIn)
        {
            if (isLoggedIn)
            {
                model.ShowAdminActions = true;
                model.ShowLogoutLink = true;
            }
            else
            {
                model.ShowLoginLink = true;
            }
        }
    }
}