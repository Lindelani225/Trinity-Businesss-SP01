using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BusinesssTrinitySP01.Models
{
    public class RentalViewModel
    {
        public List<Equipment> Tents { get; set; }

        public List<Equipment> Tables { get; set; }

        public List<Equipment> Chairs { get; set; }

    }
}