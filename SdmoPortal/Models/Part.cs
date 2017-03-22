using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SdmoPortal.Models
{
    public class Part
    {
        public int PartId { get; set; }
        public int  WorkOrderId { get; set; }
        public WorkOrder WorkOrder { get; set; }

        [Required(ErrorMessage = "You must enter Item Code.")]
        [StringLength(15, ErrorMessage = "Item Code must be 15 characters long or shorter.")]
        [Display(Name = "Item Code")]
        public string InventoryItemCode { get; set; }

        [Required(ErrorMessage = "You must enter a Name.")]
        [StringLength(80, ErrorMessage = "Item Code must be 80 characters long or shorter.")]
        [Display(Name = "Name")]
        public string InventoryItemName { get; set; }

        [Required(ErrorMessage = "You must enter a Quantity")]
        [Range(1, 1000000, ErrorMessage = "Quantity must be between 1 and 1,000,000.")]
        public int Quantity { get; set; }

        [Range(typeof(decimal), "0", "79228162514264")]
        [Display(Name = "Unit Price")]
        public decimal UnitPrice { get; set; }

        [Display(Name = "Extended")]
        public decimal ExtendedPrice { get; set; }

        [StringLength(140, ErrorMessage = "Notes must be 140 characters or shorter")]
        public string Notes { get; set; }
        public bool IsInstalled { get; set; }
    }
}