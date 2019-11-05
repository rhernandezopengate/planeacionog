using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebAppOpenGate.Models.Planeacion;
using System.Linq.Dynamic;
using WebAppOpenGate.ViewModels;
using System.IO;
using System.Data;

namespace WebAppOpenGate.Controllers
{
    public class onhandController : Controller
    {
        DB_A3F19C_PruebasEntities db = new DB_A3F19C_PruebasEntities();
        // GET: onhand
        public ActionResult Index()
        {
            return View();
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

                    dt.Columns.AddRange(new DataColumn[31]
                    {
                        //0
                        new DataColumn("fechaalta", typeof(DateTime)),
                        //1
                        new DataColumn("org", typeof(string)),
                        //2
                        new DataColumn("item", typeof(string)),
                        //3
                        new DataColumn("rev", typeof(int)),
                        //4
                        new DataColumn("description", typeof(string)),
                        //5
                        new DataColumn("category", typeof(string)),
                        //6
                        new DataColumn("planner_code", typeof(string)),
                        //7
                        new DataColumn("country_of_origin", typeof(string)),
                        //8
                        new DataColumn("item_status", typeof(string)),
                        //9
                        new DataColumn("uom", typeof(string)),
                        //10
                        new DataColumn("subinv", typeof(string)),
                        //11
                        new DataColumn("locator", typeof(string)),
                        //12
                        new DataColumn("locator_status", typeof(string)),
                        //13
                        new DataColumn("lot_number", typeof(string)),
                        //14
                        new DataColumn("lot_status", typeof(string)),
                        //15 -- 
                        new DataColumn("expiration_date", typeof(DateTime)),
                        //16
                        new DataColumn("lpn", typeof(string)),
                        //17 -- 
                        new DataColumn("frozen_unit_cost", typeof(decimal)),
                        //18 --
                        new DataColumn("on_hand_qty", typeof(int)),
                        //19 --
                        new DataColumn("on_hand_value", typeof(decimal)),
                        //20 --
                        new DataColumn("reserved_qty", typeof(int)),
                        //21 --
                        new DataColumn("reserved_value", typeof(decimal)),
                        //22 --
                        new DataColumn("available_qty", typeof(int)),
                        //23 --
                        new DataColumn("available_value", typeof(decimal)),
                        //24
                        new DataColumn("curr_cd", typeof(string)),
                        //25 --
                        new DataColumn("unit_weight", typeof(decimal)),
                        //26
                        new DataColumn("weight_uom", typeof(string)),
                        //27 --
                        new DataColumn("case_quantity", typeof(int)),
                        //28 -- 
                        new DataColumn("pallet_size", typeof(int)),
                        //29
                        new DataColumn("hl_itemlc", typeof(string)),
                        //30 --
                        new DataColumn("case_quantity_lot", typeof(int)),
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

                                    if (dt.Rows[dt.Rows.Count - 1][17].ToString() == "")
                                    {
                                        dt.Rows[dt.Rows.Count - 1][17] = 0;
                                    }

                                    if (dt.Rows[dt.Rows.Count - 1][18].ToString() == "")
                                    {
                                        dt.Rows[dt.Rows.Count - 1][18] = 0;
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

                                    if (dt.Rows[dt.Rows.Count - 1][27].ToString() == "")
                                    {
                                        dt.Rows[dt.Rows.Count - 1][27] = 0;
                                    }

                                    if (dt.Rows[dt.Rows.Count - 1][28].ToString() == "")
                                    {
                                        dt.Rows[dt.Rows.Count - 1][28] = 0;
                                    }
                                }
                                else
                                {
                                    if (cell.Equals("\r"))
                                    {
                                        dt.Rows[dt.Rows.Count - 1][30] = 0;
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

                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connectionString))
                    {
                        bulkCopy.DestinationTableName = "dbo.onhand";

                        bulkCopy.ColumnMappings.Add("fechaalta", "fechaalta");
                        bulkCopy.ColumnMappings.Add("org", "org");
                        bulkCopy.ColumnMappings.Add("item", "item");
                        bulkCopy.ColumnMappings.Add("rev", "rev");
                        bulkCopy.ColumnMappings.Add("description", "description");
                        bulkCopy.ColumnMappings.Add("category", "category");
                        bulkCopy.ColumnMappings.Add("planner_code", "planner_code");
                        bulkCopy.ColumnMappings.Add("country_of_origin", "country_of_origin");
                        bulkCopy.ColumnMappings.Add("item_status", "item_status");
                        bulkCopy.ColumnMappings.Add("uom", "uom");
                        bulkCopy.ColumnMappings.Add("subinv", "subinv");
                        bulkCopy.ColumnMappings.Add("locator", "locator");
                        bulkCopy.ColumnMappings.Add("locator_status", "locator_status");
                        bulkCopy.ColumnMappings.Add("lot_number", "lot_number");
                        bulkCopy.ColumnMappings.Add("lot_status", "lot_status");
                        bulkCopy.ColumnMappings.Add("expiration_date", "expiration_date");
                        bulkCopy.ColumnMappings.Add("lpn", "lpn");
                        bulkCopy.ColumnMappings.Add("frozen_unit_cost", "frozen_unit_cost");
                        bulkCopy.ColumnMappings.Add("on_hand_qty", "on_hand_qty");
                        bulkCopy.ColumnMappings.Add("on_hand_value", "on_hand_value");
                        bulkCopy.ColumnMappings.Add("reserved_qty", "reserved_qty");
                        bulkCopy.ColumnMappings.Add("reserved_value", "reserved_value");
                        bulkCopy.ColumnMappings.Add("available_qty", "available_qty");
                        bulkCopy.ColumnMappings.Add("available_value", "available_value");
                        bulkCopy.ColumnMappings.Add("curr_cd", "curr_cd");
                        bulkCopy.ColumnMappings.Add("unit_weight", "unit_weight");
                        bulkCopy.ColumnMappings.Add("weight_uom", "weight_uom");
                        bulkCopy.ColumnMappings.Add("case_quantity", "case_quantity");
                        bulkCopy.ColumnMappings.Add("pallet_size", "pallet_size");
                        bulkCopy.ColumnMappings.Add("hl_itemlc", "hl_itemlc");
                        bulkCopy.ColumnMappings.Add("case_quantity_lot", "case_quantity_lot");

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

        [HttpPost]
        public ActionResult ObtenerOnHand()
        {
            var Draw = Request.Form.GetValues("draw").FirstOrDefault();
            var Start = Request.Form.GetValues("start").FirstOrDefault();
            var Length = Request.Form.GetValues("length").FirstOrDefault();
            var SortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][data]").FirstOrDefault();
            var SortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();

            var sku = Request.Form.GetValues("columns[0][search][value]").FirstOrDefault();
            var rev = Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();
            var subinv = Request.Form.GetValues("columns[2][search][value]").FirstOrDefault();
            var lot = Request.Form.GetValues("columns[3][search][value]").FirstOrDefault();

            int PageSize = Length != null ? Convert.ToInt32(Length) : 0;
            int Skip = Start != null ? Convert.ToInt32(Start) : 0;
            int TotalRecords = 0;

            try
            {
                List<OnHandViewModels> lista = new List<OnHandViewModels>();
                List<masterarticulos> listamaster = db.masterarticulos.ToList();

                using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["BDConnection"].ToString()))
                {
                    con.Open();

                    string sql = "exec [SP_OnHand_ParametrosOpcionales] @sku, @rev, @subinv, @lot";
                    var query = new SqlCommand(sql, con);

                    if (sku != "")
                    {
                        query.Parameters.AddWithValue("@sku", sku);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@sku", DBNull.Value);
                    }

                    if (rev != "")
                    {
                        query.Parameters.AddWithValue("@rev", rev);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@rev", DBNull.Value);
                    }

                    if (subinv != "")
                    {
                        query.Parameters.AddWithValue("@subinv", subinv);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@subinv", DBNull.Value);
                    }

                    if (lot != "")
                    {
                        query.Parameters.AddWithValue("@lot", lot);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@lot", DBNull.Value);
                    }

                    using (var dr = query.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            // master
                            var onhand = new OnHandViewModels();

                            onhand.id = Convert.ToInt32(dr["id"]);
                            string item = dr["item"].ToString();
                            onhand.item = item;
                            onhand.description = dr["description"].ToString();
                            onhand.subinv = dr["subinv"].ToString();                            
                            onhand.on_hand_qty = int.Parse(dr["on_hand_qty"].ToString());
                            
                            onhand.reserved_qty = int.Parse(dr["reserved_qty"].ToString());
                            onhand.lot_number = dr["lot_number"].ToString();
                            onhand.locator = dr["locator"].ToString();
                            onhand.rev = int.Parse(dr["rev"].ToString());

                            int available = int.Parse((dr["on_hand_qty"].ToString())) - int.Parse(dr["reserved_qty"].ToString());
                            onhand.available_qty = available;

                            string val = dr["subinv"].ToString().Remove(0, 4);

                            if (val.Equals("10") || val.Equals("20"))
                            {
                                onhand.wh = dr["subinv"].ToString().Remove(0, 2).Remove(2, 2);
                                onhand.llave = item + "/" + dr["subinv"].ToString().Remove(0, 2).Remove(2, 2);
                            }
                            else
                            {
                                onhand.wh = "";
                                onhand.llave = "";
                            }

                            var itemmaster = listamaster.Where(x => x.sku.Contains(item)).FirstOrDefault();

                            if (itemmaster != null)
                            {                               

                                if (available != 0)
                                {
                                    onhand.qtypallets = (available / double.Parse(listamaster.Where(x => x.sku.Contains(item)).FirstOrDefault().qtypallet.ToString()));
                                }
                                else
                                {
                                    onhand.qtypallets = 0;
                                }
                            }
                            else
                            {
                                onhand.qtypallets = 0;
                            }

                            if (dr["expiration_date"].ToString().Equals(string.Empty))
                            {
                                onhand.expiration_date = DateTime.MinValue;
                            }
                            else
                            {
                                onhand.expiration_date = Convert.ToDateTime(dr["expiration_date"].ToString());
                            }                            

                            lista.Add(onhand);
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
    }
}