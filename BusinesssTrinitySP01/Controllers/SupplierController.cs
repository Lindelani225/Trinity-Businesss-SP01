using BusinesssTrinitySP01.Models;
using PagedList;
using Rotativa;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Text;
using Bytescout.BarCodeReader;

namespace BusinesssTrinitySP01.Controllers
{
    [Authorize(Roles = "Supplier, Super Admin")]

    public class SupplierController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Supplier
        public ActionResult Index(int? page, string searchBy, string searchDrop, string searchDate, string searchEmail)
        {
            Order order = new Order();
            Supplier supplier = db.suppliers.Where(x => x.SuppEmail == User.Identity.Name).FirstOrDefault();
            var allrequests = db.requests.Where(x => x.Type == supplier.SuppType).ToList();

            //Get Order Count by status

            ViewBag.AllRequests = allrequests.Count();
            ViewBag.Pending = allrequests.Where(x => x.Status == "Pending").Count();
            ViewBag.Processed = allrequests.Where(x => x.Status != "Pending").Count();
            ViewBag.Delivered = (allrequests.Where(x => x.Status == "Delivered").Count()) + (allrequests.Where(x => x.Status == "Complete").Count());
            ViewBag.tocomplete = allrequests.Where(x => x.Status == "Delivered").Count();
            ViewBag.pendingdel = allrequests.Where(x => x.Status == "On delivery").Count();
            ViewBag.complete = allrequests.Where(x => x.Status == "Complete").Count();

            //Search by client's Email

            //if (searchEmail != null)
            //{
            //    var Emailsearch = db.quotes.Where(x => x.ClientProfile.Email.Contains(searchEmail)).Include(x => x.orderAssignments).Include(x => x.ClientProfile).Include(x => x.ShippimgDetails).ToList().Count();

            //    if (Emailsearch == 0)
            //    {
            //        ViewBag.NullEmail = " Sorry... There are no orders related to this search (" + searchEmail + ")";
            //        return View(db.orders.Where(x => x.ClientProfile.Email.Contains(searchEmail)).Include(x => x.ClientProfile).Include(x => x.orderAssignments).Include(x => x.ShippimgDetails).ToList().ToPagedList(page ?? 1, 5));
            //    }
            //    else
            //    {
            //        return View(db.orders.Where(x => x.ClientProfile.Email.Contains(searchEmail)).Include(x => x.ClientProfile).Include(x => x.orderAssignments).Include(x => x.ShippimgDetails).ToList().ToPagedList(page ?? 1, 5));
            //    }
            //}

            //Search by Order Date

            if (searchBy == "Date" && searchDate != "")
            {
                //ModelState.Clear();
                DateTime date = DateTime.Parse(searchDate);

                if (date <= DateTime.Today)
                {
                    var datesearch = allrequests.Where(x => x.DateCreated == date).Count();

                    if (datesearch == 0)
                    {
                        
                        ViewBag.NoRecords = "Sorry...There were NO SERVICE REQUESTS on this day.";
                        return View(allrequests.OrderByDescending(x => x.RequestID).ToPagedList(page ?? 1, 5));
                    }
                    else
                    {
                        return View(allrequests.Where(x => x.DateCreated == date).OrderByDescending(x => x.RequestID).ToPagedList(page ?? 1, 5));
                    }
                }

                else
                {
                    ViewBag.DateError = "The selected date is ahead of today. NO RECORDS have been capture as yet.";
                    return View(allrequests.Where(x => x.DateCreated == date || date == null).OrderByDescending(x => x.RequestID).ToPagedList(page ?? 1, 5));
                }
            }

            //Search by Order Status 

            if (searchDrop == "Select Status" || searchDrop == "All Records")
            {
                return View(allrequests.OrderByDescending(x => x.RequestID).ToPagedList(page ?? 1, 5));
            }
            else if (searchBy == "Status")
            {
                var statusNull = allrequests.Where(x => x.Status == searchDrop).Count();
                if (statusNull == 0)
                {
                    ViewBag.NoRecords = "Sorry... There are no Orders matching the selected status.";
                }
                return View(allrequests.Where(x => x.Status == searchDrop || searchDrop == null).OrderByDescending(x => x.RequestID).ToPagedList(page ?? 1, 5));
            }
            else
            {
                return View(allrequests.OrderByDescending(x => x.RequestID).ToPagedList(page ?? 1, 5));
            }
        }


