using BudgetTracker.Domain.Enums;

namespace BudgetTracker.Domain.Entities;

public class Budget
{
    public Guid Id { get; init; }
    public decimal TotalPlannedAmount { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }

    public Budget(Guid id, decimal totalPlannedAmount, DateTime startDate, DateTime endDate)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Id cannot be empty.", nameof(id));

        if (totalPlannedAmount <= 0)
            throw new ArgumentException("Total planned amount must be greater than zero.", nameof(totalPlannedAmount));

        if (startDate >= endDate)
            throw new ArgumentException("Start date must be before end date.", nameof(startDate));

        Id = id;
        TotalPlannedAmount = totalPlannedAmount;
        StartDate = startDate;
        EndDate = endDate;
    }

    public BudgetSummary CalculateSummary(
        IEnumerable<Expense> expenses,
        IEnumerable<Income> incomes)
    {
        ArgumentNullException.ThrowIfNull(expenses);
        ArgumentNullException.ThrowIfNull(incomes);

        var expenseTotalsByCategory = expenses
            .Where(expense => IsInRange(expense.Date))
            .GroupBy(expense => expense.Category)
            .ToDictionary(group => group.Key, group => group.Sum(expense => expense.Amount));

        var incomeTotalsByType = incomes
            .Where(income => IsInRange(income.Date))
            .GroupBy(income => income.IncomeType)
            .ToDictionary(group => group.Key, group => group.Sum(income => income.Amount));

        var totalInPeriodExpenses = expenseTotalsByCategory.Values.Sum();

        return new BudgetSummary(
            expenseTotalsByCategory,
            incomeTotalsByType,
            totalInPeriodExpenses,
            TotalPlannedAmount - totalInPeriodExpenses);
    }

    private bool IsInRange(DateTime date) => date >= StartDate && date <= EndDate;
}

public sealed class BudgetSummary
{
    public IReadOnlyDictionary<Category, decimal> ExpenseTotalsByCategory { get; }
    public IReadOnlyDictionary<IncomeType, decimal> IncomeTotalsByType { get; }
    public decimal TotalInPeriodExpenses { get; }
    public decimal RemainingBudget { get; }

    public BudgetSummary(
        IReadOnlyDictionary<Category, decimal> expenseTotalsByCategory,
        IReadOnlyDictionary<IncomeType, decimal> incomeTotalsByType,
        decimal totalInPeriodExpenses,
        decimal remainingBudget)
    {
        ExpenseTotalsByCategory = expenseTotalsByCategory;
        IncomeTotalsByType = incomeTotalsByType;
        TotalInPeriodExpenses = totalInPeriodExpenses;
        RemainingBudget = remainingBudget;
    }
}
