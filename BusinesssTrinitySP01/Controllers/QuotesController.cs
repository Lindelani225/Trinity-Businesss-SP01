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
    public class QuotesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Quotes
        public ActionResult Index()
        {
            var quotes = db.quotes.Include(q => q.Request).Include(q => q.Supplier);
            return View(quotes.ToList());
        }

        // GET: Quotes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Quote quote = db.quotes.Find(id);
            if (quote == null)
            {
                return HttpNotFound();
            }
            return View(quote);
        }

        // GET: Quotes/Create
        public ActionResult Create()
        {
            ViewBag.RequestID = new SelectList(db.requests, "RequestID", "Type");
            ViewBag.SuppID = new SelectList(db.suppliers, "SuppID", "SuppName");
            return View();
        }

        // POST: Quotes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "QuoteID,SuppID,RequestID,DateCreated,Duration,ShipmentType,Acceptance,Status,QRCode")] Quote quote)
        {
            if (ModelState.IsValid)
            {
                db.quotes.Add(quote);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.RequestID = new SelectList(db.requests, "RequestID", "Type", quote.RequestID);
            ViewBag.SuppID = new SelectList(db.suppliers, "SuppID", "SuppName", quote.SuppID);
            return View(quote);
        }

        // GET: Quotes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Quote quote = db.quotes.Find(id);
            if (quote == null)
            {
                return HttpNotFound();
            }
            ViewBag.RequestID = new SelectList(db.requests, "RequestID", "Type", quote.RequestID);
            ViewBag.SuppID = new SelectList(db.suppliers, "SuppID", "SuppName", quote.SuppID);
            return View(quote);
        }

        // POST: Quotes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "QuoteID,SuppID,RequestID,DateCreated,Duration,ShipmentType,Acceptance,Status,QRCode")] Quote quote)
        {
            if (ModelState.IsValid)
            {
                db.Entry(quote).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.RequestID = new SelectList(db.requests, "RequestID", "Type", quote.RequestID);
            ViewBag.SuppID = new SelectList(db.suppliers, "SuppID", "SuppName", quote.SuppID);
            return View(quote);
        }

        // GET: Quotes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Quote quote = db.quotes.Find(id);
            if (quote == null)
            {
                return HttpNotFound();
            }
            return View(quote);
        }

        // POST: Quotes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Quote quote = db.quotes.Find(id);
            db.quotes.Remove(quote);
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
