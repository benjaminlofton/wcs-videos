package ban.client;

import com.amazonaws.auth.DefaultAWSCredentialsProviderChain;
import com.amazonaws.regions.Region;
import com.amazonaws.regions.Regions;
import com.amazonaws.services.dynamodbv2.AmazonDynamoDBClient;
import com.amazonaws.services.dynamodbv2.datamodeling.DynamoDBMapper;
import com.amazonaws.services.dynamodbv2.datamodeling.DynamoDBMapperConfig;
import com.amazonaws.services.dynamodbv2.datamodeling.DynamoDBScanExpression;
import com.amazonaws.services.dynamodbv2.document.DeleteItemOutcome;
import com.amazonaws.services.dynamodbv2.document.DynamoDB;
import com.amazonaws.services.dynamodbv2.document.Expected;
import com.amazonaws.services.dynamodbv2.document.Index;
import com.amazonaws.services.dynamodbv2.document.Item;
import com.amazonaws.services.dynamodbv2.document.spec.DeleteItemSpec;
import com.amazonaws.services.dynamodbv2.document.spec.QuerySpec;
import com.amazonaws.services.dynamodbv2.document.utils.ValueMap;
import com.amazonaws.services.dynamodbv2.model.AttributeValue;
import com.amazonaws.services.dynamodbv2.model.ScanRequest;
import com.amazonaws.services.dynamodbv2.model.ScanResult;

import org.springframework.stereotype.Component;

import java.util.ArrayList;
import java.util.HashSet;
import java.util.Iterator;
import java.util.List;
import java.util.Map;
import java.util.Set;

import ban.model.persistence.DanceCategory;
import ban.model.persistence.DancerD;
import ban.model.persistence.EventD;
import ban.model.persistence.FlaggedVideoD;
import ban.model.persistence.ProviderD;
import ban.model.persistence.SkillLevel;
import ban.model.persistence.VideoD;

// This class needs refactoring like a week old zit needs popping
@Component
public class AwsDynamoClient {

  AmazonDynamoDBClient dynamoClient;
  DynamoDBMapper dynamoMapper;

  public AwsDynamoClient() {
    dynamoClient = new AmazonDynamoDBClient(new DefaultAWSCredentialsProviderChain());
    dynamoClient.setRegion(Region.getRegion(Regions.US_WEST_2));
    dynamoMapper = new DynamoDBMapper(dynamoClient);
  }

  public VideoD getVideo(String id) {
    return dynamoMapper.load(VideoD.class,id);
  }

  public List<String> getVideoByProviderVideoId(String providerVideoId) {

    DynamoDB db = new DynamoDB(dynamoClient);
    Index index = db.getTable("Video").getIndex("ProviderVideoId-index");

    QuerySpec querySpec = new QuerySpec()
        .withKeyConditionExpression("ProviderVideoId = :v1")
        .withValueMap(new ValueMap()
            .withString(":v1", providerVideoId));

    Iterator<Item> iterator = index.query(querySpec).iterator();

    List<String> results = new ArrayList<String>();
    while(iterator.hasNext()) {
      results.add(iterator.next().getString("VideoId"));
    }

    return results;
  }

  public FlaggedVideoD createFlaggedVideo(FlaggedVideoD video) {

    video.setFlagId(null);

    // AWS Will throw exception when saving empty Number Set (!!!)
    if(video.getDancerIdList() != null && video.getDancerIdList().isEmpty()) {
      video.setDancerIdList(null);
    }

    dynamoMapper.save(video);
    return video;
  }

  public void removeFlaggedVideo(String flagId) {

    FlaggedVideoD flaggedVideoD = dynamoMapper.load(FlaggedVideoD.class, flagId);

    if(flaggedVideoD == null) {
      return;
    }

    dynamoMapper.delete(flaggedVideoD);
  }

  public FlaggedVideoD getFlaggedVideo(String flagId) {
    return dynamoMapper.load(FlaggedVideoD.class, flagId);
  }

  public List<FlaggedVideoD> getAllFlaggedVideos() {

    DynamoDBScanExpression scanExpression = new DynamoDBScanExpression();

    List<FlaggedVideoD> scanResults = dynamoMapper.scan(FlaggedVideoD.class, scanExpression);

    List<FlaggedVideoD> results = new ArrayList<>();
    for(FlaggedVideoD videoD : scanResults) {
      results.add(videoD);
    }

    return results;
  }

  public VideoD recordSuggestedVideo(VideoD video) {

    // Id will be Dynamo auto-generated UUID
    video.setId(null);

    // AWS Will throw exception when saving empty Number Set (!!!)
    if(video.getDancerIdList() != null && video.getDancerIdList().isEmpty()) {
      video.setDancerIdList(null);
    }

    dynamoMapper.save(video, new DynamoDBMapperConfig(new DynamoDBMapperConfig.TableNameOverride("SuggestedVideo")));
    return video;
  }

