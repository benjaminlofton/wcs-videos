using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WcsVideos.Contracts
{
    public class CachingDataAccess : IDataAccess
    {
    	private ConcurrentDictionary<string, Dancer> dancers;
        private ConcurrentDictionary<string, Video> videos;
        private List<Dancer> allDancers;
        private List<Video> trendingVideos;
        private IDataAccess baseDataAccess;
        
		public CachingDataAccess()
		{
			this.dancers = new ConcurrentDictionary<string, Dancer>();
            this.videos = new ConcurrentDictionary<string, Video>();
            this.baseDataAccess = new DataAccess();
		}
        
        public List<Video> GetTrendingVideos()
        {
            if (this.trendingVideos == null)
            {
                this.trendingVideos = this.baseDataAccess.GetTrendingVideos();
            }
            
            return this.trendingVideos;;
        }

		public Video GetVideoById(string id)
        {
            Video video;
            if (!this.videos.TryGetValue(id, out video))
            {
                video = this.baseDataAccess.GetVideoById(id);
                if (video != null)
                {
                    this.videos[id] = video;
                }
            }
            
            return video;
        }

		public Dancer GetDancerById(string id)
        {
            Dancer dancer;
            if (!this.dancers.TryGetValue(id, out dancer))
            {
                dancer = this.baseDataAccess.GetDancerById(id);
                if (dancer != null)
                {
                    this.dancers[id] = dancer;
                }
            }
            
            return dancer;
        }
        
        public List<Dancer> SearchForDancer(string query)
        {
            List<Dancer> result = new List<Dancer>();
            
            if (string.IsNullOrEmpty(query))
            {
                return result;    
            }
            
            IEnumerable<Dancer> all = this.GetAllDancers();
            
            string[] segments = query.Split(new[] { ' ' }, 5);
            
            foreach (Dancer dancer in all)
            {
                bool matches = true;
                
                foreach (string segment in segments)
                {
                    if ((dancer.Name.IndexOf(segment, StringComparison.OrdinalIgnoreCase) == -1) &&
                        !dancer.WsdcId.Equals(segment))
                    {
                        matches = false;
                        break;
                    }                    
                }
                
                if (matches)
                {
                    result.Add(dancer);
                }
            }
            
            return result;
        }
        
        public List<Dancer> GetAllDancers()
        {
            if (this.allDancers == null)
            {
                this.allDancers = this.baseDataAccess.GetAllDancers();
            }
            
            return this.allDancers;
        }
        
        public string AddVideo(Video video)
        {
            return this.baseDataAccess.AddVideo(video);
        }
	}
}