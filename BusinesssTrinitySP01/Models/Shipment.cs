using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BusinesssTrinitySP01.Models
{
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

        [DisplayName("Collection Date")]
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

            else{
                return 00.00;
            }
        }



    }
}