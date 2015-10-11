using System;
using System.Linq;
using System.Net.Http;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WcsVideos.Contracts
{
    public class DataAccess : IDataAccess
    {        
        private Uri WebserviceTarget { get; set; }
        
        public DataAccess(string endpoint)
        {
            this.WebserviceTarget = new Uri(endpoint);
        }
        
        public Event GetEvent(string eventId)
        {
            return this.HttpGet<Event>("event/" + eventId).Result;
        }
        
        public List<Video> GetEventVideos(string eventId)
        {
            List<Video> videos = this.HttpGet<List<Video>>("v?event-id=" + eventId).Result;
            if (videos == null)
            {
                videos = new List<Video>(0);
            }
            
            return videos;
        }
        
        public List<Event> SearchForEvent(string query)
        {
            string[] words = query.Split(
                new char[] { ' ' },
                StringSplitOptions.RemoveEmptyEntries);
                
            List<Event> events = this.HttpGet<List<Event>>(
                "event/?name-frag=" + Uri.EscapeUriString(string.Join(",", words))).Result;
            if (events == null)
            {
                events = new List<Event>();
            }
            
            return events;
        }
        
        public List<Event> GetRecentEvents()
        {
            List<Event> events;
            string afterDate = DateTime.UtcNow.Subtract(TimeSpan.FromDays(120)).ToString("yyyy-MM-dd");
            string beforeDate = DateTime.UtcNow.ToString("yyyy-MM-dd");
            events = this.HttpGet<List<Event>>(
                "event/?after-date=" + afterDate + "&before-date=" + beforeDate).Result
                .OrderByDescending(e => e.EventDate)
                .ToList();
            return events;
        }
        
        public List<Video> GetTrendingVideos()
        {
            List<Video> videos = new List<Video>();
            Dancer dancer = this.GetDancerById("9068");
            if (dancer != null)
            {
                if (dancer.VideoIdList != null)
                {
                    foreach (string videoId in dancer.VideoIdList)
                    {
                        videos.Add(this.GetVideoById(videoId));
                    }
                }
            }
            
            return videos;
        }
        
		public Video GetVideoById(string id)
        {
            return this.HttpGet<Video>("v/" + Uri.EscapeUriString(id)).Result;
        }
        
        public Dancer GetDancerById(string id)
        {
            return this.HttpGet<Dancer>("d/" + Uri.EscapeUriString(id)).Result;
        }

        public List<Dancer> SearchForDancer(string query)
        {
            throw new NotImplementedException();
        }
        
        public Dancer[] GetAllDancers()
        {
            return this.HttpGet<Dancer[]>("d/").Result;
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
            video.Id = result.Id;
            return result.Id;
        }
        
        public void UpdateVideo(Video video)
        {
            string serialized = JsonConvert.SerializeObject(
                video,
                new JsonSerializerSettings
                {
                    ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
                });
                
            this.HttpPost("v/", serialized).Wait();
        }
        
        public List<Video> SearchForVideo(
            IEnumerable<string> titleFragments,
            IEnumerable<string> dancerIds,
            IEnumerable<string> eventIds)
        {
            List<string> queryFragments = new List<string>(4);
            if (titleFragments != null && titleFragments.Any())
            {
                queryFragments.Add("title-frag=" + Uri.EscapeUriString(string.Join(",", titleFragments)));
            }
            
            if (dancerIds != null && dancerIds.Any())
            {
                queryFragments.Add("wsdc-id=" + Uri.EscapeUriString(string.Join(",", dancerIds)));
            }
            
            if (eventIds != null && eventIds.Any())
            {
                queryFragments.Add("event-id=" + Uri.EscapeUriString(string.Join(",", eventIds)));
            }
            
            if (queryFragments.Count > 0)
            {
                string url = string.Format("v?" + string.Join("&", queryFragments));
                return this.HttpGet<List<Video>>(url).Result;
            }
            else
            {
                return new List<Video>();
            }
        }

        public bool ProviderVideoIdExists(string providerId, string providerVideoId)
        {
            List<Video> videos = this.HttpGet<List<Video>>(
                "v?provider-id=" + Uri.EscapeUriString(providerVideoId)).Result;
            return videos.Count > 0;
        }

        public ResourceList GetResourceList(string name)
        {
            return this.HttpGet<ResourceList>("list/" + Uri.EscapeUriString(name)).Result;
        }

        public string AddFlaggedVideo(FlaggedVideo flaggedVideo)
        {
            string serialized = JsonConvert.SerializeObject(
                flaggedVideo,
                new JsonSerializerSettings
                {
                    ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
                });
            
            FlaggedVideo result = this.HttpPost<FlaggedVideo>("flagged/v", serialized).Result;
            return result.FlagId;
        }
        
        public List<FlaggedVideo> GetFlaggedVideos()
        {
            return this.HttpGet<List<FlaggedVideo>>("flagged/v").Result;
        }

        public FlaggedVideo GetFlaggedVideo(string flagId)
        {
            return this.HttpGet<FlaggedVideo>("flagged/v/" + Uri.EscapeUriString(flagId)).Result;
        }

        public void DeleteFlaggedVideo(string flagId)
        {
            this.HttpDelete("flagged/v/" + Uri.EscapeUriString(flagId)).Wait();
        }

        private async Task<T> HttpGet<T>(string relativeUrl)
        {
            Console.WriteLine("GET from " + relativeUrl);
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = this.WebserviceTarget;
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
            Console.WriteLine("POST to " + relativeUrl);
            Console.WriteLine(content);
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = this.WebserviceTarget;
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
        
        private async Task HttpPost(string relativeUrl, string content)
        {
            Console.WriteLine("POST to " + relativeUrl);
            Console.WriteLine(content);
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = this.WebserviceTarget;
                HttpResponseMessage response = await client.PostAsync(
                    relativeUrl,
                    new StringContent(content, Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode();
            }
        }
        
        private async Task HttpDelete(string relativeUrl)
        {
            Console.WriteLine("DELETE to " + relativeUrl);
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = this.WebserviceTarget;
                HttpResponseMessage response = await client.DeleteAsync(relativeUrl);
                response.EnsureSuccessStatusCode();
            }
        }
    }
}