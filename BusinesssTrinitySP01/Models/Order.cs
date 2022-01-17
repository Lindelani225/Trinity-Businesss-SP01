using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace BusinesssTrinitySP01.Models
{
    public class Order
    {
        [DisplayName("Order No.")]
        public int OrderID { get; set; }

        [DisplayName("Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime DateCreated { get; set; }

        [DisplayName("Payment Method")] // Cash , Credit Card
        public string PaymentMethod { get; set; }
       
        [DisplayName("Payment Status")]//Awaiting, Paid
        public string PaymentStatus { get; set; }

        [DisplayName("Status")] //Pending,Processed, On Delivery, Delivered, Complete
        public string ProcessStatus { get; set; }

        [DisplayName("Confirmtion Status")]// Yes,No
        public string ConfirmationStatus { get; set; }

        [ForeignKey("ClientProfile")]
        public string Email { get; set; }
        public ClientProfile ClientProfile { get; set; }

        public int SID { get; set; }
        public Shipment ShippimgDetails { get; set; }
    
        public ICollection<OrderItem> OrderItems { get; set; }
        public ICollection<Payment> payments { get; set; }
        public ICollection<OrderAssignment> orderAssignments { get; set; }
    }
}