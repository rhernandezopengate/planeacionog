using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebAppOpenGate.Models.Planeacion;
using System.Linq.Dynamic;

namespace WebAppOpenGate.Controllers
{
    public class familiaskusController : Controller
    {
        private DB_A3F19C_PruebasEntities db = new DB_A3F19C_PruebasEntities();

        // GET: familiaskus
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ObtenerFamilias()
        {
            var Draw = Request.Form.GetValues("draw").FirstOrDefault();
            var Start = Request.Form.GetValues("start").FirstOrDefault();
            var Length = Request.Form.GetValues("length").FirstOrDefault();
            var SortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][data]").FirstOrDefault();
            var SortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();

            var descripcion = Request.Form.GetValues("columns[0][search][value]").FirstOrDefault();

            int PageSize = Length != null ? Convert.ToInt32(Length) : 0;
            int Skip = Start != null ? Convert.ToInt32(Start) : 0;
            int TotalRecords = 0;

            try
            {
                List<familiasku> listaResultado = new List<familiasku>();

                using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["BDConnection"].ToString()))
                {
                    con.Open();

                    string sql = "exec [SP_FamiliasSKU_ParametrosOpcionales] @descripcion";
                    var query = new SqlCommand(sql, con);

                    if (descripcion != "")
                    {
                        query.Parameters.AddWithValue("@descripcion", descripcion);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@descripcion", DBNull.Value);
                    }

                    using (var dr = query.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            // master
                            var familiaTemp = new familiasku();

                            familiaTemp.id = Convert.ToInt32(dr["id"]);
                            familiaTemp.bloque = int.Parse(dr["bloque"].ToString());
                            familiaTemp.posicion = int.Parse(dr["posicion"].ToString());
                            familiaTemp.descripcion = dr["descripcion"].ToString();

                            listaResultado.Add(familiaTemp);
                        }
                    }
                }

                if (!(string.IsNullOrEmpty(SortColumn) && string.IsNullOrEmpty(SortColumnDir)))
                {
                    listaResultado = listaResultado.OrderBy(SortColumn + " " + SortColumnDir).ToList();
                }

                TotalRecords = listaResultado.ToList().Count();
                var NewItems = listaResultado.Skip(Skip).Take(PageSize == -1 ? TotalRecords : PageSize).ToList();

                return Json(new { draw = Draw, recordsFiltered = TotalRecords, recordsTotal = TotalRecords, data = NewItems }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception _ex)
            {
                Console.WriteLine(_ex.Message.ToString());
                return null;
            }
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
        public ActionResult Create([Bind(Include = "id,bloque,descripcion,posicion")] familiasku familiasku)
        {
            if (ModelState.IsValid)
            {
                db.familiasku.Add(familiasku);
                db.SaveChanges();
                return Json(new { Respuesta = "Correcto" }, JsonRequestBehavior.AllowGet);
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
        public ActionResult Edit([Bind(Include = "id,bloque,descripcion,posicion")] familiasku familiasku)
        {
            if (ModelState.IsValid)
            {
                db.Entry(familiasku).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { Respuesta = "Correcto" }, JsonRequestBehavior.AllowGet);
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
