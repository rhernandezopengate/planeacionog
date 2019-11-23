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
using WebAppOpenGate.ViewModels;

namespace WebAppOpenGate.Controllers
{
    public class masterarticulosController : Controller
    {
        private DB_A3F19C_PruebasEntities db = new DB_A3F19C_PruebasEntities();

        // GET: masterarticulos
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ObtenerMaster()
        {
            var Draw = Request.Form.GetValues("draw").FirstOrDefault();
            var Start = Request.Form.GetValues("start").FirstOrDefault();
            var Length = Request.Form.GetValues("length").FirstOrDefault();
            var SortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][data]").FirstOrDefault();
            var SortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();

            var item = Request.Form.GetValues("columns[0][search][value]").FirstOrDefault();
            var familia = Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();

            int PageSize = Length != null ? Convert.ToInt32(Length) : 0;
            int Skip = Start != null ? Convert.ToInt32(Start) : 0;
            int TotalRecords = 0;

            try
            {
                List<masterarticulos> listFacturas = new List<masterarticulos>();

                using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["BDConnection"].ToString()))
                {
                    con.Open();

                    string sql = "exec SP_MasterArticulos_ParametrosOpcionales @sku, @familia";
                    var query = new SqlCommand(sql, con);

                    if (item != "")
                    {
                        query.Parameters.AddWithValue("@sku", item);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@sku", DBNull.Value);
                    }

                    if (familia != "" && familia != "0")
                    {
                        query.Parameters.AddWithValue("@familia", familia);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@familia", DBNull.Value);
                    }

                    using (var dr = query.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            // master
                            var master = new MasterArticulosViewModel();

                            master.id = Convert.ToInt32(dr["id"]);
                            master.sku = dr["sku"].ToString();
                            master.descripcion = dr["Nombre"].ToString();
                            master.familia = dr["Familia"].ToString();
                            master.qtycaja = int.Parse(dr["qtycaja"].ToString());
                            master.qtypallet = int.Parse(dr["qtypallet"].ToString());
                            master.multiplosurtido = int.Parse(dr["multiplosurtido"].ToString());
                            master.cajaspallet = int.Parse(dr["cajaspallet"].ToString());
                            master.kgcaja = decimal.Parse(dr["kgcaja"].ToString());
                            master.pesopallet = decimal.Parse(dr["pesopallet"].ToString());
                            master.activo = bool.Parse(dr["activo"].ToString());

                            listFacturas.Add(master);
                        }
                    }
                }

                if (!(string.IsNullOrEmpty(SortColumn) && string.IsNullOrEmpty(SortColumnDir)))
                {
                    listFacturas = listFacturas.OrderBy(SortColumn + " " + SortColumnDir).ToList();
                }

                TotalRecords = listFacturas.ToList().Count();
                var NewItems = listFacturas.Skip(Skip).Take(PageSize == -1 ? TotalRecords : PageSize).ToList();

                return Json(new { draw = Draw, recordsFiltered = TotalRecords, recordsTotal = TotalRecords, data = NewItems }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception _ex)
            {
                Console.WriteLine(_ex.Message.ToString());
                return null;
            }
        }

        [HttpPost]
        public JsonResult ListaFamilias()
        {
            List<SelectListItem> lista = new List<SelectListItem>();

            foreach (var item in db.familiasku.GroupBy(x => x.descripcion).ToList())
            {
                lista.Add(new SelectListItem
                {
                    Value = item.Key,
                    Text = item.Key
                });
            }

            return Json(lista);
        }

        // GET: masterarticulos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            masterarticulos masterarticulos = db.masterarticulos.Find(id);
            if (masterarticulos == null)
            {
                return HttpNotFound();
            }
            return View(masterarticulos);
        }

        // GET: masterarticulos/Create
        public ActionResult Create()
        {
            ViewBag.FamiliaSKU_Id = new SelectList(db.familiasku, "id", "descripcion");
            return View();
        }

        // POST: masterarticulos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,sku,qtycaja,qtypallet,multiplosurtido,cajaspallet,kgcaja,pesopallet,descripcion,activo,FamiliaSKU_Id")] masterarticulos masterarticulos)
        {
            if (ModelState.IsValid)
            {
                db.masterarticulos.Add(masterarticulos);
                db.SaveChanges();
                return Json(new { Respuesta = "Correcto" }, JsonRequestBehavior.AllowGet);
            }

            ViewBag.FamiliaSKU_Id = new SelectList(db.familiasku, "id", "descripcion");

            return View(masterarticulos);
        }

        // GET: masterarticulos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            masterarticulos masterarticulos = db.masterarticulos.Find(id);

            var familias = db.familiasku.ToList();

            List<familiasku> lista = new List<familiasku>();

            foreach (var item in familias)
            {
                familiasku familiasku = new familiasku();

                familiasku.id = item.id;
                familiasku.descripcion = item.bloque + " - " + item.posicion + " - " + item.descripcion;                

                lista.Add(familiasku);
            }

            ViewBag.FamiliaSKU_Id = new SelectList(lista, "id", "descripcion", masterarticulos.FamiliaSKU_Id);

            if (masterarticulos == null)
            {
                return HttpNotFound();
            }
            return View(masterarticulos);
        }

        // POST: masterarticulos/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,sku,qtycaja,qtypallet,multiplosurtido,cajaspallet,kgcaja,pesopallet,descripcion,activo,FamiliaSKU_Id")] masterarticulos masterarticulos)
        {
            if (ModelState.IsValid)
            {
                db.Entry(masterarticulos).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { Respuesta = "Correcto" }, JsonRequestBehavior.AllowGet);
            }

            ViewBag.FamiliaSKU_Id = new SelectList(db.familiasku, "id", "descripcion", masterarticulos.FamiliaSKU_Id);

            return View(masterarticulos);
        }

        // GET: masterarticulos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            masterarticulos masterarticulos = db.masterarticulos.Find(id);
            if (masterarticulos == null)
            {
                return HttpNotFound();
            }
            return View(masterarticulos);
        }

        // POST: masterarticulos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            masterarticulos masterarticulos = db.masterarticulos.Find(id);
            db.masterarticulos.Remove(masterarticulos);
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
