package ban.service.list;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Component;

import java.util.List;
import java.util.stream.Collectors;

import ban.model.persistence.VideoD;
import ban.service.LocalIndexedDataService;

/**
 * Created by bnorrish on 10/18/15.
 */
@Component
public class VideosWithNoDancers implements ILister {

  private final static int DEFAULT_DANCER_THRESHOLD = 2;
  private final static int DEFAULT_TAKE = 10;

  @Autowired
  LocalIndexedDataService localIndexedDataService;

  @Override
  public List<String> getResults(Integer skip, Integer take) {

    List<VideoD> videos = localIndexedDataService.getAllVideos();

    videos = videos.stream()
                   .filter(v -> v.getDancerIdList() == null || v.getDancerIdList().size() < DEFAULT_DANCER_THRESHOLD)
                   .sorted((VideoD v1, VideoD v2) -> v1.getId().compareTo(v2.getId()))
                   .collect(Collectors.toList());

    // Some input sanity
    int intSkip = skip == null ? 0 : skip;
    int intTake = take == null ? DEFAULT_TAKE : take;
    intSkip = Math.max(0,intSkip);
    intTake = Math.max(0,intTake);

    return videos.subList(Math.min(intSkip, videos.size()), Math.min(intSkip + intTake, videos.size())).stream()
                 .map(videoD -> videoD.getId())
                 .collect(Collectors.toList());
  }

  @Override
  public String getListType() {
    return "Video";
  }
}
