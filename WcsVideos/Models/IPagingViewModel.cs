namespace WcsVideos.Models
{
    public interface IPagingViewModel
    {
        bool ShowNextLink { get; set; }
        
        string NextLinkUrl { get; set; }
        
        bool ShowPreviousLink { get; set; }
        
        int NextLinkStart { get; set; }
        
        string PreviousLinkUrl { get; set; }
        
        int PreviousLinkStart { get; set; }
    }
}