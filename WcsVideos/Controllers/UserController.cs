using System;
using System.Linq;
using Microsoft.AspNet.Mvc;
using WcsVideos.Models;
using WcsVideos.Contracts;
using Microsoft.AspNet.Http;

namespace WcsVideos.Controllers
{
    public class UserController : Controller
    {
        private IDataAccess dataAccess;
        private IUserSessionHandler userSessionHandler;
        
        public UserController(IDataAccess dataAccess, IUserSessionHandler userSessionHandler)
        {
            this.dataAccess = dataAccess;
            this.userSessionHandler = userSessionHandler;
        }
        
        public IActionResult Login()
        {
            IReadableStringCollection requestCookies = this.HttpContext.Request.Cookies;
            IResponseCookies responseCookies = this.HttpContext.Response.Cookies;
            
            CookieOptions cookieOptions = new CookieOptions();
            cookieOptions.Expires = DateTime.UtcNow.AddMinutes(5);
            string redirectUrl = requestCookies["LoginRedirect"].FirstOrDefault();

            if (string.IsNullOrEmpty(redirectUrl))
            {            
                string referer = this.HttpContext.Request.Headers["Referer"];
                if (!string.IsNullOrEmpty(referer))
                {
                    try
                    {
                        Uri url = new Uri(referer);
                        if (string.Equals(url.Host, this.HttpContext.Request.Host.ToString().Split(':')[0]))
                        {
                            responseCookies.Append("LoginRedirect", referer, cookieOptions);
                        }
                    }
                    catch
                    {
                    }
                }
            }
            
            LoginViewModel model = new LoginViewModel();
            model.Username = requestCookies["Username"].FirstOrDefault();
            responseCookies.Delete("Username", cookieOptions);
            model.ShowFailedLoginAttempt = !string.IsNullOrEmpty(
                requestCookies["ShowFailedLoginAttempt"].FirstOrDefault());
            responseCookies.Delete("ShowFailedLoginAttempt", cookieOptions);
            
            return this.View(model);
        }
        
        public IActionResult SubmitLogin(string username, string password)
        {
            IResponseCookies responseCookies = this.HttpContext.Response.Cookies;
            CookieOptions cookieOptions = new CookieOptions();
            cookieOptions.Expires = DateTime.UtcNow.AddMinutes(90);
            
            if (!this.userSessionHandler.LoginUser(username, password, this.HttpContext.Response.Cookies))
            {
                responseCookies.Append("ShowFailedLoginAttempt", "true", cookieOptions);
                return this.RedirectToAction("Login");
            }
            
            IReadableStringCollection requestCookies = this.HttpContext.Request.Cookies;
            string redirectUrl = requestCookies["LoginRedirect"].FirstOrDefault();
            responseCookies.Delete("LoginRedirect", cookieOptions);
            if (!string.IsNullOrEmpty(redirectUrl))
            {
                return this.Redirect(redirectUrl);
            }
            
            return this.RedirectToRoute("default", new { controller = "Home", action = "Index" });
        }
        
        public IActionResult Logout()
        {
            this.userSessionHandler.LogoutUser(this.HttpContext.Response.Cookies);
            return this.RedirectToRoute("default", new { controller = "Home", action = "Index" });
        }
    }
}