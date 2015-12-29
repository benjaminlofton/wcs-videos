package ban.service;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Component;

import java.util.HashSet;
import java.util.List;
import java.util.Set;

import ban.client.AwsDynamoClient;
import ban.model.persistence.DancerD;
import ban.model.persistence.VideoD;

/**
 * Created by bnorrish on 12/29/15.
 */
@Component
public class AdminService {

  @Autowired
  LocalIndexedDataService localIndexedDataService;

  @Autowired
  AwsDynamoClient awsDynamoClient;

  public Set<String> correctDancersWithMissingVideos() {


    Set<String> badDancers = new HashSet<>();
    List<VideoD> allVideos = localIndexedDataService.getAllVideos();

    for(VideoD videoD : allVideos) {

      if(videoD.getDancerIdList() != null) {
        for(Integer dancerId : videoD.getDancerIdList()) {

          DancerD dancer = localIndexedDataService.getDancerById(dancerId);

          if(dancer.getVideoIdList() == null) {
            dancer.setVideoIdList(new HashSet<>());
            dancer.getVideoIdList().add(videoD.getId());
            badDancers.add(dancer.getWsdcId().toString());
            awsDynamoClient.saveDancer(dancer);
          } else {
            if (!dancer.getVideoIdList().contains(videoD.getId())) {
              dancer.getVideoIdList().add(videoD.getId());
              badDancers.add(dancer.getWsdcId().toString());
              awsDynamoClient.saveDancer(dancer);
            }
          }
        }
      }
    }

    return badDancers;
  }
}
