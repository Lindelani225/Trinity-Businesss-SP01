using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BusinesssTrinitySP01.Models
{
    public class OrderItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderItemid { get; set; }

        [DisplayName("Quantity")]
        public int quantity { get; set; }

        [DisplayName("Quantity Price")]
        public double price { get; set; }

        public int OrderID { get; set; }
        public virtual Order Order { get; set; }


        public int EquipmentID { get; set; }

        public virtual Equipment Equipment { get; set; }
    }
}