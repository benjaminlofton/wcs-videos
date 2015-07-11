package ban.service;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Component;

import java.util.ArrayList;
import java.util.List;

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
  public List<Video> search(String wsdcIdList, String titleFragList, String eventIdList) {

    if(wsdcIdList == null && titleFragList == null && eventIdList == null) {
      return videoMapper.mapToViewModel(localIndexedDataService.getAllVideos());
    }

    List<Video> wsdcIdSearchResults = null;
    if(wsdcIdList != null) {
      wsdcIdSearchResults = new ArrayList<>();

      for(String wsdcId : wsdcIdList.split(",")) {

        List<Video> singleWsdcIdResultList = videoMapper
            .mapToViewModel(localIndexedDataService.getVideosByDancerWsdcId(wsdcId));

        if(wsdcIdSearchResults.isEmpty()) {
          wsdcIdSearchResults.addAll(singleWsdcIdResultList);
        } else {
          wsdcIdSearchResults = CollectionUtil.merge(wsdcIdSearchResults, singleWsdcIdResultList);
        }
      }
    }

    List<Video> eventSerchResults = null;
    if(eventIdList != null) {
      eventSerchResults = new ArrayList<>();

      for(String eventId : eventIdList.split(",")) {

        List<Video> singleEventResultsList = videoMapper
            .mapToViewModel(localIndexedDataService.getVideosByEventId(eventId));

        if(eventSerchResults.isEmpty()) {
          eventSerchResults.addAll(singleEventResultsList);
        } else {
          eventSerchResults = CollectionUtil.merge(eventSerchResults, singleEventResultsList);
        }
      }
    }

    List<Video> titleFragSearchResults = null;
    if(titleFragList != null) {
      titleFragSearchResults = new ArrayList<>();

      for(String titleFrag : titleFragList.split(",")) {

        List<Video> singleWordTitleFragResults = videoMapper
            .mapToViewModel(localIndexedDataService.getVideosBySingleWordTitleFrag(titleFrag));

        if(titleFragSearchResults.isEmpty()) {
          titleFragSearchResults.addAll(singleWordTitleFragResults);
        } else {
          titleFragSearchResults = CollectionUtil.merge(titleFragSearchResults, singleWordTitleFragResults);
        }
      }
    }

    return CollectionUtil.merge(wsdcIdSearchResults, titleFragSearchResults, eventSerchResults);
  }


}
