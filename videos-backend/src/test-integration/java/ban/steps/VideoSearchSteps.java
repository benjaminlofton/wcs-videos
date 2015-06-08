package ban.steps;

import cucumber.api.java.en.Then;
import cucumber.api.java.en.When;

public class VideoSearchSteps {

  @cucumber.api.java.en.Given("^the video (\\d+) has been saved$")
  public void saveVideoById(int videoId) throws Throwable {

    System.out.println("compile me");;

  }

  @When("^I retrieve video (\\d+)$")
  public void retrieveVideo(int videoId) throws Throwable {

  }

  @Then("^a video with ID (\\d+) is returned$")
  public void verifyVideo(int videoId) throws Throwable {

  }

}
