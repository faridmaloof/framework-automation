Feature: Multi-actor sessions
  As an automation engineer
  I want two actors to act through independent sessions
  So that one actor's state never leaks into another actor's session

  @web @multi-actor
  Scenario: Two actors keep independent sessions
    Given the user is on the page "data:text/html,<title>USERPAGE</title>"
    And the administrator is on the page "data:text/html,<title>ADMINPAGE</title>"
    Then the user should be on the page "USERPAGE"
    And the administrator should be on the page "ADMINPAGE"
