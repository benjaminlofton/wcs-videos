package ban.service;

import org.joda.time.DateTime;
import org.joda.time.format.DateTimeFormat;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Component;

import java.util.ArrayList;
import java.util.List;

import ban.exception.InvalidRequestException;
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

  public List<Event> search(String nameFragList, Boolean isWsdcPointed, Integer year, String afterDateString, String beforeDateString) {

    DateTime afterDate = null;
    DateTime beforeDate = null;
    try {
      afterDate = (afterDateString == null) ? null : DateTimeFormat.forPattern("yyyy-MM-dd").parseDateTime(afterDateString);
      beforeDate = (beforeDateString == null) ? null : DateTimeFormat.forPattern("yyyy-MM-dd").parseDateTime(beforeDateString);

      if(afterDate != null && beforeDate != null) {
        if(afterDate.isAfter(beforeDate)) {
          throw new InvalidRequestException();
        }
      }

    } catch (java.lang.IllegalArgumentException e) {
      throw new InvalidRequestException();
    }

    List<Event> dateSearchResults = null;
    if(afterDate != null || beforeDate != null) {
      dateSearchResults = eventMapper.mapToViewModel(localIndexedDataService.getEventsBetween(afterDate,beforeDate));
    }

    List<Event> wsdcPointedSearchResults = null;
    if(isWsdcPointed != null) {
      wsdcPointedSearchResults = eventMapper.mapToViewModel(localIndexedDataService.getEventsByWsdcPointed(isWsdcPointed));
    }

    List<Event> yearSearchResults = null;
    if(year != null) {
      yearSearchResults = eventMapper.mapToViewModel(localIndexedDataService.getEventsByYear(year));
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

    return CollectionUtil.mergeEventLists(nameFragSearchResults,wsdcPointedSearchResults, yearSearchResults, dateSearchResults);
  }
}
