package ban.model.view;

import java.util.List;
import java.util.Set;

/**
 * Created by bnorrish on 6/11/15.
 */
public class Dancer {

  private Integer wsdcId;
  private String name;
  private Set<String> videoIdList;

  public Integer getWsdcId() {
    return wsdcId;
  }

  public void setWsdcId(Integer wsdcId) {
    this.wsdcId = wsdcId;
  }

  public String getName() {
    return name;
  }

  public void setName(String name) {
    this.name = name;
  }

  public Set<String> getVideoIdList() {
    return videoIdList;
  }

  public void setVideoIdList(Set<String> videoIdList) {
    this.videoIdList = videoIdList;
  }

}
