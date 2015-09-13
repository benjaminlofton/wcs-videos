using WcsVideos.Contracts;

namespace WcsVideos.Models.Population
{
    public class YoutubeVideoViewModelPopulator : IVideoViewModelPopulator
    {
        private const string EmbedFormatString = 
@"<div class=""embed-responsive embed-responsive-16by9"">
    <iframe title=""YouTube video player"" class=""embed-responsive-item"" src=""http://www.youtube.com/embed/{0}"" frameborder=0 allowfullscreen></iframe>
</div>";

        private Video video;
        
        public YoutubeVideoViewModelPopulator(Video video)
        {
            this.video = video;
        }
        
        public void Populate(WatchViewModel model)
        {
            model.FacebookThumbnailUrl = string.Format("http://img.youtube.com/vi/{0}/mqdefault.jpg", video.ProviderVideoId);
            model.ExternalUrl = string.Format("https://www.youtube.com/watch?v={0}", video.ProviderVideoId);
            model.EmbedCode = string.Format(
                YoutubeVideoViewModelPopulator.EmbedFormatString,
                video.ProviderVideoId);
            model.ProviderName = "YouTube";
            model.Title = video.Title;
            model.ProviderVideoId = video.ProviderVideoId;
        }
    }
}