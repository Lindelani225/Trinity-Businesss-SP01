using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BusinesssTrinitySP01.Models
{
    public class Equipment
    {
        [Key]
        public int EquipmentID { get; set; }

        [Required]
        [DisplayName("Name")]
        public string Name { get; set; }

        [Required]
        [DisplayName("Description")]
        public string Description { get; set; }

        [Required]
        [DisplayName("Rent Cost")]
        [DataType(DataType.Currency)]
        public double Rentprice { get; set; }

        [Required]
        [DisplayName("Unit Cost")]
        [DataType(DataType.Currency)]
        public double UnitCost { get; set; }

        [Required]
        [DisplayName("Stock Quantity")]
        public int Quantity { get; set; }

        [Required]
        [DisplayName("Front Picture")]
        public byte[] Image1 { get; set; }

        [Required]
        [DisplayName("Picture 1")]
        public byte[] Image2 { get; set; }

        [Required]
        [DisplayName("Picture 2")]
        public byte[] Image3 { get; set; }

        [Required]
        [DisplayName("Category")]
        public int CategoryID { get; set; }

        public Category Category { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }
    }
}