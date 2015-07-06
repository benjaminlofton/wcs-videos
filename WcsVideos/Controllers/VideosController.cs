using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using WcsVideos.Models;
using WcsVideos.Contracts;
using Microsoft.AspNet.Http;

namespace WcsVideos.Controllers
{
    public class VideosController : Controller
    {
        private IDataAccess dataAccess;
        
        public VideosController(IDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
        }
        
        public IActionResult Add()
        {
            VideosAddViewModel model = new VideosAddViewModel();
            model.ProviderId = this.Context.Request.Cookies.Get("ProviderId");
            model.ProviderVideoIdValidationError = !bool.Parse(this.Context.Request.Cookies.Get("ProviderIdValid") ?? "True");
            model.ProviderVideoId = this.Context.Request.Cookies.Get("ProviderVideoId");
            model.ProviderVideoIdValidationError = !bool.Parse(this.Context.Request.Cookies.Get("ProviderVideoIdValid") ?? "True");
            model.Title = this.Context.Request.Cookies.Get("Title");
            model.TitleValidationError = !bool.Parse(this.Context.Request.Cookies.Get("TitleValid") ?? "True");
            model.DancerIdList = this.Context.Request.Cookies.Get("DancerIdList");
            model.DancerIdListValidationError = !bool.Parse(this.Context.Request.Cookies.Get("DancerIdListValid") ?? "True");
            
            CookieOptions cookieOptions = new CookieOptions();
            this.Context.Response.Cookies.Delete("ProviderId", cookieOptions);
            this.Context.Response.Cookies.Delete("ProviderIdValid", cookieOptions);
            this.Context.Response.Cookies.Delete("ProviderVideoId", cookieOptions);
            this.Context.Response.Cookies.Delete("ProviderVideoIdValid", cookieOptions);
            this.Context.Response.Cookies.Delete("Title", cookieOptions);
            this.Context.Response.Cookies.Delete("TitleValid", cookieOptions);
            this.Context.Response.Cookies.Delete("DancerIdList", cookieOptions);
            this.Context.Response.Cookies.Delete("DancerIdListValid", cookieOptions);

            return this.View(model);
        }
        
        public IActionResult SubmitAdd(
            string providerId,
            string providerVideoId,
            string title,
            string dancerIdList)
        {
            bool providerIdValid = true;
            bool providerVideoIdValid = true;
            bool titleValid = true;
            bool dancerIdListValid = true;
            if (string.IsNullOrEmpty(providerId) ||
                !string.Equals(providerId, "youtube", StringComparison.Ordinal))
            {
                providerIdValid = false;
            }
            
            if (string.IsNullOrEmpty(providerVideoId))
            {
                providerVideoIdValid = false;
            }
            
            if (string.IsNullOrEmpty(title))
            {
                titleValid = false;
            }
            
            string[] dancerIds = null;
            if (string.IsNullOrEmpty(dancerIdList))
            {
                dancerIdListValid = false;
            }
            else
            {
                dancerIds = dancerIdList.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string dancerId in dancerIds)
                {
                    if (this.dataAccess.GetDancerById(dancerId) == null)
                    {
                        dancerIdListValid = false;
                    }
                }
            }
            
            if (providerIdValid && providerVideoIdValid && titleValid && dancerIdListValid)
            {
                Video video = new Video();
                video.ProviderId = providerId;
                video.ProviderVideoId = providerVideoId;
                video.Title = title;
                video.DancerIdList = dancerIds;
                string videoId = this.dataAccess.AddVideo(video);
                
                return this.RedirectToAction(
                    "AddSuccess",
                    new { id = videoId });
            }
            else
            {
                CookieOptions cookieOptions = new CookieOptions();
                cookieOptions.Expires = DateTime.UtcNow.AddDays(1);
                this.Context.Response.Cookies.Append("ProviderId", providerId, cookieOptions);
                this.Context.Response.Cookies.Append("ProviderIdValid", providerIdValid.ToString(), cookieOptions);
                this.Context.Response.Cookies.Append("ProviderVideoId", providerVideoId, cookieOptions);
                this.Context.Response.Cookies.Append("ProviderVideoIdValid", providerVideoIdValid.ToString(), cookieOptions);
                this.Context.Response.Cookies.Append("Title", title, cookieOptions);
                this.Context.Response.Cookies.Append("TitleValid", titleValid.ToString(), cookieOptions);
                this.Context.Response.Cookies.Append("DancerIdList", dancerIdList, cookieOptions);
                this.Context.Response.Cookies.Append("DancerIdListValid", dancerIdListValid.ToString(), cookieOptions);
                
                return this.RedirectToAction(
                    "Add");
            }
        }
        
        public IActionResult AddSuccess(string id)
        {
            VideosAddSuccessViewModel model = new VideosAddSuccessViewModel();
            
            Video video = this.dataAccess.GetVideoById(id);
            model.ProviderId = video.ProviderId;
            model.ProviderVideoId = video.ProviderVideoId;
            model.Title = video.Title;
            model.DancerIdList = string.Join(";", video.DancerIdList);
            model.VideoUrl = this.Url.Link(
                "default",
                new { controller = "Home", action = "Watch", id = id });
            model.AddVideoUrl = this.Url.Link(
                "default",
                new { controller = "Videos", action = "Add" });

            return this.View(model);
        }
        
        public IActionResult Error()
        {
            return View("~/Views/Shared/Error.cshtml");
        }
    }
}