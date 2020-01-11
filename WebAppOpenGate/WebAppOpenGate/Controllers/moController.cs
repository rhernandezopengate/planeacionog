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
using WebAppOpenGate.ViewModels;

namespace WebAppOpenGate.Controllers
{
    public class moController : Controller
    {

        DB_A3F19C_PruebasEntities db = new DB_A3F19C_PruebasEntities();

        // GET: mo
        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public JsonResult ListaStatus()
        {
            List<SelectListItem> lista = new List<SelectListItem>();

            SelectListItem item1 = new SelectListItem();
            item1.Text = "Allocated";
            item1.Value = "Allocated";

            lista.Add(item1);

            SelectListItem item2 = new SelectListItem();
            item2.Text = "Unallocated";
            item2.Value = "Unallocated";

            lista.Add(item2);

            return Json(lista);
        }

        public ActionResult ObtenerMoveOrders ()
        {
            var Draw = Request.Form.GetValues("draw").FirstOrDefault();
            var Start = Request.Form.GetValues("start").FirstOrDefault();
            var Length = Request.Form.GetValues("length").FirstOrDefault();
            var SortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][data]").FirstOrDefault();
            var SortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();

            var item = Request.Form.GetValues("columns[0][search][value]").FirstOrDefault();
            var reference = Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();
            var status = Request.Form.GetValues("columns[2][search][value]").FirstOrDefault();

            int PageSize = Length != null ? Convert.ToInt32(Length) : 0;
            int Skip = Start != null ? Convert.ToInt32(Start) : 0;
            int TotalRecords = 0;

