using System;
using Microsoft.AspNet.Http;

namespace WcsVideos.Controllers
{
    public class UserSessionHandler : IUserSessionHandler
    {
        private string username;
        private string password;
        private string validSessionCookieValue;
        
        public  UserSessionHandler(string username, string password, string validSessionCookieValue)
        {
            this.username = username;
            this.password = password;
            this.validSessionCookieValue = validSessionCookieValue;
        }
        
        public bool GetUserLoginState(
            IReadableStringCollection requestCookies,
            IResponseCookies responseCookies)
        {
            string loginSessionCookieValue = requestCookies[Constants.LoginSessionCookieName];
            
            if (string.IsNullOrEmpty(loginSessionCookieValue))
            {
                return false;
            }
            else
            {
                CookieOptions cookieOptions = new CookieOptions();
                if (string.Equals(loginSessionCookieValue, this.validSessionCookieValue, StringComparison.Ordinal))
                {
                    cookieOptions.Expires = DateTime.UtcNow.AddMinutes(30);
                    responseCookies.Append(
                        Constants.LoginSessionCookieName,
                        this.validSessionCookieValue,
                        cookieOptions);

                    return true;
                }
                else
                {
                    responseCookies.Delete(Constants.LoginSessionCookieName, cookieOptions);
                    return false;
                }
            }
        }
        
        public bool LoginUser (
            string username,
            string password,
            IResponseCookies responseCookies)
        {
            if (string.Equals(username, this.username, StringComparison.Ordinal) &&
                string.Equals(password, this.password, StringComparison.Ordinal))
            {
                CookieOptions cookieOptions = new CookieOptions();
                cookieOptions.Expires = DateTime.UtcNow.AddMinutes(30);
                responseCookies.Append(Constants.LoginSessionCookieName, this.validSessionCookieValue, cookieOptions);
                return true;
            }
            
            return false;
        }
        
        public void LogoutUser(IResponseCookies responseCookies)
        {
            CookieOptions cookieOptions = new CookieOptions();
            responseCookies.Delete(Constants.LoginSessionCookieName, cookieOptions);
        }
    }
}