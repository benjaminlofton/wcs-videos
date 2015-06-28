
using System.Collections.Generic;

namespace WcsVideos.Models
{
	public class DancerSearchResultsViewModel
	{
		public string Title { get; set; }
        
        public string Query { get; set; }
        
        public bool ShowResults { get; set; }
        
        public int ResultsStart { get; set; }
        
        public int ResultsEnd { get; set; }
        
        public int ResultsTotal { get; set; }
		
        public bool ShowNextLink { get; set; }
        
        public string NextLinkUrl { get; set; }
        
        public bool ShowPreviousLink { get; set; }
        
        public string PreviousLinkUrl { get; set; }
        
		public IEnumerable<DancerListItemViewModel> Dancers { get; set; }
	}
    
    public class DancerListItemViewModel
    {
        public string Name { get; set; }
        
        public string WsdcId { get; set; }
        
        public int VideoCount { get; set; }
        
        public string Url { get; set; }
    }
}