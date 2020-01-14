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
    public class asignacionwhintransitsController : Controller
    {
        private DB_A3F19C_PruebasEntities db = new DB_A3F19C_PruebasEntities();

        // GET: asignacionwhintransits
        public ActionResult Index()
        {
            return View(db.asignacionwhintransit.ToList());
        }

        // GET: asignacionwhintransits/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            asignacionwhintransit asignacionwhintransit = db.asignacionwhintransit.Find(id);
            if (asignacionwhintransit == null)
            {
                return HttpNotFound();
            }
            return View(asignacionwhintransit);
        }

        [HttpPost]
        public ActionResult AsignacionWarehouse(string warehouse, string ronumber)
        {
            try
            {
                var roduplicada = db.asignacionwhintransit.Where(x => x.requisition.Equals(ronumber)).FirstOrDefault();

                if (roduplicada == null)
                {
                    asignacionwhintransit asignacion = new asignacionwhintransit();
                    asignacion.wh = warehouse;
                    asignacion.requisition = ronumber;
                    asignacion.fechaalta = DateTime.Now;
                    db.asignacionwhintransit.Add(asignacion);
                    db.SaveChanges();

                    return Json(new { Respuesta = "Correcto" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Respuesta = "Registro Duplicado" }, JsonRequestBehavior.AllowGet);
                }                
            }
            catch (Exception _ex)
            {
                return Json(new { Respuesta = _ex.Message.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: asignacionwhintransits/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: asignacionwhintransits/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,fechaalta,requisition,wh")] asignacionwhintransit asignacionwhintransit)
        {
            if (ModelState.IsValid)
            {
                db.asignacionwhintransit.Add(asignacionwhintransit);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(asignacionwhintransit);
        }

        // GET: asignacionwhintransits/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            asignacionwhintransit asignacionwhintransit = db.asignacionwhintransit.Find(id);
            if (asignacionwhintransit == null)
            {
                return HttpNotFound();
            }
            return View(asignacionwhintransit);
        }

        // POST: asignacionwhintransits/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,fechaalta,requisition,wh")] asignacionwhintransit asignacionwhintransit)
        {
            if (ModelState.IsValid)
            {
                db.Entry(asignacionwhintransit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(asignacionwhintransit);
        }

        // GET: asignacionwhintransits/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            asignacionwhintransit asignacionwhintransit = db.asignacionwhintransit.Find(id);
            if (asignacionwhintransit == null)
            {
                return HttpNotFound();
            }
            return View(asignacionwhintransit);
        }

        // POST: asignacionwhintransits/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            asignacionwhintransit asignacionwhintransit = db.asignacionwhintransit.Find(id);
            db.asignacionwhintransit.Remove(asignacionwhintransit);
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
