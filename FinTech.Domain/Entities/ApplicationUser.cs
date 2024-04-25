using System.Text;
using Microsoft.AspNetCore.Identity;


namespace FinTech.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public ApplicationUser() 
    {
        this.Accounts = [];
        this.Operations = [];

        this.DateCreated = DateTime.Now;
    }

    public DateTime DateCreated { get; set; }
    public virtual ICollection<Account> Accounts { get; set; }
    public virtual ICollection<Operation> Operations { get; set; }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
    public override string ToString()
    {
        StringBuilder stringBuilder = new();
        stringBuilder.AppendLine($"{Id}, Name: {UserName}");
        return stringBuilder.ToString();
    }

    public override bool Equals(object obj)
    {
        if (obj is not ApplicationUser other || obj is not ApplicationUser) { return false; }

        return Id.Equals(other.Id);
    }
}