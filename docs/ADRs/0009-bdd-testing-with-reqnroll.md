# ADR 0009: BDD Testing with Reqnroll

## Status

Accepted

## Context

Testing is essential for maintaining confidence in application correctness as the codebase evolves. We needed a testing approach that not only verifies technical behaviour but also serves as living documentation that non-technical stakeholders can read and validate.

Traditional unit tests verify implementation details but often fail to communicate business intent. Integration tests confirm system behaviour but their code-heavy format makes them inaccessible to product owners and business analysts.

Behaviour-Driven Development (BDD) bridges this gap by expressing tests in Gherkin syntax — a structured natural language format using Given/When/Then steps. These scenarios serve dual purposes: executable automated tests and human-readable specifications.

Reqnroll is the community-maintained successor to SpecFlow, fully compatible with modern .NET versions. It integrates with standard test frameworks (MSTest, NUnit, xUnit), supports step definition reuse, scenario outlines for data-driven tests, and hooks for test lifecycle management. The Gherkin feature files act as a shared language between developers, testers, and business stakeholders.

## Decision

We will use Reqnroll with Gherkin syntax for BDD-style integration and acceptance tests. Feature files will describe business scenarios in structured natural language. Step definitions will implement the automation logic, leveraging Testcontainers for realistic database interactions.

## Consequences

**Positive:**
- Tests serve as living documentation readable by all stakeholders
- Gherkin scenarios clearly express business requirements and acceptance criteria
- Step definitions are reusable across multiple scenarios
- Scenario outlines enable concise data-driven testing
- Forces thinking about behaviour from the user's perspective
- Catches requirement misunderstandings early in the development cycle

**Negative:**
- More verbose than traditional unit tests for simple assertions
- Requires maintaining both feature files and step definition code
- Gherkin syntax has a learning curve for developers unfamiliar with BDD
- Poorly written scenarios can become brittle and hard to maintain

**Neutral:**
- Team must agree on Gherkin writing conventions and abstraction levels
- Step definitions require careful scoping to remain reusable without becoming overly generic
