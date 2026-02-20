using System.Collections.Generic;
using FinancialProfileManagerAPI.Models;
using System.Linq;
using System;
using FinancialProfileManagerAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace FinancialProfileManagerAPI.Services;

public class UserProfileService
{
    private readonly AppDbContext _context;

    public UserProfileService(AppDbContext context)
    {
        _context = context;
    }

    public UserProfile AddUser(UserProfile user)
    {
        ValidateUser(user);
        UpdateTier(user);

        _context.UserProfiles.Add(user);
        _context.SaveChanges();

        return user;
    }

    public IEnumerable<UserProfile> GetAll()
    {
        return _context.UserProfiles
            .Include(u => u.Transactions)
            .ToList();
    }

    public UserProfile GetUserById(Guid id)
    {
        return _context.UserProfiles
            .Include(u => u.Transactions)
            .FirstOrDefault(u => u.Id == id);
    }

    public void UpdateUser(Guid id, UserProfile updatedUser)
    {
        ValidateUser(updatedUser);

        var existing = _context.UserProfiles.FirstOrDefault(u => u.Id == id);
        if (existing != null)
        {
            existing.Name = updatedUser.Name;
            existing.Age = updatedUser.Age;
            existing.Balance = updatedUser.Balance;
            existing.UpdatedAt = DateTime.UtcNow;
            UpdateTier(existing);
            _context.SaveChanges();
        }
    }

    public void DeleteUser(UserProfile user)
    {
        _context.UserProfiles.Remove(user);
        _context.SaveChanges();
    }

    public void AddTransaction(Guid userId, UserTransaction transaction)
    {
        var user = _context.UserProfiles.FirstOrDefault(u => u.Id == userId);
        if (user != null)
        {
            transaction.UserId = userId;

            // Update user balance
            if (transaction.Type == TransactionType.Income)
                user.Balance += transaction.Amount;
            else
                user.Balance -= transaction.Amount;

            user.UpdatedAt = DateTime.UtcNow;
            UpdateTier(user);

            _context.UserTransactions.Add(transaction);
            _context.SaveChanges();
        }
    }

    public IEnumerable<UserTransaction> GetUserTransactions(Guid userId)
    {
        return _context.UserTransactions
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .ToList();
    }

    public IEnumerable<UserProfile> SearchByName(string name)
    {
        return _context.UserProfiles
            .Where(u => u.Name.Contains(name))
            .Include(u => u.Transactions)
            .ToList();
    }

    public IEnumerable<UserProfile> GetByTier(AccountTier tier)
    {
        return _context.UserProfiles
            .Where(u => u.Tier == tier)
            .Include(u => u.Transactions)
            .ToList();
    }

    public IEnumerable<UserProfile> GetByBalanceRange(decimal minBalance, decimal maxBalance)
    {
        return _context.UserProfiles
            .Where(u => u.Balance >= minBalance && u.Balance <= maxBalance)
            .Include(u => u.Transactions)
            .OrderByDescending(u => u.Balance)
            .ToList();
    }

    public Analytics GetAnalytics()
    {
        var users = _context.UserProfiles.Include(u => u.Transactions).ToList();
        var transactions = _context.UserTransactions.ToList();

        var analytics = new Analytics
        {
            TotalUsers = users.Count,
            AverageBalance = users.Any() ? users.Average(u => u.Balance) : 0,
            TotalBalance = users.Sum(u => u.Balance),
            HighestBalanceUser = users.OrderByDescending(u => u.Balance).FirstOrDefault(),
            LowestBalanceUser = users.OrderBy(u => u.Balance).FirstOrDefault(),
            UsersByTier = users.GroupBy(u => u.Tier)
                .ToDictionary(g => g.Key, g => g.Count()),
            TopSpenders = users
                .OrderByDescending(u => u.Transactions.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount))
                .Take(5)
                .ToList(),
            RecentTransactions = transactions
                .OrderByDescending(t => t.CreatedAt)
                .Take(10)
                .Select(t => new TransactionStats
                {
                    UserName = users.FirstOrDefault(u => u.Id == t.UserId)?.Name ?? "Unknown",
                    Amount = t.Amount,
                    Type = t.Type,
                    CreatedAt = t.CreatedAt
                })
                .ToList()
        };

        return analytics;
    }

    private void UpdateTier(UserProfile user)
    {
        user.Tier = user.Balance switch
        {
            < 1000 => AccountTier.Basic,
            < 5000 => AccountTier.Silver,
            _ => AccountTier.Gold
        };
    }

    private void ValidateUser(UserProfile user)
    {
        if (string.IsNullOrWhiteSpace(user.Name))
            throw new ArgumentException("Name cannot be empty");
        if (user.Balance < 0)
            throw new ArgumentException("Balance cannot be negative");
    }
}
