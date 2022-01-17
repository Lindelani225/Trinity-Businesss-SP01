using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BusinesssTrinitySP01.Models
{
    public class EquipmentManagement
    {
    }

    public class Damaged
    {
        public int DamagedId { get; set; }

        [Required]
        [DisplayName("Order No.")]
        public int OrderID { get; set; }

        [Required]
        [DisplayName("Equipment No.")]
        public int EquipmentID { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayName("Return Date")]
        public DateTime ReturnDate { get; set; }

        [Required]
        [DisplayName("Missing Items")]
        public int MissingItems { get; set; }

        [Required]
        [DisplayName("Damaged Items")]
        public int DamagedItems { get; set; }


        [Required]
        [DisplayName("Owing Fee")]
        [DataType(DataType.Currency)]
        public double OwingFee { get; set; }


        [Required]
        [DisplayName("Deposit Refund")]
        [DataType(DataType.Currency)]
        public double Refund { get; set; }

        public virtual Equipment Equipment { get; set; }
        public virtual Order Order { get; set; }

        [NotMapped]
        public bool selected { get; set; }

    
    }

    public class Request
    {
        [DisplayName("Repair ID")]
        public int RequestID { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Date")]
        public DateTime DateCreated { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Respond By")]
        public DateTime DueDate { get; set; }

        [DisplayName("Repair For")]
        public string Type { get; set; }

        [DisplayName("No. Items")]
        public int Quantity { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Shipment Date")]
        public DateTime ShipmentDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Return Date")]
        public DateTime ReturnDate { get; set; }

        [DisplayName("Status")]
        public string Status { get; set; } //

        [DisplayName("Payment Status")]
        public string PaymentStatus { get; set; }

        public ICollection<RepairItem> repairItems { get; set; }

        public ICollection<Quote> quotes { get; set; }

    }

    public class Quote
    {
        [Key]
        public int QuoteID { get; set; }

        public int SuppID { get; set; }

        public int RequestID { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Date")]
        public DateTime DateCreated { get; set; }

        [DisplayName("Repair Duration")]
        public int Duration { get; set; }

      
        [DisplayName("Delivery Type")]
        public string ShipmentType { get; set; }

        [NotMapped]
        [DisplayName("Shipment Date")]
        [DataType(DataType.Date)]
        public DateTime ShipmentDate { get; set; }

        public string Acceptance { get; set; }//Pending, Accepted, Declined

        public string Status { get; set; }

        public string QRCode { get; set; }

        public virtual Supplier Supplier { get; set; }
        public virtual Request Request { get; set; }
    }

    public class QuoteItem
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [Key]
        public int QItem { get; set; }

        [DisplayName("Quote")]
        public int QuoteID { get; set; }

        [DisplayName("Item")]
        public string ItemCode { get; set; }

        [DisplayName("Repairability")]
        public string Repairability { get; set; }

        [DisplayName("Cost Estimate")]
        [DataType(DataType.Currency)]
        public double ECost { get; set; }

        [DisplayName("Actual Cost")]
        [DataType(DataType.Currency)]
        public double ACost { get; set; }

        public virtual Quote Quote { get; set; }

        [NotMapped]
        [DisplayName("Item ID")]
        public int ItemID { get; set; }

        [NotMapped]
        [DisplayName("Description")]
        public string Description { get; set; }

        [NotMapped]
        [DisplayName("Picture 1")]
        public byte[] img { get; set; }

        [NotMapped]
        [DisplayName("Picture 2")]
        public byte[] img2 { get; set; }

        public double GetCost(int id)
        {
            double amount = 0;
            foreach (var item in db.quoteitems.ToList().FindAll(match: x => x.QuoteID == id))
            {
                amount += item.ECost;
            }
            return amount;
        }

        public double GetActualCost(int id)
        {
            double amount = 0;
            foreach (var item in db.quoteitems.ToList().FindAll(match: x => x.QuoteID == id))
            {
                amount += item.ACost;
            }
            return amount;
        }
    }

    public class RepairItem
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [Key]
        [DisplayName("Item ID")]
        public int ItemID { get; set; }

        [DisplayName("Request")]
        public int RequestID { get; set; }

        [DisplayName("Equipment ID")]
        public int EquipmentID { get; set; }

        [DisplayName("Item Code")]
        public string ItemCode { get; set; }

        [DisplayName("Description")]
        public string Description { get; set; }

        [DisplayName("Picture 1")]
        public byte[] img { get; set; }

        [DisplayName("Picture 2")]
        public byte[] img2 { get; set; }

        public string Repairability { get; set; }

        [DisplayName("Cost Estimate")]
        [DataType(DataType.Currency)]
        public double ECost { get; set; }

        [DisplayName("Actual Cost")]
        [DataType(DataType.Currency)]
        public double ACost { get; set; }

        public virtual Equipment Equipment { get; set; }
        public virtual Request Request { get; set; }

        [NotMapped]
        public string EquipName { get; set; }



        public double GetCost(int id)
        {
            double amount = 0;
            foreach (var item in db.repairItems.ToList().FindAll(match: x => x.RequestID == id))
            {
                amount += item.ECost;
            }
            return amount;
        }

    }

    public class EquipmentReturn
    {
        [Key]
        public int ReturnID { get; set; }

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

    public class QuoteAssignment
    {
        [Key]
        public int QtsID { get; set; }

        [DisplayName("Driver")]
        public int AssignedDriver { get; set; }

        [DisplayName("Driver Assistants")]
        public string AssignedGen { get; set; }

        [DisplayName("Request No.")]
        public int RequestID { get; set; }

        public virtual Request Request{ get; set; }

        [NotMapped]
        public IEnumerable<Employee> AssignDriver { get; set; }

        [NotMapped]
        public IEnumerable<Employee> AssignInstallers { get; set; }

        public virtual Employee Employee { get; set; }

        [NotMapped]
        public string[] SelectedIDArray { get; set; }
    }

    public class QuoteReport
    {
        [Key]
        public int QuoteID { get; set; }

        public Supplier Supplier { get; set; }

        public Quote Quote { get; set; }

        public Request Request { get; set; }

        public ICollection<QuoteItem>  quoteItems { get; set; }
    }
}