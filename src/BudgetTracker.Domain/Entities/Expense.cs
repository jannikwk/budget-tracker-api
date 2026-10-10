using System;

namespace BudgetTracker.Domain.Entities;

public class Expense
{
	public Guid Id { get; init; }
	public DateTime Date { get; init; }
	public decimal Amount { get; init; }
	public string Merchant { get; init; }
	public string Category { get; init; }
	public string? SubCategory { get; init; }
	public string PaymentMethod { get; init; }
	public string ExpenseType { get; init; }

	private Expense(Guid id, DateTime date, decimal amount, string merchant, string category, string? subCategory, string paymentMethod, string expenseType)
	{
		Id = id;
		Date = date;
		Amount = amount;
		Merchant = merchant;
		Category = category;
		SubCategory = subCategory;
		PaymentMethod = paymentMethod;
		ExpenseType = expenseType;
	}

	public static Builder Create() => new();
	public sealed class Builder
	{
		private Guid _id = Guid.NewGuid();
		private DateTime _date;
		private decimal _amount;
		private string _merchant;
		private string _category;
		private string? _subCategory;
		private string _paymentMethod;
		private string _expenseType;

		public Builder WithId(Guid id)
		{
			_id = id;
			return this;
		}

		public Builder WithDate(DateTime date)
		{
			_date = date;
			return this;
		}

		public Builder WithAmount(decimal amount)
		{
			_amount = amount;
			return this;
		}

		public Builder WithMerchant(string merchant)
		{
			_merchant = merchant;
			return this;
		}

		public Builder WithCategory(string category)
		{
			_category = category;
			return this;
		}

		public Builder WithSubCategory(string subCategory)
		{
			_subCategory = subCategory;
			return this;
		}

		public Builder WithPaymentMethod(string paymentMethod)
		{
			_paymentMethod = paymentMethod;
			return this;
		}

		public Builder WithExpenseType(string expenseType)
		{
			_expenseType = expenseType;
			return this;
		}

		public Expense Build()
		{
			return new Expense(_id, _date, _amount, _merchant, _category, _subCategory, _paymentMethod, _expenseType);
		}
	}
}