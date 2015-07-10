package ban.service;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Component;

import java.util.ArrayList;
import java.util.List;

import ban.model.view.Event;
import ban.service.mapper.EventMapper;
import ban.util.CollectionUtil;

/**
 * Created by bnorrish on 7/10/15.
 */
@Component
public class EventSearchService {

  @Autowired
  EventMapper eventMapper;

  @Autowired
  LocalIndexedDataService localIndexedDataService;

  public List<Event> search(String nameFragList, Boolean isWsdcPointed) {


    List<Event> wsdcPointedSearchResults = null;

    if(isWsdcPointed != null) {
      wsdcPointedSearchResults = eventMapper.mapToViewModel(localIndexedDataService.getEventsByWsdcPointed(isWsdcPointed));
    }


    List<Event> nameFragSearchResults = null;

    if (nameFragList != null) {
      nameFragSearchResults = new ArrayList<>();

      for (String nameFrag : nameFragList.split(",")) {

        List<Event> singleWordNameFragResults = eventMapper
            .mapToViewModel(localIndexedDataService.getEventsBySingleWordNameFrag(nameFrag));

        if (nameFragSearchResults.isEmpty()) {
          nameFragSearchResults.addAll(singleWordNameFragResults);
        } else {
          nameFragSearchResults = CollectionUtil.mergeEventLists(nameFragSearchResults, singleWordNameFragResults);
        }
      }
    }

    return CollectionUtil.mergeEventLists(nameFragSearchResults,wsdcPointedSearchResults);
  }
}
