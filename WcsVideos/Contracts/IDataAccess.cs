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
        
        List<Dancer> GetAllDancers();
        
        string AddVideo(Video video);
        
        List<Video> SearchForVideo(
            IEnumerable<string> titleFragments,
            IEnumerable<string> dancerIds,
            IEnumerable<string> eventIds);
            
        bool ProviderVideoIdExists(string providerId, string providerVideoId);
    }
}