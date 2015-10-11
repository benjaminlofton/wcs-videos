namespace WcsVideos.Models
{
    public class FlaggedVideoListViewModel : PagingViewModelBase<FlaggedVideoListItemViewModel>
    {
        public string Title { get; set; }
    }
    
    public class FlaggedVideoListItemViewModel
    {
        public string Title { get; set; }
        
        public string ReviewUrl { get; set; }
    }
}