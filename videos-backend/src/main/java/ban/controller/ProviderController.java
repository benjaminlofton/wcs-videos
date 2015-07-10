package ban.controller;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.context.annotation.ComponentScan;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;
import org.springframework.web.bind.annotation.RestController;

import java.util.List;

import ban.model.view.Provider;
import ban.service.ProviderService;

/**
 * Created by bnorrish on 7/9/15.
 */
@RestController
@ComponentScan
public class ProviderController {

  @Autowired
  ProviderService providerService;

  @RequestMapping(value="/provider", method = RequestMethod.GET)
  public List<Provider> getProvider() {

    return providerService.getProviderList();
  }

}
