
using System.Collections.Generic;

namespace WcsVideos.Models
{
	public class DancerSearchResultsViewModel : PagingViewModelBase<DancerListItemViewModel>
	{
		public string Title { get; set; }
        
        public string Query { get; set; }
	}
    
    public class DancerListItemViewModel
    {
        public string Name { get; set; }
        
        public string WsdcId { get; set; }
        
        public int VideoCount { get; set; }
        
        public string Url { get; set; }
    }
}