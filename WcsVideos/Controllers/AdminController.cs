using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Http;
using WcsVideos.Models;
using WcsVideos.Contracts;

namespace WcsVideos.Controllers
{
    public class AdminController : Controller
    {
        private const string MissingEventVideoListId = "no-event";
        private const string MissingEventVideoListTitle = "Videos Without Events";
        private const string MissingDancersVideoListId = "videos-needing-dancers";
        private const string MissingDancersVideoListTitle = "Videos Missing Dancers";
        private static readonly Dictionary<string, string> ListIdTitleMapping = new Dictionary<string, string>
        {
            { AdminController.MissingEventVideoListId, MissingEventVideoListTitle },
            { AdminController.MissingDancersVideoListId, MissingDancersVideoListTitle },
        };
        
        private IDataAccess dataAccess;
        private IUserSessionHandler userSessionHandler;
        
        public AdminController(IDataAccess dataAccess, IUserSessionHandler userSessionHandler)
        {
            this.dataAccess = dataAccess;
            this.userSessionHandler = userSessionHandler;
        }
        
        public IActionResult Index()
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
            
            AdminIndexViewModel model = new AdminIndexViewModel();
            ViewModelHelper.PopulateUserInfo(model, loggedIn);
            
            model.MissingDancersVideoListUrl = this.Url.Link(
                "default",
                new { controller = "Admin", action = "VideoList", id = AdminController.MissingDancersVideoListId });
            
            model.MissingEventVideoListUrl = this.Url.Link(
                "default",
                new { controller = "Admin", action = "VideoList", id = AdminController.MissingEventVideoListId});
            
            return this.View(model);
        }
        
        public IActionResult SuggestedVideoList(int start)
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
            
            AdminVideoListViewModel model = new AdminVideoListViewModel();
            ViewModelHelper.PopulateUserInfo(model, loggedIn);
            List<Video> suggestedVideos = this.dataAccess.GetSuggestedVideos();
            
            model.Title = "Review Suggested Videos";

            ViewModelHelper.PopulateSearchResults(
                model,
                suggestedVideos,
                start,
                20,
                (video) => 
                {
                    var li = ViewModelHelper.PopulateVideoListItem(video, this.Url);
                    li.Url = this.Url.Link(
                        "default",
                        new
                        {
                            controller = "SuggestedVideos",
                            action = "Review",
                            video.Id
                        });
                    return li;
                },  
                (s) => this.Url.Link(
                    "default",
                    new
                    {
                        controller = "Admin",
                        action = "SuggestedVideoList",
                        start = s
                    }));
            
            return this.View("VideoList", model);            
        }
        
        public IActionResult VideoList(string id, int start)
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
            
            ResourceList resourceList = this.dataAccess.GetResourceList(id);
            if (resourceList == null ||
                !string.Equals(resourceList.ResourceType, "video", StringComparison.OrdinalIgnoreCase))
            {
                return this.HttpNotFound();
            }
            
            AdminVideoListViewModel model = new AdminVideoListViewModel();
            ViewModelHelper.PopulateUserInfo(model, loggedIn);
            
            string title;
            if (!AdminController.ListIdTitleMapping.TryGetValue(id, out title))
            {
                title = id;
            }
            
            model.Title = title;
            
            ViewModelHelper.PopulateSearchResults(
                model,
                resourceList.Ids.ToList(),
                start,
                20,
                (videoId) => 
                {
                    Video video = this.dataAccess.GetVideoById(videoId);
                    var li = ViewModelHelper.PopulateVideoListItem(video, this.Url);
                    li.Url = this.Url.Link(
                        "default",
                        new
                        {
                            controller = "Videos",
                            action = "Edit",
                            video.Id
                        });
                    return li;
                },  
                (s) => this.Url.Link(
                    "default",
                    new
                    {
                        controller = "Admin",
                        action = "VideoList",
                        start = s
                    }));
            
            return this.View(model);
        }
        
        public IActionResult FlaggedVideoList(int start)
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
            
            FlaggedVideoListViewModel model = new FlaggedVideoListViewModel();
            ViewModelHelper.PopulateUserInfo(model, loggedIn);
            ViewModelHelper.PopulateSearchResults(
                model,
                this.dataAccess.GetFlaggedVideos(),
                0,
                30,
                f => new FlaggedVideoListItemViewModel()
                {
                    Title = f.Title,
                    ReviewUrl = this.Url.Link(
                        "default",
                        new { controller = "Videos", action = "ReviewFlag", id = f.FlagId })                    
                },
                s => this.Url.Link(
                    "default",
                    new { controller = "Admin", action = "FlaggedVideoList", start = s }));
            
            return View(model);
        }
    }
}