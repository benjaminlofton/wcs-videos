package ban.service;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Component;

import java.util.ArrayList;
import java.util.List;
import java.util.function.Predicate;
import java.util.stream.Collectors;

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

  public List<Video> search(String wsdcIdList, String titleFragList, String eventIdList, String strLevel) {

    // This should / will throw exception if strLevel cannot be mapped to SkillLevel Enum
    SkillLevel level = strLevel == null ? null : SkillLevel.valueOf(strLevel);

    List<VideoD> videos = localIndexedDataService.getAllVideos();

    return videos.stream()
                 .filter(keepWithLevel(level))
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

  /**
   * Get list of Videos by provided search parameters
   * If any parameter is null, it is ignored.
   * If all parameters are null, all videos are returned (!!!)
   * This is an inefficient algorithm, as it does each search on the full set of Videos, then merges.
   *
   * @param wsdcIdList - comma separated list of wsdc Ids
   * @param titleFragList - comma separated list of title words
   * @param eventIdList - comma separated list of event Ids
   * @return List of video objects matching all non-null search parameters
   */
//  public List<Video> searchOld(String wsdcIdList, String titleFragList, String eventIdList, String level) {
//
//    if(wsdcIdList == null && titleFragList == null && eventIdList == null && level == null) {
//      return videoMapper.mapToViewModel(localIndexedDataService.getAllVideos());
//    }
//
//    List<Video> wsdcIdSearchResults = null;
//    if(wsdcIdList != null) {
//      wsdcIdSearchResults = new ArrayList<>();
//
//      for(String wsdcId : wsdcIdList.split(",")) {
//
//        List<Video> singleWsdcIdResultList = videoMapper
//            .mapToViewModel(localIndexedDataService.getVideosByDancerWsdcId(wsdcId));
//
//        if(wsdcIdSearchResults.isEmpty()) {
//          wsdcIdSearchResults.addAll(singleWsdcIdResultList);
//        } else {
//          wsdcIdSearchResults = CollectionUtil.merge(wsdcIdSearchResults, singleWsdcIdResultList);
//        }
//      }
//    }
//
//    List<Video> eventSerchResults = null;
//    if(eventIdList != null) {
//      eventSerchResults = new ArrayList<>();
//
//      for(String eventId : eventIdList.split(",")) {
//
//        List<Video> singleEventResultsList = videoMapper
//            .mapToViewModel(localIndexedDataService.getVideosByEventId(eventId));
//
//        if(eventSerchResults.isEmpty()) {
//          eventSerchResults.addAll(singleEventResultsList);
//        } else {
//          eventSerchResults = CollectionUtil.merge(eventSerchResults, singleEventResultsList);
//        }
//      }
//    }
//
//    List<Video> titleFragSearchResults = null;
//    if(titleFragList != null) {
//      titleFragSearchResults = new ArrayList<>();
//
//      for(String titleFrag : titleFragList.split(",")) {
//
//        List<Video> singleWordTitleFragResults = videoMapper
//            .mapToViewModel(localIndexedDataService.getVideosBySingleWordTitleFrag(titleFrag));
//
//        if(titleFragSearchResults.isEmpty()) {
//          titleFragSearchResults.addAll(singleWordTitleFragResults);
//        } else {
//          titleFragSearchResults = CollectionUtil.merge(titleFragSearchResults, singleWordTitleFragResults);
//        }
//      }
//    }
//
//    return CollectionUtil.merge(wsdcIdSearchResults, titleFragSearchResults, eventSerchResults);
//  }


}
