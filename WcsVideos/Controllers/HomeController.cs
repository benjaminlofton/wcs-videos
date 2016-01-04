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
            bool loggedIn = this.userSessionHandler.GetUserLoginState(
                this.Context.Request.Cookies, this.Context.Response.Cookies);
            IndexViewModel model = new IndexViewModel();
            
            ViewModelHelper.PopulateUserInfo(model, loggedIn);

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
            List<Tuple<DateTime, Video>> videos;
            Dictionary<DateTime, VideoGroupViewModel> groups;
            
            videos = new List<Tuple<DateTime, Video>>();
            groups = new Dictionary<DateTime, VideoGroupViewModel>();
            
            if (dancer.VideoIdList != null)
            {
                foreach (string videoId in dancer.VideoIdList)
                {
                    Video video = this.dataAccess.GetVideoById(videoId);
                    if (video != null)
                    {
                        Event evt  = null;
                        if (!string.IsNullOrEmpty(video.EventId))
                        {
                            evt = this.dataAccess.GetEvent(video.EventId);
                        }
                        
                        DateTime rawDate = evt == null ? DateTime.MinValue : evt.EventDate;
                        videos.Add(Tuple.Create(rawDate, video));
                    }  
                }
                
                foreach (Tuple<DateTime, Video> entry in videos)
                {
                    DateTime date = new DateTime(
                        entry.Item1.Date.Year,
                        entry.Item1.Date.Month,
                        1,
                        0,
                        0,
                        0,
                        DateTimeKind.Utc);
                    
                    VideoGroupViewModel groupView;
                    if (!groups.TryGetValue(date, out groupView))
                    {
                        groupView = new VideoGroupViewModel
                        {
                            Name = date > DateTime.MinValue ?
                                "Videos from " + date.ToString("MMMM yyyy") :
                                "Undated Videos",
                            Videos = new List<VideoListItemViewModel>(),
                        };
                        
                        groups[date] = groupView;
                    }
                                            
                    VideoListItemViewModel listItemModel = ViewModelHelper.PopulateVideoListItem(entry.Item2, this.Url);
                    groupView.Videos.Add(listItemModel);
                }
            }
           
            model.VideoGroups = groups.OrderByDescending(e => e.Key).Select(e => e.Value).ToList();
            return View(model);
        }
        
        public IActionResult Error()
        {
            return View("~/Views/Shared/Error.cshtml");
        }
    }
}