            try
            {
                List<MoveOrderViewModel> lista = new List<MoveOrderViewModel>();
                var wh = db.wharehouse.ToList();

                using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["BDConnection"].ToString()))
                {
                    con.Open();

                    string sql = "exec [SP_MoOpen_ParametrosOpcionales] @item, @reference, @estado";
                    var query = new SqlCommand(sql, con);

                    if (item != "")
                    {
                        query.Parameters.AddWithValue("@item", item);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@item", DBNull.Value);
                    }

                    if (reference != string.Empty && reference != "0")
                    {
                        query.Parameters.AddWithValue("@reference", reference);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@reference", DBNull.Value);
                    }

                    if (status.Equals("Allocated"))
                    {
                        query.Parameters.AddWithValue("@estado", status);
                    }
                    else if (status.Equals("Unallocated"))
                    {
                        query.Parameters.AddWithValue("@estado", status);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@estado", DBNull.Value);
                    }

                    using (var dr = query.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            // master
                            var moopen = new MoveOrderViewModel();

                            moopen.id = Convert.ToInt32(dr["id"]);
                            moopen.Number = dr["Number"].ToString();
                            moopen.item = dr["item"].ToString();
                            moopen.transaction_qty = Convert.ToInt32(dr["transaction_qty"]);
                            moopen.requested_qty = Convert.ToInt32(dr["requested_qty"]);
                            moopen.allocated_qty = Convert.ToInt32(dr["allocated_qty"]);
                            moopen.status_date = DateTime.Parse(dr["status_date"].ToString());
                            moopen.created_by = dr["created_by"].ToString();

                            string referencia = dr["reference"].ToString();
                            var wharehouse = wh.Where(x => x.nomenclatura.Contains(referencia)).FirstOrDefault();

                            moopen.reference = referencia;
                            moopen.Clave = dr["item"].ToString() + "/" + referencia;
                            moopen.Status = dr["Estado"].ToString();


                            lista.Add(moopen);
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

                    dt.Columns.AddRange(new DataColumn[46]
                    {
                        //0 
                        new DataColumn("FechaAlta", typeof(DateTime)),
                        //1
                        new DataColumn("Allocation", typeof(string)),
                        //2
                        new DataColumn("Number", typeof(string)),
                        //3
                        new DataColumn("type", typeof(string)),
                        //4 --
                        new DataColumn("line", typeof(int)),
                        //5
                        new DataColumn("transaction_type", typeof(string)),
                        //6
                        new DataColumn("item", typeof(string)),
                        //7 --
                        new DataColumn("rev", typeof(int)),
                        //8
                        new DataColumn("source_subinv", typeof(string)),
                        //9
                        new DataColumn("source_locator", typeof(string)),
                        //10
                        new DataColumn("destination_subinv", typeof(string)),
                        //11
                        new DataColumn("destination_locator", typeof(string)),
                        //12
                        new DataColumn("destination_account", typeof(string)),
                        //13
                        new DataColumn("lot_number", typeof(string)),
                        //14 --
                        new DataColumn("expiration_date", typeof(DateTime)),
                        //15 
                        new DataColumn("serial_from", typeof(string)),               
                        //16
                        new DataColumn("serial_to", typeof(string)),
                        //17  --
                        new DataColumn("unit_number", typeof(int)),
                        //18 
                        new DataColumn("uom", typeof(string)),
                        //19 --
                        new DataColumn("transaction_qty", typeof(int)),
                        //20 --
                        new DataColumn("requested_qty", typeof(int)),
                        //21 --
                        new DataColumn("required_qty", typeof(int)),
                        //22 --
                        new DataColumn("delivered_qty", typeof(int)),
                        //23 --
                        new DataColumn("allocated_qty", typeof(int)),
                        //24
                        new DataColumn("secondary_uom", typeof(string)),
                        //25 --
                        new DataColumn("secondary_qty", typeof(decimal)),
                        //26 --
                        new DataColumn("secondary_requested_qty", typeof(int)),
                        //27 --
                        new DataColumn("secondary_required_qty", typeof(int)),
                        //28 --
                        new DataColumn("secondary_delivered_qty", typeof(int)),
                        //29 --
                        new DataColumn("secondary_allocated_qty", typeof(int)),
                        //30 
                        new DataColumn("grade", typeof(string)),
                        //31 --
                        new DataColumn("daterequired", typeof(DateTime)),
                        //32 
                        new DataColumn("reason", typeof(string)),
                        //33 
                        new DataColumn("reference", typeof(string)),
                        //34 
                        new DataColumn("reference_number", typeof(string)),
                        //35 
                        new DataColumn("reference_type", typeof(string)),
                        //36 
                        new DataColumn("sales_order", typeof(string)),
                        //37 
                        new DataColumn("so_line", typeof(string)),
                        //38 
                        new DataColumn("ship_set", typeof(string)),
                        //39 
                        new DataColumn("project", typeof(string)),
                        //40 
                        new DataColumn("task", typeof(string)),
                        //41 
                        new DataColumn("line_status", typeof(string)),
                        //42 --
                        new DataColumn("status_date", typeof(DateTime)),
                        //43 
                        new DataColumn("created_by", typeof(string)),
                        //44 
                        new DataColumn("ship_to_location", typeof(string)),  
                        //45
                        new DataColumn("ultimo", typeof(string)),
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
                                    if (dt.Rows[dt.Rows.Count - 1][4].ToString() == "")
                                    {
                                        dt.Rows[dt.Rows.Count - 1][4] = 0;
                                    }

                                    if (dt.Rows[dt.Rows.Count - 1][7].ToString() == "")
                                    {
                                        dt.Rows[dt.Rows.Count - 1][7] = 0;
                                    }

                                    if (dt.Rows[dt.Rows.Count - 1][14].ToString() == "")
                                    {
                                        dt.Rows[dt.Rows.Count - 1][14] = DateTime.Parse("01/01/1900");
                                    }

                                    if (dt.Rows[dt.Rows.Count - 1][17].ToString() == "")
                                    {
                                        dt.Rows[dt.Rows.Count - 1][17] = 0;
                                    }

                                    if (dt.Rows[dt.Rows.Count - 1][19].ToString() == "")
                                    {
                                        dt.Rows[dt.Rows.Count - 1][19] = 0;
                                    }

                                    if (dt.Rows[dt.Rows.Count - 1][20].ToString() == "")
                                    {
                                        dt.Rows[dt.Rows.Count - 1][20] = 0;
                                    }

                                    if (dt.Rows[dt.Rows.Count - 1][21].ToString() == "")
                                    {
                                        dt.Rows[dt.Rows.Count - 1][21] = 0;
                                    }

                                    if (dt.Rows[dt.Rows.Count - 1][22].ToString() == "")
                                    {
                                        dt.Rows[dt.Rows.Count - 1][22] = 0;
                                    }

                                    if (dt.Rows[dt.Rows.Count - 1][23].ToString() == "")
                                    {
                                        dt.Rows[dt.Rows.Count - 1][23] = 0;
                                    }

                                    if (dt.Rows[dt.Rows.Count - 1][25].ToString() == "")
                                    {
                                        dt.Rows[dt.Rows.Count - 1][25] = 0;
                                    }

                                    if (dt.Rows[dt.Rows.Count - 1][26].ToString() == "")
                                    {
                                        dt.Rows[dt.Rows.Count - 1][26] = 0;
                                    }

                                    if (dt.Rows[dt.Rows.Count - 1][27].ToString() == "")
                                    {
                                        dt.Rows[dt.Rows.Count - 1][27] = 0;
                                    }

                                    if (dt.Rows[dt.Rows.Count - 1][28].ToString() == "")
                                    {
                                        dt.Rows[dt.Rows.Count - 1][28] = 0;
                                    }

                                    if (dt.Rows[dt.Rows.Count - 1][29].ToString() == "")
                                    {
                                        dt.Rows[dt.Rows.Count - 1][29] = 0;
                                    }

                                    if (dt.Rows[dt.Rows.Count - 1][31].ToString() == "")
                                    {
                                        dt.Rows[dt.Rows.Count - 1][31] = DateTime.Parse("01/01/1900");
                                    }

                                    if (dt.Rows[dt.Rows.Count - 1][42].ToString() == "")
                                    {
                                        dt.Rows[dt.Rows.Count - 1][42] = DateTime.Parse("01/01/1900");
                                    }
                                }
                                else
                                {
                                    if (i == 14)
                                    {
                                        dt.Rows[dt.Rows.Count - 1][14] = DateTime.Parse(cell);
                                    }
                                    else if (i == 31)
                                    {
                                        dt.Rows[dt.Rows.Count - 1][31] = DateTime.Parse(cell);
                                    }
                                    else if (i == 42)
                                    {
                                        dt.Rows[dt.Rows.Count - 1][42] = DateTime.Parse(cell);
                                    }
                                    else
                                    {
                                        dt.Rows[dt.Rows.Count - 1][i] = cell;
                                    }
                                }
                            }
                        }
                    }

