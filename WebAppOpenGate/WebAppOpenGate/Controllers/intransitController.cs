using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebAppOpenGate.Models.Planeacion;
using System.Linq.Dynamic;

namespace WebAppOpenGate.Controllers
{
    public class intransitController : Controller
    {
        DB_A3F19C_PruebasEntities db = new DB_A3F19C_PruebasEntities();
        // GET: intransit
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ObtenerInTransit()
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
                List<hl_transit> lista = new List<hl_transit>();

                using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["BDConnection"].ToString()))
                {
                    con.Open();

                    string sql = "exec [SP_InTransit_ParametrosOpcionales] @item";
                    var query = new SqlCommand(sql, con);

                    if (item != "")
                    {
                        query.Parameters.AddWithValue("@item", item);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@item", DBNull.Value);
                    }                    

                    using (var dr = query.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            // master
                            var intransit = new hl_transit();

                            intransit.id = Convert.ToInt32(dr["id"]);
                            intransit.item_num = dr["item_num"].ToString();
                            intransit.item_description = dr["item_description"].ToString();
                            intransit.lot_number = dr["lot_number"].ToString();

                            lista.Add(intransit);
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

        public ActionResult Cargar(string error)
        {
            ViewBag.Error = error;
            return View();
        }

        [HttpPost]
        public ActionResult Importar(HttpPostedFileBase archivoPosteado)
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
                    List<hl_transit> listaTemp = new List<hl_transit>();

                    dt.Columns.AddRange(new DataColumn[20]
                    {
                        //0
                        new DataColumn("fechaalta", typeof(DateTime)),
                        //1
                        new DataColumn("receiving_org", typeof(string)),
                        //2
                        new DataColumn("shipping_org", typeof(string)),
                        //3
                        new DataColumn("item_num", typeof(string)),
                        //4
                        new DataColumn("revision", typeof(int)),
                        //5
                        new DataColumn("item_description", typeof(string)),
                        //6
                        new DataColumn("requisition", typeof(string)),
                        //7
                        new DataColumn("order_num", typeof(string)),
                        //8
                        new DataColumn("line_num", typeof(int)),
                        //9
                        new DataColumn("shipment_num", typeof(string)),
                        //10
                        new DataColumn("ship_date", typeof(DateTime)),
                        //11
                        new DataColumn("eta_date", typeof(DateTime)),
                        //12
                        new DataColumn("orderlineqty_intransit", typeof(int)),
                        //13
                        new DataColumn("qty_from_lot", typeof(int)),
                        //14
                        new DataColumn("lot_number", typeof(string)),
                        //15
                        new DataColumn("exp_date", typeof(DateTime)),
                        //16
                        new DataColumn("lot_status", typeof(string)),
                        //17
                        new DataColumn("carrier", typeof(string)),
                        //18
                        new DataColumn("way_bill", typeof(string)),
                        //19
                        new DataColumn("vendor", typeof(string)),
                    });

                    string csvData = System.IO.File.ReadAllText(filePath);

                    DateTime fechaAlta = DateTime.Now;

                    foreach (string row in csvData.Split('\n'))
                    {
                        if (!string.IsNullOrEmpty(row))
                        {
                            dt.Rows.Add();
                            int i = 0;

                            foreach (string cell in row.Split(','))
                            {
                                dt.Rows[dt.Rows.Count - 1][0] = DateTime.Now;

                                i++;

                                if (cell.Equals(string.Empty))
                                {    
                                    if (dt.Rows[dt.Rows.Count - 1][15].ToString() == "")
                                    {
                                        dt.Rows[dt.Rows.Count - 1][15] = DateTime.MinValue;
                                    }                                 
                                }
                                else
                                {
                                    dt.Rows[dt.Rows.Count - 1][i] = cell;
                                }
                            }
                        }
                    }

                    string connectionString = @"Data Source=sql7005.site4now.net;Initial Catalog=DB_A3F19C_Pruebas;User Id=DB_A3F19C_Pruebas_admin;Password=xQ9znAhU;";

                    using (SqlConnection cn = new SqlConnection(connectionString))
                    {
                        cn.Open();
                        string query = "TRUNCATE TABLE hl_transit";
                        SqlCommand cmd = new SqlCommand(query, cn);
                        cmd.ExecuteNonQuery();
                    }

                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connectionString))
                    {
                        bulkCopy.DestinationTableName = "dbo.hl_transit";

                        bulkCopy.ColumnMappings.Add("fechaalta", "fechaalta");
                        bulkCopy.ColumnMappings.Add("receiving_org", "receiving_org");
                        bulkCopy.ColumnMappings.Add("shipping_org", "shipping_org");
                        bulkCopy.ColumnMappings.Add("item_num", "item_num");
                        bulkCopy.ColumnMappings.Add("revision", "revision");
                        bulkCopy.ColumnMappings.Add("item_description", "item_description");
                        bulkCopy.ColumnMappings.Add("requisition", "requisition");
                        bulkCopy.ColumnMappings.Add("order_num", "order_num");
                        bulkCopy.ColumnMappings.Add("line_num", "line_num");
                        bulkCopy.ColumnMappings.Add("shipment_num", "shipment_num");
                        bulkCopy.ColumnMappings.Add("ship_date", "ship_date");
                        bulkCopy.ColumnMappings.Add("eta_date", "eta_date");
                        bulkCopy.ColumnMappings.Add("orderlineqty_intransit", "orderlineqty_intransit");
                        bulkCopy.ColumnMappings.Add("qty_from_lot", "qty_from_lot");
                        bulkCopy.ColumnMappings.Add("lot_number", "lot_number");
                        bulkCopy.ColumnMappings.Add("exp_date", "exp_date");
                        bulkCopy.ColumnMappings.Add("lot_status", "lot_status");
                        bulkCopy.ColumnMappings.Add("carrier", "carrier");
                        bulkCopy.ColumnMappings.Add("way_bill", "way_bill");
                        bulkCopy.ColumnMappings.Add("vendor", "vendor");

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
                return RedirectToAction("Cargar", new { error = "Success" });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                return RedirectToAction("Cargar", new { error = "Error" });
            }
        }
    }
}