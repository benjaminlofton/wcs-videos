package ban.service;

import org.junit.Before;
import org.junit.Test;
import org.mockito.InjectMocks;
import org.mockito.Mock;
import org.mockito.MockitoAnnotations;

import java.util.ArrayList;
import java.util.List;

import ban.model.persistence.VideoD;
import ban.model.view.Video;
import ban.service.mapper.VideoMapper;

import static org.mockito.Mockito.when;

public class VideoSearchServiceTest {


  @Mock
  LocalIndexedDataService localIndexedDataService;

  @Mock
  VideoMapper videoMapper;

  @InjectMocks
  VideoSearchService videoSearchService;

  @Before
  public void initMocks() {
    MockitoAnnotations.initMocks(this);

    VideoD one = new VideoD();
    one.setId("one");
    one.setTitle("Title: one");
    videoList.add(one);
  }

  public List<VideoD> videoList = new ArrayList();

  @Test
  public void testShouldReturnAllVideos_whenNoSearchArgsPassed() {

    when(localIndexedDataService.getAllVideos()).thenReturn(videoList);

    List<Video> results = videoSearchService.search(null,null,null,null, null);


  }



}