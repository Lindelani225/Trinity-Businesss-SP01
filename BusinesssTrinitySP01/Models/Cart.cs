using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BusinesssTrinitySP01.Models
{
    public class Cart
    {
        public string CartId { get; set; }

        public DateTime Date_Created { get; set; }

        public ICollection<CartItem> CartItems { get; set; }
    }
}