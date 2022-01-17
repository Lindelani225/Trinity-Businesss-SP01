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
    public class DesignCompsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: DesignComps
        public ActionResult Index()
        {
            var designComps = db.designComps.Include(d => d.Theme);
            return View(designComps.ToList());
        }


        // GET: DesignComps/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DesignComp designComp = db.designComps.Find(id);
            if (designComp == null)
            {
                return HttpNotFound();
            }
            return View(designComp);
        }

        //public ActionResult GetOrder(int PckID)
        //{
        //    Session["PckID"] = PckID;
        //    var userOrders = db.orders.Where(x=>x.Email==User.Identity.Name).ToList();

        //    return View(userOrders);
        //}

        //public ActionResult SelectedOrder(int oID)
        //{
        //    int PckID = Convert.ToInt32(Session["PckID"]);
        //    var GetPackage = db.packages.Where(x => x.PckId == PckID).FirstOrDefault();

        //    GetPackage.OrderID = oID;
        //    db.SaveChanges();

        //    return RedirectToAction("GetTheme");
        //}

        //public ActionResult GetTheme()
        //{
        //    var AllThemes = db.themes.ToList();
        //    return View(AllThemes);
        //}

        //public ActionResult SelectedTheme(int thmId)
        //{
        //    int PckID = Convert.ToInt32(Session["PckID"]);
        //    var package = db.packages.Where(x => x.PckId == PckID).FirstOrDefault();

        //    package.ThemeID = thmId;
        //    db.SaveChanges();

        //    return RedirectToAction("GetComponents", new { thmId });
        //}


        //public ActionResult GetComponents(int thmId)
        //{
        //    Session["theme"] = thmId;
        //    var designComps = db.designComps.Where(x => x.ThemeId == thmId).ToList();

        //    return View(designComps);
        //}

        public ActionResult ComponentDetails(int? Id)
        {
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DesignComp designComp = db.designComps.Find(Id);
            if (designComp == null)
            {
                return HttpNotFound();
            }
            return PartialView(designComp);
        }

        //[HttpGet]
        //public ActionResult GetItem(int CompID)
        //{
        //    int PckID = Convert.ToInt32(Session["PckID"]);

        //    PackageItem packageItem = new PackageItem();

        //    var component = db.designComps.Where(x => x.CompId == CompID).FirstOrDefault();
        //    var getColors = db.compColors.Where(x => x.CompId == CompID).ToList();

        //    var order = db.packages.Where(x => x.PckId == PckID).Select(x => x.OrderID).FirstOrDefault();
        //    var OItems = db.OrderItems.Where(x => x.OrderID == order).Where(x=>x.Equipment.Category.CategoryName== "Chairs" || x.Equipment.Category.CategoryName=="Tables").ToList();

        //    if (OItems.Count() != 0) 
        //    {
        //        //Set DesignComps Category to display quantity only if filter is for Chairs and/or Tables Only!!!
        //        var quantity = OItems.Select(x => x.quantity).FirstOrDefault();
        //        packageItem.Qty = quantity;
        //    }
        //    else
        //    {
        //        packageItem.Qty = 1;
        //    }

        //    packageItem.compColors = getColors;
        //    packageItem.CompId = CompID;
        //    packageItem.PckId = PckID;
        //    packageItem.Price = component.RentPrice;

        //    return PartialView(packageItem);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult GetItem(PackageItem packageItem, int comp)
        //{
        //    int thmId = Convert.ToInt32(Session["theme"]);

        //    if (ModelState.IsValid)
        //    {
        //        db.packageItems.Add(packageItem);
        //        db.SaveChanges();
        //        return RedirectToAction("GetComponents", new { thmId });
        //    }
        //    return RedirectToAction("GetComponents", new { thmId });
        //}

        // GET: DesignComps/Create
        public ActionResult Create()
        {
            ViewBag.ThemeId = new SelectList(db.themes, "ThemeId", "Name");
            return View();
        }

        // POST: DesignComps/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CompId,ThemeId,Name,Type,Image,Qty,UnitPrice,RentPrice")] DesignComp designComp)
        {
            if (ModelState.IsValid)
            {
                HttpPostedFileBase file = Request.Files["DesignImage"];
                UploadImage service = new UploadImage();
                designComp.Image = service.ConvertToBytes(file);
                db.designComps.Add(designComp);
                db.SaveChanges();
                return RedirectToAction("AddColor", "CompColors", new { id = designComp.CompId });
            }

            ViewBag.ThemeId = new SelectList(db.themes, "ThemeId", "Name", designComp.ThemeId);
            return View(designComp);
        }

        public ActionResult AddColor(int? id)
        {
            if (id != null)
            {

            }
            return View();
        }

        // GET: DesignComps/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DesignComp designComp = db.designComps.Find(id);
            if (designComp == null)
            {
                return HttpNotFound();
            }
            ViewBag.ThemeId = new SelectList(db.themes, "ThemeId", "Name", designComp.ThemeId);
            return View(designComp);
        }

        // POST: DesignComps/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CompId,ThemeId,Name,Type,Image,Qty,UnitPrice,RentPrice")] DesignComp designComp)
        {
            if (ModelState.IsValid)
            {
                db.Entry(designComp).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ThemeId = new SelectList(db.themes, "ThemeId", "Name", designComp.ThemeId);
            return View(designComp);
        }

        // GET: DesignComps/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DesignComp designComp = db.designComps.Find(id);
            if (designComp == null)
            {
                return HttpNotFound();
            }
            return View(designComp);
        }

        // POST: DesignComps/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DesignComp designComp = db.designComps.Find(id);
            db.designComps.Remove(designComp);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
