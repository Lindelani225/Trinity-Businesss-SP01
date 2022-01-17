using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BusinesssTrinitySP01.Models;

namespace BusinesssTrinitySP01.Controllers
{
    public class PackageItemsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: PackageItems
        public ActionResult Index()
        {
            var packageItems = db.packageItems.Include(p => p.Color).Include(p => p.DesignComp).Include(p => p.Package);
            return View(packageItems.ToList());
        }

        // GET: PackageItems/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PackageItem packageItem = db.packageItems.Find(id);
            if (packageItem == null)
            {
                return HttpNotFound();
            }
            return View(packageItem);
        }

        // GET: PackageItems/Create
        public ActionResult Create()
        {
            ViewBag.BoxId = new SelectList(db.colors, "BoxId", "Name");
            ViewBag.CompId = new SelectList(db.designComps, "CompId", "Name");
            ViewBag.PckId = new SelectList(db.packages, "PckId", "Name");
            return View();
        }

        // POST: PackageItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ItemId,PckId,CartID,CompId,BoxId,Qty,Price")] PackageItem packageItem)
        {
            if (ModelState.IsValid)
            {
                db.packageItems.Add(packageItem);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BoxId = new SelectList(db.colors, "BoxId", "Name", packageItem.BoxId);
            ViewBag.CompId = new SelectList(db.designComps, "CompId", "Name", packageItem.CompId);
            ViewBag.PckId = new SelectList(db.packages, "PckId", "Name", packageItem.PckId);
            return View(packageItem);
        }

        // GET: PackageItems/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PackageItem packageItem = db.packageItems.Find(id);
            if (packageItem == null)
            {
                return HttpNotFound();
            }
            ViewBag.BoxId = new SelectList(db.colors, "BoxId", "Name", packageItem.BoxId);
            ViewBag.CompId = new SelectList(db.designComps, "CompId", "Name", packageItem.CompId);
            ViewBag.PckId = new SelectList(db.packages, "PckId", "Name", packageItem.PckId);
            return View(packageItem);
        }

        // POST: PackageItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ItemId,PckId,CartID,CompId,BoxId,Qty,Price")] PackageItem packageItem)
        {
            if (ModelState.IsValid)
            {
                db.Entry(packageItem).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BoxId = new SelectList(db.colors, "BoxId", "Name", packageItem.BoxId);
            ViewBag.CompId = new SelectList(db.designComps, "CompId", "Name", packageItem.CompId);
            ViewBag.PckId = new SelectList(db.packages, "PckId", "Name", packageItem.PckId);
            return View(packageItem);
        }

        // GET: PackageItems/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PackageItem packageItem = db.packageItems.Find(id);
            if (packageItem == null)
            {
                return HttpNotFound();
            }
            return View(packageItem);
        }

        // POST: PackageItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PackageItem packageItem = db.packageItems.Find(id);
            db.packageItems.Remove(packageItem);
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
