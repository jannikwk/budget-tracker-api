using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.Enums;
using BudgetTracker.Application.Expenses;

namespace BudgetTracker.Infrastructure.Repositories;

public class ExpensesRepository : IExpensesRepository
{
    private readonly List<Expense> _expenses =
    [
        new Expense(
            Guid.NewGuid(),
            DateOnly.FromDateTime(DateTime.UtcNow),
            100.00m,
            "Amazon",
            Category.Shopping,
            "Electronics",
            PaymentMethod.CreditCard,
            ExpenseType.Personal
        ),
        new Expense(
            Guid.NewGuid(),
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
            50.00m,
            "Starbucks",
            Category.Food,
            "Coffee",
            PaymentMethod.DebitCard,
            ExpenseType.Personal
        )
    ];

    public Task Add(Expense expense, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _expenses.Add(expense);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<Expense>> GetAllAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult<IReadOnlyList<Expense>>(_expenses);
    }
}
