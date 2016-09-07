package ban.controller;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.context.annotation.ComponentScan;
import org.springframework.http.HttpStatus;
import org.springframework.web.bind.annotation.CrossOrigin;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.ResponseStatus;
import org.springframework.web.bind.annotation.RestController;

import java.util.List;
import java.util.Map;

import ban.model.view.Dancer;
import ban.service.DancerService;

/**
 * Created by bnorrish on 6/11/15.
 */
@RestController
@ComponentScan
@CrossOrigin
public class DancerController {

  @Autowired
  DancerService dancerService;

  @RequestMapping(value="/d/{wsdcId}", method = RequestMethod.PUT)
  public void putVideo(@PathVariable Integer wsdcId, @RequestBody Dancer dancer) {

    if(!dancer.getWsdcId().equals(wsdcId)) {
      throw new IllegalStateException("Cannot put /d/; wsdcId not equal to wsdcId in RequestBody");
    }

    // Put dancer is Idempotent; don't check for existing dancer
    // However, this will overwrite (delete) video list

    dancerService.addDancer(dancer);
  }

  @RequestMapping(value="/d/{wsdcId}", method = RequestMethod.GET)
  public Dancer getDancer(@PathVariable Integer wsdcId) {
    return dancerService.getDancer(wsdcId);
  }

  @RequestMapping(value="/d", method = RequestMethod.GET)
  public List<Dancer> getDancerList() {
    return dancerService.getDancerList();
  }

}
