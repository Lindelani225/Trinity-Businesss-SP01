using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BusinesssTrinitySP01.Models
{
    public class OrderViewModel
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public int OrderId { get; set; }
        public IEnumerable<OrderItem> orderItems { get; set; }
        public IEnumerable<Shipment> shipments { get; set; }
        public IEnumerable<ClientAddress> clientAddresses { get; set; }
        public IEnumerable<PackageItem> PackageItems { get; set; }


        public List<OrderItem> GetItems(int id)
        {
            List<OrderItem> items = db.OrderItems.Where(o => o.OrderID == id).ToList();
            return items;
        }

        public List<Shipment> GetShipment(int id)
        {
            List<Shipment> shipments = db.ShippimgDetails.Where(o => o.SID == id).ToList();
            return shipments;
        }

        public List<ClientAddress> GetAddress(int id)
        {
            List<ClientAddress> addresses = db.cAddresses.Where(o => o.AdID == id).ToList();
            return addresses;
        }

        public List<PackageItem> GetPackageItems(int id)
        {
            List<PackageItem> packages = db.packageItems.Where(x => x.PckId == id).ToList();
            return packages;
        }
    }
}