using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BusinesssTrinitySP01.Models
{
    public class EquipmentRental
    {
    }

    public class Cart
    {
        public string CartId { get; set; }

        public DateTime Date_Created { get; set; }

        public ICollection<CartItem> CartItems { get; set; }
    }

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

        [NotMapped]
        public List<OrderAssignment> OnDuty { get; set; }

    }

    public class OrderItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderItemid { get; set; }

        [DisplayName("Quantity")]
        public int quantity { get; set; }

        [DisplayName("Quantity Price")]
        public double price { get; set; }

        [NotMapped]
        [Required]
        [DisplayName("Missing Items")]
        public int MissingItems { get; set; }

        [NotMapped]
        [Required]
        [DisplayName("Damaged Items")]
        public int DamagedItems { get; set; }

        public int OrderID { get; set; }
        public virtual Order Order { get; set; }

        public int EquipmentID { get; set; }
        public virtual Equipment Equipment { get; set; }
    }

    public class Rate
    {
        [Key]
        public int RateId { get; set; }

        [Required]
        [DisplayName("Stars (0 - 5)")]
        public int Stars { get; set; }

        [Required]
        [DisplayName("Comments")]
        public string Comment { get; set; }

        [Required]
        [DisplayName("Service")]
        public string service { get; set; }

        [Required]
        [DisplayName("Date")]
        public DateTime RateDate { get; set; }

        public string Email { get; set; }

        public int OrderID { get; set; }

        public virtual ClientProfile Client { get; set; }
        public virtual Order order { get; set; }
    }

    public class Shipment
    {
        [Key]
        public int SID { get; set; }

        [DisplayName("Event")]
        public string NameofEvent { get; set; }

        [DisplayName("Event Date")]
        [DataType(DataType.Date)]
        public DateTime EventDate { get; set; }

        [DisplayName("Rental Period")]
        public int Rentalperiod { get; set; }

        [DisplayName("End Date")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [DisplayName("Type of Delivery")]// Trinity, Selif-Pickup
        public string DeliveryType { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Delivery Date")]
        public DateTime DeliveryDate { get; set; }

        [DisplayName("Return Date")]
        [DataType(DataType.Date)]
        public DateTime ReturnDate { get; set; }

        [DisplayName("Delivery Cost")]
        [DataType(DataType.Currency)]
        public double DeliveryCost { get; set; }

        [Required]
        public int AdID { get; set; }

        public ClientAddress ClientAddress { get; set; }


        public bool CheckStartDate()
        {
            DateTime incDate = DateTime.Now.AddDays(1);

            if (EventDate <= DateTime.Now)
            {
                return false;
            }

            else if (EventDate.ToShortDateString() == incDate.ToShortDateString())
            {
                return false;
            }

            else
            {
                return true;
            }
        }

        public DateTime End()
        {
            return EventDate.AddDays(Rentalperiod);
        }

        public double CalcDeliveryCost()
        {
            if (DeliveryType == "Trinity")
            {
                return 200.00;
            }

            else
            {
                return 00.00;
            }
        }





    }

    public class Payment
    {
        [Key]
        public int PID { get; set; }

        public string PayMethod { get; set; }

        public double Amount { get; set; }

        public string PaymentFor { get; set; }

        public DateTime PayTime { get; set; }

        public DateTime PayDate { get; set; }

        public string Email { get; set; }

        public int OrderID { get; set; }
        public Order order { get; set; }
    }

    public class OrderAssignment
    {
        [Key]
        public int OrderAssID { get; set; }

        [DisplayName("Driver")]
        public int AssignedDriver { get; set; }

        [DisplayName("Driver Assistants")]
        public string AssignedGen { get; set; }

        [DisplayName("Order No.")]
        public int OrderID { get; set; }

        public Order Order { get; set; }

        [NotMapped]
        public IEnumerable<Employee> AssignDriver { get; set; }

        [NotMapped]
        public IEnumerable<Employee> AssignInstallers { get; set; }

        public Employee employee { get; set; }

        [NotMapped]
        public string[] SelectedIDArray { get; set; }

    }

}