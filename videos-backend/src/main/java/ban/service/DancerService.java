package ban.service;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Component;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.Set;
import java.util.stream.Collectors;

import ban.client.AwsDynamoClient;
import ban.client.WsdcDancer;
import ban.client.WsdcRestClient;
import ban.model.persistence.DancerD;
import ban.model.persistence.VideoD;
import ban.model.view.Dancer;
import ban.model.view.Video;

/**
 * Created by bnorrish on 6/7/15.
 */
@Component
public class DancerService {

  @Autowired
  DancerMapper dancerMapper;

  @Autowired
  VideoMapper videoMapper;

  @Autowired
  AwsDynamoClient awsDynamoClient;

  @Autowired
  WsdcRestClient wsdcRestClient;

  public List<Video> getVideosByDancerWsdcId(int wsdcId) {

    DancerD dancerD =  awsDynamoClient.getDancer(wsdcId);

    if(dancerD == null) {
      return new ArrayList<>();
    }

    Set<String> videoIdList = dancerD.getVideoIdList();

    List<Video> videos = new ArrayList<>();
    for(String videoId : videoIdList) {
      VideoD videoD = awsDynamoClient.getVideo(videoId);
      if(videoD != null) {
        videos.add(videoMapper.mapToViewModel(videoD));
      } else {
        // Log Error when logging exists: Dancer references video that does not exist

      }

    }

    return videos;
  }

  public Dancer getDancer(Integer wsdcId) {
    return dancerMapper.mapToViewModel(awsDynamoClient.getDancer(wsdcId));
  }

  public List<Dancer> getDancerList() {

    List<Dancer> viewList = new ArrayList<>();

    for(DancerD dancerD : awsDynamoClient.getAllDancers()) {
      viewList.add(dancerMapper.mapToViewModel(dancerD));
    }

    // java 1.8 FTW
    return viewList.stream().sorted((obj1,obj2) -> obj1.getWsdcId().compareTo(obj2.getWsdcId())).collect(Collectors.toList());
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

    List<DancerD> existingDancers = awsDynamoClient.getAllDancers();
    Map<Integer,DancerD> existingDancerMap = new HashMap<>();
    for(DancerD dancerD: existingDancers) {
      existingDancerMap.put(dancerD.getWsdcId(),dancerD);
    }

    for(WsdcDancer d :wsdcRestClient.getDancersByFragment(fragment)) {

      Integer wsdcId = d.getValue();

      if(!existingDancerMap.containsKey(wsdcId)) {

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
