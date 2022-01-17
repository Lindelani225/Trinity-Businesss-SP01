using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BusinesssTrinitySP01.Models
{
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
}