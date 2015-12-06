package ban.service.list;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Component;

import java.util.List;

import ban.exception.InvalidRequestException;
import ban.model.view.ResourceList;

/**
 * Created by bnorrish on 7/17/15.
 */
@Component
public class ListService {

  @Autowired
  ListManager listManager;

  public ResourceList getListByName(String name, Integer skip, Integer take) {

    ResourceList resourceList = new ResourceList();
    resourceList.setName(name);

    ILister lister = listManager.getListerByName(name);
    resourceList.setIds(lister.getResults(skip,take));
    resourceList.setResourceType(lister.getListType());

    return resourceList;
  }

  public List<String> getAllListNames() {
    return listManager.getAllListerNames();
  }
}
