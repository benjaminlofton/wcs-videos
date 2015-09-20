namespace WcsVideos.Models
{
    public class AdminVideoListViewModel : PagingViewModelBase<VideoListItemViewModel>
    {
        public string Title { get; set; }
    }
}