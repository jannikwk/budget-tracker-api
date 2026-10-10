# budget-tracker-api

## Class Diagram
```mermaid
classDiagram-v2
    direction TB

    class Expense {
        <<Entity>>
        + Id: Guid
        + Date: DateTime
        + Amount: decimal
        + Merchant: string
        + Category: string
        + SubCategory: string?
        + PaymentMethod: string
        + ExpenseType: string
        - ~~Constructor~~ Expense(...)
        + Create() Builder$
    }

    class Builder {
        + WithId(id: Guid) Buidler
        + WithDate(date: DateTime) Builder
        + WithAmount(amount: decimal) Builder
        + WithMerchant(merchant: string) Builder
        + WithCategory(category: string) Builder
        + WithSubCategory(subCategory: string) Builder
        + WithPaymentMethod(paymentMethod: string) Builder
        + WithExpenseType(expenseType: string) Builder
        + Build() Expense
    }

    Expense *-- Builder : nested
    Builder ..> Expense : creates
```