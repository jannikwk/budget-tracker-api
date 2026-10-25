using BudgetTracker.Domain.Enums;

namespace BudgetTracker.Application.Expenses;

public sealed record ExpenseResponse(
    Guid Id,
    DateOnly Date,
    decimal Amount,
    string Merchant,
    Category Category,
    string? SubCategory,
    PaymentMethod PaymentMethod,
    ExpenseType ExpenseType
);
