namespace WcsVideos.Models.Population
{
    public interface IVideoViewModelPopulator
    {
        void Populate(WatchViewModel model);
        
        void Populate(VideoListItemViewModel model);
    }
}