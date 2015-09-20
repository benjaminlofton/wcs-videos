package ban.service;

import org.joda.time.DateTime;
import org.joda.time.DateTimeZone;
import org.joda.time.format.DateTimeFormat;
import org.joda.time.format.DateTimeFormatter;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Component;

import java.util.HashSet;
import java.util.List;
import java.util.Set;
import java.util.stream.Collectors;

import ban.client.AwsDynamoClient;
import ban.exception.InvalidRequestException;
import ban.model.persistence.DancerD;
import ban.model.persistence.VideoD;
import ban.model.view.Video;
import ban.service.cache.DancerCache;
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

  @Autowired
  private DancerCache dancerCache;

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

  /**
   * If if id field of the Video object is set, attempt to udpate the existing video
   * If not, add a new video, including updates to the Dancer colleciton
   * @param video - The Video object to add or update
   * @return Returns the new state of the Video, if successful
   */
  public Video addOrUpdateVideo(Video video) {

    String id = video.getId();
    if(id != null && !id.isEmpty()) {

      if(!exists(id)) {
        throw new InvalidRequestException();
      }

      return updateVideo(video);

    } else {
      return addVideo(video);
    }
  }

  private Video updateVideo(Video newState) {

    // Retrieve the existing video object
    VideoD oldState = dynamoClient.getVideo(newState.getId());
    if( oldState == null) {
      throw new InvalidRequestException();
    }

    Set<Integer> oldStateDancers = oldState.getDancerIdList();
    Set<Integer> newStateDancers = newState.getDancerIdList();

    Set<Integer> addedDancers = newStateDancers.stream()
            .filter(v -> !oldStateDancers.contains(v))
            .collect(Collectors.toSet());

    Set<Integer> removedDancers = oldStateDancers.stream()
            .filter(v -> !newStateDancers.contains(v))
            .collect(Collectors.toSet());

    // Verify all added dancers exist
    for(Integer dancerId : addedDancers) {

      if(dynamoClient.getDancer(dancerId) == null) {
        throw new InvalidRequestException();
      }
    }

    // If event id was changed, verify event is valid
    if(newState.getEventId() != null && !newState.getEventId().equals(oldState.getEventId())) {
      if(dynamoClient.getEventById(newState.getEventId()) == null) {
        throw new InvalidRequestException();
      }
    }

    // *** Below this point we are updating dynamo objects, any exception could result in inconsistent data

    // For each added dancer, associate the Dancer with the Video
    for (Integer addedDancer : addedDancers) {
      associateDancerWithVideo(addedDancer,oldState.getId());
    }

    // For each removed dancer, disassociate the Dancer with the Video
    for (Integer removedDancer : removedDancers) {
      disassociateDancerWithVideo(removedDancer,oldState.getId());
    }

    return videoMapper.mapToViewModel(dynamoClient.updateVideo(videoMapper.mapToPersistanceModel(newState)));
  }

  private Video addVideo(Video video) {

    // Verify that the provider Video Id does not exist
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

    // Set CreatedDateTime
    DateTimeFormatter dtf = DateTimeFormat.forPattern("yyyy-MM-dd HH:mm:ss");
    video.setCreatedDateTime(dtf.print(new DateTime(DateTimeZone.UTC)));

    // Save the video
    VideoD pVideo = videoMapper.mapToPersistanceModel(video);
    pVideo = dynamoClient.createVideo(pVideo);

    // Add the id of this video to the Dancer object for each dancer found
    // in this video's dancerIdList
    for(Integer wsdcId: video.getDancerIdList()) {
      associateDancerWithVideo(wsdcId,pVideo.getId());
    }

    return videoMapper.mapToViewModel(pVideo);
  }

  private void associateDancerWithVideo(Integer wsdcId, String videoId) {

    DancerD dancer = dynamoClient.getDancer(wsdcId);

    // Dancer should never be null, as we already checked this above
    if(dancer != null) {

      if (dancer.getVideoIdList() == null) {
        dancer.setVideoIdList(new HashSet<>());
      }

      if (!dancer.getVideoIdList().contains(videoId)) {
        dancer.getVideoIdList().add(videoId);
        dynamoClient.saveDancer(dancer);
        dancerCache.refreshDancer(dancer.getWsdcId());
      }
    }
  }

  private void disassociateDancerWithVideo(Integer wsdcId, String videoId) {

    DancerD dancer = dynamoClient.getDancer(wsdcId);

    // Dancer should never be null, as we already checked this above
    if(dancer != null) {

      if (dancer.getVideoIdList() == null) {
        // This should not happen; Removing a videoId from a Dancer that has no videoIds.
        return;
      }

      dancer.getVideoIdList().remove(videoId);
      dynamoClient.saveDancer(dancer);
      dancerCache.refreshDancer(dancer.getWsdcId());
    }
  }

}
