using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace WcsVideos.Contracts
{
    public class CachingDataAccess : IDataAccess
    {
    	private ConcurrentDictionary<string, Dancer> dancers;
        private ConcurrentDictionary<string, Video> videos;
        private ConcurrentDictionary<string, List<Video>> eventVideos;
        private List<Dancer> allDancers;
        private List<Video> trendingVideos;
        private List<Event> recentEvents;
        private IDataAccess baseDataAccess;
        
		public CachingDataAccess()
		{
			this.dancers = new ConcurrentDictionary<string, Dancer>();
            this.videos = new ConcurrentDictionary<string, Video>();
            this.eventVideos = new ConcurrentDictionary<string, List<Video>>();
            this.baseDataAccess = new DataAccess();
		}
        
        public Event GetEvent(string eventId)
        {
            return this.baseDataAccess.GetEvent(eventId);
        }
        
        public List<Video> GetEventVideos(string eventId)
        {
            List<Video> videos;
            if (!this.eventVideos.TryGetValue(eventId, out videos))
            {
                videos = this.baseDataAccess.GetEventVideos(eventId);
                this.eventVideos[eventId] = videos;
            }
            
            return videos;
        }
        
        public List<Event> SearchForEvent(string query)
        {
            return this.baseDataAccess.SearchForEvent(query);
        }
        
        public List<Event> GetRecentEvents()
        {
            if (this.recentEvents == null)
            {
                this.recentEvents = this.baseDataAccess.GetRecentEvents();
            }
            
            return this.recentEvents;
        }
        
        public List<Video> GetTrendingVideos()
        {
            if (this.trendingVideos == null)
            {
                this.trendingVideos = this.baseDataAccess.GetTrendingVideos();
                
                foreach (Video video in this.trendingVideos)
                {
                    this.videos[video.Id] = video;
                }
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
        
        public List<Video> SearchForVideo(
            IEnumerable<string> titleFragments,
            IEnumerable<string> dancerIds,
            IEnumerable<string> eventIds)
        {
            return this.baseDataAccess.SearchForVideo(titleFragments, dancerIds, eventIds);
        }
	}
}