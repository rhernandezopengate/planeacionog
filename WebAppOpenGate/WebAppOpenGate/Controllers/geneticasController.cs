﻿using System;
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
using WebAppOpenGate.Filters;
using System.IO;

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

        public ActionResult ActualizarByArchivo(string error)
        {
            ViewBag.Error = error;
            return View();
        }

        public ActionResult EditarByArchivo(HttpPostedFileBase archivoPosteado)
        {
            try
            {               
                string filePath = string.Empty;

                if (archivoPosteado != null)
                {
                    string path = Server.MapPath("~/Uploads/");

                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    filePath = path + Path.GetFileName(archivoPosteado.FileName);
                    string extension = Path.GetExtension(archivoPosteado.FileName);
                    archivoPosteado.SaveAs(filePath);

                    DataTable dt = new DataTable();

                    dt.Columns.AddRange(new DataColumn[8]
                    {
                        //0
                        new DataColumn("clave", typeof(string)),
                        //1
                        new DataColumn("sku", typeof(string)),
                        //2
                        new DataColumn("wh", typeof(string)),
                        //3
                        new DataColumn("promedio", typeof(decimal)),
                        //4                       
                        new DataColumn("geneticafinal", typeof(string)),                       
                        //5
                        new DataColumn("minimo", typeof(decimal)),                       
                        //6
                        new DataColumn("maximo", typeof(decimal)),
                        //6
                        new DataColumn("organizacion", typeof(string)),
                    });

                    string csvData = System.IO.File.ReadAllText(filePath);                    

                    foreach (string row in csvData.Split('\n'))
                    {
                        if (!string.IsNullOrEmpty(row))
                        {
                            dt.Rows.Add();
                            int i = 0;

                            foreach (string cell in row.Split(','))
                            {
                                if (cell.Equals(string.Empty))
                                {
                                    if (dt.Rows[dt.Rows.Count - 1][3].ToString() == "")
                                    {
                                        dt.Rows[dt.Rows.Count - 1][3] = 0;
                                    }

                                    if (dt.Rows[dt.Rows.Count - 1][5].ToString() == "")
                                    {
                                        dt.Rows[dt.Rows.Count - 1][5] = 0;
                                    }
                                    
                                    if (dt.Rows[dt.Rows.Count - 1][6].ToString() == "")
                                    {
                                        dt.Rows[dt.Rows.Count - 1][6] = 0;
                                    }
                                }
                                else
                                {                                    
                                    dt.Rows[dt.Rows.Count - 1][i] = cell;
                                }

                                i++;
                            }
                        }
                    }

                    string connectionString = @"Data Source=sql7005.site4now.net;Initial Catalog=DB_A3F19C_Pruebas;User Id=DB_A3F19C_Pruebas_admin;Password=xQ9znAhU;";

                    using (SqlConnection cn = new SqlConnection(connectionString))
                    {
                        cn.Open();
                        string query = "TRUNCATE TABLE geneticas";
                        SqlCommand cmd = new SqlCommand(query, cn);
                        cmd.ExecuteNonQuery();
                    }
                     
                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connectionString))
                    {
                        bulkCopy.DestinationTableName = "dbo.geneticas";

                        bulkCopy.ColumnMappings.Add("clave", "clave");
                        bulkCopy.ColumnMappings.Add("sku", "sku");
                        bulkCopy.ColumnMappings.Add("wh", "wh");
                        bulkCopy.ColumnMappings.Add("promedio", "promedio");
                        bulkCopy.ColumnMappings.Add("geneticafinal", "geneticafinal");
                        bulkCopy.ColumnMappings.Add("minimo", "minimo");
                        bulkCopy.ColumnMappings.Add("maximo", "maximo");
                        bulkCopy.ColumnMappings.Add("organizacion", "organizacion");

                        try
                        {  
                            bulkCopy.WriteToServer(dt);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            return RedirectToAction("Cargar", new { error = "Error" });
                        }
                    }
                }

                return RedirectToAction("ActualizarByArchivo", new { error = "Success" });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                return RedirectToAction("ActualizarByArchivo", new { error = "Error" });
            }
        }

        public ActionResult CambiarGeneticasBySKU() 
        {
            return View();   
        }

        [HttpPost]
        public ActionResult CambiarGeneticasBySKU(string sku, decimal minimo, decimal maximo)
        {
            try
            {
                var geneticas = db.geneticas.Where(x => x.sku.Equals(sku)).ToList();

                if (geneticas != null)
                {
                    foreach (var item in geneticas)
                    {
                        geneticas geneticasTemp = db.geneticas.Where(x => x.clave == item.clave).FirstOrDefault();

                        geneticasTemp.maximo = maximo;
                        geneticasTemp.minimo = minimo;
                    }

                    db.SaveChanges();

                    return Json(new { Respuesta = "Correcto" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Respuesta = "SKU No Existe" }, JsonRequestBehavior.AllowGet);
                }                
            }
            catch (Exception _ex)
            {
                return Json(new { Respuesta = _ex.Message.ToString() }, JsonRequestBehavior.AllowGet);
            }
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
            var wh = Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();

            int PageSize = Length != null ? Convert.ToInt32(Length) : 0;
            int Skip = Start != null ? Convert.ToInt32(Start) : 0;
            int TotalRecords = 0;

            try
            {
                List<geneticas> lista = new List<geneticas>();

                using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["BDConnection"].ToString()))
                {
                    con.Open();

                    string sql = "exec [SP_Geneticas_ParametrosOpcionales] @sku, @wh";
                    var query = new SqlCommand(sql, con);

                    if (item != "")
                    {
                        query.Parameters.AddWithValue("@sku", item);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@sku", DBNull.Value);
                    }

                    if (wh != "" && wh != "0")
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
                            var genetica = new geneticas();

                            genetica.id = Convert.ToInt32(dr["id"]);
                            string clave = dr["clave"].ToString();
                            genetica.clave = clave;
                            genetica.wh = dr["clave"].ToString().Remove(0, clave.IndexOf("/") + 1);
                            genetica.sku = dr["sku"].ToString();
                            genetica.promedio = decimal.Parse(dr["promedio"].ToString());
                            genetica.geneticafinal = dr["geneticafinal"].ToString();
                            genetica.minimo = decimal.Parse(dr["minimo"].ToString());
                            genetica.maximo = decimal.Parse(dr["maximo"].ToString());
                            genetica.organizacion = dr["organizacion"].ToString();
                            
                            lista.Add(genetica);
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
                return Json(new { Respuesta = "Correcto" }, JsonRequestBehavior.AllowGet);
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
                return Json(new { Respuesta = "Correcto" }, JsonRequestBehavior.AllowGet);
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
