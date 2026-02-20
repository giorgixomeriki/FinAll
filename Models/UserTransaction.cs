using System;

namespace FinancialProfileManagerAPI.Models;

public class UserTransaction
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public TransactionType Type { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public UserProfile UserProfile { get; set; } = null!;
}

public enum TransactionType
{
    Income = 0,
    Expense = 1,
    Transfer = 2
}
