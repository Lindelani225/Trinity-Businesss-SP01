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
    public class RatesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Rates
        public ActionResult Index()
        {
            var rates = db.rates.Include(r => r.Client).Include(r => r.order);
            return View(rates.ToList());
        }

        // GET: Rates/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rate rate = db.rates.Find(id);
            if (rate == null)
            {
                return HttpNotFound();
            }
            return View(rate);
        }

        // GET: Rates/Create
        public ActionResult Create(int? id)
        {

            if(id != null)
            {
                var order = db.orders.Where(x => x.OrderID == id).FirstOrDefault();
                Rate obj = new Rate();
                obj.OrderID = order.OrderID;
                obj.Email = order.Email;
                return View(obj);

            }

            else
            {
                return View();
            }
        }

        // POST: Rates/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( Rate rate)
        {
            if (ModelState.IsValid)
            {
                rate.RateDate = DateTime.Now;
                rate.Email = User.Identity.Name;
                db.rates.Add(rate);
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Email = new SelectList(db.clientProfiles, "Email", "FirstName", rate.Email);
            ViewBag.OrderID = new SelectList(db.orders, "OrderID", "PaymentMethod", rate.OrderID);
            return View(rate);
        }

        // GET: Rates/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rate rate = db.rates.Find(id);
            if (rate == null)
            {
                return HttpNotFound();
            }
            ViewBag.Email = new SelectList(db.clientProfiles, "Email", "FirstName", rate.Email);
            ViewBag.OrderID = new SelectList(db.orders, "OrderID", "PaymentMethod", rate.OrderID);
            return View(rate);
        }

        // POST: Rates/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "RateId,Stars,Comment,service,RateDate,Email,OrderID")] Rate rate)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rate).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Email = new SelectList(db.clientProfiles, "Email", "FirstName", rate.Email);
            ViewBag.OrderID = new SelectList(db.orders, "OrderID", "PaymentMethod", rate.OrderID);
            return View(rate);
        }

        // GET: Rates/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rate rate = db.rates.Find(id);
            if (rate == null)
            {
                return HttpNotFound();
            }
            return View(rate);
        }

        // POST: Rates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Rate rate = db.rates.Find(id);
            db.rates.Remove(rate);
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
