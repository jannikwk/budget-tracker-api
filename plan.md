# Core Budget API Domain - Detailed Implementation Plan

## 1. Objective

Complete the domain model needed for a core budget API, excluding authentication and login concerns. The implementation should cover the full non-auth budgeting lifecycle: organizing money into accounts, recording income and expenses, defining budgets and category limits, supporting recurring financial items, and calculating budget summaries.

This plan distinguishes between:

- **Already present:** `Expense`, `Income`, and their supporting enums.
- **Missing and required:** budget, allocation, account, recurring-item, and summary concepts.
- **Out of scope:** users, authentication, authorization, persistence, HTTP/API plumbing, and unrelated financial features.

No implementation should begin from this plan alone; the plan is intended to guide a later implementation pass.

## 2. Current repository state

### Existing entities

- `src/BudgetTracker.Domain/Entities/Expense.cs`
  - Date, amount, merchant, category, optional subcategory, payment method, and expense type.
  - Validates required values, positive amounts, and future dates.
- `src/BudgetTracker.Domain/Entities/Income.cs`
  - Income type, amount, and date.
  - Validates required values, positive amounts, and future dates.

### Existing enums

- `Category`
- `ExpenseType`
- `IncomeType`
- `PaymentMethod`

These should be reused. New enums should be introduced only for concepts that cannot be represented clearly by the existing enums.

### Missing application/domain surfaces

- No `Budget` entity or budget period model.
- No per-category budget allocation/limit model.
- No account, wallet, bank-account, or cash-source model.
- No recurring income/expense model.
- No budget summary or reporting result model.
- No domain services for aggregation or transaction classification.
- No persistence, repositories, DTOs, controllers, routes, or API wiring.
- No user/authentication/authorization model.

### Existing test state

Tests currently cover only `Expense` and `Income` constructors and properties in `tests/BudgetTracker.UnitTests/Entities`. New domain types should follow the same xUnit and validation conventions.

## 3. Scope boundaries

### Included

1. Core financial accounts/sources.
2. Income and expense transaction model completion where the existing entities have gaps.
3. Budget periods and overall budget limits.
4. Category-level budget allocations.
5. Recurring income and expense definitions.
6. Budget and account summary result types/calculation behavior.
7. Supporting enums and value objects.
8. Domain validation and unit tests.

### Excluded

- `User`, authentication, login, registration, authorization, JWT, claims, roles, or ownership.
- EF Core, `DbContext`, migrations, database configurations, repositories, and persistence mappings.
- Controllers, routes, request/response DTOs, application services, OpenAPI changes, and API integration tests.
- Notifications, imports, exports, bank synchronization, currency conversion, investments, debt repayment, and tax reporting.
- UI-specific models.

## 4. Target domain model

### 4.1 `Budget`

Target: `src/BudgetTracker.Domain/Entities/Budget.cs`

Represents a budget for a defined period.

Proposed data:

- Identifier.
- Name or label, if multiple named budgets are expected.
- Start date and end date.
- Overall planned amount/limit.
- Budget status if lifecycle state is required.
- Read-only collection of category allocations.

Required behavior:

- Create a valid budget.
- Add, replace, update, and remove category allocations through controlled methods.
- Prevent duplicate categories.
- Prevent invalid dates and non-positive amounts.
- Expose collection state without allowing callers to bypass validation.
- Produce or delegate calculation of a budget summary.

Potential enum:

- `BudgetStatus` such as `Draft`, `Active`, `Closed`, or `Archived`, only if lifecycle behavior is actually required. Do not add a status enum merely for future API filtering.

### 4.2 `BudgetCategoryAllocation`

Target: `src/BudgetTracker.Domain/Entities/BudgetCategoryAllocation.cs` or a value-object location.

Represents the planned amount for one category within a budget.

Proposed data:

- Category using the existing `Category` enum.
- Planned amount/limit.
- Optional note or label only if the domain requires it.

Required behavior:

- Validate positive amount.
- Validate that the category is defined.
- Support controlled amount updates.
- Be uniquely addressable by category inside one budget.

Decision:

- Prefer a value object if the allocation has no independent lifecycle.
- Use an entity only if it needs its own identity or independent mutation history.

### 4.3 `FinancialAccount`

Target: `src/BudgetTracker.Domain/Entities/FinancialAccount.cs`

Represents where money is held or from where a transaction is paid/received. This is distinct from `PaymentMethod`: cash, card, transfer, or voucher describes how a transaction occurred, while an account describes the financial source or destination.

Proposed data:

- Identifier.
- Name.
- Account type.
- Current/opening balance if balance tracking is part of the domain.
- Currency code if the API is intended to support more than one currency.
- Active/closed state if accounts can be retired.

Potential enum:

