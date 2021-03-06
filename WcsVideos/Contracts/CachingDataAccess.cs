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
        private Dancer[] allDancers;
        private List<Video> trendingVideos;
        private List<Event> recentEvents;
        private IDataAccess baseDataAccess;
        private ManualResetEvent allDancersLoaded;
        private ReaderWriterLockSlim allDancersLock;
        
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
        
        public Dancer[] GetAllDancers()
        {
            if (this.allDancersLoaded.WaitOne(TimeSpan.FromSeconds(30)))
            {            
                return this.allDancers;
            }
            else
            {
                return new Dancer[0];
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
                    this.UpdateDancerInCache(dancerId);                    
                }
            }
            
            if (video.EventId != null)
            {
                // remove the event video mapping
                List<string> eventVideos;
                this.eventVideos.TryRemove(video.EventId, out eventVideos);
                
                // let the mapping be reloaded automatically when it is requested next
            }

            ResourceList removed;
            this.resourceLists.TryRemove("latest-videos", out removed);
            
            this.HttpPost("admin/cache-reset").Wait();
            
            return videoId;
        }
        
        public void UpdateVideo(Video video)
        {
            Video existingVideo = this.GetVideoById(video.Id);
            
            if (existingVideo == null)
            {
                throw new ArgumentException("Video does not exist", "video");
            }
            
            HashSet<string> modifiedDancers = new HashSet<string>();
            HashSet<string> modifiedEvents = new HashSet<string>();
            
            if (existingVideo.DancerIdList != null)
            {
                modifiedDancers.UnionWith(existingVideo.DancerIdList);
            }
            
            if (video.DancerIdList != null)
            {
                foreach (string dancerId in video.DancerIdList)
                {
                    if (!modifiedDancers.Remove(dancerId))
                    {
                        modifiedDancers.Add(dancerId);
                    }
                }
            }
            
            if (existingVideo.EventId != null)
            {
                modifiedEvents.Add(existingVideo.EventId);
            }
            
            if (video.EventId != null)
            {
                if (!modifiedEvents.Remove(video.EventId))
                {
                    modifiedEvents.Add(video.EventId);
                }
            }
                       
            this.baseDataAccess.UpdateVideo(video);
            this.HttpPost("admin/cache-reset").Wait();
            foreach (string dancerId in modifiedDancers)
            {
                this.UpdateDancerInCache(dancerId);                    
            }
            
            foreach (string eventId in modifiedEvents)
            {
                List<string> eventVideos;
                this.eventVideos.TryRemove(eventId, out eventVideos);
            }
            
            this.videos.TryRemove(video.Id, out video);
        }
        
        public List<Video> SearchForVideo(
            IEnumerable<string> titleFragments,
            IEnumerable<string> skillLevels,
            IEnumerable<string> danceCategories,
            IEnumerable<string> dancerIds,
            IEnumerable<string> eventIds)
        {
            return this.baseDataAccess.SearchForVideo(
                titleFragments,
                skillLevels,
                danceCategories,
                dancerIds,
                eventIds);
        }
        
        public bool ProviderVideoIdExists(string providerId, string providerVideoId)
        {
            return this.baseDataAccess.ProviderVideoIdExists(providerId, providerVideoId);
        }
        
        public string AddSuggestedVideo(Video suggestedVideo)
        {
            return this.baseDataAccess.AddSuggestedVideo(suggestedVideo);
        }
        
        public Video GetSuggestedVideo(string suggestedVideoId)
        {
            return this.baseDataAccess.GetSuggestedVideo(suggestedVideoId);
        }
        
        public List<Video> GetSuggestedVideos()
        {
            return this.baseDataAccess.GetSuggestedVideos();
        }
        
        public void DeleteSuggestedVideo(string suggestedVideoId)
        {
            this.baseDataAccess.DeleteSuggestedVideo(suggestedVideoId);
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
        
        private void UpdateDancerInCache(string dancerId)
        {
            Dancer dancer = this.baseDataAccess.GetDancerById(dancerId);
            
            if (dancer != null && !string.IsNullOrEmpty(dancer.WsdcId))
            {
                this.dancers[dancer.WsdcId] = dancer;

                int index = Array.FindIndex(this.allDancers, d => string.Equals(d.WsdcId, dancer.WsdcId));
                if (index > -1)
                {
                    this.allDancers[index] = dancer;
                }
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
        
        public string AddFlaggedVideo(FlaggedVideo flaggedVideo)
        {
            return this.baseDataAccess.AddFlaggedVideo(flaggedVideo);
        }
        
        public List<FlaggedVideo> GetFlaggedVideos()
        {
            return this.baseDataAccess.GetFlaggedVideos();
        }
        
        public FlaggedVideo GetFlaggedVideo(string flagId)
        {
            return this.baseDataAccess.GetFlaggedVideo(flagId);
        }
        
        public void DeleteFlaggedVideo(string flagId)
        {
            this.baseDataAccess.DeleteFlaggedVideo(flagId);
        }
        
        public string AddEvent(Event contractEvent)
        {
            string eventId = this.baseDataAccess.AddEvent(contractEvent);
            this.HttpPost("admin/cache-reset").Wait();
            return eventId;
        }
        
        public Stats GetStats()
        {
            return this.baseDataAccess.GetStats();
        }
	}
}