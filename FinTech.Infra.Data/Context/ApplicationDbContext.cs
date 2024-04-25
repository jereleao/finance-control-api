using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using FinTech.Domain.Entities;

namespace FinTech.Infra.Data.Context;


public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :
    IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Account> Accounts { get; set; }
    //public DbSet<Category> Categories { get; set; }
    //public DbSet<CategoryGroup> CategoryGroups { get; set; }
    //public DbSet<Currency> Currencies { get; set; }
    //public DbSet<Operation> Operations { get; set; }
    //public DbSet<Transaction> Transactions { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }


}