package ban.controller;

import org.springframework.context.annotation.ComponentScan;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

import java.util.List;

/**
 * Created by bnorrish on 7/17/15.
 */
@RestController
@ComponentScan
public class ListController {

  @RequestMapping(value="/list/{listName}",method = RequestMethod.GET)
  public List<String> getListByName( @PathVariable String listName) {

    return null;
  }



}
