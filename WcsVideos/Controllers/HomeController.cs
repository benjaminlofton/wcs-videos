using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using WcsVideos.Models;

namespace WcsVideos.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            IndexViewModel model = new IndexViewModel();
            model.Videos = new List<VideoListItemViewModel>();
            model.Videos.Add(new VideoListItemViewModel { Url = "#", Title = "Awesome Video 1" });
            model.Videos.Add(new VideoListItemViewModel { Url = "#", Title = "Awesome Video 2" });
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

        public IActionResult Error()
        {
            return View("~/Views/Shared/Error.cshtml");
        }
    }
}