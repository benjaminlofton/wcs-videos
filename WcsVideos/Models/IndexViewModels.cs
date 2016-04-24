using System.Collections.Generic;

namespace WcsVideos.Models
{
    public class EventListItemViewModel
    {
        public string Name { get; set; }
        
        public string Url { get; set; }
        
        public string Id { get; set; }
        
        public int VideoCount { get; set; }
        
        public bool ShowVideoCount { get; set; }
    }
    
	public class IndexViewModel : BasePageViewModel
	{
		public List<VideoListItemViewModel> Videos { get; set; }
        
        public List<EventListItemViewModel> Events { get; set; }
        
        public string AddVideoUrl { get; set; }
	}
}