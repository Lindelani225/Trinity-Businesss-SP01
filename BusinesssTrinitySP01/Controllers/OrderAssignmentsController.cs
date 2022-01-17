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
    public class OrderAssignmentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: OrderAssignments
        public ActionResult Index()
        {
            var orderAssignments = db.orderAssignments.Include(o => o.Order).Include(x=>x.Order.ClientProfile);
            return View(orderAssignments.ToList());
        }

        // GET: OrderAssignments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderAssignment orderAssignment = db.orderAssignments.Find(id);
            if (orderAssignment == null)
            {
                return HttpNotFound();
            }
            return View(orderAssignment);
        }

        // GET: OrderAssignments/Create
        public ActionResult Create()
        {
            ViewBag.OrderID = new SelectList(db.orders, "OrderID", "PaymentMethod");
            return View();
        }

        // POST: OrderAssignments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OrderAssID,AssignedDriver,AssignedGen,OrderID")] OrderAssignment orderAssignment)
        {
            if (ModelState.IsValid)
            {
                db.orderAssignments.Add(orderAssignment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.OrderID = new SelectList(db.orders, "OrderID", "PaymentMethod", orderAssignment.OrderID);
            return View(orderAssignment);
        }

        // GET: OrderAssignments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderAssignment orderAssignment = db.orderAssignments.Find(id);
            if (orderAssignment == null)
            {
                return HttpNotFound();
            }
            ViewBag.OrderID = new SelectList(db.orders, "OrderID", "PaymentMethod", orderAssignment.OrderID);
            return View(orderAssignment);
        }

        // POST: OrderAssignments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OrderAssID,AssignedDriver,AssignedGen,OrderID")] OrderAssignment orderAssignment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(orderAssignment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.OrderID = new SelectList(db.orders, "OrderID", "PaymentMethod", orderAssignment.OrderID);
            return View(orderAssignment);
        }

        // GET: OrderAssignments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderAssignment orderAssignment = db.orderAssignments.Find(id);
            if (orderAssignment == null)
            {
                return HttpNotFound();
            }
            return View(orderAssignment);
        }

        // POST: OrderAssignments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            OrderAssignment orderAssignment = db.orderAssignments.Find(id);
            db.orderAssignments.Remove(orderAssignment);
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
