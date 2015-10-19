package ban.service;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Component;

import java.util.List;

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

    VideoD videoD = videoMapper.mapToPersistanceModel(video);

    videoD = awsDynamoClient.recordSuggestedVideo(videoD);

    return videoMapper.mapToViewModel(videoD);
  }

  public List<Video> getSuggestedVideos() {

    List<VideoD> videos = awsDynamoClient.getAllVideos("SuggestedVideo");

    return videoMapper.mapToViewModel(videos);
  }
}
