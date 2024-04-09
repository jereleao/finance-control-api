using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using FinTech.Domain.Validation;
using FluentValidation;

namespace FinTech.Domain.Entities;

public sealed class Currency
{
    private static readonly CurrencyValidator validator = new();
    public string Id { get; private set; }
    public decimal CurrentRatio { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    /// <summary>
    /// Constructor for Entity Framework
    /// </summary>
    public Currency(string id, decimal currentRatio, DateTime createdAt, DateTime updatedAt)
    {
        Id = id;
        CurrentRatio = currentRatio;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;

        validator.ValidateAndThrow(this);
    }

    public Currency(string currency, decimal currentRatio)
    {
        Id = currency;
        CurrentRatio = currentRatio;

        validator.ValidateAndThrow(this);
    }

    public void SetRatio(decimal currentRatio)
    {
        CurrentRatio = currentRatio;

        validator.ValidateAndThrow(this);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
    public override string ToString()
    {
        StringBuilder stringBuilder = new();
        stringBuilder.AppendLine($"Currency: {Id}");
        stringBuilder.AppendLine($"Ratio: {CurrentRatio}");
        return stringBuilder.ToString().ToUpperInvariant();
    }
    public override bool Equals(object obj)
    {
        Currency other = obj as Currency;
        if (obj is not Currency) { return false; }
        return Id.Equals(other.Id);
    }
}