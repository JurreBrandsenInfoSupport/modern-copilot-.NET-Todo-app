Feature: Health Checks
  As an operations engineer
  I want health check endpoints
  So that I can monitor application status

  Background:
    Given the application is running

  Scenario: Liveness health check returns healthy
    When I request the health endpoint
    Then the response status code should be 200
    And the health status should be "Healthy"

  Scenario: Readiness health check returns healthy
    When I request the readiness endpoint
    Then the response status code should be 200
    And the health status should be "Healthy"

  Scenario: Health check returns check details
    When I request the health endpoint
    Then the response should contain health check entries
