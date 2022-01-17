using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BusinesssTrinitySP01.Models
{
    public class DecorRental
    {

    }
    public class Theme
    {
        [Key]
        public int ThemeId { get; set; }

        [Required]
        [DisplayName("Theme")]
        public string Name { get; set; }

        [Required]
        [DisplayName("Description")]
        public string Description { get; set; }

        [DisplayName("Theme Image")]
        public byte[] Image { get; set; }
    }

    public class Color
    {
        [Key]
        public int BoxId { get; set; }

        [Required]
        [DisplayName("Name")]
        public string Name { get; set; }

        [DisplayName("Color Image")]
        public byte[] Image { get; set; }

        [NotMapped]
        public int qty { get; set; }
    }

    public class DesignComp
    {
        [Key]
        public int CompId { get; set; }

        [ForeignKey("Theme")]
        public int ThemeId { get; set; }

        [Required]
        [DisplayName("Name")]
        public string Name { get; set; }

        [Required]
        [DisplayName("Category")]
        public string Type { get; set; }


        [DisplayName("Component Image")]
        public byte[] Image { get; set; }

        [Required]
        [DisplayName("Quantity")]
        public int Qty { get; set; }

        [Required]
        [DisplayName("Unit Price")]
        [DataType(DataType.Currency)]
        public double UnitPrice { get; set; }

        [Required]
        [DisplayName("Rent Price")]
        [DataType(DataType.Currency)]
        public double RentPrice { get; set; }

        public virtual Theme Theme { get; set; }

    }

    public class CompColor
    {
        [Key]
        public int ColorId { get; set; }

        public int CompId { get; set; }

        public int BoxId { get; set; }

        [NotMapped]
        public bool Select { get; set; }

        [Required]
        [DisplayName("Quantity")]
        public int Qty { get; set; }

        [NotMapped]
        public List<Color> colors { get; set; }

        public virtual DesignComp DesignComp { get; set; }
        public virtual Color Color { get; set; }

    }

    public class Package
    {
        [Key]
        public int PckId { get; set; }

        [Required]
        [DisplayName("Name")]
        public string Name { get; set; }

        [Required]
        [DisplayName("Date")]
        public DateTime DateCreated { get; set; }

        public string CartID { get; set; }

        //[Required]
        public int ThemeID { get; set; }

        public int OrderID { get; set; }

        [ForeignKey("ClientProfile")]
        public string Email { get; set; }

        public virtual ClientProfile ClientProfile { get; set; }

        [NotMapped]
        public IEnumerable<Theme> themes { get; set; }

    }

    public class PackageItem
    {
        [Key]
        public int ItemId { get; set; }

        public int PckId { get; set; }


        public int CompId { get; set; }

        public int BoxId { get; set; }

        [NotMapped]
        public IEnumerable<CompColor> compColors { get; set; }

        [Required]
        [DisplayName("Quantity")]
        public int Qty { get; set; }

        [Required]
        [DisplayName("Price")]
        public double Price { get; set; }

        public virtual Package Package { get; set; }
        public virtual DesignComp DesignComp { get; set; }
        public virtual Color Color { get; set; }
    }
}