using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BusinesssTrinitySP01.Models
{
    public class CartItem
    {
        [Key]
        public string ItemID { get; set; }

        [DisplayName("Cart ID")]
        public string CartID { get; set; }

        [DisplayName("Date")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [DisplayName("Quantity")]
        public int Quantity { get; set; }


        [DisplayName("Quantity Price")]
        [DataType(DataType.Currency)]
        public double price { get; set; }

        [DisplayName("Equipment")]
        public int EquipmentID { get; set; }

        public Cart Cart { get; set; }
        public Equipment Equipments { get; set; }
    }
}