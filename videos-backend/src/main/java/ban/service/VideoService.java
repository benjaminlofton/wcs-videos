package ban.service;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Component;

import java.util.ArrayList;
import java.util.HashSet;
import java.util.List;

import ban.client.AwsDynamoClient;
import ban.exception.InvalidRequestException;
import ban.model.persistence.DancerD;
import ban.model.persistence.VideoD;
import ban.model.view.Video;
import ban.service.mapper.VideoMapper;

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
    return dynamoClient.getVideo(id) != null;
  }

  public boolean existsByProviderId(String providerVideoId) {
    return !dynamoClient.getVideoByProviderVideoId(providerVideoId).isEmpty();
  }

  public Video getByProviderId(String providerVideoId) {


    List<String> videoIds = dynamoClient.getVideoByProviderVideoId(providerVideoId);
    if(videoIds.isEmpty()) {
      return null;
    }

    return videoMapper.mapToViewModel(dynamoClient.getVideo(videoIds.get(0)));
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

  public List<Video> getAllVideos() {

    List<Video> videos =  new ArrayList<>();
    for(VideoD videoD : dynamoClient.getAllVideos()) {
      videos.add(videoMapper.mapToViewModel(videoD));
    }
    return videos;
  }

  public Video addVideo(Video video) {

    // Verify that the provider Id does not exist
    if(existsByProviderId(video.getProviderVideoId())) {
      throw new InvalidRequestException();
    }

    // Verify that all dancers exist
    for(Integer dancerId : video.getDancerIdList()) {

      if(dynamoClient.getDancer(dancerId) == null) {
        throw new InvalidRequestException();
      }
    }

    // If the video was posted with an EventId, verify that the EventId is valid
    if(video.getEventId() != null) {
      if(dynamoClient.getEventById(video.getEventId()) == null) {
        throw new InvalidRequestException();
      }
    }

    // Save the video
    VideoD pVideo = videoMapper.mapToPersistanceModel(video);
    pVideo = dynamoClient.createVideo(pVideo);

    // Add the id of this video to the Dancer object for each dancer found
    // in this video's dancerIdList
    for(Integer wsdcId: video.getDancerIdList()) {

      DancerD dancer = dynamoClient.getDancer(wsdcId);

      // Dancer should never be null, as we already checked this above
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
