package ban.service;

import org.junit.Before;
import org.junit.Test;
import org.mockito.InjectMocks;
import org.mockito.Mock;
import org.mockito.MockitoAnnotations;

import ban.client.AwsDynamoClient;
import ban.model.persistence.VideoD;
import ban.service.mapper.VideoMapper;

import static org.hamcrest.MatcherAssert.assertThat;
import static org.hamcrest.core.Is.is;
import static org.mockito.Matchers.anyString;
import static org.mockito.Mockito.when;

/**
 * Created by bnorrish on 6/28/15.
 */
public class VideoServiceTest {

  @Mock
  private AwsDynamoClient awsDynamoClient;

  @Mock
  private VideoMapper videoMapper;

  @InjectMocks
  VideoService service = new VideoService();

  @Before
  public void initMocks() {
    MockitoAnnotations.initMocks(this);
  }

  @Test
  public void testExistsReturnsFalse_whenVideoDoesNotExist() {

    when(awsDynamoClient.getVideo(anyString())).thenReturn(null);

    assertThat(service.exists("some-key-that-does-not-exist"), is(false));
  }

  @Test
  public void testExistsReturnsTrue_whenVideoExist() {
    String key = "key-that-exists";

    when(awsDynamoClient.getVideo(key)).thenReturn(new VideoD());

    assertThat(service.exists(key),is(true));
  }
}
