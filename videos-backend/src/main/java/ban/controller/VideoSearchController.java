package ban.controller;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.context.annotation.ComponentScan;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

import java.util.ArrayList;
import java.util.List;

import ban.exception.ResourceNotFoundException;
import ban.model.view.Video;
import ban.service.DancerSearchService;
import ban.service.VideoSearchService;

@RestController
@ComponentScan
public class VideoSearchController {

  @Autowired
  VideoSearchService videoSearchService;

  @Autowired
  DancerSearchService dancerSearchService;

  @RequestMapping(value="/v/{videoId}",method= RequestMethod.GET)
  public Video getVideo(
      @PathVariable String videoId
  ) {

    Video video =  videoSearchService.getVideo(videoId);

    if(video == null) {
      throw new ResourceNotFoundException();
    }

    return video;
  }

  @RequestMapping(value="/v")
  public List<Video> searchVideos(
      @RequestParam(value = "wsdc-id", required = false) Integer wsdcId) {

    List<Video> videos = new ArrayList<Video>();

    for(Integer videoId : dancerSearchService.getVideosByWsdcId(wsdcId)) {

      Video v = videoSearchService.getVideo(videoId.toString());

      if(v!=null) {
        videos.add(v);
      }
    }

    return videos;
  }

}
