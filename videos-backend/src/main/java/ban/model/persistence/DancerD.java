package ban.model.persistence;

import com.amazonaws.services.dynamodbv2.datamodeling.DynamoDBAttribute;
import com.amazonaws.services.dynamodbv2.datamodeling.DynamoDBHashKey;
import com.amazonaws.services.dynamodbv2.datamodeling.DynamoDBIndexHashKey;
import com.amazonaws.services.dynamodbv2.datamodeling.DynamoDBTable;

import java.util.List;
import java.util.Set;

/**
 * Created by bnorrish on 6/11/15.
 */
@DynamoDBTable(tableName="Dancer")
public class DancerD {

  @DynamoDBHashKey(attributeName="WsdcId")
  private Integer wsdcId;

  @DynamoDBIndexHashKey(attributeName="Name")
  private String name;

  @DynamoDBAttribute(attributeName="VideoIdList")
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
