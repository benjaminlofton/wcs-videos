package ban.model.view;

import java.util.HashSet;
import java.util.Set;

/**
 * Video class as presented as Resource in REST endpoints 
 */
public class Video {

  public Video() {};

  public Video(String id) {
    this.id = id;
  }

  private String id;
  public String getId() {
    return id;
  }
  public void setId(String id) {
    this.id = id;
  }

  private Integer providerId;
  public Integer getProviderId() { return providerId; }
  public void setProviderId(Integer providerId) { this.providerId = providerId; }

  private String providerVideoId;
  public String getProviderVideoId() { return providerVideoId; }
  public void setProviderVideoId(String providerVideoId) { this.providerVideoId = providerVideoId; }

  private String title;
  public String getTitle() { return title; }
  public void setTitle(String title) { this.title = title;  }

  private Set<Integer> dancerIdList = new HashSet<>();
  public Set<Integer> getDancerIdList() { return dancerIdList == null ? new HashSet<>() : dancerIdList; }
  public void setDancerIdList(Set<Integer> dancerIdList) { this.dancerIdList = dancerIdList; }

  private String eventId;
  public String getEventId() {
    return eventId;
  }
  public void setEventId(String eventId) {
    this.eventId = eventId;
  }

  private String CreatedDateTime;
  public String getCreatedDateTime() {
    return CreatedDateTime;
  }
  public void setCreatedDateTime(String createdDateTime) {
    CreatedDateTime = createdDateTime;
  }
}
