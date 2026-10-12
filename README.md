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
        + ExpenseType: string
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
        Voucher = 10
        
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

    class Income {
        <<Entity>>
        + Id: Guid
        + IncomeType: string
        + Amount: decimal
        + Date: DateTime
        - ~~Constructor~~ Income(Id, IncomeType, Amount, Date)
    }
```