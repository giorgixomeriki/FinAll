using System;
using System.Collections.Generic;

namespace FinancialProfileManagerAPI.Models
{
    public class Analytics
    {
        public int TotalUsers { get; set; }
        public decimal AverageBalance { get; set; }
        public decimal TotalBalance { get; set; }
        public UserProfile HighestBalanceUser { get; set; }
        public UserProfile LowestBalanceUser { get; set; }
        public Dictionary<AccountTier, int> UsersByTier { get; set; } = new();
        public List<UserProfile> TopSpenders { get; set; } = new();
        public List<TransactionStats> RecentTransactions { get; set; } = new();
    }

    public class TransactionStats
    {
        public string UserName { get; set; }
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
