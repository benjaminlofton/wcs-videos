package ban.service.list;

import org.joda.time.DateTime;
import org.joda.time.format.DateTimeFormat;
import org.joda.time.format.DateTimeFormatter;
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
public class MostRecentVideoAggregatorTest {

  private static final List<VideoD> EMPTY_LIST = new ArrayList<>();
  private static final List<VideoD> LIST_OF_FIVE = new ArrayList<>();

  @Mock
  LocalIndexedDataService localIndexedDataService;

  @InjectMocks
  MostRecentVideoAggregator mostRecentVideoAggregator = new MostRecentVideoAggregator();

  @Before
  public void initMocks() {

    LIST_OF_FIVE.clear();
    for (int i = 1; i < 6; i++) {

      VideoD v = new VideoD();
      v.setId(Integer.toString(i));

      DateTimeFormatter dtf = DateTimeFormat.forPattern("yyyy-MM-dd HH:mm:ss");
      v.setCreatedDateTime(dtf.print(new DateTime(2015,1,i,0,0)));

      LIST_OF_FIVE.add(v);
    }

    MockitoAnnotations.initMocks(this);
  }

  @Test
  public void testReturnFirstOfMany() {

    when(localIndexedDataService.getAllVideos()).thenReturn(LIST_OF_FIVE);

    List<String> result = mostRecentVideoAggregator.getResults(0,1);

    assertThat(result.size(), equalTo(1));
    assertThat(result.get(0), equalTo("5"));
  }

  @Test
  public void testReturnSecondOfMany() {

    when(localIndexedDataService.getAllVideos()).thenReturn(LIST_OF_FIVE);

    List<String> result = mostRecentVideoAggregator.getResults(1,1);

    assertThat(result.size(), equalTo(1));
    assertThat(result.get(0), equalTo("4"));
  }

  @Test
  public void testReturnSublist() {

    when(localIndexedDataService.getAllVideos()).thenReturn(LIST_OF_FIVE);

    List<String> result = mostRecentVideoAggregator.getResults(1,3);

    assertThat(result.size(), equalTo(3));
    assertThat(result.get(0), equalTo("4"));
    assertThat(result.get(1), equalTo("3"));
    assertThat(result.get(2), equalTo("2"));
  }

  @Test
  public void testReturnZeroOfMany() {

    when(localIndexedDataService.getAllVideos()).thenReturn(LIST_OF_FIVE);

    List<String> result = mostRecentVideoAggregator.getResults(1,0);

    assertThat(result.size(), equalTo(0));
  }

  @Test
  public void testNullSkipInterpretedAsZero() {

    when(localIndexedDataService.getAllVideos()).thenReturn(LIST_OF_FIVE);

    List<String> result = mostRecentVideoAggregator.getResults(null,1);

    assertThat(result.size(), equalTo(1));
    assertThat(result.get(0), equalTo("5"));
  }

  @Test
  public void testNullTakeInterpretedAsDefault() {

    when(localIndexedDataService.getAllVideos()).thenReturn(LIST_OF_FIVE);

    List<String> result = mostRecentVideoAggregator.getResults(0,null);

    // test list size < default (10), all 5 should be returned
    assertThat(result.size(), equalTo(5));
  }

  @Test
  public void testSkipOverflowReturnsEmpty() {

    when(localIndexedDataService.getAllVideos()).thenReturn(LIST_OF_FIVE);

    List<String> result = mostRecentVideoAggregator.getResults(10,1);

    assertThat(result.size(), equalTo(0));
  }

  @Test
  public void testTakeOverflowReturnsSublist() {

    when(localIndexedDataService.getAllVideos()).thenReturn(LIST_OF_FIVE);

    List<String> result = mostRecentVideoAggregator.getResults(0,10);

    assertThat(result.size(), equalTo(5));
    assertThat(result.get(0), equalTo("5"));
    assertThat(result.get(1), equalTo("4"));
    assertThat(result.get(2), equalTo("3"));
    assertThat(result.get(3), equalTo("2"));
    assertThat(result.get(4), equalTo("1"));
  }

  @Test
  public void testNullDatesAreIgnored() {

    // This will remove the "2" video
    LIST_OF_FIVE.get(1).setCreatedDateTime(null);

    when(localIndexedDataService.getAllVideos()).thenReturn(LIST_OF_FIVE);

    List<String> result = mostRecentVideoAggregator.getResults(0,5);

    assertThat(result.size(), equalTo(4));
    assertThat(result.get(0), equalTo("5"));
    assertThat(result.get(1), equalTo("4"));
    assertThat(result.get(2), equalTo("3"));
    assertThat(result.get(3), equalTo("1"));
  }

  @Test
  public void testInvalidDateStringsAreIgnored() {

    // This will remove the "2" video
    LIST_OF_FIVE.get(1).setCreatedDateTime("A bad date string!");

    when(localIndexedDataService.getAllVideos()).thenReturn(LIST_OF_FIVE);

    List<String> result = mostRecentVideoAggregator.getResults(0,5);

    assertThat(result.size(), equalTo(4));
    assertThat(result.get(0), equalTo("5"));
    assertThat(result.get(1), equalTo("4"));
    assertThat(result.get(2), equalTo("3"));
    assertThat(result.get(3), equalTo("1"));
  }





}