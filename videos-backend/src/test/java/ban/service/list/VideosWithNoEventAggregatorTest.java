package ban.service.list;

import org.junit.Before;
import org.junit.Test;
import org.mockito.InjectMocks;
import org.mockito.Mock;
import org.mockito.MockitoAnnotations;

import java.util.ArrayList;
import java.util.List;

import ban.model.persistence.VideoD;
import ban.service.LocalIndexedDataService;

import static org.hamcrest.MatcherAssert.assertThat;
import static org.hamcrest.core.IsEqual.equalTo;
import static org.mockito.Mockito.when;

/**
 * Created by bnorrish on 9/8/15.
 */
public class VideosWithNoEventAggregatorTest {

  private static final List<VideoD> LIST_OF_FIVE = new ArrayList<>();

  @Mock
  LocalIndexedDataService localIndexedDataService;

  @InjectMocks
  VideosWithNoEventAggregator videosWithNoEventAggregator = new VideosWithNoEventAggregator();

  @Before
  public void initMocks() {

    for (int i = 1; i < 6; i++) {

      VideoD v = new VideoD();
      v.setId(Integer.toString(i));
      v.setEventId("EVENT_" + i);

      LIST_OF_FIVE.add(v);
    }

    MockitoAnnotations.initMocks(this);
  }

  @Test
  public void testReturnFirstOfMany() {

    LIST_OF_FIVE.get(0).setEventId(null);
    LIST_OF_FIVE.get(1).setEventId(null);

    when(localIndexedDataService.getAllVideos()).thenReturn(LIST_OF_FIVE);

    List<String> result = videosWithNoEventAggregator.getResults(0,1);

    assertThat(result.size(), equalTo(1));
    assertThat(result.get(0), equalTo("1"));
  }

  @Test
  public void testReturnSubset() {

    LIST_OF_FIVE.get(0).setEventId(null);
    LIST_OF_FIVE.get(1).setEventId(null);

    when(localIndexedDataService.getAllVideos()).thenReturn(LIST_OF_FIVE);

    List<String> result = videosWithNoEventAggregator.getResults(0,5);

    assertThat(result.size(), equalTo(2));
    assertThat(result.get(0), equalTo("1"));
    assertThat(result.get(1), equalTo("2"));
  }

}