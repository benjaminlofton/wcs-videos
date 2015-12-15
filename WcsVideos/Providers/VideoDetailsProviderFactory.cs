using System;

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
    }
}