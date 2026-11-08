# Budget Tracker API

Budget Tracker is an ASP.NET Core Web API for recording and reviewing personal expenses. It provides a small foundation for a budget-management application by separating the API, application services, domain model, and infrastructure concerns, making the project straightforward to extend with persistence and more.

## Tech Stack

- **Language:** C#
- **Framework:** .NET 10 and ASP.NET Core Web API
- **API documentation:** OpenAPI and Scalar
- **Architecture:** Layered architecture with API, application, domain, and infrastructure projects
- **Testing:** xUnit with Coverlet code coverage

## Features

- Create expenses with date, amount, merchant, category, payment method, and expense type.
- List all recorded expenses.
- Interactive API documentation with Scalar and OpenAPI in development.
- Domain model organized around expenses, income, budgets, and supporting enums.
- Unit-test project for the domain layer.

> **Current storage:** expenses are held in memory and the repository is seeded with two sample records when the API starts. Data is lost when the application stops or restarts.

## Run locally

From the repository root:

```powershell
dotnet restore .\BudgetTracker.slnx
dotnet build .\BudgetTracker.slnx
dotnet run --project .\src\BudgetTracker.Api\BudgetTracker.Api.csproj
```

The API is available at:

- HTTP: `http://localhost:5119`
- Scalar API reference: `http://localhost:5119/scalar`
- OpenAPI document: `http://localhost:5119/openapi/v1.json`

## Current status

The API currently supports creating and listing expenses. Expense data is stored in memory and is reset whenever the application restarts. Retrieving an individual expense, updating expenses, deleting expenses, and persistent database storage are not implemented yet.

## Documentation

- [API - Class diagram](docs/uml/ClassDiagram.md)

## Project structure

```text
src/
├── BudgetTracker.Api/                         HTTP endpoints, middleware, and application startup
├── BudgetTracker.Application/                 Use cases, services, requests, and responses
├── BudgetTracker.Domain/                      Entities and domain enums
└── BudgetTracker.Infrastructure/Repositories  In-memory data access
tests/
└── BudgetTracker.UnitTests/                   Unit tests
```

The solution follows a layered design:

1. **API** receives HTTP requests and exposes the public contract.
2. **Application** coordinates use cases and maps domain entities to responses.
3. **Domain** contains business entities and value classifications.
4. **Infrastructure** provides data-access implementations.

## Development notes

- Development-only Scalar and OpenAPI endpoints are registered when `ASPNETCORE_ENVIRONMENT` is `Development`.
- The API uses the default ASP.NET Core configuration files in `src/BudgetTracker.Api`.
- No database or external service is required for local development.
- The repository currently implements expense creation and listing. Additional operations should be completed before being exposed as supported functionality.
