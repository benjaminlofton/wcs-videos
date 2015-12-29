package ban.service;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Component;

import java.util.ArrayList;
import java.util.HashSet;
import java.util.List;
import java.util.Set;
import java.util.function.Predicate;
import java.util.stream.Collectors;

import ban.client.AwsDynamoClient;
import ban.model.persistence.DanceCategory;
import ban.model.persistence.DancerD;
import ban.model.persistence.EventD;
import ban.model.persistence.SkillLevel;
import ban.model.persistence.VideoD;
import ban.model.view.Video;
import ban.service.mapper.VideoMapper;
import ban.util.CollectionUtil;

/**
 * Created by bnorrish on 6/28/15.
 */
@Component
public class VideoSearchService {

  @Autowired
  LocalIndexedDataService localIndexedDataService;

  @Autowired
  VideoMapper videoMapper;

  @Autowired
  AwsDynamoClient awsDynamoClient;

  /**
   * Get list of Videos by provided search parameters
   * If any parameter is null, it is ignored.
   * If all parameters are null, all videos are returned (!!!)
   *
   * @param wsdcIdList - comma separated list of wsdc Ids
   * @param titleFragList - comma separated list of title words
   * @param eventIdList - comma separated list of event Ids
   * @param strLevel - string representation of SkillLevel
   * @param strCategory - string representation of DanceCategory
   * @return List of video objects matching all non-null search parameters
   */
  public List<Video> search(String wsdcIdList, String titleFragList, String eventIdList, String strLevel, String strCategory) {

    // This should / will throw exception if strings cannot be mapped to Enum values
    SkillLevel level = strLevel == null ? null : SkillLevel.valueOf(strLevel);
    DanceCategory category = strCategory == null ? null : DanceCategory.valueOf(strCategory);

    List<VideoD> videos = localIndexedDataService.getAllVideos();

    return videos.stream()
                 .filter(keepWithLevel(level))
                 .filter(keepWithCategory(category))
                 .filter(keepWithAnyWsdc(wsdcIdList))
                 .filter(keepWithAnyTitleFrag(titleFragList))
                 .filter(keepWithAnyEventId(eventIdList))
                 .map(videoMapper::mapToViewModel)
                 .collect(Collectors.toList());
  }

  public static Predicate<VideoD> keepWithLevel(SkillLevel level) {
    return d -> {
      if(level == null) {
        return true;
      }

      if(d.getSkillLevel() == null) {
        return false;
      }

      return d.getSkillLevel().equals(level);
    };
  }

  public static Predicate<VideoD> keepWithCategory(DanceCategory category) {
    return d -> {
      if(category == null) {
        return true;
      }

      if(d.getDanceCategory() == null) {
        return false;
      }

      return d.getDanceCategory().equals(category);
    };
  }

  /**
   * Returns true if VideoD.getEvent is equal to ANY of the events in eventIdList
   * ... OR if eventIdList is null
   */
  public static Predicate<VideoD> keepWithAnyEventId(String eventIdList) {
    return d -> {
      if(eventIdList == null) {
        return true;
      }

      if(d.getEventId() == null) {
        return false;
      }

      for(String eventId : eventIdList.split(",")) {
        if(d.getEventId().equals(eventId)) {
          return true;
        }
      }

      return false;
    };
  }

  /**
   * Returns true if VideoD Title contains ANY of the titleFrag strings in titleFragList
   * ... OR if titleFragList is null
   */
  public static Predicate<VideoD> keepWithAnyTitleFrag(String titleFragList) {
    return d -> {
      if(titleFragList == null) {
        return true;
      }

      if(d.getTitle() == null) {
        return false;
      }

      for(String titleFrag : titleFragList.split(",")) {
        if(d.getTitle().toLowerCase().contains(titleFrag.toLowerCase())) {
          return true;
        }
      }

      return false;
    };
  }

  /**
   * Returns true if VideoD contains a wsdc id matching ANY in wsdcIDList
   * .. OR if wsdcIDList is null
   */
  public static Predicate<VideoD> keepWithAnyWsdc(String wsdcIdList) {
    return d -> {
      if(wsdcIdList == null) {
        return true;
      }

      if(d.getDancerIdList() == null) {
        return false;
      }

      for(String wsdcId : wsdcIdList.split(",")) {

        if(d.getDancerIdList().contains(Integer.parseInt(wsdcId))) {
          return true;
        }
      }

      return false;
    };
  }
}
