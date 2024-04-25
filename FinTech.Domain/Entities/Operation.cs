using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using FinTech.Domain.Validation;
using FluentValidation;

namespace FinTech.Domain.Entities;

/// <summary>
/// Operations can be undestood like purchases
/// </summary>
public sealed class Operation : Entity
{
    private static readonly OperationValidator validator = new();
    public string Description { get; private set; }
    public int CategoryId { get; private set; }
    public Category Category { get; private set; }
    public bool IsForecast { get; private set; }
    //public int CreatedById { get; private set; }
    //public ApplicationUser CreatedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public ICollection<Transaction> Transactions { get; set; }

    /// <summary>
    /// Constructor for Entity Framework
    /// </summary>
    public Operation(int id, string description, int categoryId, bool isForecast/*, int createdById*/, DateTime createdAt)
    {
        Id = id;
        Description = description;
        CategoryId = categoryId;
        IsForecast = isForecast;
        //CreatedById = createdById;
        CreatedAt = createdAt;

        validator.ValidateAndThrow(this);
    }

    public Operation(string description, Category category, ApplicationUser creator, bool isForecast = false)
    {
        Description = description;
        Category = category;
        CategoryId = category.Id;
        IsForecast = isForecast;
        //CreatedBy = creator;
        //CreatedById = creator.Id;

        validator.ValidateAndThrow(this);
    }

    public void SetDescription(string description)
    {
        Description = description;

        validator.ValidateAndThrow(this);
    }

    public void SetCategoty(Category category)
    {
        Category = category;
        CategoryId = category.Id;

        validator.ValidateAndThrow(this);
    }

    public void SetForecast(bool isForecast)
    {
        IsForecast = isForecast;

        validator.ValidateAndThrow(this);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
    public override string ToString()
    {
        StringBuilder stringBuilder = new();
        if (IsForecast) stringBuilder.AppendLine($"Forecast: ");
        stringBuilder.AppendLine($"Operation: {Description}");
        stringBuilder.AppendLine($"Total Value: {Transactions.Sum(t => t.Id)}");
        
        //stringBuilder.AppendLine($"{CreatedBy.UserName}, {CreatedAt.ToString("d")}");
        return stringBuilder.ToString().ToUpperInvariant();
    }
    public override bool Equals(object obj)
    {
        Operation other = obj as Operation;
        if (obj is not Operation) { return false; }
        return Id.Equals(other.Id);
    }
}