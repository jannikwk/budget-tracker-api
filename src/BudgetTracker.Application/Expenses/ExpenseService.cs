using BudgetTracker.Domain.Entities;

namespace BudgetTracker.Application.Expenses;

public class ExpenseService : IExpensesService
{
    private readonly IExpensesRepository _expensesRepository;

    public ExpenseService(IExpensesRepository expensesRepository)
    {
        _expensesRepository = expensesRepository;
    }

    public Task<ExpenseResponse> CreateAsync(CreateExpenseRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAsync(string id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<IReadOnlyList<ExpenseResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        var exenses = await _expensesRepository.GetAllAsync(cancellationToken);
        return [.. exenses.Select(MapToExpenseResponse)];
    }

    public Task<ExpenseResponse?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateAsync(string id, UpdateExpenseRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    private static ExpenseResponse MapToExpenseResponse(Expense expense)
    {
        return new ExpenseResponse(
            expense.Id,
            expense.Date,
            expense.Amount,
            expense.Merchant,
            expense.Category,
            expense.SubCategory,
            expense.PaymentMethod,
            expense.ExpenseType
        );
    }
}
