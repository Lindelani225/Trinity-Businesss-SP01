using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace BusinesssTrinitySP01.Models
{
    public class Category
    {
        public int CategoryID { get; set; }

        [DisplayName("Category")]
        public string CategoryName { get; set; }

        [DisplayName("Description")]
        public string Description { get; set; }

        public ICollection<Equipment> Equipments { get; set; }
    }
}