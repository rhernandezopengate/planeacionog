using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebAppOpenGate.Models.Planeacion;

namespace WebAppOpenGate.Controllers
{
    public class familiaskusController : Controller
    {
        private DB_A3F19C_PruebasEntities db = new DB_A3F19C_PruebasEntities();

        // GET: familiaskus
        public ActionResult Index()
        {
            return View(db.familiasku.ToList());
        }

        // GET: familiaskus/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            familiasku familiasku = db.familiasku.Find(id);
            if (familiasku == null)
            {
                return HttpNotFound();
            }
            return View(familiasku);
        }

        // GET: familiaskus/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: familiaskus/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,bloque,descripcion")] familiasku familiasku)
        {
            if (ModelState.IsValid)
            {
                db.familiasku.Add(familiasku);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(familiasku);
        }

        // GET: familiaskus/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            familiasku familiasku = db.familiasku.Find(id);
            if (familiasku == null)
            {
                return HttpNotFound();
            }
            return View(familiasku);
        }

        // POST: familiaskus/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,bloque,descripcion")] familiasku familiasku)
        {
            if (ModelState.IsValid)
            {
                db.Entry(familiasku).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(familiasku);
        }

        // GET: familiaskus/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            familiasku familiasku = db.familiasku.Find(id);
            if (familiasku == null)
            {
                return HttpNotFound();
            }
            return View(familiasku);
        }

        // POST: familiaskus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            familiasku familiasku = db.familiasku.Find(id);
            db.familiasku.Remove(familiasku);
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
