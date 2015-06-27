package ban.service;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Component;

import java.util.List;

import ban.client.AwsDynamoClient;
import ban.model.persistence.DancerD;
import ban.model.persistence.VideoD;
import ban.model.view.WscVideoStats;

/**
 * Created by bnorrish on 6/26/15.
 */

@Component
public class StatsService {

  @Autowired
  AwsDynamoClient dynamoClient;

  public WscVideoStats getBasicStats() {

    WscVideoStats stats = new WscVideoStats();

    List<DancerD> dancers = dynamoClient.getAllDancers();
    stats.setNumDancers(dancers.size());

    int dancersWithVideos = 0;
    for(DancerD dancerD: dancers) {
      if(dancerD.getVideoIdList() != null && !dancerD.getVideoIdList().isEmpty()) {
        dancersWithVideos++;
      }
    }
    stats.setNumDancersWithVideos(dancersWithVideos);

    List<VideoD> videos = dynamoClient.getAllVideos();
    stats.setNumVideos(videos.size());

    return stats;
  }

}
