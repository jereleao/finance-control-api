using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FinTech.Domain.Validation;
using FluentValidation;

namespace FinTech.Domain.Entities;

public sealed class CategoryGroup : Entity
{
    private static readonly CategoryGroupValidator validator = new();
    public string Name { get; private set; }
    public int Budget { get; private set; }
    public string Description { get; private set; }
    public ICollection<Category> Categories { get; set; }

    /// <summary>
    /// Constructor for Entity Framework
    /// </summary>
    public CategoryGroup(int id, string name, int budget, string description)
    {
        Id = id;
        Name = name;
        Budget = budget;
        Description = description;

        validator.ValidateAndThrow(this);
    }

    public CategoryGroup(string name, int budget, string description = "")
    {
        Name = name;
        Budget = budget;
        Description = description;

        validator.ValidateAndThrow(this);
    }

    public void SetName(string name)
    {
        Name = name;

        validator.ValidateAndThrow(this);
    }

    public void SetBudget(int budget)
    {
        Budget = budget;

        validator.ValidateAndThrow(this);
    }    
    
    public void SetDescription(string description)
    {
        Description = description;

        validator.ValidateAndThrow(this);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
    public override string ToString()
    {
        StringBuilder stringBuilder = new();
        stringBuilder.AppendLine($"Name category: {Name}");
        stringBuilder.AppendLine($"Identifier: {Id}");
        return stringBuilder.ToString().ToUpperInvariant();
    }
    public override bool Equals(object obj)
    {
        CategoryGroup other = obj as CategoryGroup;
        if (obj is not CategoryGroup) { return false; }
        return Id.Equals(other.Id);
    }
}