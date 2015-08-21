package ban.model.view;

import java.util.List;

/**
 * Created by bnorrish on 8/20/15.
 */
public class ResourceList {

  private String name;
  private String resourceType;
  private List<String> ids;

  public String getName() {
    return name;
  }

  public void setName(String name) {
    this.name = name;
  }

  public List<String> getIds() {
    return ids;
  }

  public void setIds(List<String> ids) {
    this.ids = ids;
  }

  public String getResourceType() {
    return resourceType;
  }

  public void setResourceType(String resourceType) {
    this.resourceType = resourceType;
  }
}
