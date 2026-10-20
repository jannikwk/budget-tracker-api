using BudgetTracker.Domain.Entities;

namespace BudgetTracker.Application.Expenses;

public interface IExpensesRepository
{
    Task<IReadOnlyList<Expense>> GetAllAsync(CancellationToken cancellationToken);

}