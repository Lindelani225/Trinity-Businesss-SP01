using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using PagedList;
using PagedList.Mvc;
using System.Web.Mvc;
using BusinesssTrinitySP01.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using BusinesssTrinitySP01.Logic;
using SendGrid.Helpers.Mail;
using SendGrid;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using ZXing;
using System.Text;
using System.Configuration;

namespace BusinesssTrinitySP01.Controllers
{
    [Authorize(Roles = "Super Admin")]
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin
        public ActionResult Index(int? page, string searchBy, string searchDrop, string searchDate, string searchEmail)
        {
            Order order = new Order();

            //Get Order Count by status

            ViewBag.AllRequests = db.orders.ToList().Count();
            ViewBag.Pending = db.orders.Where(x => x.ProcessStatus == "Pending").Count();
            ViewBag.Processed = db.orders.Where(x => x.ProcessStatus != "Pending").Count();
            ViewBag.Delivered = (db.orders.Where(x => x.ProcessStatus == "Delivered").Count()) + (db.orders.Where(x => x.ProcessStatus == "Complete").Count());
            ViewBag.ToComplete = db.orders.Where(x => x.ProcessStatus == "Delivered").Count();
            ViewBag.PendingDel = db.orders.Where(x => x.ProcessStatus == "On Delivery").Count();
            ViewBag.Complete = db.orders.Where(x => x.ProcessStatus == "Complete").Count();

            //Search by client's Email

            if (searchEmail != null)
            {
                var Emailsearch = db.orders.Where(x => x.ClientProfile.Email.Contains(searchEmail)).Include(x => x.orderAssignments).Include(x => x.ClientProfile).Include(x => x.ShippimgDetails).ToList().Count();

                if (Emailsearch == 0)
                {
                    ViewBag.NullEmail = " Sorry... There are no orders related to this search (" + searchEmail + ")";
                    return View(db.orders.Where(x => x.ClientProfile.Email.Contains(searchEmail)).Include(x => x.ClientProfile).Include(x => x.orderAssignments).Include(x => x.ShippimgDetails).ToList().ToPagedList(page ?? 1, 5));
                }
                else
                {
                    return View(db.orders.Where(x => x.ClientProfile.Email.Contains(searchEmail)).Include(x => x.ClientProfile).Include(x => x.orderAssignments).Include(x => x.ShippimgDetails).ToList().ToPagedList(page ?? 1, 5));
                }
            }

            //Search by Order Date

            if (searchBy == "Date" && searchDate != "")
            {
                //ModelState.Clear();
                DateTime date = DateTime.Parse(searchDate);

                if (date <= DateTime.Today)
                {
                    var datesearch = db.orders.Where(x => x.DateCreated == date).ToList().Count();

                    if (datesearch == 0)
                    {
                        ViewBag.NoRecords = "Sorry...There were NO SERVICE REQUESTS on this day.";
                        return View(db.orders.Where(x => x.DateCreated == date).Include(x => x.ClientProfile).Include(x => x.orderAssignments).Include(x => x.ShippimgDetails).OrderByDescending(x => x.OrderID).ToList().ToPagedList(page ?? 1, 5));
                    }
                    else
                    {
                        return View(db.orders.Where(x => x.DateCreated == date).Include(x => x.ClientProfile).Include(x => x.orderAssignments).Include(x => x.ShippimgDetails).OrderByDescending(x => x.OrderID).ToList().ToPagedList(page ?? 1, 5));
                    }
                }

                else
                {
                    ViewBag.DateError = "The selected date is ahead of today. NO RECORDS have been capture as yet.";
                    return View(db.orders.Where(x => x.DateCreated == date || date == null).Include(x => x.ClientProfile).Include(x => x.orderAssignments).Include(x => x.ShippimgDetails).OrderByDescending(x => x.OrderID).ToList().ToPagedList(page ?? 1, 5));
                }
            }

            //Search by Order Status 

            if (searchDrop == "Select Status" || searchDrop == "All Records")
            {
                return View(db.orders.Include(x => x.ClientProfile).Include(x => x.orderAssignments).Include(x => x.ShippimgDetails).ToList().OrderByDescending(x => x.OrderID).ToPagedList(page ?? 1, 5));
            }
            else if (searchBy == "Status")
            {
                var statusNull = db.orders.Where(x => x.ProcessStatus == searchDrop).Count();
                if (statusNull == 0)
                {
                    ViewBag.NoRecords = "Sorry... There are no Orders matching the selected status.";
                }
                return View(db.orders.Where(x => x.ProcessStatus == searchDrop || searchDrop == null).Include(x => x.orderAssignments).Include(x => x.ClientProfile).Include(x => x.ShippimgDetails).OrderByDescending(x => x.OrderID).ToList().ToPagedList(page ?? 1, 5));
            }
            else
            {
                return View(db.orders.Include(x => x.ClientProfile).Include(x => x.ShippimgDetails).Include(x => x.orderAssignments).OrderByDescending(x => x.OrderID).ToList().ToPagedList(page ?? 1, 5));
            }
        }

