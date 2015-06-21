using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WcsVideos.Contracts
{
    public class DataAccess : IDataAccess
    {
        private static readonly Uri WebserviceTarget = new Uri("http://localhost:8085/");
        
        public List<Video> GetTrendingVideos()
        {
            List<Video> videos = new List<Video>();
            Dancer dancer = this.HttpGet<Dancer>("d/9068").Result;
            foreach (string videoId in dancer.VideoIdList)
            {
                videos.Add(this.GetVideoById(videoId));
            }
            
            return videos;
        }
        
		public Video GetVideoById(string id)
        {
            return this.HttpGet<Video>("v/" + id).Result;
        }
        
        public Dancer GetDancerById(string id)
        {
            return this.HttpGet<Dancer>("d/" + id).Result;
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
    }
}