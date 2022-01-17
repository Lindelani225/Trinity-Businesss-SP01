using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BusinesssTrinitySP01.Models
{
    public class ClientAddress
    {
        [Key]
        public int AdID { get; set; }

        [Required]
        [DisplayName("Street")]
        public string Streetno { get; set; }

        [Required]
        [DisplayName("Suburb")]
        public string Suburb { get; set; }

        [Required]
        [DisplayName("City")]
        public string City { get; set; }

        [Required]
        [DisplayName("Province")]
        public string Province { get; set; }

        [Required]
        [DisplayName("Postal Code")]
        public string PostalCode { get; set; }

        public string Email { get; set; }
       
        public ClientProfile ClientProfile { get; set; }

        public ICollection<Shipment> shipments { get; set; }
    }
}