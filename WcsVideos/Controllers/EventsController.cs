using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.AspNet.Mvc;
using WcsVideos.Models;
using WcsVideos.Contracts;
using Microsoft.AspNet.Http;

namespace WcsVideos.Controllers
{
    public class EventsController : Controller
    {
        private IDataAccess dataAccess;
        private IUserSessionHandler userSessionHandler;
        
        public EventsController(IDataAccess dataAccess, IUserSessionHandler userSessionHandler)
        {
            this.dataAccess = dataAccess;
            this.userSessionHandler = userSessionHandler;
        }
        
        public IActionResult Event(string id)
        {           
            if (string.IsNullOrEmpty(id))
            {
                return this.Error();
            }
            
            Event contractEvent = this.dataAccess.GetEvent(id);
            if (contractEvent == null)
            {
                return this.Error();
            }
            
            EventViewModel model = new EventViewModel();
            ViewModelHelper.PopulateUserInfo(
                model,
                this.userSessionHandler.GetUserLoginState(this.Context.Request.Cookies, this.Context.Response.Cookies));
            model.EventDate = contractEvent.EventDate.ToString("MMMM d, yyyy");
            model.Location = contractEvent.LocationName;
            model.Pointed = contractEvent.WsdcPointed;
            
            model.Title = contractEvent.Name + " " + contractEvent.EventDate.Year;
            
            var videos = this.dataAccess.GetEventVideos(id);
            model.Videos = videos.Select(x => ViewModelHelper.PopulateVideoListItem(x, this.Url)).ToList(); 
            
            return this.View(model);
        }

        public IActionResult Add()
        {
            bool loggedIn = this.userSessionHandler.GetUserLoginState(
                this.Context.Request.Cookies,
                this.Context.Response.Cookies);
            
            if (!loggedIn)
            {
                CookieOptions loginCookieOptions = new CookieOptions();
                loginCookieOptions.Expires = DateTime.UtcNow.AddDays(1);
                this.Context.Response.Cookies.Append(
                    "LoginRedirect",
                    this.Request.Path.ToUriComponent() + this.Request.QueryString,
                    loginCookieOptions);
                return this.RedirectToRoute("default", new { controller = "User", action = "Login" });
            }
            
            EventAddViewModel model = new EventAddViewModel();
            ViewModelHelper.PopulateUserInfo(model, loggedIn);
                        
            // Populate the page based on Cookies.  This will populate the page in the case of an error during submit
            // which will redirect to this page with all of the necessary cookies populated.
            IReadableStringCollection requestCookies = this.Context.Request.Cookies;
            model.Name = requestCookies.Get("Name");
            model.NameValidationError = !bool.Parse(requestCookies.Get("NameValid") ?? "True");
            model.Location = requestCookies.Get("Location");
            model.LocationValidationError = !bool.Parse(requestCookies.Get("LocationValid") ?? "True");
            model.Date = requestCookies.Get("Date");
            model.DateValidationError = !bool.Parse(requestCookies.Get("DateValid") ?? "True");
            model.WsdcPointed = bool.Parse(requestCookies.Get("WsdcPointed") ?? "False");
            
            CookieOptions cookieOptions = new CookieOptions();
            IResponseCookies responseCookies = this.Context.Response.Cookies;
            responseCookies.Delete("Name", cookieOptions);
            responseCookies.Delete("NameValid", cookieOptions);
            responseCookies.Delete("Location", cookieOptions);
            responseCookies.Delete("LocationValid", cookieOptions);
            responseCookies.Delete("Date", cookieOptions);
            responseCookies.Delete("DateValid", cookieOptions);
            responseCookies.Delete("WsdcPointed", cookieOptions);

            return this.View(model);
        }

        public IActionResult SubmitAdd(
            string name,
            string location,
            string date,
            bool wsdcPointed)
        {
            bool loggedIn = this.userSessionHandler.GetUserLoginState(
                this.Context.Request.Cookies,
                this.Context.Response.Cookies);
            
            if (!loggedIn)
            {
                return this.Error();
            }
            
            bool nameValid = true;
            bool locationValid = true;
            bool dateValid = true;

            if (string.IsNullOrEmpty(name))
            {
                nameValid = false;
            }
            
            if (string.IsNullOrEmpty(location))
            {
                locationValid = false;
            }
            
            DateTime parsedDate = default(DateTime);
            if (string.IsNullOrEmpty(date) ||
                !DateTime.TryParseExact(
                    date,
                    "yyyy-MM-dd",
                    new DateTimeFormatInfo(),
                    DateTimeStyles.AssumeUniversal,
                    out parsedDate))
            {
                dateValid = false;
            }
                        
            if (nameValid && locationValid && dateValid)
            {
                Event contractEvent = new Event();
                contractEvent.Name = name;
                contractEvent.LocationName = location;
                contractEvent.EventDate = parsedDate;
                contractEvent.WsdcPointed = wsdcPointed;
                string eventId = this.dataAccess.AddEvent(contractEvent);
                
                return this.RedirectToRoute(
                    "default",
                    new { controller = "Events", action = "Event", id = eventId });
            }
            else
            {
                CookieOptions cookieOptions = new CookieOptions();
                cookieOptions.Expires = DateTime.UtcNow.AddDays(1);
                
                // Populate cookies with form data so that we can repopulate the form after the redirect.
                // Traditionally this would be done using session variables, but we are using cookies here so
                // that we don't need to worry about session management.
                IResponseCookies responseCookies = this.Context.Response.Cookies;
                responseCookies.Append("Name", name, cookieOptions);
                responseCookies.Append("NameValid", nameValid.ToString(), cookieOptions);
                responseCookies.Append("Location", location, cookieOptions);
                responseCookies.Append("LocationValid", locationValid.ToString(), cookieOptions);
                responseCookies.Append("Date", date, cookieOptions);
                responseCookies.Append("DateValid", dateValid.ToString(), cookieOptions);
                responseCookies.Append("WsdcPointed", wsdcPointed.ToString(), cookieOptions);
                
                return this.RedirectToAction("Add");
            }
        }

        public IActionResult Error()
        {
            return View("~/Views/Shared/Error.cshtml");
        }
    }
}