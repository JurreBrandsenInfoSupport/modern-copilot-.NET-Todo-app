Feature: Task Completion
  As a user of the Todo application
  I want to mark tasks as complete
  So that I can track my progress

  Background:
    Given the application is running
    And a user "worker" exists
    And I am authenticated as "worker"

  Scenario: Complete an existing task
    Given a task "Finish report" exists for the user
    When I complete the task
    Then the response status code should be 200
    And the task should be marked as completed

  Scenario: Complete a non-existent task fails
    When I complete task with id 9999
    Then the response status code should be 404

  Scenario: Complete an already completed task
    Given a task "Already done" exists for the user
    And the task is already completed
    When I complete the task
    Then the response status code should be 200
