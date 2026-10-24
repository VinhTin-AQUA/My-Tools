# AGENTS.md

## 1. Core Principles

- Prioritize implementation speed, correctness, and simplicity.
- Understand only what is necessary to complete the current task.
- Avoid unnecessary analysis, excessive planning, and redundant tool calls.
- Do not analyze the entire project for small or localized tasks.
- Prefer minimal, targeted changes over broad refactoring.
- Follow the existing architecture, coding conventions, and patterns.
- Do not introduce new dependencies unless necessary.
- Do not modify unrelated files or functionality.
- Never claim that a command or test passed unless it was actually executed successfully.

## 2. Project Understanding

At the beginning of a task:

1. Read this `AGENTS.md` file.
2. Identify the relevant files and existing implementation.
3. Inspect nearby code and dependencies only when needed.
4. Reuse existing utilities, components, services, and patterns.
5. Check project scripts or documentation only when necessary.

Do not:

- Re-analyze the entire project for every task.
- Read unrelated files.
- Generate lengthy summaries before implementation.
- Create a detailed implementation plan for straightforward changes.
- Repeat project discovery when the existing context is sufficient.

For complex tasks, investigate the relevant architecture before making changes.

## 3. Task Execution Strategy

### Small tasks

Examples: fixing a bug, updating UI, changing text, adjusting styles, or adding a small field.

- Understand the relevant code.
- Implement the change immediately.
- Run only the most relevant lightweight checks, if needed.
- Avoid full builds and complete test suites unless justified.

### Medium tasks

Examples: adding an API endpoint, implementing a component, or modifying business logic.

- Inspect the affected modules and their dependencies.
- Implement the complete feature.
- Run targeted tests and relevant static checks.
- Run a build only when the changes warrant it.

### Large or high-risk tasks

Examples: architectural changes, database migrations, authentication, payment flows, or cross-module refactoring.

- Analyze the affected architecture and dependencies.
- Create a concise plan when it reduces risk.
- Implement changes incrementally.
- Run appropriate tests and builds.
- Review potential regressions and compatibility issues.

Use the simplest workflow appropriate for the task.

## 4. Analysis and Planning

- Do not produce a lengthy analysis before coding.
- Do not create a formal plan for straightforward tasks.
- Start implementation as soon as the requirements and relevant code are sufficiently understood.
- Ask a question only when missing information materially affects the implementation or a safe decision cannot be made.
- When requirements are clear, make reasonable decisions based on existing project conventions.
- Do not investigate unrelated edge cases or hypothetical scenarios unless relevant.

For complex tasks, provide a short plan and then proceed with implementation without unnecessary pauses.

## 5. Coding Rules

- Follow the existing language, framework, and project conventions.
- Prefer readable, maintainable, and straightforward solutions.
- Keep changes focused on the requested task.
- Reuse existing functions, components, and utilities.
- Avoid unnecessary abstractions, wrappers, and duplicated logic.
- Preserve existing behavior unless the task explicitly requires changing it.
- Handle errors and edge cases appropriate to the affected functionality.
- Do not silently replace established patterns with a different architecture.
- Do not add dependencies for functionality that can reasonably use existing tools.
- Do not rewrite working code simply to make it look different.

## 6. Build and Test Policy

Do not automatically run every build and test command after every task.

Choose verification based on risk and scope.

### Lightweight changes

For text, styling, and localized UI changes:

- Inspect the resulting diff.
- Run a relevant lint or check only if useful.
- Skip production builds when they provide little additional confidence.

### Logic changes

For business logic, API, state management, and data processing:

- Run targeted tests when available.
- Run relevant type checks or lint checks when appropriate.
- Check important edge cases.

### High-risk or broad changes

For authentication, authorization, database schema changes, payments, shared interfaces, and large refactors:

- Run the relevant test suite.
- Run required type checks and lint checks.
- Run a build when appropriate.
- Inspect the final diff for unintended changes.

### General rules

- Do not run the same successful command repeatedly without a reason.
- Do not run a full test suite if targeted tests provide sufficient coverage for a low-risk task.
- Do not run a production build merely to verify every small edit.
- If a check fails, determine whether the failure is caused by the current changes.
- Fix relevant failures when feasible.
- Clearly report checks that were skipped or could not be completed.
- Never claim that unexecuted tests passed.

## 7. Tool Usage

- Prefer targeted file searches over listing or reading the entire repository.
- Read only the necessary portions of large files.
- Avoid repeated searches for information already discovered.
- Avoid unnecessary terminal commands.
- Reuse command output when it remains valid.
- Prefer the project's existing scripts for linting, testing, and building.
- Do not install packages or modify dependencies without a valid reason.
- Do not run destructive commands without explicit authorization.

## 8. Dependencies and Configuration

- Preserve the existing package manager and lockfile.
- Do not upgrade unrelated dependencies.
- Do not modify environment files, secrets, or deployment configuration unless required.
- Do not expose credentials, tokens, or other sensitive information.
- If a dependency change is necessary, explain its purpose briefly.

## 9. Git and Change Management

- Do not revert unrelated user changes.
- Do not overwrite uncommitted work.
- Do not create commits, push changes, or perform destructive Git operations unless requested.
- Review the diff when practical.
- Keep the final changes limited to the requested scope.

## 10. Completion

After implementation:

1. Briefly summarize what changed.
2. List the important files modified.
3. Report the checks actually executed and their results.
4. Mention relevant limitations or remaining issues.
5. Avoid lengthy explanations unless requested.

Do not repeat the entire implementation process in the final response.

## 11. Priority Rule

The current user request takes priority over general workflow preferences.

If the user explicitly requests full analysis, comprehensive testing, a production build, or a detailed explanation, perform the requested work when appropriate.

Otherwise, default to a focused implementation workflow with minimal unnecessary analysis and verification.