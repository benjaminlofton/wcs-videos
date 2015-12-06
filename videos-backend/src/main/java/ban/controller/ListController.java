package ban.controller;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.context.annotation.ComponentScan;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

import java.util.List;

import ban.model.view.ResourceList;
import ban.service.list.ListService;

/**
 * Created by bnorrish on 7/17/15.
 */
@RestController
@ComponentScan
public class ListController {

  @Autowired
  ListService listService;

  @RequestMapping(value="/list/{listName}",method = RequestMethod.GET)
  public ResourceList getListByName(
      @PathVariable String listName,
      @RequestParam(value = "skip", required = false) Integer skip,
      @RequestParam(value = "take", required = false) Integer take) {

    return listService.getListByName(listName, skip, take);
  }

  @RequestMapping(value="/list/", method = RequestMethod.GET)
  public List<String> getAllListNames() {
    return listService.getAllListNames();
  }

}
