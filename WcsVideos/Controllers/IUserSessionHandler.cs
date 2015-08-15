using Microsoft.AspNet.Http;

namespace WcsVideos.Controllers
{
    public interface IUserSessionHandler
    {
        bool GetUserLoginState(
            IReadableStringCollection requestCookies,
            IResponseCookies responseCookies);
            
        bool LoginUser (
            string username,
            string password,
            IResponseCookies responseCookies);
            
        void LogoutUser(IResponseCookies responseCookies);
    }
}