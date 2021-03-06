package ban.service.mapper;

import org.springframework.stereotype.Component;

import java.util.ArrayList;
import java.util.List;

import ban.model.persistence.EventD;
import ban.model.view.Event;

/**
 * Created by bnorrish on 7/9/15.
 */
@Component
public class EventMapper {

  public EventD mapToPersistenceModel(Event event) {

    if(event == null) {
      return null;
    }

    EventD eventD = new EventD();
    eventD.setEventDate(event.getEventDate());
    eventD.setEventId(event.getEventId());
    eventD.setWsdcPointed(event.isWsdcPointed());
    eventD.setName(event.getName());
    eventD.setEventId(event.getEventId());
    eventD.setLocationName(event.getLocationName());

    return eventD;
  }

  public Event mapToViewModel(EventD eventD) {

    if(eventD == null) {
      return null;
    }

    Event event = new Event();
    event.setEventDate(eventD.getEventDate());
    event.setEventId(eventD.getEventId());
    event.setWsdcPointed(eventD.isWsdcPointed());
    event.setName(eventD.getName());
    event.setEventId(eventD.getEventId());
    event.setLocationName(eventD.getLocationName());

    return event;
  }

  public List<Event> mapToViewModel(List<EventD> eventDs) {

    List<Event> events = new ArrayList<>();
    for (EventD eventD : eventDs) {
      events.add(mapToViewModel(eventD));
    }

    return events;
  }

}
