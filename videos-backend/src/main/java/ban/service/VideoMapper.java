package ban.service;

import org.springframework.stereotype.Component;

import ban.model.persistence.VideoD;
import ban.model.persistence.VideoDBuilder;
import ban.model.view.Video;

/**
 * This is TEMPORARY till I figure out how I want to map between domain models
 * http://kenblair.net/orika-spring-easy-bean-mapping/
 */
@Component
public class VideoMapper {

  public Video mapToViewModel(VideoD pVideo) {

    if(pVideo == null) {
      return null;
    }

    Video video = new Video();

    video.setId(pVideo.getId());
    video.setProviderId(pVideo.getProviderId());
    video.setProviderVideoId(pVideo.getProviderVideoId());
    video.setTitle(pVideo.getTitle());
    video.setDancerIdList(pVideo.getDancerIdList());

    return video;
  }

  public VideoD mapToPersistanceModel(Video video) {

    return new VideoDBuilder()
        .withId(video.getId())
        .withProviderId(video.getProviderId())
        .withProviderVideoId(video.getProviderVideoId())
        .withTitle(video.getTitle())
        .withDancerIdList(video.getDancerIdList())
        .build();
  }
}
