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
    public class wharehousesController : Controller
    {
        private DB_A3F19C_PruebasEntities db = new DB_A3F19C_PruebasEntities();

        // GET: wharehouses
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ObtenerWharehouse()
        {
            var Draw = Request.Form.GetValues("draw").FirstOrDefault();
            var Start = Request.Form.GetValues("start").FirstOrDefault();
            var Length = Request.Form.GetValues("length").FirstOrDefault();
            var SortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][data]").FirstOrDefault();
            var SortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();

            var wh = Request.Form.GetValues("columns[0][search][value]").FirstOrDefault();

            int PageSize = Length != null ? Convert.ToInt32(Length) : 0;
            int Skip = Start != null ? Convert.ToInt32(Start) : 0;
            int TotalRecords = 0;

            try
            {
                List<wharehouse> listaResultado = new List<wharehouse>();

                using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["BDConnection"].ToString()))
                {
                    con.Open();

                    string sql = "exec [SP_Wharehouse_ParametrosOpcionales] @wh";
                    var query = new SqlCommand(sql, con);

                    if (wh != "")
                    {
                        query.Parameters.AddWithValue("@wh", wh);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@wh", DBNull.Value);
                    }

                    using (var dr = query.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            // master
                            var whTemp = new wharehouse();

                            whTemp.id = Convert.ToInt32(dr["id"]);
                            whTemp.nomenclatura = dr["nomenclatura"].ToString();
                            whTemp.descripcion = dr["descripcion"].ToString();

                            listaResultado.Add(whTemp);
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
                return Json(new { Respuesta = "Correcto" }, JsonRequestBehavior.AllowGet);
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
                return Json(new { Respuesta = "Correcto" }, JsonRequestBehavior.AllowGet);
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
