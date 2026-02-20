namespace FinancialProfileManagerAPI.Models;

public struct Transaction
{
    public decimal Amount { get; }
    public string Description { get; }

    public Transaction(decimal amount, string description)
    {
        Amount = amount;
        Description = description;
    }
}
