namespace WcsVideos.Models
{
	public class VideosAddSuccessViewModel : BasePageViewModel
	{              
        public string ProviderId { get; set; }
        
        public string ProviderVideoId { get; set; }
        
        public string Title { get; set; }
        
        public string DancerNameList { get; set; }
        
        public string EventName { get; set; }
        
        public string VideoUrl { get; set; }
        
        public string AddVideoUrl { get; set; }
	}
}