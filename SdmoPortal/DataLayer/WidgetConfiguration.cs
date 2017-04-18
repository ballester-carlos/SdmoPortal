using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity.ModelConfiguration;
using SdmoPortal.Models;

namespace SdmoPortal.DataLayer
{
    public class WidgetConfiguration : EntityTypeConfiguration<Widget>
    {
        public WidgetConfiguration()
        {
            Property(w => w.Description).HasMaxLength(256).IsRequired();
            Property(w => w.MainBusCode).HasMaxLength(12).IsOptional();
            HasOptional(w => w.CurrentWorker).WithMany().WillCascadeOnDelete(false);
        }
    }
}