namespace WcsVideos.Contracts
{
    public class FlaggedVideo
    {
        public string FlagId { get; set; }
        
        public string FlaggedVideoId { get; set; }
        
        public int ProviderId;
        
        public string ProviderVideoId { get; set; }
        
        public string Title { get; set; }
        
        public string[] DancerIdList { get; set; }
        
        public string EventId { get; set; }
        
        public string SkillLevel { get; set; }
        
        public string Explanation { get; set; }

        public static FlaggedVideo FromVideo(Video video)
        {
            return new FlaggedVideo
            {
                FlaggedVideoId = video.Id,
                ProviderId = video.ProviderId,
                ProviderVideoId = video.ProviderVideoId,
                Title = video.Title,
                DancerIdList = video.DancerIdList == null ? null : (string[])video.DancerIdList.Clone(),
                EventId = video.EventId,
                SkillLevel = video.SkillLevel,
            };
        }
        
        public Video ToVideo()
        {
            return new Video
            {
                Id = this.FlaggedVideoId,
                ProviderId = this.ProviderId,
                ProviderVideoId = this.ProviderVideoId,
                Title = this.Title,
                DancerIdList = this.DancerIdList == null ? null : (string[])this.DancerIdList.Clone(),
                EventId = this.EventId,
                SkillLevel = this.SkillLevel,
            };
        }
    }
}