using System.Collections.Generic;

namespace WcsVideos.Models
{
	public class WatchViewModel : BasePageViewModel
	{
		public string Title { get; set; }

		public string ProviderVideoId { get; set; }
		
		public string ExternalUrl { get; set; }
        
        public string FacebookThumbnailUrl { get; set; }
        
        public int FacebookThumbnailWidth { get; set; }
        
        public int FacebookThumbnailHeight { get; set; }
		
		public string EmbedCode { get; set; }
		
		public string ProviderName { get; set; }
		
		public List<DancerLinkViewModel> Dancers { get; set; }
        
        public string SkillLevel { get; set; }
        
        public string DanceCategory { get; set; }
        
        public string EventName { get; set; }
        
        public string EventUrl { get; set; }
        
        public string EditUrl { get; set; }
	}
	
	public class DancerLinkViewModel
	{
		public string DisplayName { get; set; }
		
		public string Url { get; set; }
	}
}