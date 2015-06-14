using System;
using System.Net.Http;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace WcsVideos.Contracts
{
    public class DataAccess : IDataAccess
    {
        private static readonly Uri WebserviceTarget = new Uri("http://10.0.0.10:8085/");
        
        public List<Video> GetTrendingVideos()
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = DataAccess.WebserviceTarget;
                HttpResponseMessage response = client.GetAsync("v/?wsdc-id=1").Result;
                string serialized = response.Content.ReadAsStringAsync().Result;
                Video[] videos = (Video[])JsonConvert.DeserializeObject<Video[]>(
                    serialized,
                    new JsonSerializerSettings { ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver() });  
                return new List<Video>(videos);   
            }
        }
		public Video GetVideoById(string id)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = DataAccess.WebserviceTarget;
                HttpResponseMessage response = client.GetAsync("v/" + id).Result;
                string serialized = response.Content.ReadAsStringAsync().Result;
                Video video = JsonConvert.DeserializeObject<Video>(
                    serialized,
                    new JsonSerializerSettings { ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver() });  
                return video;
            }
        }
        
        public Dancer GetDancerById(string id)
        {
            throw new NotImplementedException();
        }
    }
}