package ban.model.persistence;

import com.amazonaws.services.dynamodbv2.datamodeling.DynamoDBAttribute;
import com.amazonaws.services.dynamodbv2.datamodeling.DynamoDBAutoGeneratedKey;
import com.amazonaws.services.dynamodbv2.datamodeling.DynamoDBHashKey;
import com.amazonaws.services.dynamodbv2.datamodeling.DynamoDBIndexHashKey;
import com.amazonaws.services.dynamodbv2.datamodeling.DynamoDBMarshalling;
import com.amazonaws.services.dynamodbv2.datamodeling.DynamoDBTable;

import java.util.HashSet;
import java.util.Set;

/**
 * Video class as persisted in AWS Dynamo
 */

@DynamoDBTable(tableName="Video")
public class VideoD {

  @DynamoDBAutoGeneratedKey
  @DynamoDBHashKey(attributeName="VideoId")
  private String id;

  @DynamoDBAttribute(attributeName="ProviderId")
  private Integer providerId;

  @DynamoDBIndexHashKey(globalSecondaryIndexName = "ProviderVideoId-index", attributeName="ProviderVideoId")
  private String providerVideoId;

  @DynamoDBAttribute(attributeName="Title")
  private String title;

  @DynamoDBAttribute(attributeName="DancerIdList")
  private Set<Integer> dancerIdList;

  @DynamoDBAttribute(attributeName="EventId")
  private String eventId;

  @DynamoDBAttribute(attributeName="CreatedDateTime")
  private String createdDateTime;

  @DynamoDBAttribute(attributeName = "SkillLevel")
  @DynamoDBMarshalling(marshallerClass = SkillLevelConverter.class)
  private SkillLevel skillLevel;

  @DynamoDBAttribute(attributeName = "DanceCategory")
  @DynamoDBMarshalling(marshallerClass = DanceCategoryConverter.class)
  private DanceCategory danceCategory;

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

  public Set<Integer> getDancerIdList() { return dancerIdList; }
  public void setDancerIdList(Set<Integer> dancerIdList) { this.dancerIdList = dancerIdList; }

  public String getEventId() {
    return eventId;
  }
  public void setEventId(String eventId) {
    this.eventId = eventId;
  }

  public String getCreatedDateTime() {
    return createdDateTime;
  }
  public void setCreatedDateTime(String createdDateTime) {
    this.createdDateTime = createdDateTime;
  }

  public SkillLevel getSkillLevel() {
    return skillLevel;
  }
  public void setSkillLevel(SkillLevel skillLevel) {
    this.skillLevel = skillLevel;
  }

  public DanceCategory getDanceCategory() {
    return danceCategory;
  }
  public void setDanceCategory(DanceCategory danceCategory) {
    this.danceCategory = danceCategory;
  }
}
