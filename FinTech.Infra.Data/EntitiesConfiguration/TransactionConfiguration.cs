using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinTech.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace FinTech.Infra.Data.EntitiesConfiguration
{
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id).ValueGeneratedOnAdd();

            builder.Property(t => t.Value).IsRequired();
            builder.Property(t => t.CurrencyStr).IsRequired();
            builder.Property(t => t.FinalValue).IsRequired();

            builder.HasOne(e => e.Operation)
                .WithMany(e => e.Transactions)
                .HasForeignKey(e => e.OperationId);

            builder.Property(t => t.Date).IsRequired();

            builder.HasOne(e => e.FromAccount)
                .WithMany(e => e.OutTransactions)
                .HasForeignKey(e => e.FromAccountId);

            builder.HasOne(e => e.ToAccount)
                .WithMany(e => e.InTransactions)
                .HasForeignKey(e => e.ToAccountId);
        }
    }
}

/*

  
 

        RuleFor(e => e.Date).NotNull();
        RuleFor(e => e.FromAccountId).NotEmpty();
        RuleFor(e => e.ToAccountId).NotEmpty();
 */