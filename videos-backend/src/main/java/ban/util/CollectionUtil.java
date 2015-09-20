package ban.util;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import ban.model.view.Event;
import ban.model.view.Video;

/**
 * Created by bnorrish on 6/28/15.
 */
public class CollectionUtil {

  public static List<Video> merge(List<Video> ... videoLists) {

    Map<String,Video> mergedMap = new HashMap<>();
    boolean firstPass = true;

    for(List<Video> videoList : videoLists) {

      if(videoList == null) {
        continue;
      }

      Map<String, Video> tempMap = new HashMap<>();
      for(Video video : videoList) {
        tempMap.put(video.getId(),video);
      }

      if(firstPass) {
        mergedMap.putAll(tempMap);
      } else {
        mergedMap.keySet().retainAll(tempMap.keySet());
      }

      firstPass = false;
    }

    return new ArrayList(mergedMap.values());
  }

  public static List<Event> mergeEventLists(List<Event> ... eventLists) {

    Map<String,Event> mergedMap = new HashMap<>();
    boolean firstPass = true;

    for(List<Event> eventList : eventLists) {

      if(eventList == null) {
        continue;
      }

      Map<String, Event> tempMap = new HashMap<>();
      for(Event event : eventList) {
        tempMap.put(event.getEventId(), event);
      }

      if(firstPass) {
        mergedMap.putAll(tempMap);
      } else {
        mergedMap.keySet().retainAll(tempMap.keySet());
      }

      firstPass = false;
    }

    return new ArrayList(mergedMap.values());
  }

  public static List<String> convertToStringList(String input) {

    String[] frags = input.split(",");

    return Arrays.asList(frags);
  }

  public static List<Integer> convertToIntegerList(String input) {

    String[] frags = input.split(",");

    List<Integer> result = new ArrayList<>();
    for(String frag : frags) {
      result.add(Integer.parseInt(frag));
    }

    return result;
  }
}
