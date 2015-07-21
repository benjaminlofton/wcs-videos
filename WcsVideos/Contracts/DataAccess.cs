using System;
using System.Linq;
using System.Net.Http;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Diagnostics;

namespace WcsVideos.Contracts
{
    public class DataAccess : IDataAccess
    {
        private static readonly Uri WebserviceTarget = new Uri("http://localhost:8085/");
        
        public List<Event> GetRecentEvents()
        {
            List<Event> events;
            string afterDate = DateTime.UtcNow.Subtract(TimeSpan.FromDays(30)).ToString("yyyy-MM-dd");
            string beforeDate = DateTime.UtcNow.ToString("yyyy-MM-dd");
            events = this.HttpGet<List<Event>>(
                "event/?after-date=" + afterDate + "&before-date=" + beforeDate).Result
                .OrderByDescending(e => e.EventDate)
                .Take(5)
                .ToList();
            return events;
        }
        
        public List<Video> GetTrendingVideos()
        {
            List<Video> videos = new List<Video>();
            Dancer dancer = GetDancerById("9068");
            if (dancer != null)
            {
                foreach (string videoId in dancer.VideoIdList)
                {
                    videos.Add(this.GetVideoById(videoId));
                }
            }
            
            return videos;
        }
        
		public Video GetVideoById(string id)
        {
            Console.WriteLine("Requesting video for id=" + id);
            return this.HttpGet<Video>("v/" + id).Result;
        }
        
        public Dancer GetDancerById(string id)
        {
            Console.WriteLine("Requesting dancer for id=" + id);
            return this.HttpGet<Dancer>("d/" + id).Result;
        }

        public List<Dancer> SearchForDancer(string query)
        {
            throw new NotImplementedException();
        }
        
        public List<Dancer> GetAllDancers()
        {
            Console.WriteLine("Requesting all dancers");
            return this.HttpGet<List<Dancer>>("d/").Result;
        }
        
        public string AddVideo(Video video)
        {
            string serialized = JsonConvert.SerializeObject(
                video,
                new JsonSerializerSettings
                {
                    ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
                });
                
            Video result = this.HttpPost<Video>("v/", serialized).Result;
            return result.Id;
        }

        private async Task<T> HttpGet<T>(string relativeUrl)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = DataAccess.WebserviceTarget;
                HttpResponseMessage response = await client.GetAsync(relativeUrl);
                string serialized = await response.Content.ReadAsStringAsync();
                T result = JsonConvert.DeserializeObject<T>(
                    serialized,
                    new JsonSerializerSettings { ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver() });  
                return result;
            }
        }
        
        private async Task<T> HttpPost<T>(string relativeUrl, string content)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = DataAccess.WebserviceTarget;
                HttpResponseMessage response = await client.PostAsync(
                    relativeUrl,
                    new StringContent(content, Encoding.UTF8, "application/json"));
                string serialized = await response.Content.ReadAsStringAsync();
                response.EnsureSuccessStatusCode();
                T result = JsonConvert.DeserializeObject<T>(
                    serialized,
                    new JsonSerializerSettings { ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver() });  
                return result;
            }
        }
    }
}