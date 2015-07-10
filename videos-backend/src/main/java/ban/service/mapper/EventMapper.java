package ban.service.mapper;

import org.springframework.stereotype.Component;

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
    eventD.setIsWsdcPointed(event.isWsdcPointed());
    eventD.setName(event.getName());

    return eventD;
  }

  public Event mapToViewModel(EventD eventD) {

    if(eventD == null) {
      return null;
    }

    Event event = new Event();
    event.setEventDate(eventD.getEventDate());
    event.setEventId(eventD.getEventId());
    event.setIsWsdcPointed(eventD.isWsdcPointed());
    event.setName(eventD.getName());

    return event;
  }

}
