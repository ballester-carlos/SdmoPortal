using System.Data.Entity.ModelConfiguration;
using SdmoPortal.Models;
using System.Data.Entity.Infrastructure.Annotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SdmoPortal.DataLayer
{
    public class WorkOrderConfiguration : EntityTypeConfiguration<WorkOrder>
    {
        public WorkOrderConfiguration()
        {
            Property(wo => wo.OrderDateTime).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed);
            Property(wo => wo.Description).HasMaxLength(256).IsOptional();
            Property(wo => wo.Total).HasPrecision(18, 2).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed);
            Property(wo => wo.CertificationRequirements).HasMaxLength(120).IsOptional();
            //TO DO: Make the relationship require and avoide cascade delete
            HasOptional(wo => wo.CurrentWorker).WithMany(au=>au.WorkOrders).WillCascadeOnDelete(false);
            HasRequired(wo => wo.Customer).WithMany(c => c.WorkOrders).WillCascadeOnDelete(false);
            //TO DO: Added later
            Property(wo => wo.ReworkNotes).HasMaxLength(256).IsOptional();
            Property(wo => wo.RowVersion).IsRowVersion();
        }
    }
}