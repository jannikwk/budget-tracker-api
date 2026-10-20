namespace BudgetTracker.Application.Expenses;

public interface IExpensesService
{
    Task<IReadOnlyList<ExpenseResponse>> GetAllAsync(
        CancellationToken cancellationToken);

    Task<ExpenseResponse?> GetByIdAsync(
        string id,
        CancellationToken cancellationToken);

    Task<ExpenseResponse> CreateAsync(
        CreateExpenseRequest request,
        CancellationToken cancellationToken);

    Task<bool> UpdateAsync(
        string id,
        UpdateExpenseRequest request,
        CancellationToken cancellationToken);

    Task<bool> DeleteAsync(
        string id,
        CancellationToken cancellationToken);

}
