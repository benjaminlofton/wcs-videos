package ban.model.persistence;

import java.util.List;

/**
 * Created by bnorrish on 7/17/15.
 */
public class IdListD {

  private String name;
  private List<String> listIds;

  public String getName() {
    return name;
  }

  public void setName(String name) {
    this.name = name;
  }

  public List<String> getListIds() {
    return listIds;
  }

  public void setListIds(List<String> listIds) {
    this.listIds = listIds;
  }
}
