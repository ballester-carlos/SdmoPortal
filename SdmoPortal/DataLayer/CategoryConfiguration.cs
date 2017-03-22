using System.Data.Entity.ModelConfiguration;
using SdmoPortal.Models;
using System.Data.Entity.Infrastructure.Annotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SdmoPortal.DataLayer
{
    public class CategoryConfiguration : EntityTypeConfiguration<Category>
    {
        public CategoryConfiguration()
        {
            Property(c => c.Id).HasColumnName("CategoryId");  //TO DO: Added after ITreeNode implementation
            Property(c => c.CategoryName).HasMaxLength(20).IsRequired()
                .HasColumnAnnotation("Index",
                new IndexAnnotation(new IndexAttribute("AK_Category_CategoryName") { IsUnique = true }));
            //TO DO: Added after ITreeNode implementation
            HasOptional(c => c.Parent).WithMany(c => c.Children).HasForeignKey(c => c.ParentCategoryId).WillCascadeOnDelete(false);
        }
    }
}