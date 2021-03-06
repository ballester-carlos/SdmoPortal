﻿using System.Data.Entity.ModelConfiguration;
using SdmoPortal.Models;
using System.Data.Entity.Infrastructure.Annotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SdmoPortal.DataLayer
{
    public class ServiceItemConfiguration : EntityTypeConfiguration<ServiceItem>
    {
        public ServiceItemConfiguration()
        {
            Property(si => si.ServiceItemCode).HasMaxLength(15).IsRequired()
                .HasColumnAnnotation("Index",
                new IndexAnnotation(new IndexAttribute("AK_ServiceItem_ServiceItemCode") { IsUnique = true }));
            Property(si => si.ServiceItemName).HasMaxLength(80).IsRequired()
                .HasColumnAnnotation("Index",
                new IndexAnnotation(new IndexAttribute("AK_ServiceItem_ServiceItemName") { IsUnique = true }));
            Property(si => si.Rate).HasPrecision(18, 2);
        }
    }
}