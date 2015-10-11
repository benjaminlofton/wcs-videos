using System.Collections.Generic;

namespace WcsVideos.Models
{
    public class PagingViewModelBase<T> : BasePageViewModel, IPagingViewModel
    {        
        public bool ShowResults { get; set; }
        
        public int ResultsStart { get; set; }
        
        public int ResultsEnd { get; set; }
        
        public int ResultsTotal { get; set; }
		
        public bool ShowNextLink { get; set; }
        
        public string NextLinkUrl { get; set; }
        
        public bool ShowPreviousLink { get; set; }
        
        public int NextLinkStart { get; set; }
        
        public string PreviousLinkUrl { get; set; }
        
        public int PreviousLinkStart { get; set; }
        
		public IEnumerable<T> Entries { get; set; }
    }
}