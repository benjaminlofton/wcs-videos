package ban.controller;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.context.annotation.ComponentScan;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

import java.util.List;

import ban.model.view.Dancer;
import ban.model.view.WscVideoStats;
import ban.service.DancerService;
import ban.service.LocalIndexedDataService;
import ban.service.StatsService;
import ban.service.cache.DancerCache;

/**
 * Created by bnorrish on 7/11/15.
 */
@RestController
@ComponentScan
public class AdminController {

  @Autowired
  DancerService dancerService;

  @Autowired
  LocalIndexedDataService localIndexedDataService;

  @Autowired
  DancerCache dancerCache;

  @Autowired
  StatsService statsService;

  @RequestMapping(value="/admin/add-by-frag", method = RequestMethod.POST)
  public List<Dancer> addDancersByFrag(@RequestParam(value = "frag", required = true) String fragment) {
    return dancerService.addDancersByFragment(fragment);
  }

  @RequestMapping(value="/admin/cache-reset", method = RequestMethod.POST)
  public WscVideoStats resetCache() {
    localIndexedDataService.clear();
    localIndexedDataService.load();

    dancerCache.clear();

    return statsService.getBasicStats();
  }

}
