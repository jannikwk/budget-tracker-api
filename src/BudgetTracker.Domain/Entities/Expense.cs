using BudgetTracker.Domain.Enums;

namespace BudgetTracker.Domain.Entities;

public class Expense
{
	public Guid Id { get; init; }
	public DateTime Date { get; init; }
	public decimal Amount { get; init; }
	public string Merchant { get; init; }
	public Category Category { get; init; }
	public string? SubCategory { get; init; }
	public PaymentMethod PaymentMethod { get; init; }
	public string ExpenseType { get; init; }

	public Expense(Guid id, DateTime date, decimal amount, string merchant, Category category,
		string? subCategory, PaymentMethod paymentMethod, string expenseType)
	{
		if (id == Guid.Empty)
			throw new ArgumentException("Id cannot be empty.", nameof(id));

		if (date > DateTime.Now)
			throw new ArgumentException("Date cannot be in the future.", nameof(date));

		if (amount <= 0)
			throw new ArgumentException("Expense amount must be greater than zero.", nameof(amount));

		if (string.IsNullOrWhiteSpace(merchant))
			throw new ArgumentException("Merchant name is required.", nameof(merchant));

		if (!Enum.IsDefined(category))
			throw new ArgumentException("Category is required.", nameof(category));

		if (!Enum.IsDefined(paymentMethod))
			throw new ArgumentException("Payment method is required.", nameof(paymentMethod));

		if (string.IsNullOrWhiteSpace(expenseType))
			throw new ArgumentException("Expense type is required.", nameof(expenseType));

		Id = id;
		Date = date;
		Amount = amount;
		Merchant = merchant;
		Category = category;
		SubCategory = subCategory;
		PaymentMethod = paymentMethod;
		ExpenseType = expenseType;
	}
}