package ban.service;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Component;

import java.util.List;

import ban.client.AwsDynamoClient;

/**
 * Created by bnorrish on 7/17/15.
 */
@Component
public class ListService {

  @Autowired
  AwsDynamoClient awsDynamoClient;

  public List<String> getListByName(String name) {

    return null;
  }
}
