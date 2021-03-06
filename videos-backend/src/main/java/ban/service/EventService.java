package ban.service;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Component;

import java.util.ArrayList;
import java.util.List;

import ban.client.AwsDynamoClient;
import ban.exception.InvalidEventException;
import ban.model.persistence.EventD;
import ban.model.view.Event;
import ban.service.mapper.EventMapper;

/**
 * Created by bnorrish on 7/9/15.
 */
@Component
public class EventService {

  @Autowired
  AwsDynamoClient awsDynamoClient;

  @Autowired
  EventMapper eventMapper;

  @Autowired
  EventValidationService eventValidationService;

  public Event getEventById(String eventId) {

    return eventMapper.mapToViewModel(awsDynamoClient.getEventById(eventId));
  }

  public List<Event> getEventList() {

    List<EventD> eventDs = awsDynamoClient.getEventList();

    List<Event> result = new ArrayList<>();
    for (EventD eventD : eventDs) {
      result.add(eventMapper.mapToViewModel(eventD));
    }

    return result;
  }

  public Event addEvent(Event event) throws InvalidEventException {

    eventValidationService.validateEvent(event);

    EventD eventD = eventMapper.mapToPersistenceModel(event);

    return eventMapper.mapToViewModel(awsDynamoClient.addEvent(eventD));
  }
}
