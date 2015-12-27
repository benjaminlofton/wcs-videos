using WcsVideos.Contracts;

namespace WcsVideos.Models.Population
{
    public class VimeoVideoViewModelPopulator : IVideoViewModelPopulator
    {
        private const string EmbedFormatString = 
@"<div class=""embed-responsive embed-responsive-16by9"">
    <iframe class=""embed-responsive-item"" title=""Vimeo video player"" src=""https://player.vimeo.com/video/{0}"" width=""1280"" height=""720"" frameborder=""0"" webkitallowfullscreen mozallowfullscreen allowfullscreen></iframe>
</div>";

        private Video video;
        
        public VimeoVideoViewModelPopulator(Video video)
        {
            this.video = video;
        }
        
        public void Populate(WatchViewModel model)
        {
            model.FacebookThumbnailUrl = string.Format(
                "/img/placeholder.png",
                video.ProviderVideoId);
            model.FacebookThumbnailWidth = 160;
            model.FacebookThumbnailHeight = 90;
            model.ExternalUrl = string.Format("https://vimeo.com/{0}", video.ProviderVideoId);
            model.EmbedCode = string.Format(
                VimeoVideoViewModelPopulator.EmbedFormatString,
                video.ProviderVideoId);
            model.ProviderName = "Vimeo";
            model.Title = video.Title;
            model.ProviderVideoId = video.ProviderVideoId;
        }
        
        public void Populate(VideoListItemViewModel model)
        {
            model.ThumbnailUrl = "/img/placeholder.png";
            model.Title = this.video.Title;
            model.Width = 160;
            model.Height = 90;
        }
    }
}