        public ActionResult GetItems(int? id)
        {
            var request = db.requests.Find(id);
            if (request != null)
            {
                
                Quote newQuote = new Quote();
                newQuote.DateCreated = DateTime.Now;
                newQuote.RequestID = request.RequestID;
                newQuote.SuppID = db.suppliers.Where(x => x.SuppEmail == User.Identity.Name).Select(x => x.SuppID).FirstOrDefault();
                db.quotes.Add(newQuote);
                db.SaveChanges();

                Session["Quote"] = newQuote.QuoteID;
                List<QuoteItem> items = new List<QuoteItem>();
                var quoteitems = db.repairItems.Where(x => x.RequestID == id).Include(x => x.Equipment);
                foreach(var item in quoteitems)
                {
                    QuoteItem newItem = new QuoteItem();
                    newItem.QuoteID = newQuote.QuoteID;
                    newItem.ItemCode = item.ItemCode;
                    newItem.ItemID = item.ItemID;
                    newItem.Description = item.Description;
                    newItem.img = item.img;
                    newItem.img2 = item.img2;
                    items.Add(newItem);
                }

                ViewBag.Count = items.Count();
                return View(items.ToList());
            }
            else
            {
                ViewBag.NotFound = "No items were found for this quote request. Please contact administrator";
                return View();
            }
        }

        [HttpPost]
        public ActionResult GetItems(List<QuoteItem> Items)
        {
            int qt = Convert.ToInt32(Session["Quote"]);
            foreach (var item in Items)
            {
                db.quoteitems.Add(item);
                db.SaveChanges();
            }

            return RedirectToAction("SHipmentDetails", new { id = qt });
        }

        public ActionResult ViewItem(int id)
        {
            var rItem = db.repairItems.Where(x => x.ItemID == id).FirstOrDefault();
            return PartialView(rItem);
        }

        public ActionResult ShipmentDetails(int id)
        {
            Quote quote = db.quotes.Find(id);
            return View(quote);
        }

        [HttpPost]
        public ActionResult ShipmentDetails(Quote shipment)
        {
            if (shipment == null)
            {
                ViewBag.NoReport = "No quote was referenced.";
                return View();
            }
            var quote = db.quotes.Find(shipment.QuoteID);
            quote.ShipmentType = shipment.ShipmentType;
            quote.Duration = shipment.Duration;
            quote.DateCreated = DateTime.Now;
            db.SaveChanges();
            return RedirectToAction("QuoteReport", new { id = quote.QuoteID });
        }

        public ActionResult QuoteReport(int? id)
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
                string url = "https://2021grp24.azurewebsites.net/Home/PrintPartialViewToPdf/" + quote.QuoteID;
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

                    var client = new SendGridClient("");
                    var from = new EmailAddress("", "Trinity Pty(Ltd)");
                    var subject = "Quote " + id + " | New Quote";
                    var to = new EmailAddress("", supplier.SuppName );
                    var htmlContent = "A new quotation of " + request.Type +  " has been recieved from " + supplier.SuppName + ".";
                    var msg = MailHelper.CreateSingleEmail(from, to, subject, null, htmlContent);
                    var response = client.SendEmailAsync(msg);

