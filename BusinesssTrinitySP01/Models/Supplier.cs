using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BusinesssTrinitySP01.Models
{
    public class Supplier
    {
        [Key]
        public int SuppID { get; set; }

        [Required]
        [DisplayName("Name")]
        public string SuppName { get; set; }

     
        [DisplayName("Payment Method")]
        public string SuppPaymentMethod { get; set; }

        [DisplayName("Account (If exists)")]
        public string AccountNo { get; set; }

        [Required]
        [DisplayName("Telephone")]
        public string SuppTelephone { get; set; }

        [Required]
        [DisplayName("Email")]
        public string SuppEmail { get; set; }

        [Required]
        [DisplayName("Supplier Type")]
        public string SuppType { get; set; }

    }
}