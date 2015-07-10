package ban.service;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Component;

import java.util.ArrayList;
import java.util.List;

import ban.client.AwsDynamoClient;
import ban.model.persistence.ProviderD;
import ban.model.view.Provider;
import ban.service.mapper.ProviderMapper;

/**
 * Created by bnorrish on 7/9/15.
 */
@Component
public class ProviderService {

  @Autowired
  AwsDynamoClient awsDynamoClient;

  @Autowired
  ProviderMapper providerMapper;

  public List<Provider> getProviderList() {

    List<ProviderD> providerDs = awsDynamoClient.getProviderList();

    List<Provider> result = new ArrayList<Provider>();
    for (ProviderD providerD : providerDs) {
      result.add(providerMapper.mapToViewModel(providerD));
    }

    return result;
  }
}
