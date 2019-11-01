using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebAppOpenGate.Models.Permisos;

namespace WebAppOpenGate.Controllers
{
    public class roloperacionsController : Controller
    {
        private DB_A3F19C_PruebasEntities1 db = new DB_A3F19C_PruebasEntities1();

        // GET: roloperacions
        public ActionResult Index()
        {
            var roloperacion = db.roloperacion.Include(r => r.operaciones).Include(r => r.rol);
            return View(roloperacion.ToList());
        }

        // GET: roloperacions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            roloperacion roloperacion = db.roloperacion.Find(id);
            if (roloperacion == null)
            {
                return HttpNotFound();
            }
            return View(roloperacion);
        }

        // GET: roloperacions/Create
        public ActionResult Create()
        {
            ViewBag.operaciones_Id = new SelectList(db.operaciones, "id", "Nombre");
            ViewBag.rol_Id = new SelectList(db.rol, "id", "nombre");
            return View();
        }

        // POST: roloperacions/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,operaciones_Id,rol_Id")] roloperacion roloperacion)
        {
            if (ModelState.IsValid)
            {
                db.roloperacion.Add(roloperacion);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.operaciones_Id = new SelectList(db.operaciones, "id", "Nombre", roloperacion.operaciones_Id);
            ViewBag.rol_Id = new SelectList(db.rol, "id", "nombre", roloperacion.rol_Id);
            return View(roloperacion);
        }

        // GET: roloperacions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            roloperacion roloperacion = db.roloperacion.Find(id);
            if (roloperacion == null)
            {
                return HttpNotFound();
            }
            ViewBag.operaciones_Id = new SelectList(db.operaciones, "id", "Nombre", roloperacion.operaciones_Id);
            ViewBag.rol_Id = new SelectList(db.rol, "id", "nombre", roloperacion.rol_Id);
            return View(roloperacion);
        }

        // POST: roloperacions/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,operaciones_Id,rol_Id")] roloperacion roloperacion)
        {
            if (ModelState.IsValid)
            {
                db.Entry(roloperacion).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.operaciones_Id = new SelectList(db.operaciones, "id", "Nombre", roloperacion.operaciones_Id);
            ViewBag.rol_Id = new SelectList(db.rol, "id", "nombre", roloperacion.rol_Id);
            return View(roloperacion);
        }

        // GET: roloperacions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            roloperacion roloperacion = db.roloperacion.Find(id);
            if (roloperacion == null)
            {
                return HttpNotFound();
            }
            return View(roloperacion);
        }

        // POST: roloperacions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            roloperacion roloperacion = db.roloperacion.Find(id);
            db.roloperacion.Remove(roloperacion);
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
