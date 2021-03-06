﻿using System.Data.Entity.ModelConfiguration;
using SdmoPortal.Models;
using System.Data.Entity.Infrastructure.Annotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SdmoPortal.DataLayer
{
    public class CustomerConfiguration : EntityTypeConfiguration<Customer>
    {
        public CustomerConfiguration()
        {
            Property(c => c.AccountNumber).HasMaxLength(8).IsRequired()
                .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("AK_Customer_AccountNumber") { IsUnique = true }));
            Property(c => c.CompanyName).HasMaxLength(30).IsRequired()
                .HasColumnAnnotation("Index",
                    new IndexAnnotation(new IndexAttribute("AK_Customer_CompanyName") { IsUnique = true }));
            Property(c => c.Address).HasMaxLength(30).IsRequired();
            Property(c => c.City).HasMaxLength(15).IsRequired();
            Property(c => c.State).HasMaxLength(2).IsRequired();
            Property(c => c.ZipCode).HasMaxLength(10).IsRequired();
            Property(c => c.Phone).HasMaxLength(15).IsOptional();
        }
    }
}