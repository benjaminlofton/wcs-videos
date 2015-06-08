package ban.model.view;

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
  public void setTitle(String title) { this.title = title; }
}
