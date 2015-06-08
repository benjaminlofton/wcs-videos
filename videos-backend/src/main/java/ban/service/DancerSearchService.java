package ban.service;

import org.springframework.stereotype.Component;

import java.util.ArrayList;
import java.util.List;

/**
 * Created by bnorrish on 6/7/15.
 */
@Component
public class DancerSearchService {

  public List<Integer> getVideosByWsdcId(int wsdcId) {

    List<Integer> foundVideos = new ArrayList<Integer>();

    foundVideos.add(1234);
    foundVideos.add(8877);
    foundVideos.add(9876);

    return foundVideos;
  }


}
