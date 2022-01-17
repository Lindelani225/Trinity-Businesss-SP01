using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace BusinesssTrinitySP01.Models
{
    public class Damaged
    {
        public int DamagedId { get; set; }

        [DisplayName("Description")]
        public string Description { get; set; }

        [Required]
        [DisplayName("Missing Items")]
        public int MissingItems { get; set; }

        [Required]
        [DisplayName("Damaged Items")]
        public int DamagedItems { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayName("Return Date")]
        public DateTime ReturnDate { get; set; }

        [DisplayName("Image")]
        public byte[] Image { get; set; }

        [Required]
        [DisplayName("Equipment No.")]
        public int EquipmentID { get; set; }

        public Equipment equipment { get; set; }

        [Required]
        [DisplayName("Order No.")]
        public int OrderID { get; set; }

        public Order order { get; set; }
    }
}