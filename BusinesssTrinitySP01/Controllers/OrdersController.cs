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
    public class OrdersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Orders
        public ActionResult Index()
        {
            var orders = db.orders.Include(o => o.ClientProfile).Include(o => o.ShippimgDetails);
            return View(orders.ToList());
        }

        // GET: Orders/Details/5
        public ActionResult Details(int id)
        {
            Order order = db.orders.Find(id);
           
            if(order != null)
            {
                OrderView orderDetails = new OrderView()
                {
                    order = order,
                    client = db.clientProfiles.Where(x => x.Email == order.Email).FirstOrDefault(),
                    shipment = db.ShippimgDetails.Where(x => x.SID == order.SID).Include(X => X.ClientAddress).FirstOrDefault(),
                    orderItems = db.OrderItems.Where(x => x.OrderID == id).ToList()

                };

                return View(orderDetails);
            }

            return View();
         
        }

        // GET: Orders/Create
        public ActionResult Create()
        {
            ViewBag.Email = new SelectList(db.clientProfiles, "Email", "FirstName");
            ViewBag.SID = new SelectList(db.ShippimgDetails, "SID", "NameofEvent");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OrderID,DateCreated,PaymentMethod,PaymentStatus,ProcessStatus,ConfirmationStatus,Email,SID")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.orders.Add(order);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Email = new SelectList(db.clientProfiles, "Email", "FirstName", order.Email);
            ViewBag.SID = new SelectList(db.ShippimgDetails, "SID", "NameofEvent", order.SID);
            return View(order);
        }

        // GET: Orders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            ViewBag.Email = new SelectList(db.clientProfiles, "Email", "FirstName", order.Email);
            ViewBag.SID = new SelectList(db.ShippimgDetails, "SID", "NameofEvent", order.SID);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OrderID,DateCreated,PaymentMethod,PaymentStatus,ProcessStatus,ConfirmationStatus,Email,SID")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Email = new SelectList(db.clientProfiles, "Email", "FirstName", order.Email);
            ViewBag.SID = new SelectList(db.ShippimgDetails, "SID", "NameofEvent", order.SID);
            return View(order);
        }

        // GET: Orders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Order order = db.orders.Find(id);
            db.orders.Remove(order);
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
