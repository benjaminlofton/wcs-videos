package ban.util;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

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
}
