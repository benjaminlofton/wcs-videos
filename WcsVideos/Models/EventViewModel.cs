using System.Collections.Generic;

namespace WcsVideos.Models
{
	public class EventViewModel : BasePageViewModel
	{
		public string Title { get; set; }
		
        public string EventDate { get; set; }
        
        public string Location { get; set; }
        
        public bool Pointed { get; set; }
        
		public List<VideoGroupViewModel> VideoGroups { get; set; }
        
        public List<JumpListItemViewModel> JumpList { get; set; }
	}
}