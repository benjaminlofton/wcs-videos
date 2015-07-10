package ban.service;

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

  public void clear() {
    wsdcIdToDancerMap.clear();
    videoIdToVideoMap.clear();
    videoTitleFragToVideoMap.clear();
    eventTitleFragToEventMap.clear();
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
    }

    for(EventD eventD : awsDynamoClient.getEventList()) {

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

  public int size() {
    return wsdcIdToDancerMap.size() + videoIdToVideoMap.size() + videoTitleFragToVideoMap.size();
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
