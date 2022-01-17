using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BusinesssTrinitySP01.Logic;
using BusinesssTrinitySP01.Models;

namespace BusinesssTrinitySP01.Controllers
{
    public class DecorRentalsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private Client_Logic obj = new Client_Logic();
        public string shoppingCartID { get; set; }

        public ActionResult AddPackage(int id)
        {
            ViewBag.oID = id;
            return View();
        }


        // GET: DecorRentals
        public ActionResult GetOrder(int PckID)
        {
            //Session["PckID"] = PckID;
            var userOrders = db.orders.Where(x => x.Email == User.Identity.Name).ToList();

            return View(userOrders);
        }


        public ActionResult SelectedOrder(int PckID)
        {
            Session["PckID"] = PckID;
            //int pckid = Convert.ToInt32(Session["PckID"]);
            //var GetPackage = db.packages.Where(x => x.PckId == pckid).FirstOrDefault();

            //GetPackage.OrderID = oID;
            //db.SaveChanges();

            return RedirectToAction("GetTheme");
        }

        public ActionResult GetTheme()
        {
            var AllThemes = db.themes.ToList();
            return View(AllThemes);
        }

        public ActionResult SelectedTheme(int thmId)
        {
            int PckID = Convert.ToInt32(Session["PckID"]);
            var package = db.packages.Where(x => x.PckId == PckID).FirstOrDefault();

            package.ThemeID = thmId;
            db.SaveChanges();

            return RedirectToAction("GetComponents", new { thmId });
        }

        public ActionResult GetComponents(int thmId)
        {
            Session["theme"] = thmId;
            var designComps = db.designComps.Where(x => x.ThemeId == thmId).ToList();

            //Passes PckID to PackageReview from GetComponents View
            ViewBag.PckID = Convert.ToInt32(Session["PckID"]);
            return View(designComps);
        }

        [HttpGet]
        public ActionResult GetItem(int CompID)
        {
            int PckID = Convert.ToInt32(Session["PckID"]);

            PackageItem packageItem = new PackageItem();

            var component = db.designComps.Where(x => x.CompId == CompID).FirstOrDefault();
            var getColors = db.compColors.Where(x => x.CompId == CompID).ToList();

            var order = db.packages.Where(x => x.PckId == PckID).Select(x => x.OrderID).FirstOrDefault();
            var OItems = db.OrderItems.Where(x => x.OrderID == order).Where(x => x.Equipment.Category.CategoryName == "Chairs" || x.Equipment.Category.CategoryName == "Tables").ToList();

            if (OItems.Count() != 0)
            {
                //Set DesignComps Category to display quantity only if filter is for Chairs and/or Tables Only!!!
                var quantity = OItems.Select(x => x.quantity).FirstOrDefault();
                packageItem.Qty = quantity;
            }
            else
            {
                packageItem.Qty = 1;
            }

            packageItem.compColors = getColors;
            packageItem.CompId = CompID;
            packageItem.PckId = PckID;
            packageItem.Price = component.RentPrice;

            return PartialView(packageItem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetItem(PackageItem packageItem, int comp)
        {
            int thmId = Convert.ToInt32(Session["theme"]);

            if (ModelState.IsValid)
            {
                db.packageItems.Add(packageItem);
                db.SaveChanges();
                return RedirectToAction("GetComponents", new { thmId });
            }
            return RedirectToAction("GetComponents", new { thmId });
        }

        public ActionResult PackageReview(int PckID)
        {
            var packageItems = db.packageItems.Where(x => x.PckId == PckID).Include(x => x.Package).ToList();

            foreach (var item in packageItems)
            {
                item.Price = (item.Price * item.Qty);
            }

            ViewBag.thmID = Convert.ToInt32(Session["theme"]);
            ViewBag.PckID = PckID;

            ViewBag.TotalCost = obj.GetPackageTotal(PckID).ToString("C");
            //ViewBag.TotalCost = packageItems.Select(x => x.Price).Sum().ToString("C");
            ViewBag.Package = packageItems.Select(x => x.Package.Name).FirstOrDefault();


            return View(packageItems);
        }

        public ActionResult AddToCart(int PckID)
        {
            var package = db.packages.Find(PckID);
            package.CartID = obj.GetCartID();
            Session["CartID"] = package.CartID;
            
            db.SaveChanges();

            return RedirectToAction("ViewCart", "Home", new { PckID });
        }

        public ActionResult ViewCart(int PckID)
        {
            string CartID = Convert.ToString(Session["CartID"]);
            var package = db.packages.Where(x => x.CartID == CartID).FirstOrDefault();

            ViewBag.Theme = db.themes.Where(x => x.ThemeId == package.ThemeID).Select(x => x.Name).FirstOrDefault();
            ViewBag.NumItems = db.packageItems.Where(x => x.Package.PckId == PckID).ToList().Count();
            ViewBag.TotalCost = obj.GetPackageTotal(PckID).ToString("C");

            return PartialView(package);
        }

        public ActionResult RemovePackage()
        {
            return RedirectToAction("ViewCart", "Home");
        }

        [HttpGet]
        public ActionResult EditItems(int itmID)
        {
            var pckItem = db.packageItems.Find(itmID);
            var getColors = db.compColors.Where(x => x.CompId == pckItem.CompId).ToList();

            if (pckItem == null)
            {
                return HttpNotFound();
            }
            pckItem.compColors = getColors;

            return PartialView("EditItems", pckItem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditItems(PackageItem packageItem)
        {
            if (ModelState.IsValid)
            {
                db.Entry(packageItem).State = EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("PackageReview", new { PckID = packageItem.PckId });
        }

        public ActionResult RemoveItem(int itmID)
        {
            var pckItem = db.packageItems.Find(itmID);

            if (pckItem == null)
            {
                return HttpNotFound();
            }

            return PartialView("RemoveItem", pckItem);
        }

        [HttpPost, ActionName("RemoveItem")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int itmID)
        {
            var pckItem = db.packageItems.Find(itmID);
            db.packageItems.Remove(pckItem);
            db.SaveChanges();

            return RedirectToAction("PackageReview", new { PckID = pckItem.PckId });
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}