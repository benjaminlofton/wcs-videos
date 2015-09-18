package ban.service;

import org.junit.Before;
import org.junit.Test;
import org.mockito.InjectMocks;
import org.mockito.Mock;
import org.mockito.MockitoAnnotations;

import java.util.ArrayList;
import java.util.List;
import java.util.stream.IntStream;

import ban.client.AwsDynamoClient;
import ban.model.persistence.FlaggedVideoD;

import static org.hamcrest.core.Is.is;
import static org.junit.Assert.*;
import static org.mockito.Matchers.intThat;
import static org.mockito.Mockito.when;

/**
 * Created by bnorrish on 9/18/15.
 */
public class FlaggedVideoServiceTest {

  @Mock
  AwsDynamoClient awsDynamoClient;

  @InjectMocks
  FlaggedVideoService flaggedVideoService = new FlaggedVideoService();

  private List<FlaggedVideoD> flaggedVideos = new ArrayList<>();
  private List<FlaggedVideoD> emptyList = new ArrayList<>();
  private FlaggedVideoD someFlaggedVideo = new FlaggedVideoD();

  @Before
  public void initMocks() {
    MockitoAnnotations.initMocks(this);

    IntStream.range(0,5).forEach( i -> {
          FlaggedVideoD video = new FlaggedVideoD();
          video.setFlagId(Integer.toString(i));
          video.setExplanation("Explanation for " + i);
          video.setFlaggedVideoId("id_" + i);
          flaggedVideos.add(video);
        }
    );

    someFlaggedVideo.setFlagId("some_flag_id");
    someFlaggedVideo.setExplanation("some_explanation");
  }

  @Test
  public void testGetAll_ShouldReturnAllFlaggedVideos() {

    when(awsDynamoClient.getAllFlaggedVideos()).thenReturn(flaggedVideos);

    List<FlaggedVideoD> videos = flaggedVideoService.getAllFlaggedVideos();

    assertThat(videos.size(), is(flaggedVideos.size()));
  }

  @Test
  public void testGetByVideoId_ShouldReturnCorrectVideos() {

    when(awsDynamoClient.getAllFlaggedVideos()).thenReturn(flaggedVideos);

    String flaggedVideoId = flaggedVideos.get(3).getFlaggedVideoId();
    List<FlaggedVideoD> videos = flaggedVideoService.getAllFlaggedByVideoId(flaggedVideoId);

    assertThat(videos.size(), is(1));
    assertThat(videos.get(0).getFlaggedVideoId(), is(flaggedVideoId));
  }

  @Test
  public void testGetByUnknownVideoId_ShouldReturnEmptyList() {

    when(awsDynamoClient.getAllFlaggedVideos()).thenReturn(flaggedVideos);

    String flaggedVideoId = "some unknown ID";
    List<FlaggedVideoD> videos = flaggedVideoService.getAllFlaggedByVideoId(flaggedVideoId);

    assertThat(videos.size(), is(0));
  }

  @Test
  public void testGetByFlagId_ShouldReturnCorrectVideo() {
    String flagId = someFlaggedVideo.getFlagId();
    when(awsDynamoClient.getFlaggedVideo(flagId)).thenReturn(someFlaggedVideo);

    FlaggedVideoD video = flaggedVideoService.getFlaggedVideo(flagId);

    assertThat(video.getFlagId(), is(flagId));
  }

}