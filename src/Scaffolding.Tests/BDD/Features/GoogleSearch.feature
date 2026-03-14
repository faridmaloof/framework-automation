Feature: Google Search
  As a user
  I want to search for concepts in Google
  So that I can find relevant information

  @smoke @web
  Scenario: Successful search validates the page title
    Given the actor is on the Google home page
    When they search for "Automatización .NET"
    Then the page title should contain "Automatización"
    And I should see search results
