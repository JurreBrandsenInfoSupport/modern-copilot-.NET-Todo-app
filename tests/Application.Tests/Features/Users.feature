Feature: User Management
  As an administrator
  I want to manage users
  So that people can use the application

  Background:
    Given the application is running

  Scenario: Register a new user
    When I register a user with username "newuser"
    Then the response status code should be 200
    And the response should contain a user with username "newuser"

  Scenario: Get all users
    Given a user "alice" exists
    And a user "bob" exists
    When I request all users
    Then the response status code should be 200
    And the response should contain 2 users

  Scenario: Register user with empty username
    When I register a user with username ""
    Then the response status code should be 200
