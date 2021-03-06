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
            
            bool loggedIn = this.userSessionHandler.GetUserLoginState(
                this.HttpContext.Request.Cookies,
                this.HttpContext.Response.Cookies);
            
            EventViewModel model = new EventViewModel();
            ViewModelHelper.PopulateUserInfo(model, loggedIn);
            model.EventDate = contractEvent.EventDate.ToString("MMMM d, yyyy");
            model.Location = contractEvent.LocationName;
            model.Pointed = contractEvent.WsdcPointed;
            
            model.Title = contractEvent.Name + " " + contractEvent.EventDate.Year;
            
            var videos = this.dataAccess.GetEventVideos(id);
            
            Dictionary<Tuple<string, string>, VideoGroupViewModel> groups;
            groups = new Dictionary<Tuple<string, string>, VideoGroupViewModel>();
            
            foreach (Video video in videos)
            {
                Tuple<string, string> groupId = Tuple.Create(
                    string.IsNullOrEmpty(video.SkillLevel) ?  string.Empty : video.SkillLevel,
                    string.IsNullOrEmpty(video.DanceCategory) ? string.Empty : video.DanceCategory);
                
                VideoGroupViewModel group;
                if (!groups.TryGetValue(groupId, out group))
                {
                    string anchor = (string.IsNullOrEmpty(groupId.Item1) ? "Default" : groupId.Item1) + "_" +
                        (string.IsNullOrEmpty(groupId.Item2) ? "Default" : groupId.Item2);
                    
                    string name;
                    if (string.IsNullOrEmpty(video.DanceCategory) &&
                        string.IsNullOrEmpty(video.SkillLevel))
                    {
                        name = "Uncategorized Videos";
                    }
                    else
                    {
                        name =
                            (DanceCategory.IncludeSkillLevel(video.DanceCategory) ? 
                                (string.IsNullOrEmpty(video.SkillLevel) ?
                                    "Uncategorized" :
                                    SkillLevel.GetSkillLevelDisplayName(video.SkillLevel) + " ") :
                                string.Empty) +
                            (string.IsNullOrEmpty(video.DanceCategory) ?
                                "Uncategorized" :
                                DanceCategory.GetDanceCategoryDisplayName(video.DanceCategory)) + " Videos";
                    }
                    group = new VideoGroupViewModel
                    {
                        Anchor = anchor,
                        Name = name,
                        Videos = new List<VideoListItemViewModel>()
                    };
                    
                    groups[groupId] = group;
                }
                
                group.Videos.Add(ViewModelHelper.PopulateVideoListItem(video, this.Url));
            }
            
            model.VideoGroups = groups
                .OrderBy(x => SkillLevel.GetOrder(x.Key.Item1))
                .ThenBy(x => DanceCategory.GetOrder(x.Key.Item2))
                .Select(x => x.Value).ToList();
            
            model.JumpList = new List<JumpListItemViewModel>();
            if (model.VideoGroups.Count > 1)
            {
                model.JumpList.AddRange(model.VideoGroups.Select(
                    x => new JumpListItemViewModel
                    {
                        Label = x.Name,
                        Url = "#" + x.Anchor
                    }
                ));
            }
            
            if (loggedIn)
            {
                model.AddVideoUrl = this.Url.Link(
                    "default",
                    new { controller = "Videos", action = "AddUrl" });
            }
            else
            {
                model.AddVideoUrl = this.Url.Link(
                    "default",
                    new { controller = "SuggestedVideos", action = "AddUrl" });
            }
            
            return this.View(model);
        }

        public IActionResult Add()
        {
            bool loggedIn = this.userSessionHandler.GetUserLoginState(
                this.HttpContext.Request.Cookies,
                this.HttpContext.Response.Cookies);
            
            if (!loggedIn)
            {
                CookieOptions loginCookieOptions = new CookieOptions();
                loginCookieOptions.Expires = DateTime.UtcNow.AddDays(1);
                this.HttpContext.Response.Cookies.Append(
                    "LoginRedirect",
                    this.Request.Path.ToUriComponent() + this.Request.QueryString,
                    loginCookieOptions);
                return this.RedirectToRoute("default", new { controller = "User", action = "Login" });
            }
            
            EventAddViewModel model = new EventAddViewModel();
            ViewModelHelper.PopulateUserInfo(model, loggedIn);
                        
            // Populate the page based on Cookies.  This will populate the page in the case of an error during submit
            // which will redirect to this page with all of the necessary cookies populated.
            IReadableStringCollection requestCookies = this.HttpContext.Request.Cookies;
            model.Name = requestCookies["Name"].FirstOrDefault();
            model.NameValidationError = !bool.Parse(requestCookies["NameValid"].FirstOrDefault() ?? "True");
            model.Location = requestCookies["Location"].FirstOrDefault();
            model.LocationValidationError = !bool.Parse(requestCookies["LocationValid"].FirstOrDefault() ?? "True");
            model.Date = requestCookies["Date"].FirstOrDefault();
            model.DateValidationError = !bool.Parse(requestCookies["DateValid"].FirstOrDefault() ?? "True");
            model.WsdcPointed = bool.Parse(requestCookies["WsdcPointed"].FirstOrDefault() ?? "False");
            
            CookieOptions cookieOptions = new CookieOptions();
            IResponseCookies responseCookies = this.HttpContext.Response.Cookies;
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
                this.HttpContext.Request.Cookies,
                this.HttpContext.Response.Cookies);
            
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
                IResponseCookies responseCookies = this.HttpContext.Response.Cookies;
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