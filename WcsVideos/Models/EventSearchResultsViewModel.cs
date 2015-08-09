
namespace WcsVideos.Models
{
	public class EventSearchResultsViewModel : PagingViewModelBase<EventListItemViewModel>
	{
        public string Query { get; set; }
	}
}