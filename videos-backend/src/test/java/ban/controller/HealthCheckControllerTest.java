package ban.controller;

import org.junit.Test;

import static org.hamcrest.CoreMatchers.is;
import static org.hamcrest.MatcherAssert.assertThat;

public class HealthCheckControllerTest {

  @Test
  public void testHealthCheck() {

    assertThat("placeholdertest", is("placeholdertest"));
  }

}