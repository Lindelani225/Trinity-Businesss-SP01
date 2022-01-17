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
    public class CompColorsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: CompColors
        public ActionResult Index()
        {
            var compColors = db.compColors.Include(c => c.Color).Include(c => c.DesignComp);
            return View(compColors.ToList());
        }

        // GET: CompColors/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompColor compColor = db.compColors.Find(id);
            if (compColor == null)
            {
                return HttpNotFound();
            }
            return View(compColor);
        }

        public ActionResult AddColor(int id)
        {
            Session["compId"] = id;
            var colors = db.colors.ToList();
            ViewBag.Qty = db.designComps.Where(x => x.CompId == id).Select(x => x.Qty).Single();
            ViewBag.Count = colors.Count();
            return View(colors);
        }

        [HttpPost]
        public ActionResult AddColor(List<Color> colors)
        {
            int id = Convert.ToInt32(Session["compId"]);
            foreach (var c in colors)
            {
                if (c.qty > 0)
                {
                    CompColor newColor = new CompColor()
                    {
                        BoxId = c.BoxId,
                        CompId = id,
                        Qty = c.qty
                    };
                    db.compColors.Add(newColor);
                    db.SaveChanges();
                }

            }
            return RedirectToAction("Index", "DesignComps");

        }
        // GET: CompColors/Create
        public ActionResult Create(int id)
        {

            CompColor obj = new CompColor();
            obj.CompId = id;
            obj.colors = db.colors.ToList();
            return View(obj);
        }

        // POST: CompColors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CompColor compColor)
        {
            if (ModelState.IsValid)
            {
                foreach (var color in compColor.colors)
                {
                    if (color.qty > 0)
                    {
                        CompColor newColor = new CompColor()
                        {
                            BoxId = color.BoxId,
                            CompId = compColor.CompId,
                            Qty = color.qty
                        };
                        db.compColors.Add(newColor);
                        db.SaveChanges();
                    }
                }
                return RedirectToAction("Index");
            }

            ViewBag.BoxId = new SelectList(db.colors, "BoxId", "Name", compColor.BoxId);
            ViewBag.CompId = new SelectList(db.designComps, "CompId", "Name", compColor.CompId);
            return View(compColor);
        }

        // GET: CompColors/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompColor compColor = db.compColors.Find(id);
            if (compColor == null)
            {
                return HttpNotFound();
            }
            ViewBag.BoxId = new SelectList(db.colors, "BoxId", "Name", compColor.BoxId);
            ViewBag.CompId = new SelectList(db.designComps, "CompId", "Name", compColor.CompId);
            return View(compColor);
        }

        // POST: CompColors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ColorId,CompId,BoxId,Qty")] CompColor compColor)
        {
            if (ModelState.IsValid)
            {
                db.Entry(compColor).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BoxId = new SelectList(db.colors, "BoxId", "Name", compColor.BoxId);
            ViewBag.CompId = new SelectList(db.designComps, "CompId", "Name", compColor.CompId);
            return View(compColor);
        }

        // GET: CompColors/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompColor compColor = db.compColors.Find(id);
            if (compColor == null)
            {
                return HttpNotFound();
            }
            return View(compColor);
        }

        // POST: CompColors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CompColor compColor = db.compColors.Find(id);
            db.compColors.Remove(compColor);
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
