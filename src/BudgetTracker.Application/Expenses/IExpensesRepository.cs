using BudgetTracker.Domain.Entities;

namespace BudgetTracker.Application.Expenses;

public interface IExpensesRepository
{
    Task Add(Expense expense, CancellationToken cancellationToken);
    Task<IReadOnlyList<Expense>> GetAllAsync(CancellationToken cancellationToken);

}