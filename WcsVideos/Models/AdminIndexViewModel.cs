namespace WcsVideos.Models
{
	public class AdminIndexViewModel : BasePageViewModel
	{
        public string MissingEventVideoListUrl { get; set; }
        
        public int MissingEventCount { get; set; }
        
        public string MissingDancersVideoListUrl { get; set; }
        
        public int MissingDancersCount { get; set; }
        
        public string MissingLevelVideoListUrl { get; set; }
        
        public int MissingLevelCount { get; set; }
        
        public string MissingCategoryVideoListUrl { get; set; }
        
        public int MissingCategoryCount { get; set; }
        
        public int EventCount { get; set; }
        
        public int DancerCount { get; set; }
        
        public int VideoCount { get; set; }
        
        public int SuggestedVideoCount { get; set; }
        
        public int FlaggedVideoCount { get; set; }
    }
}