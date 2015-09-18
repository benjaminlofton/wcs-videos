package ban.service;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Controller;

import java.util.List;
import java.util.stream.Collector;
import java.util.stream.Collectors;

import ban.client.AwsDynamoClient;
import ban.exception.InvalidRequestException;
import ban.exception.ResourceNotFoundException;
import ban.model.persistence.FlaggedVideoD;
import ban.model.persistence.VideoD;
import ban.service.mapper.VideoMapper;

/**
 * Created by bnorrish on 9/14/15.
 */
@Controller
public class FlaggedVideoService {

  @Autowired
  AwsDynamoClient awsDynamoClient;

  @Autowired
  LocalIndexedDataService localIndexedDataService;

  @Autowired
  VideoMapper videoMapper;

  public FlaggedVideoD flagVideo(FlaggedVideoD flaggedVideo) {

    if (flaggedVideo.getFlaggedVideoId() == null || flaggedVideo.getFlaggedVideoId().isEmpty()) {
      throw new InvalidRequestException();
    }

    VideoD videoD = localIndexedDataService.getVideoById(flaggedVideo.getFlaggedVideoId());
    if (videoD == null) {
      throw new ResourceNotFoundException();
    }

    return awsDynamoClient.createFlaggedVideo(flaggedVideo);
  }

  public List<FlaggedVideoD> getAllFlaggedVideos() {
    return awsDynamoClient.getAllFlaggedVideos();
  }

  public void deleteFlaggedVideo(String flagId) {
    awsDynamoClient.removeFlaggedVideo(flagId);
  }

  public FlaggedVideoD getFlaggedVideo(String flagId) {
    FlaggedVideoD videoD =  awsDynamoClient.getFlaggedVideo(flagId);

    if (videoD == null) {
      throw new ResourceNotFoundException();
    }

    return videoD;
  }

  public List<FlaggedVideoD> getAllFlaggedByVideoId(String videoId) {

    List<FlaggedVideoD> all = awsDynamoClient.getAllFlaggedVideos();

    return all.stream()
        .filter(v -> v.getFlaggedVideoId().equals(videoId))
        .collect(Collectors.toList());
  }
}
