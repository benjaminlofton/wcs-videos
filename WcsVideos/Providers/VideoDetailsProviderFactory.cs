using System;

namespace WcsVideos.Providers
{
    public static class VideoDetailsProviderFactory
    {
        public static bool TryGetProvider(Uri url, out IVideoDetailsProvider provider)
        {
            provider = null;
            
            if (string.Equals(url.Host, "www.youtube.com", StringComparison.Ordinal))
            {
                provider = new YoutubeVideoDetailsProvider(url);
                return true;
            }
            
            if (string.Equals(url.Host, "youtu.be", StringComparison.Ordinal))
            {
                provider = new YoutubeVideoDetailsProvider(url);
                return true;
            }

            return false;
        }
    }
}