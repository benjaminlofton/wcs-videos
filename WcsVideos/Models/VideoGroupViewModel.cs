using System.Collections.Generic;

namespace WcsVideos.Models
{
	public class VideoGroupViewModel
	{
        public string Anchor { get; set; }
        public string Name { get; set; }
        public List<VideoListItemViewModel> Videos { get; set; }
    }
}