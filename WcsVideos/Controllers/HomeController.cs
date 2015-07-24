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
        
        public HomeController(IDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
        }
        
        public IActionResult Index()
        {
            IndexViewModel model = new IndexViewModel();
            var videos = this.dataAccess.GetTrendingVideos();
            model.Videos = videos.Select(
                x => new VideoListItemViewModel
                {
                    Url = this.Url.Link("default", new { controller = "Videos", action = "Watch", id = x.Id }),
                    Title = x.Title,
                    ThumbnailUrl = string.Format("http://img.youtube.com/vi/{0}/default.jpg", x.ProviderVideoId)
                }).ToList(); 
            
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
                    if (++eventCount >= 10)
                    {
                        break;
                    }
                }
            }
            
            model.Events = modelEvents;
            
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
            model.Title = dancer.Name;
            model.Videos = new List<VideoListItemViewModel>();
            
            foreach (string videoId in dancer.VideoIdList)
            {
                Video video = this.dataAccess.GetVideoById(videoId);
                if (video != null)
                {
                    VideoListItemViewModel listItemModel = new VideoListItemViewModel();
                    listItemModel.Title = video.Title;
                    listItemModel.Url = this.Url.Link(
                        "default",
                        new { controller = "Videos", action = "Watch", id = video.Id });
                    listItemModel.ThumbnailUrl = string.Format(
                        "http://img.youtube.com/vi/{0}/default.jpg",
                        video.ProviderVideoId);
                    model.Videos.Add(listItemModel);
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