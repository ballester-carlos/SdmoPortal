using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TreeUtility;

namespace SdmoPortal.Models
{
    // ITreeNode : www.codeproject.com/articles/23949/building-trees-from-lists-in-net
    public class Category : ITreeNode<Category> //TO DO: Change add add ITreeNode implementation (change configuration as well)
    {
        public int Id { get; set; }
        //[Display(Name = "Parent Category")]
        public int? ParentCategoryId { get; set; }

        // TODO: ViewModel is a better place to add annotations than the class itself
        //[Required(ErrorMessage = "You must enter a category name.")]
        //[StringLength(20, ErrorMessage = "Category names must be 20 characters or shorter.")]
        [Display(Name = "Category")]
        public string CategoryName { get; set; }

        //TODO: No need to navigate from Category to InventoryItem 
        /////public virtual List<InventoryItem> InventoryItems { get; set; }
        //TODO: Add "virtual" for lazy loading data for parents
        public virtual Category Parent { get; set; }

        public IList<Category> Children { get; set; }
    }
}