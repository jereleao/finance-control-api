using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using FinTech.Domain.Validation;
using FluentValidation;

namespace FinTech.Domain.Entities;

/// <summary>
/// Transactions can be undestood like payments
/// </summary>
public sealed class Transaction : Entity
{
    private static readonly TransactionValidator validator = new();
    public int Value { get; private set; }
    public string CurrencyStr { get; private set; }
    public Currency Currency { get; private set; }
    public int FinalValue { get; private set; }
    public int OperationId { get; private set; }
    public Operation Operation { get; private set; }
    public DateTime Date { get; private set; }
    public int FromAccountId { get; private set; }
    public Account FromAccount { get; private set; }
    public int ToAccountId { get; private set; }
    public Account ToAccount { get; private set; }

    /// <summary>
    /// Constructor for Entity Framework
    /// </summary>
    public Transaction(int id, int value, string currencyStr, int finalValue, int operationId, int fromAccountId, int toAccountId)
    {
        Id = id;
        Value = value;
        CurrencyStr = currencyStr;
        FinalValue = finalValue;
        OperationId = operationId;
        FromAccountId = fromAccountId;
        ToAccountId = toAccountId;

        validator.ValidateAndThrow(this);
    }

    public Transaction(int value,
                       Currency currency,
                       Operation operation,
                       DateTime date,
                       Account fromAccount,
                       Account toAccount)
    {
        Id = 0;
        Value = value;
        CurrencyStr = currency.Id;
        Currency = currency;
        FinalValue = Convert.ToInt32(decimal.Round(value * currency.CurrentRatio));
        OperationId = operation.Id;
        Operation = operation;
        Date = date;
        FromAccountId = fromAccount.Id;
        FromAccount = fromAccount;
        ToAccountId = toAccount.Id;
        ToAccount = toAccount;

        validator.ValidateAndThrow(this);
    }

    public void SetValue(int value, Currency currency)
    {
        Value = value;
        Currency = currency;
        FinalValue = Convert.ToInt32(decimal.Round(value * currency.CurrentRatio));

        validator.ValidateAndThrow(this);
    }    
    
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
    public override string ToString()
    {
        StringBuilder stringBuilder = new();
        stringBuilder.AppendLine($"Identifier: {Id}");
        stringBuilder.AppendLine($"Value: {Currency.Id} {Value}");
        stringBuilder.AppendLine($"{Date.ToString("d")}");
        stringBuilder.AppendLine($"Transaction: {Operation.Description}");
        return stringBuilder.ToString().ToUpperInvariant();
    }
    public override bool Equals(object obj)
    {
        Transaction other = obj as Transaction;
        if (obj is not Transaction) { return false; }
        return Id.Equals(other.Id);
    }
}