using System;
using WcsVideos.Contracts;

namespace WcsVideos.Models.Population
{    public static class VideoViewModelPopulatorFactory
    {
        public static IVideoViewModelPopulator GetPopulator(Video video)
        {
            if (video.ProviderId == 1)
            {
                return new YoutubeVideoViewModelPopulator(video);
            }
            else if (video.ProviderId == 2)
            {
                return new VimeoVideoViewModelPopulator(video);
            }
            
            throw new NotImplementedException();
        }
    }
}