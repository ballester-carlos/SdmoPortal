using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SdmoPortal.Models
{
    public class InventoryItem
    {
        public int InventoryItemId { get; set; }

        [Required(ErrorMessage = "You must enter Item Code.")]
        [StringLength(15,ErrorMessage = "Item Code must be 15 characters long or shorter.")]
        [Display(Name = "Item Code")]
        public string InventoryItemCode { get; set; }

        [Required(ErrorMessage = "You must enter a Name.")]
        [StringLength(80, ErrorMessage = "Item Code must be 80 characters long or shorter.")]
        [Display(Name = "Name")]
        public string InventoryItemName { get; set; }

        [Range(typeof(decimal), "0", "79228162514264" )]
        [Display(Name = "Unit Price")]
        public decimal UnitPrice { get; set; }

        [Display(Name = "Category")]
        public virtual Category Category { get; set; }

        public int CategoryId { get; set; }
    }
}