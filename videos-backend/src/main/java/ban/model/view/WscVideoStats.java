package ban.model.view;

/**
 * Created by bnorrish on 6/26/15.
 */
public class WscVideoStats {

  private long numEvents;
  private long numEventsWithVideos;

  private Integer numVideos;
  private Integer numVideosWithEvents;
  private Integer numVideosWithSkillLevel;
  private Integer numVideosWithDanceCategory;
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

  public Integer getNumVideosWithEvents() {
    return numVideosWithEvents;
  }

  public void setNumVideosWithEvents(Integer numVideosWithEvents) {
    this.numVideosWithEvents = numVideosWithEvents;
  }

  public Integer getNumVideosWithSkillLevel() {
    return numVideosWithSkillLevel;
  }

  public void setNumVideosWithSkillLevel(Integer numVideosWithSkillLevel) {
    this.numVideosWithSkillLevel = numVideosWithSkillLevel;
  }

  public Integer getNumVideosWithDanceCategory() {
    return numVideosWithDanceCategory;
  }

  public void setNumVideosWithDanceCategory(Integer numVideosWithDanceCategory) {
    this.numVideosWithDanceCategory = numVideosWithDanceCategory;
  }

  public long getNumEvents() {
    return numEvents;
  }

  public void setNumEvents(long numEvents) {
    this.numEvents = numEvents;
  }

  public long getNumEventsWithVideos() {
    return numEventsWithVideos;
  }

  public void setNumEventsWithVideos(long numEventsWithVideos) {
    this.numEventsWithVideos = numEventsWithVideos;
  }
}
