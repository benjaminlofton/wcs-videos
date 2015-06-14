package ban.model.persistence;

import java.util.HashSet;
import java.util.Set;

/**
 * Created by bnorrish on 6/10/15.
 */
public class VideoDBuilder {

  private String id = null;
  private Integer providerId = null;
  private String providerVideoId = null;
  private String title = null;
  private Set<Integer> dancerIdList = new HashSet<>();

  public VideoDBuilder withId(String id) {
    this.id = id;
    return this;
  }

  public VideoDBuilder withProviderId(Integer providerId) {
    this.providerId = providerId;
    return this;
  }

  public VideoDBuilder withProviderVideoId(String providerVideoId) {
    this.providerVideoId = providerVideoId;
    return this;
  }

  public VideoDBuilder withTitle(String title) {
    this.title = title;
    return this;
  }

  public VideoDBuilder withDancerIdList(Set<Integer> dancerIdList) {
    this.dancerIdList = dancerIdList;
    return this;
  }

  public VideoDBuilder addDancerId(Integer dancerId) {
    this.dancerIdList.add(dancerId);
    return this;
  }

  public VideoD build() {

    VideoD video = new VideoD();
    video.setId(id);
    video.setProviderId(providerId);
    video.setProviderVideoId(providerVideoId);
    video.setTitle(title);
    video.setDancerIdList(dancerIdList);

    return video;
  }

}
