namespace WcsVideos.Models
{
	public class VideoEditViewModel : BasePageViewModel
	{
        public string VideoId { get; set; }
                       
        public string Title { get; set; }
        
        public bool TitleValidationError { get; set; }
        
        public string DancerNameList { get; set; }
        
        public string DancerIdList { get; set; }
        
        public bool DancerIdListValidationError { get; set; }
        
        public string EventId { get; set; }
        
        public string EventName { get; set; }
        
        public bool EventIdValidationError { get; set; }
        
        public WatchViewModel Existing { get; set; } 
	}
}