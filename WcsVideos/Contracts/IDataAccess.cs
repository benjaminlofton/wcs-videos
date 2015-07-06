using System.Collections.Generic;

namespace WcsVideos.Contracts
{
    public interface IDataAccess
    {
        List<Video> GetTrendingVideos();

		Video GetVideoById(string id);

		Dancer GetDancerById(string id);
        
        List<Dancer> SearchForDancer(string query);
        
        List<Dancer> GetAllDancers();
        
        string AddVideo(Video video);
    }
}