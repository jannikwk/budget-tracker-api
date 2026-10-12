using BudgetTracker.Domain.Entities;

namespace BudgetTracker.Unittests.Entities;

public class IncomeTests
{
    [Fact]
    public void Constructor_WithValidValues_SetsProperties()
    {
        var id = Guid.NewGuid();
        var date = new DateTime(2026, 1, 15);

        var income = new Income(id, "Salary", 4200.50m, date);

        Assert.Equal(id, income.Id);
        Assert.Equal("Salary", income.IncomeType);
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
            CreateIncome(date: DateTime.Now.AddMinutes(1)));

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
    public void Constructor_WithMissingRequiredText_ThrowsArgumentException(string field)
    {
        var exception = Assert.Throws<ArgumentException>(() => field switch
        {
            nameof(Income.IncomeType) => CreateIncome(incomeType: " "),
            _ => throw new ArgumentOutOfRangeException(nameof(field))
        });

        Assert.Equal(field[..1].ToLowerInvariant() + field[1..], exception.ParamName);
    }

    private static Income CreateIncome(
        Guid? id = null,
        string incomeType = "Salary",
        decimal amount = 1000m,
        DateTime? date = null)
    {
        return new Income(
            id ?? Guid.NewGuid(),
            incomeType,
            amount,
            date ?? DateTime.Now.AddDays(-1));
    }
}
