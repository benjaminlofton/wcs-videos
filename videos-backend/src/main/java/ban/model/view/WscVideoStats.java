package ban.model.view;

/**
 * Created by bnorrish on 6/26/15.
 */
public class WscVideoStats {

  private Integer numVideos;
  private Integer numDancers;
  private Integer numDancersWithVideos;
  private Integer cacheSizeItems;

  public Integer getCacheSizeItems() {
    return cacheSizeItems;
  }

  public void setCacheSizeItems(Integer cacheSizeItems) {
    this.cacheSizeItems = cacheSizeItems;
  }

  public Integer getNumVideos() {
    return numVideos;
  }

  public void setNumVideos(Integer numVideos) {
    this.numVideos = numVideos;
  }

  public Integer getNumDancers() {
    return numDancers;
  }

  public void setNumDancers(Integer numDancers) {
    this.numDancers = numDancers;
  }

  public Integer getNumDancersWithVideos() {
    return numDancersWithVideos;
  }

  public void setNumDancersWithVideos(Integer numDancersWithVideos) {
    this.numDancersWithVideos = numDancersWithVideos;
  }

}
