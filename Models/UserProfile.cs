using System;

namespace FinancialProfileManagerAPI.Models;

public class UserProfile
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public int? Age { get; set; }
    public AccountTier Tier { get; set; } = AccountTier.Basic;
    public decimal Balance { get; set; }
}
