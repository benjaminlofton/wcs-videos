package ban.service;

import org.springframework.stereotype.Component;

import ban.model.persistence.VideoD;
import ban.model.view.Video;

/**
 * This is TEMPORARY till I figure out how I want to map between domain models
 * http://kenblair.net/orika-spring-easy-bean-mapping/
 */
@Component
public class Mapper {

  public Video mapToViewModel(VideoD pVideo) {

    if(pVideo == null) {
      return null;
    }

    Video video = new Video();

    video.setId(pVideo.getId());
    video.setProviderId(pVideo.getProviderId());
    video.setProviderVideoId(pVideo.getProviderVideoId());
    video.setTitle(pVideo.getTitle());

    return video;
  }
}
