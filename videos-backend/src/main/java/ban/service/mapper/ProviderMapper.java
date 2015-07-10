package ban.service.mapper;

import org.springframework.stereotype.Component;

import ban.model.persistence.ProviderD;
import ban.model.view.Provider;

/**
 * Created by bnorrish on 7/9/15.
 */
@Component
public class ProviderMapper {

  public ProviderD mapToPersistenceModel(Provider provider) {

    ProviderD providerD = new ProviderD();
    providerD.setBaseUrl(provider.getBaseUrl());
    providerD.setProviderId(provider.getProviderId());
    providerD.setName(provider.getName());
    return providerD;
  }

  public Provider mapToViewModel(ProviderD provderD) {
    Provider provider = new Provider();
    provider.setBaseUrl(provderD.getBaseUrl());
    provider.setProviderId(provderD.getProviderId());
    provider.setName(provderD.getName());
    return provider;
  }

}
