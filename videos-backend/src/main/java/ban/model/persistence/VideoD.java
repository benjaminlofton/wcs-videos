package ban.model.persistence;

import com.amazonaws.services.dynamodbv2.datamodeling.DynamoDBAttribute;
import com.amazonaws.services.dynamodbv2.datamodeling.DynamoDBHashKey;
import com.amazonaws.services.dynamodbv2.datamodeling.DynamoDBTable;

/**
 * Video class as persisted in AWS Dynamo
 */

@DynamoDBTable(tableName="Video")
public class VideoD {

  @DynamoDBHashKey(attributeName="VideoId")
  private String id;

  @DynamoDBAttribute(attributeName="ProviderId")
  private Integer providerId;

  @DynamoDBAttribute(attributeName="ProviderVideoId")
  private String providerVideoId;

  @DynamoDBAttribute(attributeName="Title")
  private String title;

  public String getId() {
    return id;
  }
  public void setId(String id) {
    this.id = id;
  }

  public Integer getProviderId() { return providerId; }
  public void setProviderId(Integer providerId) { this.providerId = providerId; }

  public String getProviderVideoId() { return providerVideoId; }
  public void setProviderVideoId(String providerVideoId) { this.providerVideoId = providerVideoId; }

  public String getTitle() { return title; }
  public void setTitle(String title) { this.title = title;}
}
