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
    public class DamagedsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Damageds
        public ActionResult Index()
        {
            var damaged = db.damaged.Include(d => d.Equipment).Include(d => d.Order);
            return View(damaged.ToList());
        }

        // GET: Damageds/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Damaged damaged = db.damaged.Find(id);
            if (damaged == null)
            {
                return HttpNotFound();
            }
            return View(damaged);
        }

        // GET: Damageds/Create
        public ActionResult Create()
        {
            ViewBag.EquipmentID = new SelectList(db.Equipment, "EquipmentID", "Name");
            ViewBag.OrderID = new SelectList(db.orders, "OrderID", "PaymentMethod");
            return View();
        }

        // POST: Damageds/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DamagedId,Name,Description,MissingItems,DamagedItems,ReturnDate,Image,EquipmentID,OrderID")] Damaged damaged)
        {
            Damaged obj = new Damaged();

            // new model for easy data capture
            if (ModelState.IsValid)
            {
                db.damaged.Add(damaged);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.EquipmentID = new SelectList(db.Equipment, "EquipmentID", "Name", damaged.EquipmentID);
            ViewBag.OrderID = new SelectList(db.orders, "OrderID", "PaymentMethod", damaged.OrderID);
            return View(damaged);
        }

        // GET: Damageds/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Damaged damaged = db.damaged.Find(id);
            if (damaged == null)
            {
                return HttpNotFound();
            }
            ViewBag.EquipmentID = new SelectList(db.Equipment, "EquipmentID", "Name", damaged.EquipmentID);
            ViewBag.OrderID = new SelectList(db.orders, "OrderID", "PaymentMethod", damaged.OrderID);
            return View(damaged);
        }

        // POST: Damageds/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DamagedId,Name,Description,MissingItems,DamagedItems,ReturnDate,Image,EquipmentID,OrderID")] Damaged damaged)
        {
            if (ModelState.IsValid)
            {
                db.Entry(damaged).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EquipmentID = new SelectList(db.Equipment, "EquipmentID", "Name", damaged.EquipmentID);
            ViewBag.OrderID = new SelectList(db.orders, "OrderID", "PaymentMethod", damaged.OrderID);
            return View(damaged);
        }

        // GET: Damageds/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Damaged damaged = db.damaged.Find(id);
            if (damaged == null)
            {
                return HttpNotFound();
            }
            return View(damaged);
        }

        // POST: Damageds/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Damaged damaged = db.damaged.Find(id);
            db.damaged.Remove(damaged);
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
