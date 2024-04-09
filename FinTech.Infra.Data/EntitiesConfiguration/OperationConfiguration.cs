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
    public class OperationConfiguration : IEntityTypeConfiguration<Operation>
    {
        public void Configure(EntityTypeBuilder<Operation> builder)
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id).ValueGeneratedOnAdd();

            builder.Property(t => t.Description).HasMaxLength(100).IsRequired();

            builder.Property(t => t.CreatedAt).ValueGeneratedOnAdd();

            builder.HasOne(e => e.Category)
                .WithMany(e => e.Operations)
                .HasForeignKey(e => e.CategoryId);

            builder.HasOne(e => e.CreatedBy)
                .WithMany(e => e.Operations)
                .HasForeignKey(e => e.CreatedById);

        }
    }
}