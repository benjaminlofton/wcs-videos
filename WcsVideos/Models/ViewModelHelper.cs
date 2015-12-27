
namespace WcsVideos.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNet.Mvc;
    using WcsVideos.Contracts;
    using WcsVideos.Models.Population;
    
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
                viewModel.NextLinkStart = viewModel.ResultsEnd + 1;
                viewModel.NextLinkUrl = createPageLinkUrl(viewModel.ResultsEnd + 1);
            }
            
            if (start > 1)
            {
                viewModel.ShowPreviousLink = true;
                viewModel.PreviousLinkStart = start - resultsPerPage;
                viewModel.PreviousLinkUrl = createPageLinkUrl(start - resultsPerPage);
            }
        }
        
        public static VideoListItemViewModel PopulateVideoListItem(Video video, IUrlHelper urlHelper)
        {
            VideoListItemViewModel listItem = new VideoListItemViewModel();
            IVideoViewModelPopulator populator = VideoViewModelPopulatorFactory.GetPopulator(video);
            populator.Populate(listItem);
            listItem.Url = urlHelper.Link(
                "default",
                new { controller = "Videos", action = "Watch", id = video.Id });
            
            return listItem;
        }
        
        public static DancerListItemViewModel PopulateDancerListItem(Dancer dancer, IUrlHelper urlHelper)
        {
            DancerListItemViewModel listItem = new DancerListItemViewModel();
            listItem.Name = dancer.Name;
            listItem.VideoCount = dancer.VideoIdList == null ? 0 : dancer.VideoIdList.Length;
            listItem.WsdcId = dancer.WsdcId;
            listItem.Url = urlHelper.Link(
                "default",
                new { controller = "Home", action = "Dancer", id = dancer.WsdcId });
            return listItem;
        }
        
        public static string GetDancerNames(IDataAccess dataAccess, IEnumerable<string> dancerIds)
        {
            if (dancerIds != null)
            {
                List<string> dancerNameList = new List<string>();
                foreach (string dancerId in dancerIds)
                {
                    Dancer dancer = dataAccess.GetDancerById(dancerId);
                    if (dancer != null)
                    {
                        dancerNameList.Add(dancer.Name + " (" + dancerId + ")");
                    }
                }
                
                if (dancerNameList.Count > 0)
                {
                    return string.Join("; ", dancerNameList);
                }
            }

            return "(None)";
        }
        
        public static string GetEventName(IDataAccess dataAccess, string eventId)
        {
            Event contractEvent = null;
            if (!string.IsNullOrEmpty(eventId))
            {
                contractEvent = dataAccess.GetEvent(eventId);
            }
            
            if (contractEvent == null)
            {
                return "(None)";
            }
            else
            {
                return contractEvent.Name + " " + contractEvent.EventDate.Year;
            } 
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