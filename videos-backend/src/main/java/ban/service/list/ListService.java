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

  public ResourceList getListByName(String name) {

    ResourceList resourceList = new ResourceList();

    switch(name) {
      case "latest-videos":
        resourceList.setIds(mostRecentVideoAggregator.getMostRecent(10));
        resourceList.setName(name);
        resourceList.setResourceType("Video");
        break;
      default:
        throw new InvalidRequestException();
    }

    return resourceList;
  }


}
