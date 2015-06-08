using System;

namespace WcsVideos.Contracts
{
    public class Video
    {
        public string Id { get; set; }
        public string ProviderId { get; set; }
        public string ProviderVideoId { get; set; }
        public string Title { get; set; }
    }
}