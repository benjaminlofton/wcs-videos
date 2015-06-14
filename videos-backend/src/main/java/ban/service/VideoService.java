package ban.service;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Component;

import java.util.HashSet;

import ban.client.AwsDynamoClient;
import ban.model.persistence.DancerD;
import ban.model.persistence.VideoD;
import ban.model.view.Video;

/**
 * Created by bnorrish on 6/10/15.
 */

@Component
public class VideoService {

  @Autowired
  private AwsDynamoClient dynamoClient;

  @Autowired
  private VideoMapper videoMapper;

  /**
   * Determines if an existing video key already exists
   * @param id key for the video
   * @return true if the video exists, false otherwise
   */
  public boolean exists(String id) {
    return dynamoClient.getVideo(id) == null;
  }

  public boolean existsByProviderId(String providerVideoId) {
    return !dynamoClient.getVideoByProviderVideoId(providerVideoId).isEmpty();
  }

  /**
   * Retrieves the persistence Video from DynamoClient, maps to View model
   * @param videoId Video Identifier
   * @return The video in View model
   */
  public Video getVideo(String videoId){
    VideoD pVideo = dynamoClient.getVideo(videoId);
    return videoMapper.mapToViewModel(pVideo);
  }

  public Video addVideo(Video video) {

    if(existsByProviderId(video.getProviderVideoId())) {
      throw new IllegalStateException("Cannot add video that already exists; id: " + video.getId() + " providedVideoId: " + video.getProviderVideoId());
    }

    // Save the video
    VideoD pVideo = videoMapper.mapToPersistanceModel(video);
    pVideo = dynamoClient.createVideo(pVideo);

    // Add the id of this video to the Dancer object for each dancer found
    // in this video's dancerIdList
    for(Integer wsdcId: video.getDancerIdList()) {

      DancerD dancer = dynamoClient.getDancer(wsdcId);

      // Currently, if the Video is marked as containing a dancer we don't yet know about
      // .. then that dancer is ignored.

      if(dancer != null) {

        if (dancer.getVideoIdList() == null) {
          dancer.setVideoIdList(new HashSet<>());
        }

        if (!dancer.getVideoIdList().contains(wsdcId)) {
          dancer.getVideoIdList().add(pVideo.getId());
          dynamoClient.saveDancer(dancer);
        }

      }
    }

    return videoMapper.mapToViewModel(pVideo);
  }
}
