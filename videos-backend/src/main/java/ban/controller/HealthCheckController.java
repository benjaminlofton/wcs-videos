package ban.controller;

import org.springframework.context.annotation.ComponentScan;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;
import org.springframework.web.bind.annotation.RestController;

import ban.model.view.HealthCheck;

@RestController
@ComponentScan
public class HealthCheckController {

  @RequestMapping(value="/healthcheck",method=RequestMethod.GET)
  public HealthCheck healthCheck() {
    HealthCheck hc = new HealthCheck();
    hc.setName("video-service");
    hc.setVersion("0.0.1");
    return hc;
  }

  @RequestMapping("/")
  public String index() {
    return "Greetings from Spring Boot!";
  }
}
