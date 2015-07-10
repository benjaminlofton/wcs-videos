package ban.model.persistence;

import com.amazonaws.services.dynamodbv2.datamodeling.DynamoDBAttribute;
import com.amazonaws.services.dynamodbv2.datamodeling.DynamoDBHashKey;
import com.amazonaws.services.dynamodbv2.datamodeling.DynamoDBTable;

/**
 * Created by bnorrish on 7/9/15.
 */
@DynamoDBTable(tableName="Provider")
public class ProviderD {

  @DynamoDBHashKey(attributeName="ProviderId")
  private int providerId;

  @DynamoDBAttribute(attributeName="Name")
  private String name;

  @DynamoDBAttribute(attributeName="BaseUrl")
  private String baseUrl;

  public String getName() {
    return name;
  }

  public void setName(String name) {
    this.name = name;
  }

  public int getProviderId() {
    return providerId;
  }

  public void setProviderId(int providerId) {
    this.providerId = providerId;
  }

  public String getBaseUrl() {
    return baseUrl;
  }

  public void setBaseUrl(String baseUrl) {
    this.baseUrl = baseUrl;
  }
}
