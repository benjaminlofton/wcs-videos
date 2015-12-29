package ban.service.list;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.context.ApplicationContext;
import org.springframework.stereotype.Component;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

/**
 * Created by bnorrish on 9/13/15.
 */
@Component
public class ListManager {

  @Autowired
  private ApplicationContext appContext;

  private Map<String,String> listNameToBeanNameMap = new HashMap<>();

  // When we get > dozens of lists, this should be moved to configuration
  public ListManager() {
    listNameToBeanNameMap.put("latest-videos", "mostRecentVideoAggregator");
    listNameToBeanNameMap.put("no-event", "videosWithNoEventAggregator");
    listNameToBeanNameMap.put("videos-needing-dancers", "videosWithNoDancers");
    listNameToBeanNameMap.put("no-level", "videosWithNoLevelAggregator");
    listNameToBeanNameMap.put("no-category", "videosWithNoCategoryAggregator");
  }

  public List<String> getAllListerNames() {
    return new ArrayList(listNameToBeanNameMap.keySet());
  }

  public ILister getListerByName(String name) {

    if(!listNameToBeanNameMap.containsKey(name)) {
      throw new IllegalArgumentException("No Lister named: '" + name + "'");
    }

    ILister lister = null;
    try {
      lister = appContext.getBean(listNameToBeanNameMap.get(name), ILister.class);
    } catch (Exception ex) {
      throw new IllegalStateException("List of type: " + listNameToBeanNameMap.get(name) + " cannot be found; has it been moved or renamed?", ex);
    }

    return lister;

  }
}
