using System.Collections.Generic;
using System.Reflection.Emit;
using FinTech.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinTech.Infra.Data.Context;


public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Account> Accounts { get; set; }
    //public DbSet<Category> Categories { get; set; }
    //public DbSet<CategoryGroup> CategoryGroups { get; set; }
    //public DbSet<Currency> Currencies { get; set; }
    //public DbSet<Operation> Operations { get; set; }
    //public DbSet<Transaction> Transactions { get; set; }
    public DbSet<User> Users { get; set; }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
