# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project overview

UserService is a minimal ASP.NET Core 8 Web API (`Microsoft.NET.Sdk.Web`) scaffold for a user authentication service, structured around CQRS (via MediatR). It is currently a skeleton: every command/query handler is a stub (`throw new NotImplementedException()`), and there is no database/persistence layer, no actual authentication logic, and no test project yet.

## Commands

Run all commands from the repo root (`UserService.sln`) or from `UserService/` (the project directory).

```powershell
dotnet build                 # build the solution
dotnet run --project UserService   # run the API (from repo root)
dotnet watch run              # run with hot reload (from UserService/ project dir)
```

There is no test project in the solution currently — `dotnet test` has nothing to run.

The app launches to the Swagger UI by default (see `Properties/launchSettings.json`). Default dev URLs: `http://localhost:5176` (http profile) and `https://localhost:7054` (https profile).

`UserService/UserService.http` contains sample HTTP requests usable with the REST Client / VS Code `.http` file runner, but currently only references a `weatherforecast` endpoint that no longer exists in the controllers — it has not been kept in sync with `AuthenticationController`.

## Architecture

- Single project, `UserService/UserService.csproj`, targeting `net8.0`, nullable reference types and implicit usings enabled.
- Standard ASP.NET Core minimal hosting model in `Program.cs`: registers controllers, Swagger/OpenAPI (Swashbuckle), MediatR, `UseHttpsRedirection`, `UseAuthorization`, `MapControllers`. Swagger UI is only enabled in `Development`.
- **CQRS via MediatR**: `Controllers/AuthenticationController.cs` is a thin HTTP layer only — it maps each action to a MediatR command/query via `IMediator.Send(...)` and translates the result to an `IActionResult`. It contains no business logic itself.
- Commands/queries and their handlers live in **feature folders** under `Features/Authentication/`, one folder per use case, not grouped by type:
  - `Features/Authentication/Register/` — `RegisterCommand(Email, Password) : IRequest<AuthResponse>` + `RegisterCommandHandler`
  - `Features/Authentication/Login/` — `LoginCommand(Email, Password) : IRequest<AuthResponse>` + `LoginCommandHandler`
  - `Features/Authentication/Logout/` — `LogoutCommand : IRequest` (no response) + `LogoutCommandHandler`
  - `Features/Authentication/Me/` — `GetCurrentUserQuery : IRequest<object?>` + `GetCurrentUserQueryHandler`
  - New use cases should follow this same pattern: a `Features/<Area>/<UseCase>/` folder containing the `IRequest`-implementing command/query record and its `IRequestHandler` side by side.
- `Dtos/` — plain C# records used as HTTP request/response contracts: `RegisterRequest(string Email, string Password)`, `LoginRequest(string Email, string Password)`, `AuthResponse(string Token)`. Commands reuse `AuthResponse` as their result type where a token is returned.
- No database, EF Core, identity provider, or JWT/cookie auth scheme is configured yet, despite `UseAuthorization()` being present in the pipeline — there's no corresponding `AddAuthentication()`/`AddAuthorization()` setup or auth scheme configured. All handlers currently throw `NotImplementedException`; when implementing real auth, this wiring and the handler bodies will need to be filled in.