                    string connectionString = @"Data Source=sql7005.site4now.net;Initial Catalog=DB_A3F19C_Pruebas;User Id=DB_A3F19C_Pruebas_admin;Password=xQ9znAhU;";

                    using (SqlConnection cn = new SqlConnection(connectionString))
                    {
                        cn.Open();
                        string query = "TRUNCATE TABLE dbo.moopen";
                        SqlCommand cmd = new SqlCommand(query, cn);
                        cmd.ExecuteNonQuery();
                    }

                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connectionString))
                    {
                        bulkCopy.DestinationTableName = "dbo.moopen";

                        bulkCopy.ColumnMappings.Add("FechaAlta", "FechaAlta");
                        bulkCopy.ColumnMappings.Add("Allocation", "Allocation");
                        bulkCopy.ColumnMappings.Add("Number", "Number");
                        bulkCopy.ColumnMappings.Add("type", "type");
                        bulkCopy.ColumnMappings.Add("line", "line");
                        bulkCopy.ColumnMappings.Add("transaction_type", "transaction_type");
                        bulkCopy.ColumnMappings.Add("item", "item");
                        bulkCopy.ColumnMappings.Add("rev", "rev");
                        bulkCopy.ColumnMappings.Add("source_subinv", "source_subinv");
                        bulkCopy.ColumnMappings.Add("source_locator", "source_locator");
                        bulkCopy.ColumnMappings.Add("destination_subinv", "destination_subinv");
                        bulkCopy.ColumnMappings.Add("destination_locator", "destination_locator");
                        bulkCopy.ColumnMappings.Add("destination_account", "destination_account");
                        bulkCopy.ColumnMappings.Add("lot_number", "lot_number");
                        bulkCopy.ColumnMappings.Add("expiration_date", "expiration_date");
                        bulkCopy.ColumnMappings.Add("serial_from", "serial_from");
                        bulkCopy.ColumnMappings.Add("serial_to", "serial_to");
                        bulkCopy.ColumnMappings.Add("unit_number", "unit_number");
                        bulkCopy.ColumnMappings.Add("uom", "uom");
                        bulkCopy.ColumnMappings.Add("transaction_qty", "transaction_qty");
                        bulkCopy.ColumnMappings.Add("requested_qty", "requested_qty");
                        bulkCopy.ColumnMappings.Add("required_qty", "required_qty");
                        bulkCopy.ColumnMappings.Add("delivered_qty", "delivered_qty");
                        bulkCopy.ColumnMappings.Add("allocated_qty", "allocated_qty");
                        bulkCopy.ColumnMappings.Add("secondary_uom", "secondary_uom");
                        bulkCopy.ColumnMappings.Add("secondary_qty", "secondary_qty");
                        bulkCopy.ColumnMappings.Add("secondary_requested_qty", "secondary_requested_qty");
                        bulkCopy.ColumnMappings.Add("secondary_required_qty", "secondary_required_qty");
                        bulkCopy.ColumnMappings.Add("secondary_delivered_qty", "secondary_delivered_qty");
                        bulkCopy.ColumnMappings.Add("secondary_allocated_qty", "secondary_allocated_qty");
                        bulkCopy.ColumnMappings.Add("grade", "grade");
                        bulkCopy.ColumnMappings.Add("daterequired", "daterequired");
                        bulkCopy.ColumnMappings.Add("reason", "reason");
                        bulkCopy.ColumnMappings.Add("reference", "reference");
                        bulkCopy.ColumnMappings.Add("reference_number", "reference_number");
                        bulkCopy.ColumnMappings.Add("reference_type", "reference_type");
                        bulkCopy.ColumnMappings.Add("sales_order", "sales_order");
                        bulkCopy.ColumnMappings.Add("so_line", "so_line");
                        bulkCopy.ColumnMappings.Add("ship_set", "ship_set");
                        bulkCopy.ColumnMappings.Add("project", "project");
                        bulkCopy.ColumnMappings.Add("task", "task");
                        bulkCopy.ColumnMappings.Add("line_status", "line_status");
                        bulkCopy.ColumnMappings.Add("status_date", "status_date");
                        bulkCopy.ColumnMappings.Add("created_by", "created_by");
                        bulkCopy.ColumnMappings.Add("ship_to_location", "ship_to_location");

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