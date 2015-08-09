using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Mvc;
using WcsVideos.Models;
using WcsVideos.Contracts;

namespace WcsVideos.Controllers
{
    public class EventsController : Controller
    {
        private IDataAccess dataAccess;
        
        public EventsController(IDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
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
            
            model.Title = contractEvent.Name;
            
            var videos = this.dataAccess.GetEventVideos(id);
            model.Videos = videos.Select(
                x => new VideoListItemViewModel
                {
                    Url = this.Url.Link("default", new { controller = "Videos", action = "Watch", id = x.Id }),
                    Title = x.Title,
                    ThumbnailUrl = string.Format("http://img.youtube.com/vi/{0}/default.jpg", x.ProviderVideoId)
                }).ToList(); 
            
            return this.View(model);
        }

        public IActionResult Error()
        {
            return View("~/Views/Shared/Error.cshtml");
        }
    }
}