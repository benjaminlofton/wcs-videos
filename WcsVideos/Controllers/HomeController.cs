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
                    Url = this.Url.Link("default", new { controller = "Home", action = "Watch", id = x.Id }),
                    Title = x.Title
                }).ToList(); 
            
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

        public IActionResult Watch(string id)
        {
            Video video = this.dataAccess.GetVideoById(id);
            
            if (video == null)
            {
                return this.HttpNotFound();    
            }

            WatchViewModel model = new WatchViewModel();
            model.ExternalUrl = string.Format("https://www.youtube.com/watch?v={0}", video.ProviderVideoId);
            model.EmbedUrl = string.Format("http://www.youtube.com/embed/{0}", video.ProviderVideoId);
            model.ProviderName = "YouTube";
            model.Title = video.Title;
            model.ProviderVideoId = video.ProviderVideoId;
            model.Dancers = new List<DancerLinkViewModel>();
            
            foreach (string dancerId in video.DancerIdList)
            {
                Dancer dancer = this.dataAccess.GetDancerById(dancerId);
                if (dancer != null)
                {
                    DancerLinkViewModel dancerModel = new DancerLinkViewModel();
                    dancerModel.DisplayName = dancer.Name;
                    dancerModel.Url = this.Url.Link("default", new { controller = "Home", action = "Dancer", id = dancer.WsdcId });
                    model.Dancers.Add(dancerModel);
                }    
            }
            
            return View(model);
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
                    listItemModel.Url = this.Url.Link("default", new { controller = "Home", action = "Watch", id = video.Id });
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