package ban.controller;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.context.annotation.ComponentScan;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

import java.util.List;

import ban.exception.ResourceNotFoundException;
import ban.model.view.Video;
import ban.service.DancerService;
import ban.service.VideoService;

@RestController
@ComponentScan
public class VideoSearchController {

  @Autowired
  VideoService videoService;

  @Autowired
  DancerService dancerService;

  @RequestMapping(value="/v/{videoId}",method = RequestMethod.GET)
  public Video getVideo(
      @PathVariable String videoId
  ) {

    Video video =  videoService.getVideo(videoId);

    if(video == null) {
      throw new ResourceNotFoundException();
    }

    return video;
  }

  @RequestMapping(value="/v", method = RequestMethod.GET)
  public List<Video> searchVideos(
      @RequestParam(value = "wsdc-id", required = false) Integer wsdcId) {

    if(wsdcId == null) {
      return videoService.getAllVideos();
    }

    return dancerService.getVideosByDancerWsdcId(wsdcId);

  }

}