                    return View(report);
                }
                ViewBag.NoReport = "No report can be generated for this quote. Please contact the administartor";
                return View();
            }
        }

        // View Quote Report
        public ActionResult Report(int? id)
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
                string url = "https://2021grp24.azurewebsites.net/Home/PrintPartialViewToPdf/" + quote.QuoteID;
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

                    return View("QuoteReport", report);
                }
                ViewBag.NoReport = "No report can be generated for this quote. Please contact the administartor";
                return View();
            }
        }

        public ActionResult ViewQReport(int id)
        {
            Supplier supplier1 = db.suppliers.Where(x => x.SuppEmail == User.Identity.Name).FirstOrDefault();
            var myquote = db.quotes.Where(x => x.RequestID == id && x.SuppID == supplier1.SuppID).FirstOrDefault();
            Request request = db.requests.Where(x => x.RequestID == myquote.RequestID).FirstOrDefault();
            QuoteItem getTot = new QuoteItem();

            if (myquote != null)
            {
                QuoteReport report = new QuoteReport()
                {
                    QuoteID = myquote.QuoteID,
                    Quote = db.quotes.Find(myquote.QuoteID),
                    Supplier = supplier1,
                    quoteItems = db.quoteitems.Where(x => x.QuoteID == myquote.QuoteID).ToList()
                };
                ViewBag.QuoteType = request.Type;
                ViewBag.QuoteMnth = myquote.DateCreated.ToString("MMM");
                ViewBag.TotalCost = getTot.GetCost(myquote.QuoteID).ToString("C");

                return View("QuoteReport", report);
            }
            return View("QuoteReport");
        }


        public ActionResult QRScanner(int id)
        {
            Session["QuoteID"] = id;
            return View();
        }
        public ActionResult CollectionSuccess()
        {
            return View();
        }
        public ActionResult CollectionError()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DecodeQR(string image, string type)
        {
            var id = Session["QuoteID"];
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
                                var quote = db.quotes.Find(id);

                                if (barcode.Value == "Items Recieved " + id)
                                {

                                    quote.Status = "Collected";
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
                        var qreport = new PartialViewAsPdf("~/Views/Supplier/QuoteReport.cshtml", report);
                        return qreport;
                    }
                    ViewBag.NoReport = "No report can be generated for this quote. Please contact the administartor";
                    return View("QuoteReport");
                }
            }
        }

        public ActionResult GenerateInvoice(int id)
        {
            Session["quote"] = id;
            var quoteitems = db.quoteitems.Where(x => x.QuoteID == id).ToList();
            ViewBag.Count = quoteitems.Count();
            return View(quoteitems);
        }

        [HttpPost]
        public ActionResult GenerateInvoice(List<QuoteItem> quoteItems)
        {
            foreach(var quoteitem in quoteItems)
            {
                var getQuoteItem = db.quoteitems.Where(x => x.ItemCode == quoteitem.ItemCode).FirstOrDefault();
                getQuoteItem.ACost = quoteitem.ACost;
                db.SaveChanges();
            }

            return RedirectToAction("RepairInvoice", quoteItems);
        }

        public ActionResult RepairInvoice()
        {
            int id = Convert.ToInt32(Session["quote"]);
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
                    Request = request,
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

                return View("RepairInvoice",report);
            }
            ViewBag.NoReport = "No report can be generated for this quote. Please contact the administartor";
            return View();
        }

        [HttpGet]
        public ActionResult ConfirmInvoice(int id)
        {
            var request = db.requests.Find(id);
            return PartialView(request);
        }

        [HttpPost]
        public ActionResult ConfirmInvoice(Request request)
        {
            var crequest = db.requests.Find(request.RequestID);
            crequest.Status = "Items Repaired";
            db.SaveChanges();
            var quote = db.quotes.Where(x => x.RequestID == request.RequestID && x.Acceptance == "Accepted").FirstOrDefault();
            

            string url = "https://2021grp24.azurewebsites.net/Home/ViewInvoice" + quote.RequestID;
            var client = new SendGridClient("");
            var from = new EmailAddress("", "Trinity Pty(Ltd)");
            var subject = "Repair Invoice" + request.RequestID;
            var to = new EmailAddress("", "Admin");
            var htmlContent = "The supplier has repaired the items. Please view the generated invoice using this link." + url;
            var msg = MailHelper.CreateSingleEmail(from, to, subject, null, htmlContent);
            var response = client.SendEmailAsync(msg);
            return RedirectToAction("InvoiceSuccess", new { id = request.RequestID });
        }

        public ActionResult InvoiceSuccess(int id)
        {
            var request = db.requests.Find(id);
            return View(request);
        }
    }

   
}
