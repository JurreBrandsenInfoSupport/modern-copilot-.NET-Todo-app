Feature: Comment Management
  As a user of the Todo application
  I want to add comments to tasks
  So that I can collaborate with others

  Background:
    Given the application is running
    And a user "commentuser" exists
    And a task "Commentable task" exists for the user

  Scenario: Add a comment to a task
    When I add a comment "Great progress!" to the task
    Then the response status code should be 200
    And the response should contain a comment with text "Great progress!"

  Scenario: Get comments for a task
    Given a comment "First comment" exists on the task
    And a comment "Second comment" exists on the task
    When I request comments for the task
    Then the response status code should be 200
    And the response should contain 2 comments

  Scenario: Add empty comment fails
    When I add a comment "" to the task
    Then the response status code should be 400

  Scenario: Add comment to non-existent task fails
    When I add a comment "Orphan comment" to task id 9999
    Then the response status code should be 400
