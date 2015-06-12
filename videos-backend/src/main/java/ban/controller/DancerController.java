package ban.controller;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.context.annotation.ComponentScan;
import org.springframework.http.HttpStatus;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;
import org.springframework.web.bind.annotation.ResponseStatus;
import org.springframework.web.bind.annotation.RestController;

import ban.model.view.Dancer;
import ban.service.DancerService;

/**
 * Created by bnorrish on 6/11/15.
 */
@RestController
@ComponentScan
public class DancerController {

    @Autowired
    DancerService dancerService;

    @ResponseStatus(HttpStatus.OK)
    @RequestMapping(value="/d/{wsdcId}", method = RequestMethod.GET)
    public Dancer getDancer(@PathVariable Integer wsdcId)
    {
      return dancerService.getDancer(wsdcId);
    }

}
