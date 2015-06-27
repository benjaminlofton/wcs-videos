package ban.client;

import com.amazonaws.auth.profile.ProfileCredentialsProvider;
import com.amazonaws.regions.Region;
import com.amazonaws.regions.Regions;
import com.amazonaws.services.dynamodbv2.AmazonDynamoDBClient;
import com.amazonaws.services.dynamodbv2.datamodeling.DynamoDBMapper;
import com.amazonaws.services.dynamodbv2.datamodeling.DynamoDBQueryExpression;
import com.amazonaws.services.dynamodbv2.document.DynamoDB;
import com.amazonaws.services.dynamodbv2.document.Index;
import com.amazonaws.services.dynamodbv2.document.Item;
import com.amazonaws.services.dynamodbv2.document.ItemCollection;
import com.amazonaws.services.dynamodbv2.document.KeyAttribute;
import com.amazonaws.services.dynamodbv2.document.QueryOutcome;
import com.amazonaws.services.dynamodbv2.document.spec.QuerySpec;
import com.amazonaws.services.dynamodbv2.document.utils.ValueMap;
import com.amazonaws.services.dynamodbv2.model.AttributeValue;
import com.amazonaws.services.dynamodbv2.model.ScanRequest;
import com.amazonaws.services.dynamodbv2.model.ScanResult;

import org.springframework.stereotype.Component;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.HashMap;
import java.util.HashSet;
import java.util.Iterator;
import java.util.List;
import java.util.Map;
import java.util.Set;

import ban.model.persistence.DancerD;
import ban.model.persistence.VideoD;
import ban.service.DancerMapper;

@Component
public class AwsDynamoClient {

  AmazonDynamoDBClient dynamoClient;
  DynamoDBMapper dynamoMapper;

  public AwsDynamoClient() {
    dynamoClient = new AmazonDynamoDBClient(new ProfileCredentialsProvider());
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

  public VideoD createVideo(VideoD video) {

    // Id will be Dynamo auto-generated UUID
    video.setId(null);

    dynamoMapper.save(video);
    return video;
  }

  public DancerD getDancer(Integer wsdcId) {
    return dynamoMapper.load(DancerD.class, wsdcId);
  }

  public void saveDancer(DancerD dancerD) {
    dynamoMapper.save(dancerD);
  }

  public List<VideoD> getAllVideos() {

    ScanRequest scanRequest = new ScanRequest()
        .withTableName("Video");

    ScanResult scanResult = dynamoClient.scan(scanRequest);

    List<VideoD> result = new ArrayList<>();
    for(Map<String,AttributeValue> item : scanResult.getItems()) {

      VideoD videoD = new VideoD();
      videoD.setId(item.get("VideoId").getS());
      videoD.setProviderId(Integer.parseInt(item.get("ProviderId").getN()));
      videoD.setProviderVideoId(item.get("ProviderVideoId").getS());
      videoD.setTitle(item.get("Title").getS());

      if(item.get("DancerIdList") != null) {

        Set<Integer> dancerList = new HashSet<>();

        List<String> rawDancerList = item.get("DancerIdList").getNS();
        for(String strId : rawDancerList) {
          dancerList.add(Integer.parseInt(strId));
        }

        videoD.setDancerIdList(dancerList);
      }

      result.add(videoD);
    }

    return result;

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



}
