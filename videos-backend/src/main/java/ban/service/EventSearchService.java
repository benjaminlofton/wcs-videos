package ban.service;

import org.joda.time.DateTime;
import org.joda.time.format.DateTimeFormat;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Component;

import java.util.List;
import java.util.function.Predicate;
import java.util.stream.Collectors;

import ban.exception.InvalidRequestException;
import ban.model.persistence.EventD;
import ban.model.view.Event;
import ban.service.mapper.EventMapper;

/**
 * Created by bnorrish on 7/10/15.
 */
@Component
public class EventSearchService {

  @Autowired
  EventMapper eventMapper;

  @Autowired
  LocalIndexedDataService localIndexedDataService;

  public List<Event> search(String nameFragList, Boolean isWsdcPointed, String afterDateString, String beforeDateString) {

    // I don't have a better way to validate dates then to attempt to load them into a DateTime
    DateTime afterDate;
    DateTime beforeDate;
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

    List<EventD> events = localIndexedDataService.getAllEvents();

    return events.stream()
        .filter(matchWsdcPointed(isWsdcPointed))
        .filter(matchNameFragmentList(nameFragList))
        .filter(matchDates(afterDate,beforeDate))
        .map(e -> eventMapper.mapToViewModel(e))
        .collect(Collectors.toList());
  }

  public static Predicate<EventD> matchWsdcPointed(Boolean isPointed) {
    return e -> {
      if(isPointed == null) {
        return true;
      }
      return e.isWsdcPointed().equals(isPointed);
    };
  }

  public static Predicate<EventD> matchNameFragmentList(String nameFragList) {
    return e -> {
      if (nameFragList == null) {
        return true;
      }

      boolean containsAllFrags = true;
      for(String frag : nameFragList.split(",")) {
        containsAllFrags = containsAllFrags && e.getName().contains(frag);
      }
      return containsAllFrags;
    };
  }

  public static Predicate<EventD> matchDates(DateTime ad, DateTime bd) {
    return e -> {

      if (ad == null && bd == null) {
        return true;
      }

      boolean keep = true;

      // Seems bad to do this for every date, for every search
      DateTime eventDate = DateTimeFormat.forPattern("yyyy-MM-dd").parseDateTime(e.getEventDate());

      if (ad != null) {
        keep = keep && (eventDate.isAfter(ad) || eventDate.isEqual(ad));
      }

      if (bd != null) {
        keep = keep && (eventDate.isBefore(bd) || eventDate.isEqual(bd));
      }

      return keep;
    };
  }





}
