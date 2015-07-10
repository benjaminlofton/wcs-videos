package ban.controller;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.context.annotation.ComponentScan;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;
import org.springframework.web.bind.annotation.RestController;

import java.util.List;

import ban.model.view.Event;
import ban.service.EventService;

/**
 * Created by bnorrish on 7/9/15.
 */
@RestController
@ComponentScan
public class EventController {

  @Autowired
  EventService eventService;

  @RequestMapping(value="/event/{eventId}", method = RequestMethod.GET)
  public Event getEvent(@PathVariable String eventId) {

    return eventService.getEventById(eventId);
  }

  @RequestMapping(value="/event", method = RequestMethod.GET)
  public List<Event> getEventList() {

    return eventService.getEventList();
  }
}
