package ban.controller;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.context.annotation.ComponentScan;
import org.springframework.http.HttpStatus;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;
import org.springframework.web.bind.annotation.ResponseStatus;
import org.springframework.web.bind.annotation.RestController;

import java.util.List;

import ban.model.view.Video;
import ban.service.SuggestedVideoService;

/**
 * Created by bnorrish on 10/18/15.
 */
@RestController
@ComponentScan
public class SuggestedVideoController {

  @Autowired
  SuggestedVideoService suggestedVideoService;

  @RequestMapping(value="/sv", method = RequestMethod.POST)
  public Video putVideo(@RequestBody Video video) {

    return suggestedVideoService.recordSuggestedVideo(video);
  }

  @RequestMapping(value="sv", method = RequestMethod.GET)
  public List<Video> getSuggestedVideos() {

    return suggestedVideoService.getSuggestedVideos();
  }

  @RequestMapping(value="sv/{videoId}", method = RequestMethod.DELETE)
  public void deleteSuggestedVideo(@PathVariable String videoId) {

    suggestedVideoService.deleteVideo(videoId);

  }

}
