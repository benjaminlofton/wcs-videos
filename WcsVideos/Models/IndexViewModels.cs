using Newtonsoft.Json;
using System.Collections.Generic;

namespace WcsVideos.Models
{
	public class VideoListItemViewModel
	{
		public string Url { get; set; }
		public string Title { get; set; }
	}
	
	public class IndexViewModel
	{
		public List<VideoListItemViewModel> Videos { get; set; }
	}
}