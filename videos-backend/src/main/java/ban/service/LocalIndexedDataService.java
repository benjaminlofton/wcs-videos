package ban.service;

import org.joda.time.DateTime;
import org.joda.time.format.DateTimeFormat;
import org.springframework.beans.factory.InitializingBean;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Component;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.HashMap;
import java.util.HashSet;
import java.util.List;
import java.util.Map;
import java.util.Set;

import ban.client.AwsDynamoClient;
import ban.model.persistence.DancerD;
import ban.model.persistence.EventD;
import ban.model.persistence.VideoD;
import ban.model.view.Video;

/**
 * Class contains a local in-memory HashMap based storage of:
 *
 * [Key]    - [Object]
 * WsdcId   - Dancer
 * VideoId  - Video
 * VideoTitle    - List of Videos
 * EventTitle    - List of Events
 * EventList
 * EventId       - List of Videos
 *
 * Created by bnorrish on 6/28/15.
 */
@Component
public class LocalIndexedDataService implements InitializingBean {

  @Autowired
  AwsDynamoClient awsDynamoClient;

  private Map<Integer,DancerD> wsdcIdToDancerMap = new HashMap<>();
  private Map<String, VideoD> videoIdToVideoMap = new HashMap<>();
  private Map<String, List<VideoD>> videoTitleFragToVideoMap = new HashMap<>();
  private Map<String, List<EventD>> eventTitleFragToEventMap = new HashMap<>();
  private List<EventD> eventDList = new ArrayList<>();
  private Map<String,List<VideoD>> eventIdToVideoMap = new HashMap<>();

  public void clear() {
    wsdcIdToDancerMap.clear();
    videoIdToVideoMap.clear();
    videoTitleFragToVideoMap.clear();
    eventTitleFragToEventMap.clear();
    eventDList.clear();
    eventIdToVideoMap.clear();
  }

  public void load() {

    for(DancerD dancer : awsDynamoClient.getAllDancers()) {
      wsdcIdToDancerMap.put(dancer.getWsdcId(), dancer);
    }

    for(VideoD video : awsDynamoClient.getAllVideos()) {
      videoIdToVideoMap.put(video.getId(), video);

      List<String> wordList = Arrays.asList(video.getTitle().split(" "));

      wordList = deDuplicate(wordList);

      for(String word : wordList) {

        word = word.toLowerCase();

        if(videoTitleFragToVideoMap.get(word) == null) {
          videoTitleFragToVideoMap.put(word,new ArrayList<>());
        }

        videoTitleFragToVideoMap.get(word).add(video);
      }

      if(video.getEventId() != null && !video.getEventId().isEmpty()) {

        if(!eventIdToVideoMap.containsKey(video.getEventId())) {
          eventIdToVideoMap.put(video.getEventId(), new ArrayList<>());
        }

        eventIdToVideoMap.get(video.getEventId()).add(video);
      }
    }

    for(EventD eventD : awsDynamoClient.getEventList()) {

      eventDList.add(eventD);

      List<String> wordList = Arrays.asList(eventD.getName().split(" "));

      wordList = deDuplicate(wordList);

      for (String word : wordList) {

        word = word.toLowerCase();

        if(eventTitleFragToEventMap.get(word) == null) {
          eventTitleFragToEventMap.put(word,new ArrayList<>());
        }

        eventTitleFragToEventMap.get(word).add(eventD);
      }
    }
  }

  public List<VideoD> getVideosBySingleWordTitleFrag(String sigleWordTitleFragment) {

    List<VideoD> returnList = videoTitleFragToVideoMap.get(sigleWordTitleFragment.toLowerCase());

    if(returnList == null) {
      return new ArrayList<>();
    }

    return returnList;
  }

  public List<VideoD> getAllVideos() {
    return new ArrayList<>(videoIdToVideoMap.values());
  }

  public DancerD getDancer(String wsdcId) {
    return wsdcIdToDancerMap.get(wsdcId);
  }

  public List<VideoD> getVideosByDancerWsdcId(String wsdcId) {
    List<VideoD> results = new ArrayList<>();

    DancerD dancerD = wsdcIdToDancerMap.get(Integer.parseInt(wsdcId)
    );

    if(dancerD == null) {
      return results;
    }

    for(String videoId : dancerD.getVideoIdList()) {

      VideoD videoD = videoIdToVideoMap.get(videoId);

      if(videoD != null) {
        results.add(videoD);
      } else {
        // Log error; Video does not exist for a video referenced in the Dancer Object

      }
    }

    return results;
  }

  public List<EventD> getEventsBySingleWordNameFrag(String singleWordNameFragment) {

    List<EventD> returnList = eventTitleFragToEventMap.get(singleWordNameFragment.toLowerCase());

    if(returnList == null) {
      return new ArrayList<>();
    }

    return returnList;
  }

  public List<EventD> getEventsByWsdcPointed(boolean isWsdcPointed) {

    List<EventD> returnList = new ArrayList<>();

    for (EventD eventD : eventDList) {
      if(eventD.isWsdcPointed().equals(isWsdcPointed)) {
        returnList.add(eventD);
      }
    }

    return returnList;
  }

  public List<EventD> getEventsByYear(Integer year) {

    List<EventD> results = new ArrayList<>();
    for(EventD eventD : eventDList) {
      if(eventD.getEventDate() != null) {
        DateTime date = DateTime.parse(eventD.getEventDate());
        if(year.equals(date.getYear())) {
          results.add(eventD);
        }
      }
    }

    return results;
  }

  public List<EventD> getEventsBetween(DateTime afterDate, DateTime beforeDate) {

    List<EventD> results = new ArrayList<>();

    for (EventD eventD : eventDList) {

      boolean keep = true;

      // Seems bad to do this for every date, for every search
      DateTime eventDate = DateTimeFormat.forPattern("yyyy-MM-dd").parseDateTime(eventD.getEventDate());

      if(afterDate != null) {
        keep = keep && (eventDate.isAfter(afterDate) || eventDate.isEqual(afterDate));
      }

      if(beforeDate != null) {
        keep = keep && (eventDate.isBefore(beforeDate) || eventDate.isEqual(beforeDate));
      }

      if(keep) {
        results.add(eventD);
      }
    }

    return results;
  }

  public List<VideoD> getVideosByEventId(String eventId) {

    if(!eventIdToVideoMap.containsKey(eventId)) {
      return new ArrayList<>();
    }

    return eventIdToVideoMap.get(eventId);
  }

  public int size() {
    return wsdcIdToDancerMap.size() +
           videoIdToVideoMap.size() +
           videoTitleFragToVideoMap.size() +
           eventDList.size() +
           eventTitleFragToEventMap.size() +
           eventIdToVideoMap.size();
  }

  private List<String> deDuplicate(List<String> initialList) {

    Set<String> result = new HashSet<>();

    for( String item : initialList) {
      result.add(item);
    }

    ArrayList<String> resultList = new ArrayList<>();
    resultList.addAll(result);
    return resultList;
  }

  @Override
  public void afterPropertiesSet() throws Exception {
    this.load();
  }
}