- `AccountType`: `Cash`, `Bank`, `CreditCard`, `DebitCard`, `Savings`, `Wallet`, or `Other`, limited to actual product requirements.

Required behavior:

- Validate name and supported account type.
- Define whether balance is derived from transactions or stored and mutated.
- Prevent invalid negative balances only if overdrafts are not allowed by the product.
- Define how expenses and income reference the account.

The account model must not contain user ownership or authentication fields in this scope.

### 4.4 Existing `Expense` completion

Existing file: `src/BudgetTracker.Domain/Entities/Expense.cs`

Do not replace the existing entity. Review whether it needs narrowly scoped additions for the core API:

- Account reference, if `FinancialAccount` is included.
- Stable transaction identifier conventions.
- Optional description/notes if merchant is insufficient.
- Optional currency only if multi-currency is explicitly supported.
- Explicit recurring-item reference only if recurring instances need traceability.

Preserve existing validation and public behavior unless the new account/recurring design requires a coordinated change.

### 4.5 Existing `Income` completion

Existing file: `src/BudgetTracker.Domain/Entities/Income.cs`

Review whether it needs the same cross-cutting transaction fields as `Expense`:

- Account reference.
- Stable identifier convention.
- Optional description/notes.
- Optional currency.
- Optional recurring-item reference.

Income must remain separate from expenses unless a shared transaction abstraction is clearly justified by existing conventions and does not add unnecessary complexity.

### 4.6 Recurring financial item

Target:

- `src/BudgetTracker.Domain/Entities/RecurringExpense.cs`
- `src/BudgetTracker.Domain/Entities/RecurringIncome.cs`
- Or one carefully designed `RecurringTransaction` model if the shared shape is genuinely stable.

Represents a rule for future transactions, not an individual posted transaction.

Proposed data:

- Identifier.
- Description/merchant/source.
- Amount.
- Category or income type.
- Account reference if accounts are included.
- Start date.
- Optional end date.
- Recurrence frequency.
- Next occurrence date.
- Active/paused state.

Potential enums/value objects:

- `RecurrenceFrequency`: daily, weekly, monthly, yearly, or a constrained custom schedule.
- `RecurringItemStatus`: active, paused, ended, or cancelled.

Required behavior:

- Validate positive amount and valid dates.
- Ensure end date is not before start date.
- Calculate or advance the next occurrence deterministically.
- Prevent invalid recurrence intervals.
- Define whether recurring rules create transaction entities in the domain or only provide schedule data for a later application service.

The domain-only implementation should not silently persist or post transactions.

### 4.7 Budget summary and reporting results

Potential files:

- `src/BudgetTracker.Domain/ValueObjects/BudgetSummary.cs`
- `src/BudgetTracker.Domain/ValueObjects/CategoryBudgetSummary.cs`
- `src/BudgetTracker.Domain/ValueObjects/AccountSummary.cs`

Budget summary should support:

- Overall planned amount.
- Total eligible expenses.
- Total eligible income.
- Remaining budget.
- Total category allocation.
- Per-category planned, spent, remaining, and overspent values.
- Optional percentage used, with a defined zero-budget rule.

Account summary should support, if account balances are included:

- Opening/current balance semantics.
- Total income.
- Total expenses.
- Derived balance.
- Account period filtering.

All calculations must use `decimal`, define rounding/precision behavior, and avoid hidden current-time or external-service dependencies.

## 5. Transaction relationships and design decisions

These decisions must be made before implementation because the current entities do not contain account or budget relationships.

### 5.1 Transaction-to-budget relationship

Preferred initial option:

- Include transactions by an inclusive budget date range.
- Keep `BudgetId` out of `Expense` and `Income`.
- Let summary calculation receive transactions and filter them by date.

Alternative:

- Add an explicit `BudgetId` to transactions if the product requires one transaction to belong to a specific budget independently of its date.

Do not introduce both mechanisms without a clear precedence rule.

### 5.2 Transaction-to-account relationship

Preferred option if `FinancialAccount` is included:

- Add an optional or required account reference to `Expense` and `Income`, based on whether unassigned transactions are valid.
- Define whether transfers between accounts are in scope. If they are not, do not model transfers as income or expenses.

### 5.3 Income treatment

Choose and document whether income:

- Is reported separately while remaining is `budget - expenses`.
- Contributes to available funds as `budget + income - expenses`.
- Is included only in account summaries and excluded from spending-limit summaries.

The same formula must be used consistently in code and tests.

### 5.4 Categories

Reuse the existing `Category` enum for expense allocations. Decide whether income categories need a separate enum beyond the existing `IncomeType`; do not overload expense categories for income without a domain reason.

## 6. Implementation phases

### Phase A - domain inventory and decisions

