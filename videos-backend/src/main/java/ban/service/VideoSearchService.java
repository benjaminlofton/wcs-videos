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

  public List<Video> search(String wsdcIdList, String titleFragList) {

    if(wsdcIdList == null && titleFragList == null) {
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

    return CollectionUtil.merge(wsdcIdSearchResults, titleFragSearchResults);
  }


}
