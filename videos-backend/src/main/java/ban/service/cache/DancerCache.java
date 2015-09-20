package ban.service.cache;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Component;

import java.util.HashMap;
import java.util.Map;

import ban.client.AwsDynamoClient;
import ban.model.persistence.DancerD;

/**
 * Created by bnorrish on 9/19/15.
 */
@Component
public class DancerCache {

  @Autowired
  AwsDynamoClient awsDynamoClient;

  Map<Integer, DancerD> dancerCache = new HashMap<>();

  public DancerD getDancer(Integer wsdcId) {

    if(dancerCache.containsKey(wsdcId)) {
      return dancerCache.get(wsdcId);
    }

    DancerD dancer = awsDynamoClient.getDancer(wsdcId);

    // Currently no restriction on the size of this collection;
    // We are expecting to be able to hold all dancers in memory for foreseeable future,
    // we just can't load them all at one.
    dancerCache.put(wsdcId,dancer);

    return dancer;
  }

  public void clear() {
    dancerCache.clear();
  }

  public void refreshDancer(Integer wsdcId) {

    DancerD dancer = awsDynamoClient.getDancer(wsdcId);
    dancerCache.put(wsdcId,dancer);
  }
}
