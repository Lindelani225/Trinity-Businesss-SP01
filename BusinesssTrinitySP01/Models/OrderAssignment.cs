using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BusinesssTrinitySP01.Models
{
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

        [NotMapped]
        public string[] SelectedIDArray { get; set; }

    }
}