package boot;

import org.junit.Test;
import org.junit.runner.RunWith;
import org.springframework.boot.test.IntegrationTest;
import org.springframework.boot.test.SpringApplicationConfiguration;
import org.springframework.test.context.junit4.SpringJUnit4ClassRunner;
import org.springframework.test.context.web.WebAppConfiguration;

import ban.Application;

/**
 * Created by bnorrish on 10/17/15.
 */

@RunWith(SpringJUnit4ClassRunner.class)   // 1
@SpringApplicationConfiguration(classes = Application.class)   // 2
@WebAppConfiguration   // 3
@IntegrationTest("server.port:0")   // 4
public class SpringBootTest {

  @Test
  public void canFetchMickey() {

//    when().
//              get("/characters/{id}", mickeyId).
//              then().
//              statusCode(HttpStatus.SC_OK).
//              body("name", Matchers.is("Mickey Mouse")).
//              body("id", Matchers.is(mickeyId));
  }
}
