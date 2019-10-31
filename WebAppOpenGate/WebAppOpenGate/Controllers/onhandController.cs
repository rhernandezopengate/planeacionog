using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebAppOpenGate.Models.Planeacion;
using System.Linq.Dynamic;

namespace WebAppOpenGate.Controllers
{
    public class onhandController : Controller
    {
        // GET: onhand
        public ActionResult Index()
        {
            return View();
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
                List<onhand> lista = new List<onhand>();

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
                            var onhand = new onhand();

                            onhand.id = Convert.ToInt32(dr["id"]);
                            onhand.item = dr["item"].ToString();
                            onhand.description = dr["description"].ToString();
                            onhand.subinv = dr["subinv"].ToString();
                            onhand.on_hand_qty = int.Parse(dr["on_hand_qty"].ToString());
                            onhand.available_qty = int.Parse(dr["available_qty"].ToString());
                            onhand.reserved_qty = int.Parse(dr["reserved_qty"].ToString());
                            onhand.lot_number = dr["lot_number"].ToString();
                            onhand.locator = dr["locator"].ToString();
                            onhand.rev = int.Parse(dr["rev"].ToString());

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