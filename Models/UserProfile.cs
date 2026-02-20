using System;
using System.Collections.Generic;

namespace FinancialProfileManagerAPI.Models;

public class UserProfile
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public int? Age { get; set; }
    public AccountTier Tier { get; set; } = AccountTier.Basic;
    public decimal Balance { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public ICollection<UserTransaction> Transactions { get; set; } = new List<UserTransaction>();
}
