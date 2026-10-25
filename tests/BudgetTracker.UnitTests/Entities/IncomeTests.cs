using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.Enums;

namespace BudgetTracker.Unittests.Entities;

public class IncomeTests
{
    [Fact]
    public void Constructor_WithValidValues_SetsProperties()
    {
        var id = Guid.NewGuid();
        var date = new DateOnly(2026, 1, 15);

        var income = new Income(id, IncomeType.Salary, 4200.50m, date);

        Assert.Equal(id, income.Id);
        Assert.Equal(IncomeType.Salary, income.IncomeType);
        Assert.Equal(4200.50m, income.Amount);
        Assert.Equal(date, income.Date);
    }

    [Fact]
    public void Constructor_WithEmptyId_ThrowsArgumentException()
    {
        var exception = Assert.Throws<ArgumentException>(() => CreateIncome(id: Guid.Empty));

        Assert.Equal("id", exception.ParamName);
    }

    [Fact]
    public void Constructor_WithFutureDate_ThrowsArgumentException()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            CreateIncome(date: DateOnly.FromDateTime(DateTime.Now).AddDays(1)));

        Assert.Equal("date", exception.ParamName);
    }

    [Fact]
    public void Constructor_WithNonPositiveAmount_ThrowsArgumentException()
    {
        var exception = Assert.Throws<ArgumentException>(() => CreateIncome(amount: 0));

        Assert.Equal("amount", exception.ParamName);
    }

    [Theory]
    [InlineData(nameof(Income.IncomeType))]
    public void Constructor_WithMissingRequiredValue_ThrowsArgumentException(string field)
    {
        var exception = Assert.Throws<ArgumentException>(() => field switch
        {
            nameof(Income.IncomeType) => CreateIncome(incomeType: (IncomeType)999),
            _ => throw new ArgumentOutOfRangeException(nameof(field))
        });

        Assert.Equal(field[..1].ToLowerInvariant() + field[1..], exception.ParamName);
    }

    private static Income CreateIncome(
        Guid? id = null,
        IncomeType incomeType = IncomeType.Salary,
        decimal amount = 1000m,
        DateOnly? date = null)
    {
        return new Income(
            id ?? Guid.NewGuid(),
            incomeType,
            amount,
            date ?? DateOnly.FromDateTime(DateTime.Now).AddDays(-1));
    }
}