1. Review existing entities, enums, project references, and tests.
2. Decide identifier conventions and whether a shared transaction abstraction is warranted.
3. Decide whether accounts are required for the first core API slice.
4. Decide whether recurring items are separate income/expense entities or one shared entity.
5. Decide budget inclusion, account linking, income treatment, currency, and transfer behavior.
6. Record decisions in code-level documentation or the implementation PR, not as undocumented assumptions.

### Phase B - supporting value objects and enums

1. Add only required enums such as account type, budget status, recurrence frequency, or recurring-item status.
2. Add period/date-range and money/currency value objects only if they reduce duplicated validation.
3. Ensure enum values and value objects have explicit validation and stable semantics.

### Phase C - core entity implementation

Implement in dependency order:

1. `FinancialAccount`, if included.
2. Shared transaction fields or narrowly scoped updates to `Expense` and `Income`.
3. `BudgetCategoryAllocation`.
4. `Budget`.
5. Recurring expense/income model.
6. Summary result types and deterministic calculation behavior.

Enforce invariants through constructors/factories and controlled mutation methods. Expose collections as read-only. Avoid persistence attributes and API DTO concerns.

### Phase D - domain tests

Add tests under the existing test structure:

- `tests/BudgetTracker.UnitTests/Entities/BudgetTests.cs`
- `tests/BudgetTracker.UnitTests/Entities/BudgetCategoryAllocationTests.cs`
- `tests/BudgetTracker.UnitTests/Entities/FinancialAccountTests.cs`
- `tests/BudgetTracker.UnitTests/Entities/RecurringExpenseTests.cs`
- `tests/BudgetTracker.UnitTests/Entities/RecurringIncomeTests.cs`
- `tests/BudgetTracker.UnitTests/ValueObjects/BudgetSummaryTests.cs`
- Additional value-object tests where applicable.

Cover:

- Valid construction and property state.
- Required strings and invalid enum values.
- Positive monetary amounts and decimal precision.
- Invalid date ordering and boundary dates.
- Budget allocation uniqueness and collection protection.
- Category spending, overspending, and unallocated spending.
- In-period versus out-of-period transactions.
- Income treatment and remaining-balance formulas.
- Account balance derivation and account assignment.
- Recurrence calculation, end dates, paused/ended state, and next-occurrence advancement.
- Existing `Expense` and `Income` regression behavior.

### Phase E - verification

1. Run the full focused domain unit-test suite.
2. Run build/format checks already configured in the repository.
3. Confirm no auth/user/persistence/API dependencies were introduced.
4. Review public constructors and methods for domain clarity and future API usability.
5. Check that all acceptance criteria below are demonstrably covered by tests.

## 7. Likely file checklist

### New domain files

- `src/BudgetTracker.Domain/Entities/Budget.cs`
- `src/BudgetTracker.Domain/Entities/BudgetCategoryAllocation.cs` or a value-object equivalent.
- `src/BudgetTracker.Domain/Entities/FinancialAccount.cs`
- `src/BudgetTracker.Domain/Entities/RecurringExpense.cs`
- `src/BudgetTracker.Domain/Entities/RecurringIncome.cs`
- `src/BudgetTracker.Domain/ValueObjects/BudgetSummary.cs`
- `src/BudgetTracker.Domain/ValueObjects/CategoryBudgetSummary.cs`, if needed.
- `src/BudgetTracker.Domain/ValueObjects/AccountSummary.cs`, if needed.
- Supporting enum/value-object files only where required.

### Existing files that may change

- `src/BudgetTracker.Domain/Entities/Expense.cs`
- `src/BudgetTracker.Domain/Entities/Income.cs`
- Existing enum files if values need correction or extension.
- Existing unit tests for coordinated constructor/property changes.

### Files excluded from this plan

- API startup and controllers.
- Application services and DTOs.
- Infrastructure, database, migrations, and repositories.
- Authentication, users, authorization, and login.

## 8. Acceptance criteria

- The domain contains all selected non-auth core entities required for a usable budget API: transactions, budgets, allocations, accounts, recurring items, and summaries.
- Existing `Expense` and `Income` are reused and enhanced only where necessary; they are not duplicated.
- Budgets have validated periods, overall limits, and category allocations.
- Accounts have explicit type and balance semantics if included.
- Recurring financial items have validated schedules and deterministic next-occurrence behavior if included.
- Summary formulas and transaction inclusion rules are explicit and tested.
- Invalid dates, amounts, enum values, duplicate allocations, and invalid recurrence states cannot create invalid domain state.
- Collections are protected from mutation outside the owning aggregate.
- Unit tests cover each new entity and regression-test existing transaction behavior.
- No user, authentication, authorization, persistence, API, or unrelated financial feature is introduced.
