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
        + Category: Category
        + SubCategory: string?
        + PaymentMethod: PaymentMethod
        + ExpenseType: ExpenseType
        - ~~Constructor~~ Expense(...)
    }
    
    class PaymentMethod {
        <<Enum>>
        Cash = 1
        DebitCard = 2
        CreditCard = 3
        BankTransfer = 4
        DirectDebit = 5
        MobilePayment = 6
        DigitalWallet = 7
        BuyNowPayLater = 8
        Crypto = 9
        Voucher = 19
        
    }

    class ExpenseType {
        <<Enum>>
        Personal = 1
        Fixed = 2
        Variable = 3
        OneTime = 4
        Subscription = 5
    }

    class Category {
        <<Enum>>
        Housing = 1
        Food = 2
        Transportation = 3
        Utilities = 4
        Healthcare = 5
        Insurance = 6
        DebtRepayment = 7
        PersonalCare = 8
        Entertainment = 9
        Education = 10
        Travel = 11
        Shopping = 12
        GiftsAndDonations = 13
        SavingsAndInvestments = 14
        Taxes = 15
        Other = 16
    }
    
    Expense --> PaymentMethod : uses
    Expense --> Category : uses
    Expense --> ExpenseType : uses

    class Income {
        <<Entity>>
        + Id: Guid
        + IncomeType: IncomeType
        + Amount: decimal
        + Date: DateTime
        - ~~Constructor~~ Income(Id, IncomeType, Amount, Date)
    }

    class IncomeType {
        <<Enum>>
        Salary = 1
        Freelance = 2
        Business = 3
        Investment = 4
        Rental = 5
        GovernmentBenefit = 6
        Gift = 7
        Other = 8
    }

    Income --> IncomeType : uses
    
    class Budget {
        <<Entity>>
        + Id: Guid
        + TotalPlannedAmount: decimal
        + StartDate: DateTime
        + EndDate: DateTime
        + ~~Constructor~~ Budget(Id, TotalPlannedAmount, StartDate, EndDate)
        + CalculateSummary() BudgetSummary
        - IsInRange(date: DateTime) bool
    }
    
    class BudgetSummary {
        + ExpenseTotalsByCategory: IReadOnlyDictionary<Category, decimal>
        + IncomeTotalsByType: IReadOnlyDictionary<IncomeType, decimal>
        + TotalInPeriodExpenses: decimal
        + RemainingBudget: decimal
        + ~~Constructor~~ BudgetSummary(...)
    }
    
    Budget --> BudgetSummary : calculates
    Budget ..> Expense : summarizes
    Budget ..> Income : summarizes
    BudgetSummary ..> Category : groups by
    BudgetSummary ..> IncomeType : groups by
```