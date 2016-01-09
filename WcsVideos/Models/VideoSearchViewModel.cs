namespace WcsVideos.Models
{
	public class VideoSearchViewModel : PagingViewModelBase<VideoListItemViewModel>
	{
		public string Title { get; set; }
        
        public string Query { get; set; }
        
        public string EventId { get; set; }
        
        public string EventName { get; set; }
        
        public string DancerIdList { get; set; }
        
        public string DancerNameList { get; set; }
        
        public string SkillLevelId { get; set; }
        
        public string DanceCategoryId { get; set; }
        
        public bool AdvancedSearch { get; set; }
	}
}