using System;
using WcsVideos.Contracts;

namespace WcsVideos.Providers
{
    public static class VideoDetailsProviderFactory
    {
        public static bool TryGetProvider(Uri url, out IVideoDetailsProvider provider)
        {
            provider = null;
            
            if (string.Equals(url.Host, "www.youtube.com", StringComparison.Ordinal) ||
                string.Equals(url.Host, "youtu.be", StringComparison.Ordinal) ||
                string.Equals(url.Host, "youtube.com", StringComparison.Ordinal))
            {
                provider = new YoutubeVideoDetailsProvider(url);
                return true;
            }

            if (string.Equals(url.Host, "vimeo.com", StringComparison.Ordinal))
            {
                provider = new VimeoVideoDetailsProvider(url);
                return true;
            }

            return false;
        }
        
        public static bool TryGetProvider(Video video, out IVideoDetailsProvider provider)
        {
            switch(video.ProviderId)
            {
                case 1:
                    provider = new YoutubeVideoDetailsProvider(
                        new Uri("https://youtu.be/" + video.ProviderVideoId));
                    return true;
                case 2:
                    provider = new YoutubeVideoDetailsProvider(
                        new Uri("https://vimeo.com/" + video.ProviderVideoId));
                    return true;
                default:
                    provider = null;
                    return false;
            }
        }
    }
}