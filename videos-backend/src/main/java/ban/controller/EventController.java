package ban.controller;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.context.annotation.ComponentScan;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

import java.util.List;

import ban.exception.InvalidEventException;
import ban.model.view.Event;
import ban.service.EventSearchService;
import ban.service.EventService;

/**
 * Created by bnorrish on 7/9/15.
 */
@RestController
@ComponentScan
public class EventController {

  @Autowired
  EventService eventService;

  @Autowired
  EventSearchService eventSearchService;

  @RequestMapping(value="/event/{eventId}", method = RequestMethod.GET)
  public Event getEvent(@PathVariable String eventId) {

    return eventService.getEventById(eventId);
  }

  @RequestMapping(value="/event", method = RequestMethod.GET)
  public List<Event> eventList(
      @RequestParam(value = "name-frag", required =  false) String nameFragList,
      @RequestParam(value = "wsdc-pointed", required =  false) Boolean isPointed,
      @RequestParam(value = "after-date", required = false) String afterDate,
      @RequestParam(value = "before-date", required = false) String beforeDate) {

    return eventSearchService.search(nameFragList, isPointed, afterDate, beforeDate);
  }

  @RequestMapping(value="/event", method = RequestMethod.POST)
  public Event postEvent(@RequestBody Event event) throws InvalidEventException {

    return eventService.addEvent(event);
  }

}
