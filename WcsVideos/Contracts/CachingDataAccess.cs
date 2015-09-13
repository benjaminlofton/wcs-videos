using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WcsVideos.Contracts
{
    public class CachingDataAccess : IDataAccess
    {
        private static readonly List<Video> EmptyVideoList = new List<Video>();
        private Uri WebserviceTarget { get; set; }
    	private ConcurrentDictionary<string, Dancer> dancers;
        private ConcurrentDictionary<string, Video> videos;
        private ConcurrentDictionary<string, List<string>> eventVideos;
        private ConcurrentDictionary<string, Event> events;
        private ConcurrentDictionary<string, ResourceList> resourceLists;
        private List<Dancer> allDancers;
        private List<Video> trendingVideos;
        private List<Event> recentEvents;
        private IDataAccess baseDataAccess;
        private ManualResetEvent allDancersLoaded;
        
		public CachingDataAccess(string endpoint)
		{
			this.dancers = new ConcurrentDictionary<string, Dancer>();
            this.videos = new ConcurrentDictionary<string, Video>();
            this.events = new ConcurrentDictionary<string, Event>();
            this.eventVideos = new ConcurrentDictionary<string, List<string>>();
            this.resourceLists = new ConcurrentDictionary<string, ResourceList>();
            this.baseDataAccess = new DataAccess(endpoint);
            this.allDancersLoaded = new ManualResetEvent(false);
            this.WebserviceTarget = new Uri(endpoint);
            Thread loadDancersThread = new Thread(() =>
                {
                    try
                    {
                        this.allDancers = this.baseDataAccess.GetAllDancers();
                        this.allDancersLoaded.Set();
                        
                        foreach (Dancer dancer in this.allDancers)
                        {
                            this.dancers[dancer.WsdcId] = dancer;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error loading dancers: " + ex);
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
            List<string> videoIds;
            List<Video> videos;
            if (!this.eventVideos.TryGetValue(eventId, out videoIds))
            {
                videos = this.baseDataAccess.GetEventVideos(eventId);
                this.eventVideos[eventId] = videos.Select(v => v.Id).ToList();
                
                foreach (Video video in videos)
                {
                    this.videos[video.Id] = video;
                }
            }
            else
            {
                videos = new List<Video>();
                
                foreach (string videoId in videoIds)
                {
                    Video video = this.GetVideoById(videoId);
                    if (video != null)
                    {
                        videos.Add(video);
                    }
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
            string videoId = this.baseDataAccess.AddVideo(video);
            this.videos[videoId] = video;
            
            if (video.DancerIdList != null)
            {
                foreach (string dancerId in video.DancerIdList)
                {
                    // remove exisitng dancer
                    Dancer dancer;
                    this.dancers.TryRemove(dancerId, out dancer);
                    
                    // get updated dancer
                    dancer = this.GetDancerById(dancerId);
                    
                    // replace the updated dancer in the dictionary and allDancers array
                    if (dancer != null)
                    {
                        this.dancers[dancer.WsdcId] = dancer;
                        int index = this.allDancers.FindIndex(d => string.Equals(d.WsdcId, dancer.WsdcId));
                        if (index > -1)
                        {
                            this.allDancers[index] = dancer;
                        }
                    }
                    
                    ResourceList removed;
                    this.resourceLists.TryRemove("latest-videos", out removed);
                }
            }
            
            if (video.EventId != null)
            {
                // remove the event video mapping
                List<string> eventVideos;
                this.eventVideos.TryRemove(video.EventId, out eventVideos);
                
                // let the mapping be reloaded automatically when it is requested next
            }
            
            this.HttpPost("admin/cache-reset").Wait();
            
            return videoId;
        }
        
        public List<Video> SearchForVideo(
            IEnumerable<string> titleFragments,
            IEnumerable<string> dancerIds,
            IEnumerable<string> eventIds)
        {
            return this.baseDataAccess.SearchForVideo(titleFragments, dancerIds, eventIds);
        }
        
        public bool ProviderVideoIdExists(string providerId, string providerVideoId)
        {
            return this.baseDataAccess.ProviderVideoIdExists(providerId, providerVideoId);
        }
        
        private async Task HttpPost(string relativeUrl)
        {
            Console.WriteLine("POST to " + relativeUrl);
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = this.WebserviceTarget;
                HttpResponseMessage response = await client.PostAsync(
                    relativeUrl,
                    new StringContent(string.Empty, Encoding.UTF8, "application/json"));
                string serialized = await response.Content.ReadAsStringAsync();
                response.EnsureSuccessStatusCode();
            }
        }
        
        public ResourceList GetResourceList(string name)
        {
            ResourceList list;
            if (!this.resourceLists.TryGetValue(name, out list))
            {
                list = this.baseDataAccess.GetResourceList(name);
                if (list != null)
                {
                    this.resourceLists[name] = list;
                }
            }
            
            return list;
        }
	}
}