package ban.service;

import org.junit.Before;
import org.junit.Test;
import org.mockito.InjectMocks;
import org.mockito.Mock;
import org.mockito.MockitoAnnotations;

import ban.client.AwsDynamoClient;

import static org.hamcrest.MatcherAssert.assertThat;
import static org.hamcrest.core.Is.is;
import static org.mockito.Matchers.anyString;
import static org.mockito.Mockito.when;

public class VideoSearchServiceTest {

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

}