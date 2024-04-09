using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FinTech.Domain.Validation;
using FluentValidation;

namespace FinTech.Domain.Entities;

public sealed class Category : Entity
{
    private static readonly CategoryValidator validator = new();
    public string Name { get; private set; }
    public int CategoryGroupId { get; private set; }
    public CategoryGroup CategoryGroup { get; private set; }
    public ICollection<Operation> Operations { get; private set; }

    /// <summary>
    /// Constructor for Entity Framework
    /// </summary>
    public Category(int id, string name, int categoryGroupId)
    {
        Id = id;
        Name = name;
        CategoryGroupId = categoryGroupId;

        validator.ValidateAndThrow(this);
    }

    public Category(string name, CategoryGroup categoryGroup)
    {
        Name = name;
        CategoryGroupId = categoryGroup.Id;
        CategoryGroup = categoryGroup;

        validator.ValidateAndThrow(this);
    }

    public void SetName(string name)
    {
        Name = name;

        validator.ValidateAndThrow(this);
    }

    public void SetCategoryGroup(CategoryGroup categoryGroup)
    {
        CategoryGroup = categoryGroup;
        CategoryGroupId = categoryGroup.Id;

        validator.ValidateAndThrow(this);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
    public override string ToString()
    {
        StringBuilder stringBuilder = new();
        stringBuilder.AppendLine($"{Id}, Category: {Name}");
        stringBuilder.AppendLine($"Category Group: {CategoryGroup.Name}");

        return stringBuilder.ToString();
    }
    public override bool Equals(object obj)
    {
        Category other = obj as Category;
        if (!(obj is Category)) { return false; }
        return Id.Equals(other.Id);
    }
}