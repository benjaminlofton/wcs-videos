namespace WcsVideos.Contracts
{
    public sealed class Stats
    {
        public int? NumEvents { get; set; }
        public int? NumEventsWithVideos { get; set; }
        public int? NumVideos { get; set; }
        public int? NumVideosWithEvents { get; set; }
        public int? NumVideosWithSkillLevel { get; set; }
        public int? NumVideosWithDanceCategory { get; set; }
        public int? NumDancers { get; set; }
        public int? NumDancersWithVideos { get; set; }
        public int? CacheSizeItems { get; set; }
    }
}