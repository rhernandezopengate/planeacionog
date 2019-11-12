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
    public class wharehousesController : Controller
    {
        private DB_A3F19C_PruebasEntities db = new DB_A3F19C_PruebasEntities();

        // GET: wharehouses
        public ActionResult Index()
        {
            return View(db.wharehouse.ToList());
        }

        // GET: wharehouses/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            wharehouse wharehouse = db.wharehouse.Find(id);
            if (wharehouse == null)
            {
                return HttpNotFound();
            }
            return View(wharehouse);
        }

        // GET: wharehouses/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: wharehouses/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,nomenclatura,descripcion")] wharehouse wharehouse)
        {
            if (ModelState.IsValid)
            {
                db.wharehouse.Add(wharehouse);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(wharehouse);
        }

        // GET: wharehouses/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            wharehouse wharehouse = db.wharehouse.Find(id);
            if (wharehouse == null)
            {
                return HttpNotFound();
            }
            return View(wharehouse);
        }

        // POST: wharehouses/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,nomenclatura,descripcion")] wharehouse wharehouse)
        {
            if (ModelState.IsValid)
            {
                db.Entry(wharehouse).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(wharehouse);
        }

        // GET: wharehouses/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            wharehouse wharehouse = db.wharehouse.Find(id);
            if (wharehouse == null)
            {
                return HttpNotFound();
            }
            return View(wharehouse);
        }

        // POST: wharehouses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            wharehouse wharehouse = db.wharehouse.Find(id);
            db.wharehouse.Remove(wharehouse);
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
