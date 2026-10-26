# Budget Tracker API

Budget Tracker is an ASP.NET Core Web API for recording and reviewing personal expenses. It provides a small foundation for a budget-management application by separating the API, application services, domain model, and infrastructure concerns, making the project straightforward to extend with persistence, income tracking, budgets, and reporting.

## Features

- Create expenses with date, amount, merchant, category, payment method, and expense type.
- List all recorded expenses.
- Interactive API documentation with Scalar and OpenAPI in development.
- Domain model organized around expenses, income, budgets, and supporting enums.
- Unit-test project for the domain layer.

> **Current storage:** expenses are held in memory and the repository is seeded with two sample records when the API starts. Data is lost when the application stops or restarts.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Git, if cloning the repository

Verify the SDK installation:

```powershell
dotnet --version
```

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

To run the API with the HTTPS launch profile instead:

```powershell
dotnet run --project .\src\BudgetTracker.Api\BudgetTracker.Api.csproj --launch-profile https
```

The HTTPS endpoints are available at:

- HTTPS: `https://localhost:7167`
- Scalar API reference: `https://localhost:7167/scalar`
- OpenAPI document: `https://localhost:7167/openapi/v1.json`

If the local HTTPS certificate is not trusted, run:

```powershell
dotnet dev-certs https --trust
```

## Current status

The API currently supports creating and listing expenses. Expense data is stored in memory and is reset whenever the application restarts. Retrieving an individual expense, updating expenses, deleting expenses, and persistent database storage are not implemented yet.

## API usage

### List expenses

```http
GET http://localhost:5119/api/Expenses
```

### Create an expense

```http
POST http://localhost:5119/api/Expenses
Content-Type: application/json

{
  "date": "2026-10-26",
  "amount": 42.50,
  "merchant": "Example Market",
  "category": 2,
  "subCategory": "Groceries",
  "paymentMethod": 2,
  "expenseType": 3
}
```

The `category`, `paymentMethod`, and `expenseType` fields use the numeric values defined by the domain enums. The create endpoint returns `201 Created` with the created expense.

The included request file at [`src/BudgetTracker.Api/BudgetTracker.Api.http`](src/BudgetTracker.Api/BudgetTracker.Api.http) can also be used with an HTTP client extension in an IDE.

## Testing

Run the unit tests from the repository root:

```powershell
dotnet test .\BudgetTracker.slnx
```

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
