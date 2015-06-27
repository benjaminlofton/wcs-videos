package ban.controller;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.context.annotation.ComponentScan;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;
import org.springframework.web.bind.annotation.RestController;

import ban.model.view.WscVideoStats;
import ban.service.StatsService;

/**
 * Created by bnorrish on 6/26/15.
 */

@RestController
@ComponentScan
public class StatsController {

  @Autowired
  StatsService statsService;

  @RequestMapping(value="/stats", method = RequestMethod.GET)
  public WscVideoStats getStatistics() {

    WscVideoStats stats = statsService.getBasicStats();

    return stats;
  }

}
