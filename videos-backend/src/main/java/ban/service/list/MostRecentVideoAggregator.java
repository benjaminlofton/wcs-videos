package ban.service.list;

import org.joda.time.DateTime;
import org.joda.time.format.DateTimeFormat;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Component;

import java.util.ArrayList;
import java.util.Collections;
import java.util.Comparator;
import java.util.List;
import java.util.stream.Collector;
import java.util.stream.Collectors;

import ban.model.persistence.VideoD;
import ban.service.LocalIndexedDataService;

/**
 * Created by bnorrish on 8/17/15.
 */
@Component
public class MostRecentVideoAggregator {

  @Autowired
  LocalIndexedDataService localIndexedDataService;

  public List<String> getMostRecent(int number) {

    List<VideoD> videos = localIndexedDataService.getAllVideos();

    videos = videos.stream()
                   .filter(v -> v.getCreatedDateTime() != null)
                   .collect(Collectors.toList());

    List<VideoD> validVideos = new ArrayList<>();
    for (VideoD video : videos) {
      try {
        DateTimeFormat.forPattern("yyyy-MM-dd HH:mm:ss").parseDateTime(video.getCreatedDateTime());
        validVideos.add(video);
      } catch (IllegalArgumentException ex) {
        // swallow
      }
    }

    Collections.sort(validVideos, new VideoSortByCreateDateComparitor());

    List<VideoD> firstNVideos = validVideos.subList(0, Math.min(number, validVideos.size()));

    List<String> returnList = new ArrayList<String>();
    for(VideoD videoD : firstNVideos) {
      returnList.add(videoD.getId());
    }

    return returnList;
  }

  class VideoSortByCreateDateComparitor implements Comparator<VideoD> {

    @Override
    public int compare(VideoD v1, VideoD v2) {

      DateTime d1,d2;

      try {
        d1 = DateTimeFormat.forPattern("yyyy-MM-dd HH:mm:ss").parseDateTime(v1.getCreatedDateTime());
      } catch (IllegalArgumentException ex) {
        return -1;
      }

      try {
        d2 = DateTimeFormat.forPattern("yyyy-MM-dd HH:mm:ss").parseDateTime(v2.getCreatedDateTime());
      } catch (IllegalArgumentException ex) {
        return 1;
      }

      return d1.compareTo(d2) * -1;
    }
  }
}