  public VideoD createVideo(VideoD video) {

    // Id will be Dynamo auto-generated UUID
    video.setId(null);

    // AWS Will throw exception when saving empty Number Set (!!!)
    if(video.getDancerIdList() != null && video.getDancerIdList().isEmpty()) {
      video.setDancerIdList(null);
    }

    dynamoMapper.save(video);
    return video;
  }

  public VideoD updateVideo(VideoD video) {

    // AWS Will throw exception when saving empty Number Set (!!!)
    if(video.getDancerIdList() != null && video.getDancerIdList().isEmpty()) {
      video.setDancerIdList(null);
    }

    dynamoMapper.save(video);
    return video;
  }

  public DancerD getDancer(Integer wsdcId) {
    return dynamoMapper.load(DancerD.class, wsdcId);
  }

  public void saveDancer(DancerD dancerD) {

    // AWS Will throw exception when saving empty Number Set (!!!)
    if(dancerD.getVideoIdList() != null && dancerD.getVideoIdList().isEmpty()) {
      dancerD.setVideoIdList(null);
    }

    dynamoMapper.save(dancerD);
  }

  /**
   * Gets either all Videos, or all SuggestedVideos
   * todo: The param 'tableName' is horrible LeakyAbstraction!
   * todo: ... why should a service object know a Dynamo Table Name! EWW!
   *
   * @param tableName either "Video" or "SuggestedVideo"
   * @return all Video objects in the given table name
   */
  public List<VideoD> getAllVideos(String tableName) {

    DynamoDBScanExpression scanExpression = new DynamoDBScanExpression();

    DynamoDBMapperConfig config = new DynamoDBMapperConfig(new DynamoDBMapperConfig.TableNameOverride(tableName));
    List<VideoD> scanResults = dynamoMapper.scan(VideoD.class, scanExpression, config);

    List<VideoD> results = new ArrayList<>();
    for(VideoD videoD : scanResults) {
      results.add(videoD);
    }

    return results;
  }

  public List<DancerD> getAllDancers() {

    ScanRequest scanRequest = new ScanRequest()
        .withTableName("Dancer");

    ScanResult scanResult = dynamoClient.scan(scanRequest);

    List<DancerD> result = new ArrayList<>();
    for (Map<String, AttributeValue> item : scanResult.getItems()) {

      DancerD dancerD = new DancerD();
      dancerD.setName(item.get("Name").getS());
      dancerD.setWsdcId(Integer.parseInt(item.get("WsdcId").getN()));

      if(item.get("VideoIdList") != null) {
        dancerD.setVideoIdList(new HashSet<>(item.get("VideoIdList").getSS()));
      }

      result.add(dancerD);
    }

    return result;
  }

  public List<ProviderD> getProviderList() {

    ScanRequest scanRequest = new ScanRequest()
        .withTableName("Provider");

    ScanResult scanResult = dynamoClient.scan(scanRequest);

    List<ProviderD> result = new ArrayList<>();
    for(Map<String,AttributeValue> item : scanResult.getItems()) {

      ProviderD providerD = new ProviderD();
      providerD.setProviderId(Integer.parseInt(item.get("ProviderId").getN()));
      providerD.setBaseUrl(item.get("BaseUrl").getS());
      providerD.setName(item.get("Name").getS());

      result.add(providerD);
    }

    return result;
  }

  public EventD getEventById(String eventId) {
    return dynamoMapper.load(EventD.class, eventId);
  }

  public List<EventD> getEventList() {

    ScanRequest scanRequest = new ScanRequest()
        .withTableName("Event");

    ScanResult scanResult = dynamoClient.scan(scanRequest);

    List<EventD> result = new ArrayList<>();
    for(Map<String,AttributeValue> item : scanResult.getItems()) {

      EventD eventD = new EventD();

      if(item.get("EventDate") != null) {
        eventD.setEventDate(item.get("EventDate").getS());
      }

      if(item.get("LocationName") != null) {
        eventD.setLocationName(item.get("LocationName").getS());
      }

      eventD.setEventId(item.get("EventId").getS());

      // AWS Dynamo is converting Boolean/boolean to Number on Save().  I have no idea why.
      // Must convert back on read.
      eventD.setWsdcPointed(Integer.parseInt(item.get("IsWsdcPointed").getN()) != 0);
      eventD.setName(item.get("Name").getS());

      result.add(eventD);
    }

    return result;
  }

  public EventD addEvent(EventD eventD) {

    // Event ID is an AWS Auto-generated hash key
    eventD.setEventId(null);

    dynamoMapper.save(eventD);
    return eventD;
  }

  public void deleteSuggestedVideo(String videoId) {

    DynamoDB db = new DynamoDB(dynamoClient);

    DeleteItemSpec deleteItemSpec = new DeleteItemSpec()
        .withPrimaryKey("VideoId", videoId);

    DeleteItemOutcome outcome = db.getTable("SuggestedVideo").deleteItem(deleteItemSpec);

    // Do we need to check anything here?
  }
}
