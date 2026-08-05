# Copilot Instructions

## General Guidelines
- Prioritize Blazor guidance over MVC/Razor Pages for projects targeting .NET 10, especially when working with Blazor Web Apps that include a WebAssembly project.
- Integrate solution changes into existing Blazor pages rather than providing hard-coded example lists, particularly for FarmWorks/FarmWorksPlanning UI behavior.
- Design compact UI layouts where related list items are displayed on a single row, and limit list items to at most two rows. Remove the 'Nästa lön' badge.
- Ensure that list sections have a visible title and an outer border/frame around the list container.
- When a user requests a change in a specific code snippet, limit the solution to just that part instead of suggesting larger page changes.
- Ensure that text in UI components has a consistent style unless otherwise specified.
- Style the navigation menu with a black-to-green gradient background instead of a solid black background.
- Do not show divider lines in views when there is no information to display, including after the last data row in grid/list views.
- Verify UI issue fixes against actual project CSS/layout files instead of only providing generic suggestions.

## Project-Specific Rules
- Ensure that all instructions and code examples are tailored to the Blazor framework, leveraging its features and best practices.