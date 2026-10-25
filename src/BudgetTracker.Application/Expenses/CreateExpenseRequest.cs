using BudgetTracker.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace BudgetTracker.Application.Expenses;

/// <summary>Payload for creating an expense.</summary>
public sealed record CreateExpenseRequest
{
    [Required]
    public DateOnly Date { get; init; }

    [Range(typeof(decimal), "0.01", "1000000000", ParseLimitsInInvariantCulture = true)]
    public decimal Amount { get; init; }

    [Required, StringLength(200)]
    public string Merchant { get; init; } = string.Empty;

    [EnumDataType(typeof(Category))]
    public Category Category { get; init; }

    [StringLength(200)]
    public string? SubCategory { get; init; }

    [EnumDataType(typeof(PaymentMethod))]
    public PaymentMethod PaymentMethod { get; init; }

    [EnumDataType(typeof(ExpenseType))]
    public ExpenseType ExpenseType { get; init; }
}