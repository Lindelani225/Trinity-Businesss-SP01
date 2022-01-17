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
    public class ShipmentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Shipments
        public ActionResult Index()
        {
            var shippimgDetails = db.ShippimgDetails.Include(s => s.ClientAddress);
            return View(shippimgDetails.ToList());
        }

        // GET: Shipments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Shipment shipment = db.ShippimgDetails.Find(id);
            if (shipment == null)
            {
                return HttpNotFound();
            }
            return View(shipment);
        }

        // GET: Shipments/Create
        public ActionResult Create(int id)
        {
            Shipment obj = new Shipment();
            obj.AdID = id;
            return View(obj);
        }

        // POST: Shipments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Shipment shipment)
        {
            if (shipment.CheckStartDate() == true)
            {
                if (ModelState.IsValid)
                {
                    if(shipment.Rentalperiod <= 0)
                    {
                        ViewBag.period = "Rental period cannot be zero or less.";
                        return View(shipment);

                    }
                    shipment.EndDate = shipment.End();
                    shipment.DeliveryCost = shipment.CalcDeliveryCost();
                    shipment.DeliveryDate = shipment.EventDate.AddDays(-1);
                    shipment.ReturnDate = shipment.EndDate.AddDays(1);
                    db.ShippimgDetails.Add(shipment);
                    db.SaveChanges();
                    return RedirectToAction("PlaceOrder", "Home", new { id = shipment.SID });
                }
            }


            ViewBag.EventDate = "Your event date must be at least two days after today's date";
            return View(shipment);
        }

        // GET: Shipments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Shipment shipment = db.ShippimgDetails.Find(id);
            if (shipment == null)
            {
                return HttpNotFound();
            }
            ViewBag.AdID = new SelectList(db.cAddresses, "AdID", "Streetno", shipment.AdID);
            return View(shipment);
        }

        // POST: Shipments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SID,NameofEvent,EventDate,Rentalperiod,EndDate,DeliveryType,DeliveryCost,AdID")] Shipment shipment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(shipment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AdID = new SelectList(db.cAddresses, "AdID", "Streetno", shipment.AdID);
            return View(shipment);
        }

        // GET: Shipments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Shipment shipment = db.ShippimgDetails.Find(id);
            if (shipment == null)
            {
                return HttpNotFound();
            }
            return View(shipment);
        }

        // POST: Shipments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Shipment shipment = db.ShippimgDetails.Find(id);
            db.ShippimgDetails.Remove(shipment);
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
