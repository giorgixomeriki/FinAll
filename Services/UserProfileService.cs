using System.Collections.Generic;
using FinancialProfileManagerAPI.Models;
using System.Linq;
using System;

namespace FinancialProfileManagerAPI.Services;

public class UserProfileService
{
    private readonly List<UserProfile> _users = new();
    private readonly object _lock = new();

    public UserProfile AddUser(UserProfile user)
    {
        ValidateUser(user);
        UpdateTier(user);

        lock (_lock)
        {
            _users.Add(user);
        }

        return user;
    }

    public IEnumerable<UserProfile> GetAll()
    {
        lock (_lock)
        {
            // Copy list to avoid modification during enumeration
            return _users.ToList();
        }
    }

    public void DeleteUser(UserProfile user)
    {
        lock (_lock)
        {
            _users.Remove(user);
        }
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
