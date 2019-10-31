using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebAppOpenGate.Models.Planeacion;
using System.Linq.Dynamic;
using System.Data.SqlClient;
using System.Configuration;

namespace WebAppOpenGate.Controllers
{
    public class geneticasController : Controller
    {
        private DB_A3F19C_PruebasEntities db = new DB_A3F19C_PruebasEntities();

        // GET: geneticas
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ObtenerGeneticas()
        {
            var Draw = Request.Form.GetValues("draw").FirstOrDefault();
            var Start = Request.Form.GetValues("start").FirstOrDefault();
            var Length = Request.Form.GetValues("length").FirstOrDefault();
            var SortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][data]").FirstOrDefault();
            var SortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();

            var item = Request.Form.GetValues("columns[0][search][value]").FirstOrDefault();

            int PageSize = Length != null ? Convert.ToInt32(Length) : 0;
            int Skip = Start != null ? Convert.ToInt32(Start) : 0;
            int TotalRecords = 0;

            try
            {
                List<geneticas> lista = new List<geneticas>();

                using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {
                    con.Open();

                    string sql = "exec [SP_Geneticas_ParametrosOpcionales] @sku";
                    var query = new SqlCommand(sql, con);

                    if (item != "")
                    {
                        query.Parameters.AddWithValue("@sku", item);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@sku", DBNull.Value);
                    }

                    using (var dr = query.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            // master
                            var master = new geneticas();

                            master.id = Convert.ToInt32(dr["id"]);
                            master.clave = dr["clave"].ToString();
                            master.sku = dr["sku"].ToString();
                            master.promedio = decimal.Parse(dr["promedio"].ToString());
                            master.geneticafinal = dr["geneticafinal"].ToString();
                            master.minimo = decimal.Parse(dr["minimo"].ToString());
                            master.maximo = decimal.Parse(dr["maximo"].ToString());

                            lista.Add(master);
                        }
                    }
                }

                if (!(string.IsNullOrEmpty(SortColumn) && string.IsNullOrEmpty(SortColumnDir)))
                {
                    lista = lista.OrderBy(SortColumn + " " + SortColumnDir).ToList();
                }

                TotalRecords = lista.ToList().Count();
                var NewItems = lista.Skip(Skip).Take(PageSize == -1 ? TotalRecords : PageSize).ToList();

                return Json(new { draw = Draw, recordsFiltered = TotalRecords, recordsTotal = TotalRecords, data = NewItems }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception _ex)
            {
                Console.WriteLine(_ex.Message.ToString());
                return null;
            }
        }

        // GET: geneticas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            geneticas geneticas = db.geneticas.Find(id);
            if (geneticas == null)
            {
                return HttpNotFound();
            }
            return View(geneticas);
        }

        // GET: geneticas/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: geneticas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,clave,sku,wh,promedio,geneticafinal,minimo,maximo")] geneticas geneticas)
        {
            if (ModelState.IsValid)
            {
                db.geneticas.Add(geneticas);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(geneticas);
        }

        // GET: geneticas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            geneticas geneticas = db.geneticas.Find(id);
            if (geneticas == null)
            {
                return HttpNotFound();
            }
            return View(geneticas);
        }

        // POST: geneticas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,clave,sku,wh,promedio,geneticafinal,minimo,maximo")] geneticas geneticas)
        {
            if (ModelState.IsValid)
            {
                db.Entry(geneticas).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(geneticas);
        }

        // GET: geneticas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            geneticas geneticas = db.geneticas.Find(id);
            if (geneticas == null)
            {
                return HttpNotFound();
            }
            return View(geneticas);
        }

        // POST: geneticas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            geneticas geneticas = db.geneticas.Find(id);
            db.geneticas.Remove(geneticas);
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
