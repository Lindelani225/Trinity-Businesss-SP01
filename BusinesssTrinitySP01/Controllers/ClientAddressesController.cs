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
    [Authorize]
    public class ClientAddressesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ClientAddresses
        public ActionResult Index()
        {
            var cAddresses = db.cAddresses.Include(c => c.ClientProfile);
            return View(cAddresses.ToList());
        }

        // GET: ClientAddresses/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClientAddress clientAddress = db.cAddresses.Find(id);
            if (clientAddress == null)
            {
                return HttpNotFound();
            }
            return View(clientAddress);
        }

        // GET: ClientAddresses/Create
        public ActionResult Create()
        {
            var address = db.cAddresses.Where(x => x.Email == User.Identity.Name).OrderByDescending(x => x.AdID).FirstOrDefault();

            ClientAddress clientAddress = new ClientAddress();

            if (address != null)
            {
                clientAddress = db.cAddresses.Where(x => x.Email == address.Email).OrderByDescending(x => x.AdID).FirstOrDefault();
            }
            //ViewBag.Email = new SelectList(db.clientProfiles, "Email", "FirstName");

            return View(clientAddress);
        }

        // POST: ClientAddresses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AdID,Streetno,Suburb,City,Province,PostalCode,Email")] ClientAddress clientAddress)
        {
            if (ModelState.IsValid)
            {
                clientAddress.Email = User.Identity.Name;
                db.cAddresses.Add(clientAddress);
                db.SaveChanges();
                return RedirectToAction("Create", "Shipments", new {id = clientAddress.AdID });
            }

            ViewBag.Email = new SelectList(db.clientProfiles, "Email", "FirstName", clientAddress.Email);
            return View(clientAddress);
        }

        // GET: ClientAddresses/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClientAddress clientAddress = db.cAddresses.Find(id);
            if (clientAddress == null)
            {
                return HttpNotFound();
            }
            ViewBag.Email = new SelectList(db.clientProfiles, "Email", "FirstName", clientAddress.Email);
            return View(clientAddress);
        }

        // POST: ClientAddresses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AdID,Streetno,Suburb,City,Province,PostalCode,Email")] ClientAddress clientAddress)
        {
            if (ModelState.IsValid)
            {
                db.Entry(clientAddress).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Email = new SelectList(db.clientProfiles, "Email", "FirstName", clientAddress.Email);
            return View(clientAddress);
        }

        // GET: ClientAddresses/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClientAddress clientAddress = db.cAddresses.Find(id);
            if (clientAddress == null)
            {
                return HttpNotFound();
            }
            return View(clientAddress);
        }

        // POST: ClientAddresses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ClientAddress clientAddress = db.cAddresses.Find(id);
            db.cAddresses.Remove(clientAddress);
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
