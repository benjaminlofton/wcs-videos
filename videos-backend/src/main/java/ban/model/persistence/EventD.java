package ban.model.persistence;

import com.amazonaws.services.dynamodbv2.datamodeling.DynamoDBAttribute;
import com.amazonaws.services.dynamodbv2.datamodeling.DynamoDBAutoGeneratedKey;
import com.amazonaws.services.dynamodbv2.datamodeling.DynamoDBHashKey;
import com.amazonaws.services.dynamodbv2.datamodeling.DynamoDBTable;

/**
 * Created by bnorrish on 7/9/15.
 */
@DynamoDBTable(tableName="Event")
public class EventD {


  private String eventId;
  private String name;
  private Boolean isWsdcPointed;
  private String eventDate;

  @DynamoDBAutoGeneratedKey
  @DynamoDBHashKey(attributeName="EventId")
  public String getEventId() {
    return eventId;
  }

  public void setEventId(String eventId) {
    this.eventId = eventId;
  }

  @DynamoDBAttribute(attributeName="Name")
  public String getName() {
    return name;
  }

  public void setName(String name) {
    this.name = name;
  }

  @DynamoDBAttribute(attributeName="IsWsdcPointed")
  public Boolean isWsdcPointed() {
    return isWsdcPointed;
  }

  public void setWsdcPointed(Boolean isWsdcPointed) {
    this.isWsdcPointed = isWsdcPointed;
  }

  @DynamoDBAttribute(attributeName="EventDate")
  public String getEventDate() {
    return eventDate;
  }

  public void setEventDate(String eventDate) {
    this.eventDate = eventDate;
  }
}