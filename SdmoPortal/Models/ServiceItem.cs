using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SdmoPortal.Models
{
    public class ServiceItem
    {
        public int ServiceItemId { get; set; }

        [Required(ErrorMessage = "You must enter Item Code.")]
        [StringLength(15, ErrorMessage = "Item Code must be 15 characters long or shorter.")]
        [Display(Name = "Item Code")]
        public string ServiceItemCode { get; set; }

        [Required(ErrorMessage = "You must enter a Name.")]
        [StringLength(80, ErrorMessage = "Item Code must be 80 characters long or shorter.")]
        [Display(Name = "Name")]
        public string ServiceItemName { get; set; }

        [Range(typeof(decimal), "0", "79228162514264")]
        public decimal Rate { get; set; }
    }
}