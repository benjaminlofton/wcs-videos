package ban.controller;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.context.annotation.ComponentScan;
import org.springframework.web.bind.annotation.CrossOrigin;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

import java.util.List;

import ban.model.persistence.FlaggedVideoD;
import ban.service.FlaggedVideoService;

/**
 * Created by bnorrish on 9/14/15.
 */
@RestController
@ComponentScan
@CrossOrigin
public class FlaggedVideoController {

  @Autowired
  FlaggedVideoService flaggedVideoService;


  @RequestMapping(value="/flagged/v", method = RequestMethod.POST)
  public FlaggedVideoD postFlaggedVideo(@RequestBody FlaggedVideoD flaggedVideo)
  {
    return flaggedVideoService.flagVideo(flaggedVideo);
  }

  @RequestMapping(value="/flagged/v/{flagId}", method = RequestMethod.GET)
  public FlaggedVideoD getFlaggedVideo(@PathVariable String flagId) {
    return flaggedVideoService.getFlaggedVideo(flagId);
  }

  @RequestMapping(value="/flagged/v", method = RequestMethod.GET)
  public List<FlaggedVideoD> getFlaggedVideos(@RequestParam(value = "videoid", required = false) String videoId) {

    if (videoId == null) {
      return flaggedVideoService.getAllFlaggedVideos();
    }

    return flaggedVideoService.getAllFlaggedByVideoId(videoId);

  }

  @RequestMapping(value="/flagged/v/{flagId}", method = RequestMethod.DELETE)
  public void deleteFlaggedVideo(@PathVariable String flagId) {
    flaggedVideoService.deleteFlaggedVideo(flagId);
  }

}
