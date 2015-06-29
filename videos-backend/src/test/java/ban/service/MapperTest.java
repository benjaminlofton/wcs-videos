package ban.service;

import org.junit.Test;

import ban.model.persistence.VideoD;

import static org.hamcrest.CoreMatchers.is;
import static org.hamcrest.CoreMatchers.nullValue;
import static org.hamcrest.MatcherAssert.assertThat;

public class MapperTest {

  VideoMapper videoMapper = new VideoMapper();

  @Test
  public void shouldMapId_toId() {
    VideoD source = new VideoD();
    source.setId("TESTID");

    assertThat(videoMapper.mapToViewModel(source).getId(),is(source.getId()));
  }

}