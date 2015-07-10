package ban.model.view;

/**
 * Created by bnorrish on 7/9/15.
 */
public class Event {

  private String eventId;

  private String name;

  private boolean isWsdcPointed;

  private String eventDate;

  public String getEventId() {
    return eventId;
  }

  public void setEventId(String eventId) {
    this.eventId = eventId;
  }

  public String getName() {
    return name;
  }

  public void setName(String name) {
    this.name = name;
  }

  public boolean isWsdcPointed() {
    return isWsdcPointed;
  }

  public void setWsdcPointed(boolean isWsdcPointed) {
    this.isWsdcPointed = isWsdcPointed;
  }

  public String getEventDate() {
    return eventDate;
  }

  public void setEventDate(String eventDate) {
    this.eventDate = eventDate;
  }
}