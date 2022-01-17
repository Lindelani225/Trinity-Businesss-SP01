using BusinesssTrinitySP01.Models;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Imaging;
using BusinesssTrinitySP01.Logic;
using System.IO;
using System.Web;
using System.Linq;
using System.Web.Mvc;
using ZXing;

namespace BusinesssTrinitySP01.Controllers
{
    [Authorize(Roles = "Driver")]
    public class DriversController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private Client_Logic obj = new Client_Logic();

        // GET: Drivers
        public ActionResult Dashboard()
        {
            Employee emp = db.employees.Where(x => x.EmpEmail == User.Identity.Name).FirstOrDefault();
            var dOrders = db.orderAssignments.Where(x => x.AssignedDriver == emp.EmployeeID).Include(x => x.Order).Include(x => x.Order.ClientProfile).Include(x => x.Order.ShippimgDetails).ToList();
            return View(dOrders);
        }

        public ActionResult DeliveryDetails()
        {
            Employee emp = db.employees.Where(x => x.EmpEmail == User.Identity.Name).FirstOrDefault();
            var dOrders = db.orderAssignments.Where(x => x.AssignedDriver == emp.EmployeeID).Include(x => x.Order.ShippimgDetails.ClientAddress).ToList();
            return View(dOrders);
        }


        public ActionResult ReturnDetails()
        {
            Employee emp = db.employees.Where(x => x.EmpEmail == User.Identity.Name).FirstOrDefault();
            var dOrders = db.equipmentReturns.Where(x => x.AssignedDriver == emp.EmployeeID).Include(x => x.Order.ShippimgDetails.ClientAddress).ToList();
            return View(dOrders);
        }

        public ActionResult StartDelivery(int id)
        {
            var order = db.orders.Find(id);
            var GetAddress = db.orders.Where(x => x.OrderID == id).Include(x => x.ShippimgDetails.ClientAddress).FirstOrDefault();

            if (order.ProcessStatus == "Processed")
            {
                order.ProcessStatus = "On Delivery";
            }
            db.SaveChanges();
            return View(GetAddress);
        }
        public ActionResult FinishDelivery(int id)
        {
            Session["OrderID"] = id;

            string url = "<a href=" + "https://2021grp24.azurewebsites.net/Home/Confirmation/" + id + " >  here" + "</a>";
            var order = db.orders.Find(id);
            var client = new SendGridClient("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");
            var from = new EmailAddress("nkonzo144@gmail.com", "Trinity Pty(Ltd)");
            var subject = "Order " + id + " | Confirm Order Delivery ";

            var to = new EmailAddress(order.Email);
            var htmlContent = "Hi " + order.Email + ", Your orderd has been delivered. Please use the link to accept and confirm delivery. " + url;
            var msg = MailHelper.CreateSingleEmail(from, to, subject, null, htmlContent);
            var response = client.SendEmailAsync(msg);

            if (order.ProcessStatus == "On Delivery")
            {
                order.ProcessStatus = "Delivered";
            }
            db.SaveChanges();
            order.ConfirmationStatus = GenerateQRCode("Delivery Confirmed" + id);
            return View(order);
        }

        [HttpGet]
        public ActionResult InspectEquipment(int id)
        {
            Session["RID"] = id;
            var obj = db.OrderItems.Where(x => x.OrderID == id).Include(x => x.Order).Include(x => x.Equipment).Include(x => x.Equipment.Category);
            ViewBag.Count = obj.Count();
            return View(obj.ToList());
        }

        [HttpPost]
        public ActionResult InspectEquipment(List<OrderItem> items)
        {
            var orderitems = items;
            int orderid = Convert.ToInt32(Session["RID"]);
            Damaged add = new Damaged(); int thisSum = 0;

            foreach (OrderItem item in items)
            {
                int totalQ = db.OrderItems.Where(x => x.OrderItemid == item.OrderItemid).Sum(x => x.quantity);
                thisSum = item.MissingItems + item.DamagedItems;
                if (thisSum > totalQ || item.MissingItems < 0 || item.DamagedItems < 0)
                {
                    ViewBag.SumError = "The combination of missing and damaged items cannot be greater than the total quantity of the ordered items";
                    var obj = db.OrderItems.Where(x => x.OrderID == orderid).Include(x => x.Order).Include(x => x.Equipment).Include(x => x.Equipment.Category);
                    ViewBag.Count = obj.Count();
                    return View(obj.ToList());
                }
            }

            foreach (OrderItem item in items)
            {
                add.EquipmentID = item.EquipmentID;
                add.OrderID = item.OrderID;
                add.MissingItems = item.MissingItems;
                add.DamagedItems = item.DamagedItems;
                add.ReturnDate = DateTime.Now;
                db.damaged.Add(add);
                db.SaveChanges();
            }
            var order = db.orders.Find(orderid);
            order.ProcessStatus = "Complete";
            db.SaveChanges();

            return RedirectToAction("InspectionReport", new { id = orderid });
        }

        public ActionResult InspectionReport(int id)
        {
            double RDeposit = 0.0, owing = 0.0, missingCost = 0.0;

            var order = db.orders.Where(x => x.OrderID == id).FirstOrDefault();
            var oItems = db.OrderItems.Where(x => x.OrderID == id).Include(x => x.Order).Include(x => x.Equipment).Include(x => x.Equipment.Category);
            var dItems = db.damaged.Where(x => x.OrderID == id).Include(x => x.Equipment);
            int sum = dItems.Sum(x => x.DamagedItems);
            var totalItem = oItems.Sum(x => x.quantity);
            double DCost = (sum * 0.1) * 300;

            ViewBag.Damaged = dItems.Sum(x => x.DamagedItems);
            ViewBag.Missing = dItems.Sum(x => x.MissingItems);
            ViewBag.TotItems = totalItem;
            ViewBag.Good = totalItem - dItems.Sum(x => x.DamagedItems) - dItems.Sum(x => x.MissingItems);

            foreach (var item in dItems.Where(x => x.MissingItems > 0))
            {
                missingCost += (item.Equipment.UnitCost) * (item.MissingItems);
            }

            if (missingCost + DCost > 300)
            {
                owing = missingCost + DCost;
                ViewBag.Owing = owing.ToString("C");
                ViewBag.RDeposit = RDeposit.ToString("C");

                return View(order);
            }
            else
            {
                RDeposit = 300 - missingCost - DCost;
                ViewBag.Owing = owing.ToString("C");
                ViewBag.RDeposit = RDeposit.ToString("C");
                return View(order);
            }
        }
        private string GenerateQRCode(string qrcodeText)
        {

            string folderPath = "~/QRCode/";
            string imagePath = "~/QRCode/QrCode" + Session["OrderID"] + ".jpg";

            // create new Directory if not exist
            if (!Directory.Exists(Server.MapPath(folderPath)))
            {
                Directory.CreateDirectory(Server.MapPath(folderPath));
            }

            var barcodeWriter = new BarcodeWriter();
            barcodeWriter.Format = BarcodeFormat.QR_CODE;
            var result = barcodeWriter.Write(qrcodeText);

            string barcodePath = Server.MapPath(imagePath);
            var barcodeBitmap = new Bitmap(result);
            using (MemoryStream memory = new MemoryStream())
            {
                using (FileStream fs = new FileStream(barcodePath, FileMode.Create, FileAccess.ReadWrite))
                {
                    barcodeBitmap.Save(memory, ImageFormat.Jpeg);
                    byte[] bytes = memory.ToArray();
                    fs.Write(bytes, 0, bytes.Length);
                }
            }
            return imagePath;
        }
    }
}