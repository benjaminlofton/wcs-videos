namespace WcsVideos.Models
{
	public class EventAddViewModel : BasePageViewModel
	{              
        public string Name { get; set; }
        
        public bool NameValidationError { get; set; }
        
        public string Date { get; set; }
        
        public bool DateValidationError { get; set; }
        
        public string Location { get; set; }
        
        public bool LocationValidationError { get; set; }
        
        public bool WsdcPointed { get; set; }
	}
}