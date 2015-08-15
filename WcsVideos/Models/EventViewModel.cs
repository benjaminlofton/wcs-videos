using System.Collections.Generic;

namespace WcsVideos.Models
{
	public class EventViewModel : BasePageViewModel
	{
		public string Title { get; set; }
		
		public List<VideoListItemViewModel> Videos { get; set; }
	}
}