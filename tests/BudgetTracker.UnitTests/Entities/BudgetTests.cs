using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.Enums;

namespace BudgetTracker.Unittests.Entities;

public class BudgetTests
{
    [Fact]
    public void CalculateSummary_FiltersByDateAndAggregatesDynamicCategories()
    {
        var budget = CreateBudget();
        var expenses = new[]
        {
            CreateExpense(new DateOnly(2026, 1, 1), 100m, Category.Food),
            CreateExpense(new DateOnly(2026, 1, 15), 25m, Category.Food),
            CreateExpense(new DateOnly(2026, 1, 31), 50m, Category.Housing),
            CreateExpense(new DateOnly(2025, 12, 31), 900m, Category.Food),
            CreateExpense(new DateOnly(2026, 2, 1), 900m, Category.Housing)
        };
        var incomes = new[]
        {
            CreateIncome(new DateOnly(2026, 1, 1), 2000m, IncomeType.Salary),
            CreateIncome(new DateOnly(2026, 1, 20), 500m, IncomeType.Salary),
            CreateIncome(new DateOnly(2026, 1, 31), 300m, IncomeType.Freelance),
            CreateIncome(new DateOnly(2025, 12, 31), 5000m, IncomeType.Salary)
        };

        var summary = budget.CalculateSummary(expenses, incomes);

        Assert.Equal(125m, summary.ExpenseTotalsByCategory[Category.Food]);
        Assert.Equal(50m, summary.ExpenseTotalsByCategory[Category.Housing]);
        Assert.Equal(2500m, summary.IncomeTotalsByType[IncomeType.Salary]);
        Assert.Equal(300m, summary.IncomeTotalsByType[IncomeType.Freelance]);
        Assert.Equal(175m, summary.TotalInPeriodExpenses);
        Assert.Equal(825m, summary.RemainingBudget);
        Assert.DoesNotContain(Category.Entertainment, summary.ExpenseTotalsByCategory.Keys);
    }

    [Fact]
    public void Constructor_WhenStartDateIsNotBeforeEndDate_ThrowsArgumentException()
    {
        var exception = Assert.Throws<ArgumentException>(() => CreateBudget(
            startDate: new DateOnly(2026, 1, 31),
            endDate: new DateOnly(2026, 1, 1)));

        Assert.Equal("startDate", exception.ParamName);
    }

    [Fact]
    public void Constructor_WhenIdIsEmpty_ThrowsArgumentException()
    {
        var exception = Assert.Throws<ArgumentException>(() => CreateBudget(id: Guid.Empty));

        Assert.Equal("id", exception.ParamName);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_WhenTotalPlannedAmountIsNotPositive_ThrowsArgumentException(decimal amount)
    {
        var exception = Assert.Throws<ArgumentException>(() => CreateBudget(totalPlannedAmount: amount));

        Assert.Equal("totalPlannedAmount", exception.ParamName);
    }

    private static Budget CreateBudget(
        Guid? id = null,
        decimal totalPlannedAmount = 1000m,
        DateOnly? startDate = null,
        DateOnly? endDate = null)
    {
        return new Budget(
            id ?? Guid.NewGuid(),
            totalPlannedAmount,
            startDate ?? new DateOnly(2026, 1, 1),
            endDate ?? new DateOnly(2026, 1, 31));
    }

    private static Expense CreateExpense(DateOnly date, decimal amount, Category category)
    {
        return new Expense(
            Guid.NewGuid(),
            date,
            amount,
            "Merchant",
            category,
            null,
            PaymentMethod.CreditCard,
            ExpenseType.Personal);
    }

    private static Income CreateIncome(DateOnly date, decimal amount, IncomeType incomeType)
    {
        return new Income(Guid.NewGuid(), incomeType, amount, date);
    }
}