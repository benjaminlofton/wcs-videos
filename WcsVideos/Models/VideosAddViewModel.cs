namespace WcsVideos.Models
{
	public class VideosAddViewModel : BasePageViewModel
	{              
        public int ProviderId { get; set; }
        
        public string ProviderVideoId { get; set; }
        
        public bool ProviderVideoIdValidationError { get; set; }
        
        public string Title { get; set; }
        
        public bool TitleValidationError { get; set; }
        
        public string DancerNameList { get; set; }
        
        public string DancerIdList { get; set; }
        
        public bool DancerIdListValidationError { get; set; }
        
        public string EventId { get; set; }
        
        public string EventName { get; set; }
        
        public bool EventIdValidationError { get; set; }
	}
}