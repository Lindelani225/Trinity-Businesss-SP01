using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BusinesssTrinitySP01.Models;
using BusinesssTrinitySP01.Logic;

namespace BusinesssTrinitySP01.Controllers
{
    public class ThemesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Themes
        public ActionResult Index()
        {
            return View(db.themes.ToList());
        }

        // GET: Themes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Theme theme = db.themes.Find(id);
            if (theme == null)
            {
                return HttpNotFound();
            }
            return View(theme);
        }

        // GET: Themes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Themes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ThemeId,Name,Description,Image")] Theme theme)
        {
            if (ModelState.IsValid)
            {
                HttpPostedFileBase file = Request.Files["ThemeImage"];
                UploadImage service = new UploadImage();
                theme.Image = service.ConvertToBytes(file);
                db.themes.Add(theme);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(theme);
        }

        // GET: Themes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Theme theme = db.themes.Find(id);
            if (theme == null)
            {
                return HttpNotFound();
            }
            return View(theme);
        }

        // POST: Themes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ThemeId,Name,Description,Image")] Theme theme)
        {
            if (ModelState.IsValid)
            {
                db.Entry(theme).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(theme);
        }

        // GET: Themes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Theme theme = db.themes.Find(id);
            if (theme == null)
            {
                return HttpNotFound();
            }
            return View(theme);
        }

        // POST: Themes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Theme theme = db.themes.Find(id);
            db.themes.Remove(theme);
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
