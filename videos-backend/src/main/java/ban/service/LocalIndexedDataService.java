package ban.service;

import org.springframework.beans.factory.InitializingBean;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Component;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import ban.client.AwsDynamoClient;
import ban.model.persistence.DancerD;
import ban.model.persistence.EventD;
import ban.model.persistence.VideoD;

/**
 * Class contains a local in-memory HashMap based storage of:
 *
 * [Key]    - [Object]
 * VideoId  - Video
 * EventId  - Event
 *
 * Created by bnorrish on 6/28/15.
 */
@Component
public class LocalIndexedDataService implements InitializingBean {

  @Autowired
  AwsDynamoClient awsDynamoClient;

  // NOTE!
  // This class has synchronization issues, as it is a Singleton with internal state
  // Any modification of videoIdToVideoMap or eventIdToEventMap needs to be Synchronized
  //
  // Event and Video caches are fully loaded at start-up, and kept consistent on any Update;
  // Dancer cache is NOT fully loaded; at any time, local map will only contain partial list;
  //

  private Map<String, VideoD> videoIdToVideoMap = new HashMap<>();
  private Map<String, EventD> eventIdToEventMap = new HashMap<>();
  private Map<Integer, DancerD> wsdcIdToDancerMap = new HashMap<>();

  public void load() {

    // Load ALL videos at Start Up
    for(VideoD video : awsDynamoClient.getAllVideos("Video")) {
      videoIdToVideoMap.put(video.getId(), video);
    }

    // Load ALL Events at Start Up
    for(EventD eventD : awsDynamoClient.getEventList()) {
      eventIdToEventMap.put(eventD.getEventId(), eventD);
    }

    // Do NOT load all Dancers at Start Up

  }

  public void clear() {
    videoIdToVideoMap.clear();
    eventIdToEventMap.clear();
    wsdcIdToDancerMap.clear();
  }

  public List<VideoD> getAllVideos() {
    return new ArrayList<>(videoIdToVideoMap.values());
  }

  public VideoD getVideoById(String id) {
    return videoIdToVideoMap.get(id);
  }

  public List<EventD> getAllEvents() {
    return new ArrayList<>(eventIdToEventMap.values());
  }

  public EventD getEventById(String id) {
    return eventIdToEventMap.get(id);
  }

  public List<DancerD> getAllDancers() {
    // Local cache is not guaranteed to be full; pull from persistence

    List<DancerD> allDancers = awsDynamoClient.getAllDancers();

    for(DancerD dancer : allDancers) {
      wsdcIdToDancerMap.put(dancer.getWsdcId(),dancer);
    }

    return allDancers;
  }

  public DancerD getDancerById(Integer wsdcId) {

    if(wsdcIdToDancerMap.containsKey(wsdcId)) {
      return wsdcIdToDancerMap.get(wsdcId);
    }

    DancerD dancer = awsDynamoClient.getDancer(wsdcId);

    // Currently no restriction on the size of this collection;
    // We are expecting to be able to hold all dancers in memory for foreseeable future,
    // we just can't load them all at one.
    if(dancer != null) {
      wsdcIdToDancerMap.put(wsdcId, dancer);
    }

    return dancer;
  }

  public boolean dancerExists(Integer wsdcId) {
    return getDancerById(wsdcId) == null;
  }

  public void evictDancer(Integer wsdcId) {
    wsdcIdToDancerMap.remove(wsdcId);
  }

  public int size() {
    return videoIdToVideoMap.size() +
           eventIdToEventMap.size();
  }

  @Override
  public void afterPropertiesSet() throws Exception {
    this.load();
  }
}
