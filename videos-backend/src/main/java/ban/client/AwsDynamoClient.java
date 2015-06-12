package ban.client;

import com.amazonaws.auth.profile.ProfileCredentialsProvider;
import com.amazonaws.regions.Region;
import com.amazonaws.regions.Regions;
import com.amazonaws.services.dynamodbv2.AmazonDynamoDBClient;
import com.amazonaws.services.dynamodbv2.datamodeling.DynamoDBMapper;

import org.springframework.stereotype.Component;

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

}
