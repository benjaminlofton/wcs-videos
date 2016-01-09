using System.Collections.Generic;

namespace WcsVideos.Contracts
{
    public interface IDataAccess
    {
        List<Event> GetRecentEvents();
        
        List<Video> GetTrendingVideos();
        
        Event GetEvent(string eventId);
        
        List<Video> GetEventVideos(string eventId);
        
        List<Event> SearchForEvent(string query);

		Video GetVideoById(string id);

		Dancer GetDancerById(string id);
        
        List<Dancer> SearchForDancer(string query);
        
        Dancer[] GetAllDancers();
        
        string AddVideo(Video video);
        
        void UpdateVideo(Video video);
        
        List<Video> SearchForVideo(
            IEnumerable<string> titleFragments,
            IEnumerable<string> skillLevels,
            IEnumerable<string> danceCategories,
            IEnumerable<string> dancerIds,
            IEnumerable<string> eventIds);
            
        bool ProviderVideoIdExists(string providerId, string providerVideoId);
        
        ResourceList GetResourceList(string name);
        
        string AddFlaggedVideo(FlaggedVideo flaggedVideo);
        
        List<FlaggedVideo> GetFlaggedVideos();
        
        FlaggedVideo GetFlaggedVideo(string flagId);
        
        void DeleteFlaggedVideo(string flagId);
        
        string AddEvent(Event contractEvent);
        
        string AddSuggestedVideo(Video suggestedVideo);
        
        Video GetSuggestedVideo(string suggestedVideoId);
        
        List<Video> GetSuggestedVideos();
        
        void DeleteSuggestedVideo(string suggestedVideoId);
        
        Stats GetStats();
    }
}