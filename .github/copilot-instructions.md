# Instructions

## General Instructions

- Provide concise, precise, and direct answers, prioritizing executable code solutions.
- When suggesting code changes, clearly specify file paths and line numbers.
- Prioritize the use of frameworks and libraries already present in the user's project; do not introduce unnecessary dependencies.
- If the user's question is ambiguous, ask for clarification before making suggestions.
- Respond in English.
- When users require real-time information, file system operations, or the execution of specific tasks, actively use available tools to complete those tasks.
- **If the current agent has the `fetch_webpage` tool available and the user provides a URL, you MUST use it to retrieve the page content.**

After retrieving the content, look for related links and adjust the `maxDepth` parameter to perform a recursive crawl until all necessary information is gathered. If the current agent does not have this tool, explain the limitation or transfer the task according to the role's workflow.

## General architecture guidelines

- Follow the SOLID principles to ensure maintainable and scalable code.
- Prefer composition over inheritance where applicable.
- Keep classes and methods focused on a single responsibility.
- Keep methods short and focused, ideally fitting within a single screen without scrolling.
- Use dependency injection to manage dependencies and improve testability.
- Organize code into cohesive modules with clear boundaries.
- Avoid tight coupling between modules; use interfaces and abstractions to decouple components.

## When Commenting Code

- All documentation comments must: describe the responsibility or purpose in the user's language; for methods and functions, describe each parameter, the return value, and possible exceptions; for properties and fields, describe the data they store.
- Comment public and protected types and members, plus complex internal logic where it aids understanding.
- Write comments in Spanish by default, unless otherwise specified in [current-project-instructions](current-project-instructions.md). If comments already exist, translate them to Spanish unless that file indicates otherwise.
- Use C# for code examples whenever possible.
- Comments should focus on the "why" rather than the "what," avoiding line-by-line translation of the code.

## When writing C# code

- When initializing lists, dictionaries, etc., use `[]` instead of `new List<T>()` or `new Dictionary<TKey, TValue>()` whenever possible.
- Use LINQ for querying and manipulating collections whenever possible.

## Project-specific instructions

If the file [current-project-instructions](current-project-instructions.md) exists, use it.
