using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebAppOpenGate.Models.Planeacion;
using WebAppOpenGate.ViewModels;

namespace WebAppOpenGate.Controllers
{
    public class planeacionController : Controller
    {
        DB_A3F19C_PruebasEntities db = new DB_A3F19C_PruebasEntities();
        // GET: planeacion
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CrearPlaneacion()
        {
            return View();
        }

        public ActionResult ConsultaSkus(string wh)
        {
            ViewBag.WH = wh;
            
            try
            {
                List<PlaneacionViewModel> lista = new List<PlaneacionViewModel>();                
                var skus = db.masterarticulos.Where(x => x.activo == true).ToList();
                var onhand = db.onhand.Where(x => x.subinv.Contains(wh + "10") || x.subinv.Contains(wh + "20")).ToList();
                var geneticas = db.geneticas.Where(x => x.clave.Contains(wh)).ToList();
                var master = db.masterarticulos.ToList();                

                foreach (var item in skus)
                {
                    PlaneacionViewModel pl = new PlaneacionViewModel();
                    string clave = item.sku + "/" + wh;

                    var valgeneticas = geneticas.Where(x => x.clave == clave).FirstOrDefault();

                    if (valgeneticas != null)
                    {
                        pl.sku = item.sku;
                        pl.bloque = (int)master.Where(x => x.sku == item.sku).FirstOrDefault().familiasku.bloque;
                        pl.multiplosurtido = master.Where(x => x.sku == item.sku).FirstOrDefault().multiplosurtido;
                        pl.wh = wh;
                        pl.qtyonhand = onhand.Where(x => x.item.Equals(item.sku)).Select(x => x.on_hand_qty - x.reserved_qty).Sum();
                        lista.Add(pl);
                    }                                        
                }

                return PartialView("_ConsultaSkus", lista);
            }
            catch (Exception _ex)
            {
                Console.WriteLine(_ex.Message);
                throw;
            }            
        }        

        [HttpPost]
        public JsonResult ListaWH()
        {
            List<SelectListItem> lista = new List<SelectListItem>();

            foreach (var item in db.wharehouse.ToList())
            {
                lista.Add(new SelectListItem
                {
                    Value = item.nomenclatura,
                    Text = item.nomenclatura
                });
            }

            return Json(lista);
        }
    }
}