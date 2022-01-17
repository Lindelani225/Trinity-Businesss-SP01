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
    public class RepairItemsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: RepairItems
        public ActionResult Index()
        {
            var repairItems = db.repairItems.Include(r => r.Equipment);
            return View(repairItems.ToList());
        }

        // GET: RepairItems/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RepairItem repairItem = db.repairItems.Find(id);

            var repairItems = db.repairItems.Where(x => x.ItemID == id).Include(x => x.Equipment).FirstOrDefault();

            ViewBag.EName = repairItems.Equipment.Name;

            if (repairItem == null)
            {
                return HttpNotFound();
            }
            return View(repairItem);
        }

        // GET: RepairItems/Create
        public ActionResult Create()
        {
            ViewBag.EquipmentID = new SelectList(db.Equipment, "EquipmentID", "Name");
            return View();
        }

        // POST: RepairItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ItemID,EquipmentID,Description,img,img2,QouteID")] RepairItem repairItem)
        {
            if (ModelState.IsValid)
            {
                db.repairItems.Add(repairItem);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.EquipmentID = new SelectList(db.Equipment, "EquipmentID", "Name", repairItem.EquipmentID);
            return View(repairItem);
        }

        // GET: RepairItems/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RepairItem repairItem = db.repairItems.Find(id);
            if (repairItem == null)
            {
                return HttpNotFound();
            }
            ViewBag.EquipmentID = new SelectList(db.Equipment, "EquipmentID", "Name", repairItem.EquipmentID);
            return View(repairItem);
        }

        // POST: RepairItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ItemID,ItemCode,EquipmentID,Description,img,img2,RequestID")] RepairItem repairItem)
        {
            if (ModelState.IsValid)
            {
                HttpPostedFileBase file = Request.Files["Img1"];
                HttpPostedFileBase file2 = Request.Files["Bimg"];

                UploadImage service = new UploadImage();
                repairItem.img = service.ConvertToBytes(file);
                repairItem.img2 = service.ConvertToBytes(file2);

                db.Entry(repairItem).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EquipmentID = new SelectList(db.Equipment, "EquipmentID", "Name", repairItem.EquipmentID);
            return View(repairItem);
        }

        // GET: RepairItems/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RepairItem repairItem = db.repairItems.Find(id);
            if (repairItem == null)
            {
                return HttpNotFound();
            }
            return View(repairItem);
        }

        // POST: RepairItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            RepairItem repairItem = db.repairItems.Find(id);
            db.repairItems.Remove(repairItem);
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
