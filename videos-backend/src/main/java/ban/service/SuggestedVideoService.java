package ban.service;

import org.joda.time.DateTime;
import org.joda.time.DateTimeZone;
import org.joda.time.format.DateTimeFormat;
import org.joda.time.format.DateTimeFormatter;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Component;

import java.util.Comparator;
import java.util.List;
import java.util.stream.Collectors;

import ban.client.AwsDynamoClient;
import ban.model.persistence.VideoD;
import ban.model.view.Video;
import ban.service.mapper.VideoMapper;

/**
 * Created by bnorrish on 10/18/15.
 */
@Component
public class SuggestedVideoService {

  @Autowired
  AwsDynamoClient awsDynamoClient;

  @Autowired
  VideoMapper videoMapper;

  public Video recordSuggestedVideo(Video video) {

    // Set CreatedDateTime
    DateTimeFormatter dtf = DateTimeFormat.forPattern("yyyy-MM-dd HH:mm:ss");
    video.setCreatedDateTime(dtf.print(new DateTime(DateTimeZone.UTC)));

    VideoD videoD = videoMapper.mapToPersistanceModel(video);

    videoD = awsDynamoClient.recordSuggestedVideo(videoD);

    return videoMapper.mapToViewModel(videoD);
  }

  public List<Video> getSuggestedVideos() {

    List<VideoD> videos = awsDynamoClient.getAllVideos("SuggestedVideo");

    // Order by CreateDateTime, map to ViewModel
    return videos.stream()
          .sorted(byCreatedDateTime)
          .map(videoMapper::mapToViewModel)
          .collect(Collectors.toList());
  }

  public void deleteVideo(String videoId) {

    awsDynamoClient.deleteSuggestedVideo(videoId);

  }

  private static final Comparator<VideoD> byCreatedDateTime = (v1, v2) -> {
    if (v1.getCreatedDateTime() == null) {
      return 1;
    }

    if (v2.getCreatedDateTime() == null) {
      return -1;
    }

    return v2.getCreatedDateTime().compareTo(v1.getCreatedDateTime());
  };
}
