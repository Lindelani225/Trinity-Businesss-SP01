using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BusinesssTrinitySP01.Models
{
    public class OrderView
    {
        public Order order { get; set; }
       
        public ClientProfile client { get; set; }

        public Shipment shipment { get; set; }

        public List<OrderItem> orderItems { get; set; }

    }
}