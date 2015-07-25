namespace WcsVideos.Models
{
	public class VideoSearchViewModel : PagingViewModelBase<VideoListItemViewModel>
	{
		public string Title { get; set; }
        
        public string Query { get; set; }
	}
}