        //Drivers on Duty

        public ActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateRole(FormCollection form)
        {
            string rolename = form["RoleName"];
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));

            if (!roleManager.RoleExists(rolename))
            {
                //create super admin role
                var role = new IdentityRole(rolename);
                roleManager.Create(role);
            }
            return View();
        }

        public ActionResult CreateUserAndAssignRole()
        {
            ViewBag.Roles = db.Roles.Select(r => new SelectListItem { Value = r.Name, Text = r.Name }).ToList();
            return View();
        }

        [HttpPost]
        public ActionResult CreateUserAndAssignRole(FormCollection form)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            string Name = form["txtName"];
            string LastName = form["txtLastName"];
            string UserName = form["txtEmail"];
            string email = form["txtEmail"];
            string pwd = form["txtPassword"];

            //create default user

            var user = new ApplicationUser();
            user.FirstName = Name;
            user.LastName = LastName;
            user.UserName = UserName;
            user.UserName = email;
            user.Email = UserName;

            //string password = pwd;

            var newuser = userManager.Create(user, pwd);
            string rol = form["RoleName"];
            ApplicationUser users = db.Users.Where(u => u.UserName.Equals(UserName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            userManager.AddToRole(user.Id, rol);
            return RedirectToAction("Index", "Employees");
        }


        public ActionResult PrepareOrder(int id)
        {

            OrderAssignment prep = new OrderAssignment();
            ViewBag.Email = new SelectList(db.employees, "EmployeeID", "EmpName");

            prep.AssignDriver = db.employees.Where(x => x.EmpPosition == "Driver").ToList();
            prep.AssignInstallers = db.employees.Where(x => x.EmpPosition == "General").ToList();
            prep.OrderID = id;
            return View(prep);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PrepareOrder(OrderAssignment prep_Order)
        {
            //multi select dropdown
            prep_Order.AssignedGen = string.Join(", ", prep_Order.SelectedIDArray);
            db.orderAssignments.Add(prep_Order);
            db.SaveChanges();

            var updateOrder = db.orders.Where(x => x.OrderID == prep_Order.OrderID).FirstOrDefault();
            updateOrder.ProcessStatus = "Processed";
            db.SaveChanges();

            var driver = db.employees.Find(prep_Order.AssignedDriver);
            var client = new SendGridClient("");
            var from = new EmailAddress("", "Trinity Pty(Ltd)");
            var subject = "New Delivery";
            var to = new EmailAddress(driver.EmpEmail, driver.EmpName);
            var htmlContent = "A new order has been assigned to you for delivery";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, null, htmlContent);
            var response = client.SendEmailAsync(msg);
            ViewBag.OrderID = new SelectList(db.orders, "OrderID", "OrderID", prep_Order.OrderID);

            return RedirectToAction("Index");

        }

        public ActionResult ScheduleReturn(int id)
        {

            EquipmentReturn prep = new EquipmentReturn();

            ViewBag.Email = new SelectList(db.employees, "EmployeeID", "EmpName");

            prep.AssignDriver = db.employees.Where(x => x.EmpPosition == "Driver").ToList();
            prep.AssignInstallers = db.employees.Where(x => x.EmpPosition == "General").ToList();

            prep.OrderID = id;
            return View(prep);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ScheduleReturn(EquipmentReturn rshech_Order)
        {
            //multi select dropdown

            rshech_Order.AssignedGen = string.Join(", ", rshech_Order.SelectedIDArray);


            db.equipmentReturns.Add(rshech_Order);
            db.SaveChanges();

            ViewBag.OrderID = new SelectList(db.orders, "OrderID", "OrderID", rshech_Order.OrderID);

            return RedirectToAction("Index");


        }

        public ActionResult RequestRepair()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RequestRepair(Request request)
        {
            var damagedItems = db.damaged.Where(x => x.DamagedItems > 0 && x.Equipment.Category.CategoryName == request.Type).ToList();
            if (damagedItems.Count == 0)
            {
                ViewBag.NotFound = "No damaged items of this type were found.";
                return View();
            }

            else
            {
                ViewBag.NotFound = "";
                request.DateCreated = DateTime.Now;
                request.ShipmentDate = DateTime.Now;
                request.ReturnDate = DateTime.Now;
                request.Status = "Pending";
                db.requests.Add(request);
                db.SaveChanges();

                return RedirectToAction("ReturnItems", request);
            }

        }

        public ActionResult ReturnItems(Request request)
        {
            Session["QID"] = request.RequestID;
            var damagedItems = db.damaged.Where(x => x.Equipment.Category.CategoryName == request.Type && x.DamagedItems > 0).Include(x => x.Equipment);
            ViewBag.Count = damagedItems.Count();
            return View(damagedItems.ToList());
        }

        [HttpPost]
        public ActionResult ItemDescription(List<Damaged> items)
        {
            List<RepairItem> Items = new List<RepairItem>();
            RepairItem obj = new RepairItem();

            foreach (var item in items)
            {
                if (item.selected == true)
                {
                    for (int i = 0; i < item.DamagedItems; i++)
                    {
                        var ritems = db.damaged.Where(x => x.EquipmentID == item.EquipmentID).Include(x => x.Equipment).FirstOrDefault();
                        obj.EquipmentID = ritems.EquipmentID;
                        obj.EquipName = ritems.Equipment.Name;
                        Items.Add(obj);
                    }
                }
            }

            ViewBag.Count = Items.Count();
            return View(Items.ToList());
        }

        [HttpPost]
        public ActionResult SaveItems(List<RepairItem> damageds)
        {
            int Qid = Convert.ToInt32(Session["QID"]);
            int i = 0;
            foreach (var item in damageds)
            {
                item.ItemCode = "ITM-" + item.EquipmentID.ToString() + i;
                item.RequestID = Qid;
                db.repairItems.Add(item);
                db.SaveChanges();
                i++;
            }
            var request = db.requests.Find(Qid);
            request.Quantity = damageds.Count();
            db.SaveChanges();

            var suppliers = db.suppliers.Where(x => x.SuppType == request.Type).ToList();

            foreach(var supplier in suppliers)
            {
                var client = new SendGridClient("");
                var from = new EmailAddress("", "Trinity Pty(Ltd)");
                var subject = "New Repair Request";
                var to = new EmailAddress(supplier.SuppEmail, supplier.SuppName);
                var htmlContent = "A new repair request of " + request.Type + " has been recieved ";
                var msg = MailHelper.CreateSingleEmail(from, to, subject, null, htmlContent);
                var response = client.SendEmailAsync(msg);
            }
           

            return RedirectToAction("Index", "RepairItems");
        }

        public ActionResult ViewRequests()
        {
            var vitems = db.requests.ToList();
            return View(vitems);
        }



        public ActionResult ViewItems()
        {
            var vitems = db.requests.ToList();
            return View(vitems);
        }

        public ActionResult ViewQuotes(int id)
        {
            var request = db.requests.Find(id);
            if(request.Status == "Approved" || request.Status == "Collected" || request.Status == "Items Repaired")
            {
                var quote = db.quotes.Where(x => x.RequestID == id && x.Acceptance == "Accepted").ToList();
                return View(quote);
            }

            var quotes = db.quotes.Where(x => x.RequestID == id).ToList();
            return View(quotes);
        }

        public ActionResult Accept(int id)
        {
            var acceptequote = db.quotes.Find(id);
            return PartialView(acceptequote);
        }


        [HttpPost]
        public ActionResult Accept(Quote quote)
        {
            var acceptequote = db.quotes.Find(quote.QuoteID);
            acceptequote.Acceptance = "Accepted";
            Supplier supplier = db.suppliers.Where(x => x.SuppID == acceptequote.SuppID).FirstOrDefault();

            var request = db.requests.Find(acceptequote.RequestID);
            request.ShipmentDate = quote.ShipmentDate;
            request.Status = "Approved";
            db.SaveChanges();


            var client = new SendGridClient("");
            var from = new EmailAddress("", "Trinity Pty(Ltd)");
            var subject = "Quote Feedback";
            var to = new EmailAddress(supplier.SuppEmail, supplier.SuppName);
            var htmlContent = "Thank you for the quotation of " + request.Type +". "+ "The items will be shipped on " + request.ShipmentDate.ToShortDateString() + ".";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, null, htmlContent);
            var response = client.SendEmailAsync(msg);

            //Sending response to rejected supplier quotes
            var declinedquotes = db.quotes.Where(x => x.RequestID == request.RequestID && x.Acceptance != "Accepted").Include(x => x.Supplier).ToList();
            foreach(var quoted in declinedquotes)
            {
                Supplier dcsupplier = db.suppliers.Where(x => x.SuppID == quoted.SuppID).FirstOrDefault();
                quoted.Acceptance = "Declined";
                db.SaveChanges();
                var sclient = new SendGridClient("");
                var sfrom = new EmailAddress("", "Trinity Pty(Ltd)");
                var ssubject = "Quote Feedback";
                var sto = new EmailAddress(dcsupplier.SuppEmail, dcsupplier.SuppName);
                var shtmlContent = "Hi " + dcsupplier.SuppName + "<br/><br/>" + "Thank you for taking your time to prepare a repair quote for Trinity Rentals Enterprise." + "<br/>" + "We really appriciate the effort you put in. Unfortunately your quotation was not selected." + "<br/><br/>" + "Regards," + "<br/><br/>" + "Trinity Rentals Enterprise."; 

;
               var smsg = MailHelper.CreateSingleEmail(sfrom, sto, ssubject, null, shtmlContent);
                var sresponse = sclient.SendEmailAsync(smsg);
            }
            return RedirectToAction("AcceptSuccess", new {id = acceptequote.QuoteID } );
        }


        public ActionResult AcceptSuccess(int id)
        {
            var quote = db.quotes.Where(x => x.QuoteID == id).Include(x => x.Request).Include(x => x.Supplier).FirstOrDefault();

            quote.QRCode = GenerateQRCode("Items Recieved " + id);
            db.SaveChanges();
           
            return View(quote);
        }

        public ActionResult ScheduleDriver(int id)
        {
            QuoteAssignment quoteAssignment = new QuoteAssignment();
            ViewBag.Email = new SelectList(db.employees, "EmployeeID", "EmpName");

            quoteAssignment.AssignDriver = db.employees.Where(x => x.EmpPosition == "Driver").ToList();
            quoteAssignment.AssignInstallers = db.employees.Where(x => x.EmpPosition == "General").ToList();
            quoteAssignment.RequestID = id;
            return View(quoteAssignment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ScheduleDriver(QuoteAssignment qAssignment)
        {
            //multi select dropdown
            qAssignment.AssignedGen = string.Join(", ", qAssignment.SelectedIDArray);
            db.quoteAssignments.Add(qAssignment);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        private string GenerateQRCode(string qrcodeText)
        {

            string folderPath = "~/QRCode/";
            string imagePath = "~/QRCode/QuoteQR" + Session["QuoteID"] + ".jpg";

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

        public ActionResult ViewInvoice(int id)
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
                    Quote = quote,
                    Request = request,
                    Supplier = supplier,
                    quoteItems = db.quoteitems.Where(x => x.QuoteID == quote.QuoteID).ToList()
                };
                ViewBag.QuoteType = request.Type;
                ViewBag.QuoteMnth = quote.DateCreated.ToString("MMM");
                ViewBag.TotalCost = getTot.GetActualCost(quote.QuoteID).ToString("C");

                return View(report);
            }
            ViewBag.NoReport = "No report can be generated for this quote. Please contact the administartor";
            return View();
        }

        public ActionResult Secure_Payment(int id)
        {
            var quote = db.quotes.Where(x => x.QuoteID == id).FirstOrDefault();
            QuoteItem getTot = new QuoteItem();
            string total = getTot.GetActualCost(quote.QuoteID).ToString();


            return Redirect(PaymentLink(total, "Repair Service Payment | Repair No: " + quote.RequestID, quote.RequestID));
        }

        public string PaymentLink(string totalCost, string paymentSubjetc, int request_id)
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
            stringBuilder.Append("&return_url= " + HttpUtility.HtmlEncode("https://2021grp24.azurewebsites.net/Admin/PaymentSuccess/" + request_id));
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

        public ActionResult PaymentSuccess()
        {
            var request = db.requests.Find(1);
            request.PaymentStatus = "Paid";
            db.SaveChanges();
            var quote = db.quotes.Where(x => x.RequestID == 1 && x.Acceptance == "Accepted").FirstOrDefault();
            return View(quote);
        }

    }
}
