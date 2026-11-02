using BudgetTracker.Application.Expenses;
using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.Enums;

namespace BudgetTracker.Unittests.Expenses;

public class ExpensesServiceTests
{
    [Fact]
    public async Task CreateAsync_AddsExpenseAndReturnsMappedResponse()
    {
        var repository = new FakeExpensesRepository();
        var service = new ExpenseService(repository);
        var request = new CreateExpenseRequest
        {
            Date = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-1),
            Amount = 42.50m,
            Merchant = "Coffee Shop",
            Category = Category.Food,
            SubCategory = "Coffee",
            PaymentMethod = PaymentMethod.CreditCard,
            ExpenseType = ExpenseType.Personal
        };
        using var cancellationSource = new CancellationTokenSource();

        var response = await service.CreateAsync(request, cancellationSource.Token);

        var addedExpense = Assert.Single(repository.AddedExpenses);
        Assert.Equal(addedExpense.Id, response.Id);
        Assert.Equal(request.Date, response.Date);
        Assert.Equal(request.Amount, response.Amount);
        Assert.Equal(request.Merchant, response.Merchant);
        Assert.Equal(request.Category, response.Category);
        Assert.Equal(request.SubCategory, response.SubCategory);
        Assert.Equal(request.PaymentMethod, response.PaymentMethod);
        Assert.Equal(request.ExpenseType, response.ExpenseType);
        Assert.Equal(cancellationSource.Token, repository.LastCancellationToken);
    }

    [Fact]
    public async Task CreateAsync_WithMinimumValidValues_CreatesExpense()
    {
        var repository = new FakeExpensesRepository();
        var service = new ExpenseService(repository);
        var request = new CreateExpenseRequest
        {
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            Amount = 0.01m,
            Merchant = "M",
            Category = Category.Other,
            PaymentMethod = PaymentMethod.Cash,
            ExpenseType = ExpenseType.Variable
        };

        var response = await service.CreateAsync(request, CancellationToken.None);

        Assert.Equal(request.Date, response.Date);
        Assert.Equal(request.Amount, response.Amount);
        Assert.Equal(request.Merchant, response.Merchant);
        Assert.Null(response.SubCategory);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-0.01)]
    public async Task CreateAsync_WithNonPositiveAmount_ThrowsArgumentException(decimal amount)
    {
        var repository = new FakeExpensesRepository();
        var service = new ExpenseService(repository);
        var request = CreateRequest(amount: amount);

        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => service.CreateAsync(request, CancellationToken.None));

        Assert.Equal("amount", exception.ParamName);
        Assert.Empty(repository.AddedExpenses);
    }

    [Fact]
    public async Task CreateAsync_WithFutureDate_ThrowsArgumentException()
    {
        var repository = new FakeExpensesRepository();
        var service = new ExpenseService(repository);
        var request = CreateRequest(date: DateOnly.FromDateTime(DateTime.UtcNow).AddDays(1));

        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => service.CreateAsync(request, CancellationToken.None));

        Assert.Equal("date", exception.ParamName);
        Assert.Empty(repository.AddedExpenses);
    }

    [Theory]
    [InlineData(" ")]
    [InlineData("")]
    public async Task CreateAsync_WithMissingMerchant_ThrowsArgumentException(string merchant)
    {
        var repository = new FakeExpensesRepository();
        var service = new ExpenseService(repository);
        var request = CreateRequest(merchant: merchant);

        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => service.CreateAsync(request, CancellationToken.None));

        Assert.Equal("merchant", exception.ParamName);
        Assert.Empty(repository.AddedExpenses);
    }

    [Fact]
    public async Task CreateAsync_WhenCancelled_PropagatesCancellation()
    {
        var repository = new FakeExpensesRepository();
        var service = new ExpenseService(repository);
        var request = CreateRequest();
        using var cancellationSource = new CancellationTokenSource();
        cancellationSource.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => service.CreateAsync(request, cancellationSource.Token));
    }

    [Fact]
    public async Task GetAllAsync_ReturnsMappedResponses()
    {
        var expenses = new[]
        {
            CreateExpense("First", 10m),
            CreateExpense("Second", 20m)
        };
        var repository = new FakeExpensesRepository(expenses);
        var service = new ExpenseService(repository);
        using var cancellationSource = new CancellationTokenSource();

        var responses = await service.GetAllAsync(cancellationSource.Token);

        Assert.Equal(expenses.Length, responses.Count);
        Assert.Equal(expenses.Select(expense => expense.Id), responses.Select(response => response.Id));
        Assert.Equal(expenses.Select(expense => expense.Merchant), responses.Select(response => response.Merchant));
        Assert.Equal(cancellationSource.Token, repository.LastCancellationToken);
    }

    [Fact]
    public async Task GetAllAsync_WhenRepositoryIsEmpty_ReturnsEmptyList()
    {
        var service = new ExpenseService(new FakeExpensesRepository());

        var responses = await service.GetAllAsync(CancellationToken.None);

        Assert.Empty(responses);
    }

    [Fact]
    public async Task GetAllAsync_WhenCancelled_PropagatesCancellation()
    {
        var repository = new FakeExpensesRepository([CreateExpense("Merchant", 10m)]);
        var service = new ExpenseService(repository);
        using var cancellationSource = new CancellationTokenSource();
        cancellationSource.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => service.GetAllAsync(cancellationSource.Token));
    }

    [Fact]
    public async Task GetByIdAsync_WhenExpenseExists_ReturnsMappedResponse()
    {
        var expense = CreateExpense("Merchant", 15m);
        var repository = new FakeExpensesRepository([expense]);
        var service = new ExpenseService(repository);
        using var cancellationSource = new CancellationTokenSource();

        var response = await service.GetByIdAsync(expense.Id.ToString(), cancellationSource.Token);

        Assert.NotNull(response);
        Assert.Equal(expense.Id, response.Id);
        Assert.Equal(expense.Date, response.Date);
        Assert.Equal(expense.Amount, response.Amount);
        Assert.Equal(expense.Merchant, response.Merchant);
        Assert.Equal(expense.Category, response.Category);
        Assert.Equal(expense.SubCategory, response.SubCategory);
        Assert.Equal(expense.PaymentMethod, response.PaymentMethod);
        Assert.Equal(expense.ExpenseType, response.ExpenseType);
        Assert.Equal(cancellationSource.Token, repository.LastCancellationToken);
    }

    [Fact]
    public async Task GetByIdAsync_WhenExpenseDoesNotExist_ThrowsKeyNotFoundException()
    {
        var repository = new FakeExpensesRepository();
        var service = new ExpenseService(repository);
        const string id = "missing-id";

        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => service.GetByIdAsync(id, CancellationToken.None));

        Assert.Equal($"Expense with id {id} not found.", exception.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("not-a-guid")]
    public async Task GetByIdAsync_WithInvalidId_ThrowsKeyNotFoundException(string id)
    {
        var service = new ExpenseService(new FakeExpensesRepository());

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => service.GetByIdAsync(id, CancellationToken.None));
    }

    [Fact]
    public async Task GetByIdAsync_WhenCancelled_PropagatesCancellation()
    {
        var repository = new FakeExpensesRepository([CreateExpense("Merchant", 10m)]);
        var service = new ExpenseService(repository);
        using var cancellationSource = new CancellationTokenSource();
        cancellationSource.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => service.GetByIdAsync("missing-id", cancellationSource.Token));
    }

    private static CreateExpenseRequest CreateRequest(
        DateOnly? date = null,
        decimal amount = 10m,
        string merchant = "Merchant")
    {
        return new CreateExpenseRequest
        {
            Date = date ?? DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-1),
            Amount = amount,
            Merchant = merchant,
            Category = Category.Food,
            PaymentMethod = PaymentMethod.CreditCard,
            ExpenseType = ExpenseType.Personal
        };
    }

    private static Expense CreateExpense(string merchant, decimal amount)
    {
        return new Expense(
            Guid.NewGuid(),
            DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-1),
            amount,
            merchant,
            Category.Food,
            "Coffee",
            PaymentMethod.CreditCard,
            ExpenseType.Personal);
    }

    private sealed class FakeExpensesRepository(IEnumerable<Expense>? expenses = null) : IExpensesRepository
    {
        private readonly List<Expense> _expenses = expenses?.ToList() ?? [];

        public List<Expense> AddedExpenses { get; } = [];

        public CancellationToken LastCancellationToken { get; private set; }

        public Task Add(Expense expense, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            LastCancellationToken = cancellationToken;
            AddedExpenses.Add(expense);
            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<Expense>> GetAllAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            LastCancellationToken = cancellationToken;
            return Task.FromResult<IReadOnlyList<Expense>>(_expenses);
        }

        public Task<Expense?> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            LastCancellationToken = cancellationToken;
            return Task.FromResult(_expenses.FirstOrDefault(expense => expense.Id.ToString() == id));
        }
    }
}
