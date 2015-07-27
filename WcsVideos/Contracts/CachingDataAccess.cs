using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;

namespace WcsVideos.Contracts
{
    public class CachingDataAccess : IDataAccess
    {
    	private ConcurrentDictionary<string, Dancer> dancers;
        private ConcurrentDictionary<string, Video> videos;
        private ConcurrentDictionary<string, List<Video>> eventVideos;
        private ConcurrentDictionary<string, Event> events;
        private List<Dancer> allDancers;
        private List<Video> trendingVideos;
        private List<Event> recentEvents;
        private IDataAccess baseDataAccess;
        private ManualResetEvent allDancersLoaded;
        
		public CachingDataAccess()
		{
			this.dancers = new ConcurrentDictionary<string, Dancer>();
            this.videos = new ConcurrentDictionary<string, Video>();
            this.events = new ConcurrentDictionary<string, Event>();
            this.eventVideos = new ConcurrentDictionary<string, List<Video>>();
            this.baseDataAccess = new DataAccess();
            this.allDancersLoaded = new ManualResetEvent(false);
            Thread loadDancersThread = new Thread(() =>
                {
                    try
                    {
                        this.allDancers = this.baseDataAccess.GetAllDancers();
                    }
                    catch
                    {
                    }
                    
                    this.allDancersLoaded.Set();
                    
                    foreach (Dancer dancer in this.allDancers)
                    {
                        this.dancers[dancer.WsdcId] = dancer;
                    }
                });
            loadDancersThread.Start();
		}
        
        public Event GetEvent(string eventId)
        {
            Event contractEvent;
            if (!this.events.TryGetValue(eventId, out contractEvent))
            {
                contractEvent = this.baseDataAccess.GetEvent(eventId);
                this.events[eventId] = contractEvent;
            }
            
            return contractEvent;
        }
        
        public List<Video> GetEventVideos(string eventId)
        {
            List<Video> videos;
            if (!this.eventVideos.TryGetValue(eventId, out videos))
            {
                videos = this.baseDataAccess.GetEventVideos(eventId);
                this.eventVideos[eventId] = videos;
                
                foreach (Video video in videos)
                {
                    this.videos[video.Id] = video;
                }
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
                
                foreach (Event contractEvent in this.recentEvents)
                {
                    this.events[contractEvent.EventId] = contractEvent;
                }
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
            if (this.allDancersLoaded.WaitOne(TimeSpan.FromSeconds(30)))
            {            
                return this.allDancers;
            }
            else
            {
                return new List<Dancer>();
            }
        }
        
        public string AddVideo(Video video)
        {
            // TODO: clear appropriate caches
            string videoId = this.baseDataAccess.AddVideo(video);
            this.videos[videoId] = video;
            return videoId;
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