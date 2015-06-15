package ban.service;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Component;

import java.util.ArrayList;
import java.util.List;
import java.util.Set;

import ban.client.AwsDynamoClient;
import ban.client.WsdcDancer;
import ban.client.WsdcRestClient;
import ban.model.persistence.DancerD;
import ban.model.view.Dancer;

/**
 * Created by bnorrish on 6/7/15.
 */
@Component
public class DancerService {

  @Autowired
  DancerMapper dancerMapper;

  @Autowired
  AwsDynamoClient awsDynamoClient;

  @Autowired
  WsdcRestClient wsdcRestClient;

  public Set<String> getVideosByWsdcId(int wsdcId) {
    return awsDynamoClient.getDancer(wsdcId).getVideoIdList();
  }

  public Dancer getDancer(Integer wsdcId) {
    return dancerMapper.mapToViewModel(awsDynamoClient.getDancer(wsdcId));
  }

  public void addDancer(Dancer dancer) {

    DancerD dancerD = dancerMapper.mapToPersistenceModel(dancer);
    awsDynamoClient.saveDancer(dancerD);
  }

  /***
   * WARNING: At worst, this will iterate over every Dancer in the Dancer repository
   * @param fragment The string fragment by which to search for dancers
   * @return returns the list of Dancers <b><ADDED</b> to the repository
   */
  public List<Dancer> addDancersByFragment(String fragment) {

    List<Dancer> addedDancers = new ArrayList<>();

    for(WsdcDancer d :wsdcRestClient.getDancersByFragment(fragment)) {

      Integer wsdcId = d.getValue();

      if(getDancer(wsdcId) == null) {

        DancerD newDancer = new DancerD();
        newDancer.setWsdcId(wsdcId);

        String rawName = d.getLabel();
        String name = rawName.substring(0, rawName.indexOf('(') - 1);

        newDancer.setName(name);

        // What happens if this fails?

        awsDynamoClient.saveDancer(newDancer);

        Dancer dancer = new Dancer();
        dancer.setName(name);
        dancer.setWsdcId(wsdcId);
        addedDancers.add(dancer);
      }
    }

    return addedDancers;
  }

}
