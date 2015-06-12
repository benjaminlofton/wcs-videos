package ban.service;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Component;

import java.util.ArrayList;
import java.util.List;
import java.util.Set;

import ban.client.AwsDynamoClient;
import ban.model.view.Dancer;

/**
 * Created by bnorrish on 6/7/15.
 */
@Component
public class DancerService {

  @Autowired
  DancerMapper dancerMapper;

  @Autowired
  AwsDynamoClient awsDynamoClient;

  public Set<String> getVideosByWsdcId(int wsdcId) {
    return awsDynamoClient.getDancer(wsdcId).getVideoIdList();
  }

  public Dancer getDancer(Integer wsdcId) {
    return dancerMapper.mapToViewModel(awsDynamoClient.getDancer(wsdcId));
  }

}
