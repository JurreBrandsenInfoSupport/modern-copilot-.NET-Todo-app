Feature: Task Management
  As a user of the Todo application
  I want to manage my tasks
  So that I can track my work

  Background:
    Given the application is running
    And a user "testuser" exists

  Scenario: Create a new task
    When I create a task with title "Buy groceries" for the user
    Then the response status code should be 200
    And the response should contain a task with title "Buy groceries"

  Scenario: Get all tasks
    Given a task "Complete report" exists for the user
    And a task "Send email" exists for the user
    When I request all tasks
    Then the response status code should be 200
    And the response should contain 2 tasks

  Scenario: Get tasks by user
    Given a task "User specific task" exists for the user
    When I request tasks for the user
    Then the response status code should be 200
    And the response should contain a task with title "User specific task"

  Scenario: Create task for non-existent user fails
    When I create a task with title "Invalid task" for user id 9999
    Then the response status code should be 400
