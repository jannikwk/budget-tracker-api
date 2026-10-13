using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.Enums;

namespace BudgetTracker.Unittests.Entities;

public class ExpenseTests
{
    [Fact]
    public void Constructor_WithValidValues_SetsProperties()
    {
        var id = Guid.NewGuid();
        var date = new DateTime(2026, 1, 15);

        var expense = new Expense(
            id,
            date,
            42.50m,
            "Coffee Shop",
            Category.Food,
            "Coffee",
            PaymentMethod.CreditCard,
            ExpenseType.Personal);

        Assert.Equal(id, expense.Id);
        Assert.Equal(date, expense.Date);
        Assert.Equal(42.50m, expense.Amount);
        Assert.Equal("Coffee Shop", expense.Merchant);
        Assert.Equal(Category.Food, expense.Category);
        Assert.Equal("Coffee", expense.SubCategory);
        Assert.Equal(PaymentMethod.CreditCard, expense.PaymentMethod);
        Assert.Equal(ExpenseType.Personal, expense.ExpenseType);
    }

    [Fact]
    public void Constructor_WithEmptyId_ThrowsArgumentException()
    {
        var exception = Assert.Throws<ArgumentException>(() => CreateExpense(id: Guid.Empty));

        Assert.Equal("id", exception.ParamName);
    }

    [Fact]
    public void Constructor_WithFutureDate_ThrowsArgumentException()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            CreateExpense(date: DateTime.Now.AddMinutes(1)));

        Assert.Equal("date", exception.ParamName);
    }

    [Fact]
    public void Constructor_WithNonPositiveAmount_ThrowsArgumentException()
    {
        var exception = Assert.Throws<ArgumentException>(() => CreateExpense(amount: 0));

        Assert.Equal("amount", exception.ParamName);
    }

    [Theory]
    [InlineData(nameof(Expense.Merchant))]
    [InlineData(nameof(Expense.Category))]
    [InlineData(nameof(Expense.PaymentMethod))]
    [InlineData(nameof(Expense.ExpenseType))]
    public void Constructor_WithMissingRequiredValue_ThrowsArgumentException(string field)
    {
        var exception = Assert.Throws<ArgumentException>(() => field switch
        {
            nameof(Expense.Merchant) => CreateExpense(merchant: " "),
            nameof(Expense.Category) => CreateExpense(category: (Category)999),
            nameof(Expense.PaymentMethod) => CreateExpense(paymentMethod: (PaymentMethod)999),
            nameof(Expense.ExpenseType) => CreateExpense(expenseType: (ExpenseType)999),
            _ => throw new ArgumentOutOfRangeException(nameof(field))
        });

        Assert.Equal(field[..1].ToLowerInvariant() + field[1..], exception.ParamName);
    }

    private static Expense CreateExpense(
        Guid? id = null,
        DateTime? date = null,
        decimal amount = 10m,
        string merchant = "Merchant",
        Category category = Category.Food,
        string? subCategory = null,
        PaymentMethod paymentMethod = PaymentMethod.CreditCard,
        ExpenseType expenseType = ExpenseType.Personal)
    {
        return new Expense(
            id ?? Guid.NewGuid(),
            date ?? DateTime.Now.AddDays(-1),
            amount,
            merchant,
            category,
            subCategory,
            paymentMethod,
            expenseType);
    }
}
