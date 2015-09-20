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

  @Autowired
  LocalIndexedDataService localIndexedDataService;

  public WscVideoStats getBasicStats() {

    WscVideoStats stats = new WscVideoStats();

    List<VideoD> videos = dynamoClient.getAllVideos();
    stats.setNumVideos(videos.size());
    stats.setCacheSizeItems(localIndexedDataService.size());

    return stats;
  }

}
