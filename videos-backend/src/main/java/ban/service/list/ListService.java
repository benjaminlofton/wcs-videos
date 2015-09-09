package ban.service.list;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Component;

import ban.exception.InvalidRequestException;
import ban.model.view.ResourceList;

/**
 * Created by bnorrish on 7/17/15.
 */
@Component
public class ListService {

  @Autowired
  MostRecentVideoAggregator mostRecentVideoAggregator;

  @Autowired
  VideosWithNoEventAggregator videosWithNoEventAggregator;

  public ResourceList getListByName(String name, Integer skip, Integer take) {

    ResourceList resourceList = new ResourceList();
    resourceList.setName(name);

    switch(name) {
      case "latest-videos":
        resourceList.setIds(mostRecentVideoAggregator.getMostRecent(skip,take));
        resourceList.setResourceType("Video");
        break;
      case "no-event":
        resourceList.setIds(videosWithNoEventAggregator.getVideosWithNoEvent(skip,take));
        resourceList.setResourceType("Video");
        break;
      default:
        throw new InvalidRequestException();
    }

    return resourceList;
  }


}
