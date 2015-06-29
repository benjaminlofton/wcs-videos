package ban.util;

import org.junit.BeforeClass;
import org.junit.Test;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;

import ban.model.view.Video;

import static org.junit.Assert.assertEquals;

/**
 * Created by bnorrish on 6/28/15.
 */
public class CollectionUtilTest {

  private static Video one;
  private static Video two;
  private static Video three;

  private static List<Video> emptyList = new ArrayList<>();

  @BeforeClass
  public static void before() {

    one = new Video();
    one.setId("1");
    one.setTitle("Video One");

    two = new Video();
    two.setId("2");
    two.setTitle("Video Two");

    three = new Video();
    three.setId("3");
    three.setTitle("Video Three");
  }

  @Test
  public void testNullLists_doNotThrowException() {
    CollectionUtil.merge(null,null,null);
  }

  @Test
  public void testListMergedWithNull_isList() {
    List<Video> hasItems = Arrays.asList(one,two,three);

    List<Video> mergedList = CollectionUtil.merge(hasItems,null);

    assertEquals(hasItems, mergedList);
  }

  @Test
  public void testNullMergedWithList_isList() {
    List<Video> hasItems = Arrays.asList(one,two,three);

    List<Video> mergedList = CollectionUtil.merge(null,hasItems);

    assertEquals(hasItems,mergedList);
  }

  @Test
  public void testListMergedWithEmpty_isEmpty() {
    List<Video> hasItems = Arrays.asList(one,two,three);

    List<Video> mergedList = CollectionUtil.merge(hasItems,emptyList);

    assertEquals(emptyList, mergedList);
  }

  @Test
  public void testEmptytMergedWithList_isEmpty() {
    List<Video> hasItems = Arrays.asList(one,two,three);

    List<Video> mergedList = CollectionUtil.merge(emptyList,hasItems);

    assertEquals(emptyList, mergedList);
  }

  @Test
  public void testIntersectionOfTwoLists() {
    List<Video> hasItemOneAndTwo = Arrays.asList(one,two);
    List<Video> hasItemTwoAndThree = Arrays.asList(two,three);

    List<Video> mergedList = CollectionUtil.merge(hasItemOneAndTwo,hasItemTwoAndThree);

    assertEquals(Arrays.asList(two), mergedList);
  }

  @Test
  public void testNoIntersectionOfTwoLists() {
    List<Video> hasItemOne = Arrays.asList(one);
    List<Video> hasItemTwo = Arrays.asList(two);

    List<Video> mergedList = CollectionUtil.merge(hasItemOne,hasItemTwo);

    assertEquals(emptyList, mergedList);
  }

  @Test
  public void testIntersectionOfTwoListsMultipleItems() {
    List<Video> hasItemOneTwoAndThree = Arrays.asList(one,two,three);
    List<Video> hasItemTwoAndThree = Arrays.asList(two,three);

    List<Video> mergedList = CollectionUtil.merge(hasItemOneTwoAndThree,hasItemTwoAndThree);

    assertEquals(Arrays.asList(two,three), mergedList);
  }







}