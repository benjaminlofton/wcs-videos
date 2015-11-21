namespace WcsVideos.Models
{
    public class ReviewSuggestedVideoViewModel : VideoModifyViewModel
    {
        public string DeleteSuggestedVideoUrl { get; set; }
        
        public string SuggestedVideoId { get; set; }
        
        public int ProviderId { get; set; }
        
        public string ProviderVideoId { get; set; }
    }
}