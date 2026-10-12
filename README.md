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
    
    Expense --> PaymentMethod : uses

    class Income {
        <<Entity>>
        + Id: Guid
        + IncomeType: string
        + Amount: decimal
        + Date: DateTime
        - ~~Constructor~~ Income(Id, IncomeType, Amount, Date)
    }
```