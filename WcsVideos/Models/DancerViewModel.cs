
using System.Collections.Generic;

namespace WcsVideos.Models
{
	public class DancerViewModel : BasePageViewModel
	{
		public string Title { get; set; }
		
		public List<VideoListItemViewModel> Videos { get; set; }
	}
}