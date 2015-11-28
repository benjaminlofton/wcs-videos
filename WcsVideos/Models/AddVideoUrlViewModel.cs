namespace WcsVideos.Models
{
	public class AddVideoUrlViewModel : BasePageViewModel
	{
        public string Url { get; set; }
        
        public bool ValidationError { get; set; }
        
        public string ValidationErrorMessage { get; set; }
    }
}