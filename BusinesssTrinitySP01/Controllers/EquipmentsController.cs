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
    public class EquipmentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private Equipment_Logic ib = new Equipment_Logic();
        Category_Logic cb = new Category_Logic();
        public string shoppingCartID { get; set; }
        public const string CartSessionKey = "CartId";
        public EquipmentsController() { }
        public ActionResult Index()
        {
            return View(db.Equipment.Include(x => x.Category).ToList());
        }

        // GET: Equipments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Equipment equipment = db.Equipment.Find(id);
            if (equipment == null)
            {
                return HttpNotFound();
            }
            return View(equipment);
        }



        /// <summary>
        /// Retrive Image from database 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult RetrieveImage(int id)
        {
            byte[] cover = GetImageFromDataBase(id);
            if (cover != null)
            {
                return File(cover, "image/jpg");
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public byte[] GetImageFromDataBase(int Id)
        {
            var q = from temp in db.Equipment where temp.EquipmentID == Id select temp.Image1;
            byte[] cover = q.First();
            return cover;
        }




        // GET: Equipments/Create
        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName");
            return View();
        }

        // POST: Equipments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Equipment equipment, HttpPostedFileBase img_upload)
        {
            ViewBag.CategoryID = new SelectList(cb.all(), "CategoryID", "CategoryName");
            HttpPostedFileBase file = Request.Files["ImageData"];
            HttpPostedFileBase file2 = Request.Files["ImageData1"];
            HttpPostedFileBase file3 = Request.Files["ImageData2"];
            UploadImage service = new UploadImage();
            int i = service.UploadImageInDataBase(file, file2, file3, equipment);
            if (i == 1)
            {
                return RedirectToAction("Index");
            }


            return View(equipment);
        }

        // GET: Equipments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Equipment equipment = db.Equipment.Find(id);
            if (equipment == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", equipment.CategoryID);
            return View(equipment);
        }

        // POST: Equipments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Equipment equipment, HttpPostedFileBase img_upload)
        {

            byte[] data = null;
            data = new byte[img_upload.ContentLength];
            img_upload.InputStream.Read(data, 0, img_upload.ContentLength);
            equipment.Image1 = data;

            if (ModelState.IsValid)
            {
                db.Entry(equipment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryID = new SelectList(cb.all(), "CategoryID", "CategoryName", equipment.CategoryID);
            return View(equipment);
        }

        // GET: Equipments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Equipment equipment = db.Equipment.Find(id);
            if (equipment == null)
            {
                return HttpNotFound();
            }
            return View(equipment);
        }

        // POST: Equipments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Equipment equipment = db.Equipment.Find(id);
            db.Equipment.Remove(equipment);
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


        public ActionResult Fall_catalog()
        {
            return View(ib.all());
        }

        public string GetCartID()
        {
            if (System.Web.HttpContext.Current.Session[CartSessionKey] == null)
            {
                if (!String.IsNullOrWhiteSpace(System.Web.HttpContext.Current.User.Identity.Name))
                {
                    System.Web.HttpContext.Current.Session[CartSessionKey] = System.Web.HttpContext.Current.User.Identity.Name;
                }
                else
                {
                    Guid temp_cart_ID = Guid.NewGuid();
                    System.Web.HttpContext.Current.Session[CartSessionKey] = temp_cart_ID.ToString();
                }
            }
            return System.Web.HttpContext.Current.Session[CartSessionKey].ToString();
        }
    }
}
