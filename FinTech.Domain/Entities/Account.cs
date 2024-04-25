using System.Text;
using FinTech.Domain.Validation;
using FluentValidation;

namespace FinTech.Domain.Entities;

public sealed class Account : Entity
{
    private static readonly AccountValidator validator = new();
    public string Name { get; private set; }
    public int? ExpireDay { get; private set; }
    public int? PaymentDay { get; private set; }
    public string UserId { get; private set; }
    public ApplicationUser User { get; private set; }
    public ICollection<Transaction> OutTransactions { get; private set; }
    public ICollection<Transaction> InTransactions { get; private set; }

    /// <summary>
    /// Constructor for Entity Framework
    /// </summary>
    public Account(int id, string name, int? expireDay, int? paymentDay, string userId) 
    { 
        Id = id;
        Name = name;
        ExpireDay = expireDay;
        PaymentDay = paymentDay;
        UserId = userId;

        validator.ValidateAndThrow(this);
    }

    public Account(string name, int expireDay, int paymentDay, ApplicationUser user)
    {
        Name = name;
        ExpireDay = expireDay;
        PaymentDay = paymentDay;
        UserId = user.Id;
        User = user;

        validator.ValidateAndThrow(this);
    }

    public Account(string name, ApplicationUser user)
    {
        Name = name;
        UserId = user.Id;
        User = user;

        validator.ValidateAndThrow(this);
    }

    public void SetName(string name)
    {
        Name = name;

        validator.ValidateAndThrow(this);
    }

    public void SetExpireDay(int expireDay)
    {
        ExpireDay = expireDay;

        validator.ValidateAndThrow(this);
    }

    public void SetPaymentDay(int paymentDay)
    {
        PaymentDay = paymentDay;

        validator.ValidateAndThrow(this);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
    public override string ToString()
    {
        StringBuilder stringBuilder = new();
        stringBuilder.AppendLine($"{Id}, Acc: {Name}");

        if (ExpireDay.HasValue) stringBuilder.AppendLine($"Expires: {ExpireDay}");
        if (PaymentDay.HasValue) stringBuilder.AppendLine($"Pay Date: {PaymentDay}");

        return stringBuilder.ToString();
    }
    public override bool Equals(object obj)
    {
        Account other = obj as Account;
        if (obj is not Account) { return false; }
        return Id.Equals(other.Id);
    }
}