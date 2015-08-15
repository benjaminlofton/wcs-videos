namespace WcsVideos.Models
{
	public class LoginViewModel : BasePageViewModel
	{
		public string Username { get; set; }
        
        public bool ShowFailedLoginAttempt { get; set; }
	}
}