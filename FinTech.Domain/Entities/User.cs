using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FinTech.Domain.Validation;
using FluentValidation;

namespace FinTech.Domain.Entities;

public sealed class User : Entity
{
    private static readonly UserValidator validator = new();
    public string Name { get; private set; }
    public string Email { get; private set; }
    public bool VerifiedEmail { get; private set; }
    public string Password { get; private set; }
    public DateTime CreatedAt { get; set; }
    public ICollection<Account> Accounts { get; set; }
    public ICollection<Operation> Operations { get; set; }

    public User(int id,
                string name,
                string email,
                string password,
                bool verifiedEmail,
                DateTime createdAt)
    {
        Id = id;
        Name = name;
        Email = email;
        Password = password;
        VerifiedEmail = verifiedEmail;
        CreatedAt = createdAt;

        validator.ValidateAndThrow(this);
    }

    public User(string name, string email, string encryptedPassword)
    {
        Name = name;
        Email = email;
        VerifiedEmail = false;
        Password = encryptedPassword;
        Accounts = [];

        validator.ValidateAndThrow(this);
    }

    public void SetName(string name)
    {
        Name = name;

        validator.ValidateAndThrow(this);
    }    
    
    public void SetEmail(string email)
    {
        Email = email;

        validator.ValidateAndThrow(this);
    }

    public void VerifyEmail()
    {
        VerifiedEmail = true;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
    public override string ToString()
    {
        StringBuilder stringBuilder = new();
        stringBuilder.AppendLine($"{Id}, Name: {Name}");
        return stringBuilder.ToString();
    }
    public override bool Equals(object obj)
    {
        User other = obj as User;
        if (obj is not User) { return false; }
        return Id.Equals(other.Id);
    }
}