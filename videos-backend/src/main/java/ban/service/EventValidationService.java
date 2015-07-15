package ban.service;

import org.joda.time.format.DateTimeFormat;
import org.springframework.stereotype.Component;

import ban.exception.InvalidEventException;
import ban.model.view.Event;

/**
 * Created by bnorrish on 7/10/15.
 */
@Component
public class EventValidationService {

  /**
   * Validate an Event
   *
   * I know, calling a method for the possible effect of throwing an exception is bad form, bad form indeed.
   * Should be replaced with something like:
   * http://docs.spring.io/spring-framework/docs/4.1.x/spring-framework-reference/html/validation.html#validation-beanvalidation
   * https://spring.io/blog/2013/11/01/exception-handling-in-spring-mvc
   *
   */
  public void validateEvent(Event event) throws InvalidEventException {

    if(event.getName() == null || event.getName().isEmpty()) {
      throw new InvalidEventException("The 'Name' attribute must be defined");
    }

    try {
      DateTimeFormat.forPattern("yyyy-MM-dd").parseDateTime(event.getEventDate());
    } catch (RuntimeException ex) {
      throw new InvalidEventException("The 'EventDate' attribute is invalid");
    }
    
  }

}
