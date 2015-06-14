package ban.service;

import org.junit.Test;

import static org.hamcrest.CoreMatchers.is;
import static org.hamcrest.MatcherAssert.assertThat;

public class VideoSearchServiceTest {

  VideoSearchService service = new VideoSearchService();

  @Test
  public void testExists_whenDoesNotExist() {

    boolean expected = false;
    boolean actual = service.exists("somekey");

    assertThat(actual, is(expected));
  }

}