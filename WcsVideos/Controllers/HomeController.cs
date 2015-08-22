using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using WcsVideos.Models;
using WcsVideos.Contracts;

namespace WcsVideos.Controllers
{
    public class HomeController : Controller
    {
        private IDataAccess dataAccess;
        private IUserSessionHandler userSessionHandler;
        
        public HomeController(IDataAccess dataAccess, IUserSessionHandler userSessionHandler)
        {
            this.dataAccess = dataAccess;
            this.userSessionHandler = userSessionHandler;
        }
        
        public IActionResult Index()
        {
            IndexViewModel model = new IndexViewModel();
            ViewModelHelper.PopulateUserInfo(
                model,
                this.userSessionHandler.GetUserLoginState(this.Context.Request.Cookies, this.Context.Response.Cookies));

            var events = this.dataAccess.GetRecentEvents();
            List<EventListItemViewModel> modelEvents = new List<EventListItemViewModel>(events.Count);
            int eventCount = 0;
            
            foreach (Event contractEvent in events)
            {
                int videoCount = this.dataAccess.GetEventVideos(contractEvent.EventId).Count;
                if (videoCount > 0)
                {
                    EventListItemViewModel modelEvent = new EventListItemViewModel();
                    modelEvent.Url = this.Url.Link(
                        "default",
                        new { controller = "Events", action = "Event", id = contractEvent.EventId});
                    modelEvent.Name = contractEvent.Name;
                    modelEvent.VideoCount = videoCount;
                    modelEvent.ShowVideoCount = true;
                    modelEvents.Add(modelEvent);
                    if (++eventCount >= 20)
                    {
                        break;
                    }
                }
            }
            
            model.Events = modelEvents;

            // Retrieve the recent videos after retrieveing the event list since most of these will likely be
            // cached from the the call to get the recent events
            model.Videos = new List<VideoListItemViewModel>();
            ResourceList recentVideoList = this.dataAccess.GetResourceList("latest-videos");
            int count = 0;
            if (recentVideoList != null && recentVideoList.Ids != null)
            {
                foreach (string videoId in recentVideoList.Ids)
                {
                    Video video = this.dataAccess.GetVideoById(videoId);
                    if (video != null)
                    {
                        VideoListItemViewModel listItem = ViewModelHelper.PopulateVideoListItem(video, this.Url);
                        model.Videos.Add(listItem);
                        
                        if (++count > 5)
                        {
                            break;
                        }
                    }
                }
            }
            
            return this.View(model);
        }

        public IActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        
        public IActionResult Dancer(string id)
        {
            Dancer dancer = this.dataAccess.GetDancerById(id);
            
            if (dancer == null)
            {
                return this.HttpNotFound();    
            }
            
            DancerViewModel model = new DancerViewModel();
            ViewModelHelper.PopulateUserInfo(
                model,
                this.userSessionHandler.GetUserLoginState(this.Context.Request.Cookies, this.Context.Response.Cookies));

            model.Title = dancer.Name;
            model.Videos = new List<VideoListItemViewModel>();
            
            if (dancer.VideoIdList != null)
            {
                foreach (string videoId in dancer.VideoIdList)
                {
                    Video video = this.dataAccess.GetVideoById(videoId);
                    if (video != null)
                    {
                        VideoListItemViewModel listItemModel = ViewModelHelper.PopulateVideoListItem(video, this.Url);
                        model.Videos.Add(listItemModel);
                    }  
                }
            }
            
            return View(model);
        }
        
        public IActionResult Error()
        {
            return View("~/Views/Shared/Error.cshtml");
        }
    }
}