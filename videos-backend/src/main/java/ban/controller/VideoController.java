package ban.controller;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.context.annotation.ComponentScan;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.CrossOrigin;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.ResponseStatus;
import org.springframework.web.bind.annotation.RestController;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;

import ban.exception.ResourceNotFoundException;
import ban.model.view.Video;
import ban.service.VideoSearchService;
import ban.service.VideoService;

/**
 * Created by bnorrish on 6/10/15.
 */

@RestController
@ComponentScan
public class VideoController {

  @Autowired
  VideoService videoService;

  @Autowired
  VideoSearchService videoSearchService;

  @ResponseStatus(HttpStatus.OK)
  @RequestMapping(value="/v", method = RequestMethod.POST)
  public Video putVideo(@RequestBody Video video)
  {
    return videoService.addOrUpdateVideo(video);
  }

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

  @CrossOrigin
  @RequestMapping(value="/v", method = RequestMethod.GET)
  public ResponseEntity<List<Video>> searchVideos(
      @RequestParam(value = "wsdc-id", required = false) String wsdcIdList,
      @RequestParam(value = "level", required = false) String level,
      @RequestParam(value = "category", required = false) String category,
      @RequestParam(value = "title-frag", required =  false) String titleFragList,
      @RequestParam(value = "provider-id", required = false) String providerVideoId,
      @RequestParam(value = "event-id", required = false) String eventIdList) {

    if(wsdcIdList == null && titleFragList == null && providerVideoId == null && eventIdList == null && level == null && category == null) {
      return new ResponseEntity<>(HttpStatus.BAD_REQUEST);
    }

    if(providerVideoId != null) {

      if(providerVideoId.isEmpty()) {
        return new ResponseEntity<>(HttpStatus.BAD_REQUEST);
      }

      Video v = videoService.getByProviderId(providerVideoId);
      if (v == null) {
        return new ResponseEntity<>(new ArrayList<>(),HttpStatus.OK);
      }

      return new ResponseEntity<>(Arrays.asList(v),HttpStatus.OK);
    }

    List<Video> videos = videoSearchService.search(wsdcIdList,titleFragList,eventIdList,level, category);
    return new ResponseEntity<>(videos,HttpStatus.OK);
  }

}
