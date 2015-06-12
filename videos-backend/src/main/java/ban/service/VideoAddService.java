package ban.service;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Component;

import java.util.ArrayList;
import java.util.HashSet;

import ban.client.AwsDynamoClient;
import ban.model.persistence.DancerD;
import ban.model.persistence.VideoD;
import ban.model.view.Dancer;
import ban.model.view.Video;

/**
 * Created by bnorrish on 6/10/15.
 */

@Component
public class VideoAddService {

  @Autowired
  private AwsDynamoClient dynamoClient;

  @Autowired
  private VideoMapper videoMapper;

  public Video addVideo(Video video) {

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
