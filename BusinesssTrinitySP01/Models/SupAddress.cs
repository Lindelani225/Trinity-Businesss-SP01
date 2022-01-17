using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BusinesssTrinitySP01.Models
{
    public class SupAddress
    {
        [Key]
        public int SpID { get; set; }

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

        public int SuppID { get; set; }
        public virtual Supplier supplier { get; set; }
    }
}