package ban.service;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Component;

import java.util.HashSet;
import java.util.List;
import java.util.Set;
import java.util.stream.Collectors;

import ban.client.AwsDynamoClient;
import ban.model.persistence.DancerD;
import ban.model.persistence.EventD;
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

    List<EventD> events = dynamoClient.getEventList();
    List<VideoD> videos = dynamoClient.getAllVideos("Video");

    stats.setNumVideos(videos.size());

    Long videosWithEvents = videos
        .stream()
        .filter(v -> v.getEventId() != null)
        .count();

    Long videosWithLevel = videos
        .stream()
        .filter(v-> v.getSkillLevel() != null)
        .count();

    Long videosWithCategory = videos
        .stream()
        .filter(v-> v.getDanceCategory() != null)
        .count();

    stats.setNumVideosWithEvents(Integer.valueOf(videosWithEvents.intValue()));
    stats.setNumVideosWithSkillLevel(Integer.valueOf(videosWithLevel.intValue()));
    stats.setNumVideosWithDanceCategory(Integer.valueOf(videosWithCategory.intValue()));

    stats.setNumEvents(events.size());

    Set<String> eventIdsWithVideos = new HashSet<>();
    for(VideoD video : videos) {
      eventIdsWithVideos.add(video.getEventId());
    }
    stats.setNumEventsWithVideos(eventIdsWithVideos.size());


    return stats;
  }



}
