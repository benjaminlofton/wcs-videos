namespace WcsVideos.Contracts
{
    public class Video
    {
        public string Id { get; set; }
        
        public int ProviderId { get; set; }
        
        public string ProviderVideoId { get; set; }
        
        public string Title { get; set; }
        
        public string[] DancerIdList { get; set; }
        
        public string EventId { get; set; }
        
        public string SkillLevel { get; set; }

        public Video Clone()
        {
            return new Video
            {
                Id = this.Id,
                ProviderId = this.ProviderId,
                ProviderVideoId = this.ProviderVideoId,
                Title = this.Title,
                DancerIdList = this.DancerIdList == null ? null : (string[])DancerIdList.Clone(),
                EventId = this.EventId,
            };
        }
    }
}