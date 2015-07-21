using System;
using System.Collections.Generic;

namespace WcsVideos.Models
{
	public class VideoListItemViewModel
	{
		public string Url { get; set; }
        
		public string Title { get; set; }
        
        public string ThumbnailUrl { get; set; }
	}
	
    public class EventListItemViewModel
    {
        public string Name { get; set; }
        
        public string Url { get; set; }
        
        public int VideoCount { get; set; }
    }
    
	public class IndexViewModel
	{
		public List<VideoListItemViewModel> Videos { get; set; }
        
        public List<EventListItemViewModel> Events { get; set; }
	}
}