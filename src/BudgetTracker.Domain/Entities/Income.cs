using System;

namespace BudgetTracker.Domain.Entities;

public class Income
{
    public Guid Id { get; init; }
    public string IncomeType { get; init; }
    public decimal Amount { get; init; }
    public DateTime Date { get; init; }

    public Income(Guid id, string incomeType, decimal amount, DateTime date)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Id must be a valid Id", nameof(id));

        if (string.IsNullOrWhiteSpace(incomeType))
            throw new ArgumentException("Income type is required.", nameof(incomeType));

        if (amount <= 0)
            throw new ArgumentException("Income amount must be greater than zero.", nameof(amount));

        if (date > DateTime.Now)
            throw new ArgumentException("Date cannot be in the future.", nameof(date));

        Id = id;
        IncomeType = incomeType;
        Amount = amount;
        Date = date;
    }
}
