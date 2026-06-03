Feature: Authentication
  As a user of the Todo application
  I want to authenticate with my credentials
  So that I can access protected resources

  Background:
    Given the application is running

  Scenario: Obtain JWT token for existing user
    Given a user "authuser" exists
    When I request a token for username "authuser"
    Then the response status code should be 200
    And the response should contain a valid JWT token
    And the token should expire in the future

  Scenario: Token request for non-existent user fails
    When I request a token for username "nonexistent"
    Then the response status code should be 401

  Scenario: Access protected endpoint without token fails
    When I request all tasks without authentication
    Then the response status code should be 401

  Scenario: Access protected endpoint with valid token succeeds
    Given a user "tokenuser" exists
    And I am authenticated as "tokenuser"
    When I request all tasks
    Then the response status code should be 200
