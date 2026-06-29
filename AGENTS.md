# AGENTS.md

## Project Overview

This repository is a C# ASP.NET Core MVC application organized with Clean Architecture.

Solution file:

- `WEBAPP.slnx`

Projects:

- `APP.Domain` - domain entities and shared domain primitives.
- `APP.Application` - application services, DTOs, interfaces, authorization and permission logic.
- `APP.Infrastructure` - external implementations such as SQL-backed stores and infrastructure service registration.
- `APP.WEB` - ASP.NET Core MVC host, controllers, views, authorization handlers, static assets, and composition root.
- `App_Data` - database setup scripts and application data files.

## Architecture Rules

- Keep dependency direction inward:
  - `APP.Domain` must not reference other application projects.
  - `APP.Application` may reference `APP.Domain`.
  - `APP.Infrastructure` may reference `APP.Application` and `APP.Domain`.
  - `APP.WEB` may reference `APP.Application` and `APP.Infrastructure`.
- Put business rules and use-case logic in `APP.Application`, not in MVC controllers.
- Put persistence and external service implementations in `APP.Infrastructure`.
- Put entities and domain primitives in `APP.Domain`.
- Keep `APP.WEB` focused on HTTP concerns, view models, routing, authentication, authorization wiring, and Razor views.

## Dependency Injection

- Register application services in `APP.Application/DependencyInjection.cs`.
- Register infrastructure services in `APP.Infrastructure/DependencyInjection.cs`.
- Keep ASP.NET Core framework setup and final composition in `APP.WEB/Program.cs`.
- Prefer constructor injection. Do not use service locators or static service access.

## Coding Standards

- Use C# with nullable reference types enabled.
- Follow the existing file and namespace style.
- Use async/await for I/O-bound work and expose asynchronous APIs where persistence or external resources are involved.
- Avoid unnecessary comments. Add comments only when they clarify non-obvious behavior.
- Keep changes scoped to the requested behavior.
- Do not introduce broad refactors unless they are needed for the task.

## Web Layer Guidelines

- Controllers in `APP.WEB/Controllers` should delegate business work to application services.
- View models belong in `APP.WEB/Models`.
- Razor views belong under `APP.WEB/Views`.
- Authorization policy provider, requirements, and handlers that depend on ASP.NET Core belong in `APP.WEB/Authorization`.
- Static assets belong in `APP.WEB/wwwroot`.

## Application Layer Guidelines

- Application service interfaces and implementations belong under feature folders such as `APP.Application/Identity` and `APP.Application/Security`.
- Define abstractions for infrastructure dependencies in `APP.Application`.
- Return DTOs or result types from application services instead of web-specific types.

## Infrastructure Guidelines

- Implement application abstractions in `APP.Infrastructure`.
- Keep SQL, connection handling, and persistence details out of `APP.Application` and `APP.WEB`.
- Register implementations through `AddInfrastructure`.

## Verification

Before finishing code changes, run the most relevant checks:

```powershell
dotnet build WEBAPP.slnx
```

For web changes, also run the app when practical:

```powershell
dotnet run --project APP.WEB/APP.WEB.csproj
```

## Agent Workflow

- Explain intended changes before editing files.
- Inspect existing patterns before adding new code.
- Preserve existing user changes and do not revert unrelated files.
- Prefer small, focused commits or patches.
- If a command fails because dependencies or SDKs are unavailable, report the exact blocker and what was not verified.
