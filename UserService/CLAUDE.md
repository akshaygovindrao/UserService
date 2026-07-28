# UserService Development Rules

## General

- Write production-ready code — no placeholders, no half-finished implementations.
- Keep methods small; use meaningful names.
- Don't duplicate logic — extract shared logic instead.

## ASP.NET Core

- Use dependency injection for all services.
- Return proper HTTP status codes.
- Use async/await for all I/O.
- Unhandled exceptions are caught by `GlobalExceptionHandler` middleware — don't add redundant try/catch in controllers for exceptions it already maps (`UserAlreadyExistsException`, `InvalidCredentialsException`).

## Code Style

- Organize class members into named `#region` blocks, in this order: `Fields`, `Constructors`, `Private Methods`, `Public Methods`.
- Within the class, private methods come before public methods.

## Security

- Never log or expose sensitive data (passwords, tokens, connection strings).
- EF Core parameterizes queries by default — never bypass this with raw SQL string concatenation.
- Known gap: `RegisterRequest`/`LoginRequest` currently have no validation attributes (`[Required]`, `[EmailAddress]`, etc.). Add them when touching these DTOs.

## Testing

- No test project exists yet. When adding one, use xUnit, name it `UserService.Tests`, and reference it from `UserService.sln`.

## Before completing a change

- Build with zero warnings.
- No leftover TODOs or commented-out code.