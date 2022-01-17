using BusinesssTrinitySP01.Logic;
using BusinesssTrinitySP01.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Configuration;
using System.Text;
using SendGrid;
using System.Threading.Tasks;
using SendGrid.Helpers.Mail;
using Bytescout.BarCodeReader;
using Rotativa;
using System.Net;

namespace BusinesssTrinitySP01.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private Client_Logic obj = new Client_Logic();
        public string shoppingCartID { get; set; }

        // GET: Rental
        public ActionResult Index() //Displays products
        {
            int cartItems = obj.GetCartItems().Count();
            if (cartItems != 0)
            {
                ViewBag.TotalItems = cartItems;
            }
            int PckID = Convert.ToInt32(Session["PckID"]);
            ViewBag.PckID = PckID;
            shoppingCartID = obj.GetCartID();
            return View(db.Equipment.ToList());
        }

        public ActionResult ViewCart(int? PckID)
        {
            Session["PckID"] = PckID;
            ViewBag.PckID = PckID;
            ViewBag.OrderID = db.packages.Where(x => x.PckId == PckID).Select(x => x.OrderID).FirstOrDefault();
            shoppingCartID = obj.GetCartID();
            ViewBag.Total = obj.GetCartTotal(id: shoppingCartID);
            ViewBag.TotalQTY = obj.GetCartItems().FindAll(x => x.CartID == shoppingCartID).Sum(q => q.Quantity);
            ViewBag.TotalItems = obj.GetCartItems().FindAll(x => x.CartID == shoppingCartID).Sum(q => q.Quantity);

            var CartItems = db.CartItems.Include(c => c.Equipments).ToList().FindAll(x => x.CartID == shoppingCartID);

            return View(CartItems);
        }


        [HttpPost]
        public ActionResult ViewCart(List<CartItem> items)
        {
            shoppingCartID = obj.GetCartID();

            foreach (var i in items)
            {
                obj.UpdateCart(i.ItemID, i.Quantity);
            }
            ViewBag.Total = obj.GetCartTotal(shoppingCartID);
            ViewBag.TotalQTY = obj.GetCartItems().FindAll(x => x.CartID == shoppingCartID).Sum(q => q.Quantity);
            ViewBag.TotalItems = obj.GetCartItems().FindAll(x => x.CartID == shoppingCartID).Sum(q => q.Quantity);
            return View(db.CartItems.Include(e => e.Equipments).ToList().FindAll(x => x.CartID == shoppingCartID));
        }

        public ActionResult AddToCart(int id)
        {
            var item = db.Equipment.Find(id);
            if (item != null)
            {
                obj.AddToCart(id);
                return RedirectToAction("Index");
            }
            else
                return RedirectToAction("Not_Found", "Error");

        }

        public ActionResult RemoveFromCart(string id)
        {
            int PckID = Convert.ToInt32(Session["PckID"]);
            ViewBag.PckID = PckID;

            var item = db.CartItems.Find(id);
            if (item != null)
            {
                obj.RemoveFromCart(id: id);
                return RedirectToAction("ViewCart", new { ViewBag.PckID });
            }
            else
                return RedirectToAction("Not_Found", "Error");

        }


        public ActionResult PlaceOrder(int id)
        {
            var customer = db.clientProfiles.ToList().Find(x => x.Email == HttpContext.User.Identity.Name);
            db.orders.Add(new Order()
            {
                Email = customer.Email,
                DateCreated = DateTime.Now,
                SID = id,
                PaymentStatus = "Awaiting",
                ProcessStatus = "Pending",
                ConfirmationStatus = "Not Confirmed"
            });
            db.SaveChanges();
            var order = db.orders.ToList()
                .FindAll(x => x.Email == customer.Email)
                .LastOrDefault();

            var items = obj.GetCartItems();

            foreach (var item in items)
            {
                var x = new OrderItem()
                {

                    OrderID = order.OrderID,
                    EquipmentID = item.EquipmentID,
                    quantity = item.Quantity,
                    price = item.price
                };
                db.OrderItems.Add(x);
                db.SaveChanges();
            }
            obj.EmptyCart();

            return RedirectToAction("AddPackage","DecorRentals", new { id = order.OrderID });
        }

        public ActionResult PaymentOption(int id)
        {
            var order = db.orders.Find(id);
            return View(order);
        }

        [HttpPost]
        public ActionResult PaymentOption(Order payemnt)
        {
            Order order = db.orders.Find(payemnt.OrderID);
            order.PaymentMethod = payemnt.PaymentMethod;
            db.SaveChanges();

            return RedirectToAction("OrderSummary", new { id = payemnt.OrderID });
        }


        public ActionResult OrderSummary(int id)
        {
            var order = db.orders.Find(id);
            double pcktotal = 0.0;
            var shipm = db.ShippimgDetails.Where(s => s.SID == order.SID).FirstOrDefault();
            var package = db.packages.Where(x => x.OrderID == id).FirstOrDefault();
            OrderViewModel summary = new OrderViewModel();
            summary.OrderId = id;
            summary.orderItems = summary.GetItems(id);
            summary.shipments = summary.GetShipment(shipm.SID);
            summary.clientAddresses = summary.GetAddress(shipm.AdID);
            summary.PackageItems = new List<PackageItem>();
            if (package != null)
            {
                summary.PackageItems = summary.GetPackageItems(package.PckId);
                ViewBag.PckName = package.Name;
                ViewBag.PackageTotal = obj.GetPackageTotal(package.PckId);
                pcktotal = obj.GetPackageTotal(package.PckId);
            }
            ViewBag.Method = order.PaymentMethod;
            ViewBag.AllTotal = ((obj.GetOrdertTotal(id) * shipm.Rentalperiod) + shipm.DeliveryCost + 300);
            ViewBag.OrderTotal = ((obj.GetOrdertTotal(id) * shipm.Rentalperiod) + shipm.DeliveryCost + 300) + pcktotal;
            return View(summary);
        }

        public ActionResult OrderSuccess(int? id)
        {


            if (id != null)
            {
                var order = db.orders.Find(id);
                try
                {

                    order.PaymentStatus = "Paid";
                    db.SaveChanges();

                }
                catch (Exception ex) { }
            }

            return View();
        }

        public ActionResult Secure_Payment(int id)
        {
            var order = db.orders.Find(id);
            ViewBag.Order = order;
            ViewBag.Account = db.clientProfiles.Find(order.Email);
            ViewBag.Items = db.OrderItems.ToList().FindAll(x => x.OrderID == order.OrderID);

            ViewBag.Total = obj.GetOrdertTotal(order.OrderID);
            var ship = db.ShippimgDetails.Where(x => x.SID == order.SID).FirstOrDefault();

            double packtot = 0.0;
            var chkpackage = db.packages.Where(x => x.OrderID == id).FirstOrDefault();
            if(chkpackage != null)
            {
                packtot = obj.GetPackageTotal(chkpackage.PckId);
            }
            string tot = ((obj.GetOrdertTotal(id) * ship.Rentalperiod) + ship.DeliveryCost + 300 + packtot).ToString();

            return Redirect(PaymentLink(tot, "Order Payment | Order No: " + order.OrderID, order.OrderID));
        }

        public string PaymentLink(string totalCost, string paymentSubjetc, int order_id)
        {
            string paymentMode = ConfigurationManager.AppSettings["PaymentMode"], site, merchantId, merchantKey, returnUrl;
            site = "https://sandbox.payfast.co.za/eng/process?";
            merchantId = "10022900";
            merchantKey = "qq34viiias2on";
            returnUrl = "http://2021grp24.azurewebsites.net/Home/Payment_Successfull/";

            //else if (paymentMode == "live")
            //{
            //    site = "https://www.payfast.co.za/eng/process";
            //    merchantId = ConfigurationManager.AppSettings["PF_MerchantID"];
            //    merchantKey = ConfigurationManager.AppSettings["PF_MerchantKey"];
            //}
            //else
            //{
            //    throw new InvalidOperationException("Payment method unknown.");
            //}
            var stringBuilder = new StringBuilder();
            //string url = Url.Action("Quotes", "Order",
            //    new System.Web.Routing.RouteValueDictionary(new { id = order_id }),
            //    "http", Request.Url.Host);
            stringBuilder.Append("merchant_id=" + HttpUtility.HtmlEncode(merchantId));
            stringBuilder.Append("&merchant_key=" + HttpUtility.HtmlEncode(merchantKey));
            stringBuilder.Append("&return_url= " + HttpUtility.HtmlEncode("https://2021grp24.azurewebsites.net/Home/OrderSuccess/" + order_id));
            //stringBuilder.Append("cancel_url" + HttpUtility.HtmlEncode("https://2021grp24.azurewebsites.net/Home/Payment_Cancelled/" + order_id));
            //stringBuilder.Append("notify_url" + HttpUtility.HtmlEncode(ConfigurationManager.AppSettings["PF_NotifyURL"]));

            string amt = totalCost;
            amt = amt.Replace(",", ".");
            stringBuilder.Append("&amount=" + HttpUtility.HtmlEncode(amt));
            stringBuilder.Append("&item_name=" + HttpUtility.HtmlEncode(paymentSubjetc));
            stringBuilder.Append("&email_confirmation=" + HttpUtility.HtmlEncode("1"));
            stringBuilder.Append("&confirmation_address=" + HttpUtility.HtmlEncode(ConfigurationManager.AppSettings["PF_ConfirmationAddress"]));

            return (site + stringBuilder);
        }


        public ActionResult Payment_Successfull(int? id)
        {
            var order = db.orders.Find(id);
            var ship = db.ShippimgDetails.Where(x => x.SID == order.SID).FirstOrDefault();
            try
            {

                order.PaymentStatus = "Paid";
                order.PaymentMethod = "Credit Card";
                db.SaveChanges();
                db.payments.Add(new Payment()
                {
                    PayDate = DateTime.Now,
                    PayTime = DateTime.Now,
                    Email = db.clientProfiles.FirstOrDefault(p => p.Email == User.Identity.Name).Email,
                    Amount = obj.GetOrdertTotal(order.OrderID) + 300 + ship.DeliveryCost,
                    PaymentFor = "Order " + id + " Payment",
                    PayMethod = "PayFast Online"
                });
                db.SaveChanges();
                ViewBag.Items = db.OrderItems.ToList().FindAll(x => x.OrderID == order.OrderID);

                obj.UpdateStock((int)id);

                string table = "<br/>" +
                               "Items in this order<br/>" +
                               "<table>";
                table += "<tr>" +
                         "<th>Item</th>"
                         +
                         "<th>Quantity</th>"
                         +
                         "<th>Price</th>" +
                         "</tr>";
                foreach (var item in (List<OrderItem>)ViewBag.Items)
                {
                    string items = "<tr> " +
                                   "<td>" + item.Equipment.Name + " </td>" +
                                   "<td>" + item.quantity + " </td>" +
                                   "<td>R " + item.price + " </td>" +
                                   "<tr/>";
                    table += items;
                }

                table += "<tr>" +
                         "<th></th>"
                         +
                         "<th></th>"
                         +
                         "<th>" + obj.GetOrdertTotal(order.OrderID).ToString("R 0.00") + "</th>" +
                         "</tr>";
                table += "</table>";

                var client = new SendGridClient("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");
                var from = new EmailAddress("nkonzo144@gmail.com", "Trinity Pty(Ltd)");
                var subject = "Order " + id + " | Payment Recieved";
                var to = new EmailAddress(order.ClientProfile.Email, order.ClientProfile.FirstName + " " + order.ClientProfile.LastName);
                var htmlContent = "Hi " + order.ClientProfile.FirstName + "<br/><br/>" + "We recieved your payment, your order will be processed shortly."+"<br/>"+"Thank you for choosing us." + table;
                var msg = MailHelper.CreateSingleEmail(from, to, subject, null, htmlContent);

                var response = client.SendEmailAsync(msg);

            }
            catch (Exception ex) { }

            ViewBag.Order = order;
            ViewBag.Account = db.clientProfiles.Find(order.Email);
            //ViewBag.Address = db.cAddresses.ToList().Find(x => x. == order.OrderID);
            ViewBag.Total = obj.GetOrdertTotal(order.OrderID);

            return RedirectToAction("OrderSuccess");
        }

        //Fetches All Customer Orders 
        public ActionResult MyOrders()
        {
            var CusOrders = new List<Order>();
            var GetCustomer = db.orders.Include(x => x.ClientProfile).Where(x => x.ClientProfile.Email == User.Identity.Name).ToList();

            foreach (var item in GetCustomer)
            {
                CusOrders = db.orders.Include(x => x.ShippimgDetails).Where(x => x.Email == item.Email).ToList();
            }
            return View(CusOrders);

        }
        //Fetches Active Orders Only
        public ActionResult TrackOrder()
        {
            var ActiveOrders = new List<Order>();
            var GetCustomer = db.orders.Where(x => x.ProcessStatus != "Complete").Include(x => x.ClientProfile).Where(x => x.ClientProfile.Email == User.Identity.Name).ToList();

            foreach (var item in GetCustomer)
            {
                ActiveOrders = db.orders.Include(x => x.ShippimgDetails).Where(x => x.Email == item.Email).ToList();
            }
            return View(ActiveOrders);
        }

        public ActionResult QRScanner(int id)
        {
            Session["OrderID"] = id;
            return View();
        }
        public ActionResult Success()
        {
            return View();
        }
        public ActionResult Error()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DecodeQR(string image, string type, Order Conf)
        {
            var id = Session["OrderID"];
            StringBuilder send = new StringBuilder();

            byte[] bitmapArrayOfBytes = Convert.FromBase64String(image);
            Reader reader = new Reader();
            reader.BarcodeTypesToFind = Barcode.GetBarcodeTypeToFindFromCombobox(type);
            reader.ReadFromMemory(bitmapArrayOfBytes);

            try
            {
                lock (send)
                {
                    if (reader.FoundBarcodes != null)
                    {
                        foreach (FoundBarcode barcode in reader.FoundBarcodes)
                        {
                            using (ApplicationDbContext db = new ApplicationDbContext())
                            {
                                var order = db.orders.Find(id);

                                if (barcode.Value == "Delivery Confirmed" + id)
                                {

                                    order.ConfirmationStatus = "Delivery Confirmed";
                                    db.SaveChanges();

                                    send.AppendLine(String.Format("{0} : {1}", barcode.Type, barcode.Value));
                                    return Json(new { d = send.ToString() });
                                }
                                else
                                {
                                    //send.AppendLine(String.Format("Oops, Confirmation Failed.\nPlease Try Again..."));
                                    return View("Redirect to Error page.");
                                }
                            }
                        }
                    }
                    return Json(new { d = send.ToString() });
                }
            }
            catch (Exception ex)
            {
                return Json(new { d = ex.Message + "\r\n" + ex.StackTrace });
            }
        }

        public ActionResult Confirmation(int? id)
        {
            var order = db.orders.Find(id);
            var shipm = db.ShippimgDetails.Where(s => s.SID == order.SID).FirstOrDefault();
            Session["order"] = order;
            ViewBag.Order = order;
            Session["OrderID"] = id;
            ViewBag.Account = db.clientProfiles.Find(order.Email);
            ViewBag.Items = db.OrderItems.ToList().FindAll(x => x.OrderID == order.OrderID);
            ViewBag.Total = ((obj.GetOrdertTotal(order.OrderID) * shipm.Rentalperiod) + 300 + shipm.DeliveryCost).ToString("C");
            return View(order);
        }


        [HttpPost]
        public ActionResult Confirm()
        {

            Order d = Session["order"] as Order;
            var confirm = (from o in db.orders
                           where o.OrderID == d.OrderID
                           select o).FirstOrDefault();

            confirm.ConfirmationStatus = "Delivery Confirmed";
            db.SaveChanges();

            return RedirectToAction("Success", "Home");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult PrintPartialViewToPdf(int? id)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                else
                {
                    Quote quote = db.quotes.Find(id);
                    Request request = db.requests.Where(x => x.RequestID == quote.RequestID).FirstOrDefault();
                    Supplier supplier = db.suppliers.Where(x => x.SuppID == quote.SuppID).FirstOrDefault();
                    QuoteItem getTot = new QuoteItem();

                    if (quote != null)
                    {
                        QuoteReport report = new QuoteReport()
                        {
                            QuoteID = quote.QuoteID,
                            Quote = db.quotes.Find(id),
                            Supplier = supplier,
                            quoteItems = db.quoteitems.Where(x => x.QuoteID == quote.QuoteID).ToList()
                        };
                        ViewBag.QuoteType = request.Type;
                        ViewBag.QuoteMnth = quote.DateCreated.ToString("MMM");
                        ViewBag.TotalCost = getTot.GetCost(quote.QuoteID).ToString("C");


                        var qreport = new PartialViewAsPdf("~/Views/Home/PrintPartialViewToPdf.cshtml", report);
                        return qreport;
                    }
                    ViewBag.NoReport = "No report can be generated for this quote. Please contact the administartor";
                    return View("QuoteReport");
                }
            }
        }

        public ActionResult PrintViewToPdf(int? id)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                else
                {
                    Quote quote = db.quotes.Find(id);
                    Request request = db.requests.Where(x => x.RequestID == quote.RequestID).FirstOrDefault();
                    Supplier supplier = db.suppliers.Where(x => x.SuppID == quote.SuppID).FirstOrDefault();
                    QuoteItem getTot = new QuoteItem();

                    if (quote != null)
                    {
                        QuoteReport report = new QuoteReport()
                        {
                            QuoteID = quote.QuoteID,
                            Quote = db.quotes.Find(id),
                            Supplier = supplier,
                            quoteItems = db.quoteitems.Where(x => x.QuoteID == quote.QuoteID).ToList()
                        };
                        ViewBag.QuoteType = request.Type;
                        ViewBag.QuoteMnth = quote.DateCreated.ToString("MMM");
                        ViewBag.TotalCost = getTot.GetCost(quote.QuoteID).ToString("C");


                        var qreport = new PartialViewAsPdf("~/Views/Home/PrintPartialViewToPdf.cshtml", report);
                        return qreport;
                    }
                    ViewBag.NoReport = "No report can be generated for this quote. Please contact the administartor";
                    return View("QuoteReport");
                }
            }
        }

        public ActionResult ViewInvoice(int id)
        {
            Supplier supplier = db.suppliers.Where(x => x.SuppEmail == User.Identity.Name).FirstOrDefault();
            Quote quote = db.quotes.Where(x => x.RequestID == id && x.SuppID == supplier.SuppID).FirstOrDefault();
            Request request = db.requests.Where(x => x.RequestID == quote.RequestID).FirstOrDefault();
            QuoteItem getTot = new QuoteItem();

            if (quote != null)
            {
                QuoteReport report = new QuoteReport()
                {
                    QuoteID = quote.QuoteID,
                    Quote = quote,
                    Request = request,
                    Supplier = supplier,
                    quoteItems = db.quoteitems.Where(x => x.QuoteID == quote.QuoteID).ToList()
                };
                ViewBag.QuoteType = request.Type;
                ViewBag.QuoteMnth = quote.DateCreated.ToString("MMM");
                ViewBag.TotalCost = getTot.GetActualCost(quote.QuoteID).ToString("C");

                return View("RepairInvoice", report);
            }
            ViewBag.NoReport = "No report can be generated for this quote. Please contact the administartor";
            return View();
        }

    }
}