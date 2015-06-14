
  Feature:
    As a dancer...
    I want to find relevant videos
    So that I can grow and be entertained!

    Scenario: Retrieve Video by ID
      Given the video 1234 has been saved
      When I retrieve video 1234
      Then a video with ID 1234 is returned