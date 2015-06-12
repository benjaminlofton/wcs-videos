package ban.controller;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.context.annotation.ComponentScan;
import org.springframework.http.HttpStatus;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;
import org.springframework.web.bind.annotation.ResponseStatus;
import org.springframework.web.bind.annotation.RestController;

import ban.model.view.Video;
import ban.service.VideoAddService;

/**
 * Created by bnorrish on 6/10/15.
 */

@RestController
@ComponentScan
public class VideoController {

  @Autowired
  VideoAddService videoAddService;

  @ResponseStatus(HttpStatus.OK)
  @RequestMapping(value="/v", method = RequestMethod.POST)
  public Video putVideo(@RequestBody Video video)
  {
    return videoAddService.addVideo(video);
  }

}
