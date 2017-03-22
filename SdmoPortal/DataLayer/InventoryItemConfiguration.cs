using System.Data.Entity.ModelConfiguration;
using SdmoPortal.Models;
using System.Data.Entity.Infrastructure.Annotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SdmoPortal.DataLayer
{
    public class InventoryItemConfiguration : EntityTypeConfiguration<InventoryItem>
    {
        public InventoryItemConfiguration()
        {
            Property(ii => ii.InventoryItemCode).HasMaxLength(15).IsRequired()
                .HasColumnAnnotation("Index",
                new IndexAnnotation(new IndexAttribute("AK_InventoryItem_InventoryItemCode") { IsUnique = true }));
            Property(ii => ii.InventoryItemName).HasMaxLength(80).IsRequired()
                .HasColumnAnnotation("Index",
                new IndexAnnotation(new IndexAttribute("AK_InventoryItem_InventoryItemName") { IsUnique = true }));
            Property(ii => ii.UnitPrice).HasPrecision(18, 2);
            //TO DO: Avoid automatic cascade delete 
            //HasRequired(ii => ii.Category).WithMany(cat => cat.InventoryItems).WillCascadeOnDelete(false);
            //TODO: Remove ca.InventoryItems reference
            HasRequired(ii => ii.Category).WithMany().WillCascadeOnDelete(false);

        }
    }
}