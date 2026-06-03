Feature: Rate Limiting
  As an API administrator
  I want rate limiting on endpoints
  So that the API is protected from abuse

  Background:
    Given the application is running
    And a user "ratelimituser" exists
    And I am authenticated as "ratelimituser"

  Scenario: Requests within rate limit succeed
    When I make 5 requests to the tasks endpoint
    Then all responses should have status code 200

  Scenario: API returns rate limit headers
    When I request all tasks
    Then the response should contain rate limit headers
