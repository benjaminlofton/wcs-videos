using System.Collections.Generic;

namespace WcsVideos.Models
{
	public class WatchViewModel
	{
		public string Title { get; set; }

		public string ProviderVideoId { get; set; }
		
		public string ExternalUrl { get; set; }
		
		public string ProviderName { get; set; }
		
		public List<DancerLinkViewModel> Dancers { get; set; }
	}
	
	public class DancerLinkViewModel
	{
		public string DisplayName { get; set; }
		
		public string Url { get; set; }
	}
}