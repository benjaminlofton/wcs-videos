package ban.service.mapper;

import org.springframework.stereotype.Component;

import java.util.ArrayList;
import java.util.List;

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
    video.setEventId(pVideo.getEventId());
    video.setCreatedDateTime(pVideo.getCreatedDateTime());
    video.setSkillLevel(pVideo.getSkillLevel());
    video.setDanceCategory(pVideo.getDanceCategory());

    return video;
  }

  public List<Video> mapToViewModel(List<VideoD> videoDs) {

    List<Video> videos = new ArrayList<>();

    for(VideoD videoD : videoDs) {
      videos.add(mapToViewModel(videoD));
    }

    return videos;
  }

  public VideoD mapToPersistanceModel(Video video) {

    if(video == null) {
      return null;
    }

    return new VideoDBuilder()
        .withId(video.getId())
        .withProviderId(video.getProviderId())
        .withProviderVideoId(video.getProviderVideoId())
        .withTitle(video.getTitle())
        .withEventId(video.getEventId())
        .withDancerIdList(video.getDancerIdList())
        .withCreatedDateTime(video.getCreatedDateTime())
        .withSkillLevel(video.getSkillLevel())
        .withDanceCategory(video.getDanceCategory())
        .build();
  }
}
