using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
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

        public ActionResult CrearPlaneacion(string error)
        {
            ViewBag.Error = error;
            return View();
        }

        public async Task<ActionResult> Cargas(string wh)
        {
            try
            {
                var master = await ObtenerMasterAsync();                
                var onhand = await ObtenerOnHandAsync(wh);
                var onhandcedis = await ObtenerOnHandCedisAsync(wh);
                var onhandcalidad = await ObtenerOnHandCalidadAsync(wh);
                IEnumerable<geneticas> ListaGeneticas = await ObtenerGeneticasAsync(wh);
                IEnumerable<familiasku> ListaFamilias = await ObtenerFamiliasAsync();
                List<PlaneacionViewModel> lista = new List<PlaneacionViewModel>();

                foreach (var item in master)
                {
                    PlaneacionViewModel pl = new PlaneacionViewModel();
                    
                    string clave = item.sku + "/" + wh;

                    var gen = ListaGeneticas.Where(x => x.clave.Equals(clave)).FirstOrDefault();

                    if (gen != null)
                    {
                        pl.sku = item.sku;
                        pl.multiplosurtido = item.multiplosurtido;
                        pl.bloque = ListaFamilias.Where(x => x.id == item.FamiliaSKU_Id).FirstOrDefault().id;
                        pl.qtyonhand = onhand.Where(x => x.item.Equals(item.sku)).Select(x => x.on_hand_qty - x.reserved_qty).Sum();
                        pl.qtycedis = onhandcedis.Where(x => x.item.Equals(item.sku)).Select(x => x.on_hand_qty - x.reserved_qty).Sum();
                        pl.qtycalidad = onhandcalidad.Where(x => x.item.Equals(item.sku)).Select(x => x.on_hand_qty - x.reserved_qty).Sum();

                        lista.Add(pl);
                    }       
                }

                lista = lista.OrderBy(x => x.bloque).ThenBy(x => x.posicion).ToList();
                
                return PartialView("_ConsultaSkus", lista);
            }
            catch (Exception _ex)
            {
                return PartialView("_ConsultaSkus", null);                
            }      
        }


        async Task<IEnumerable<masterarticulos>> ObtenerMasterAsync()
        {
            return await db.masterarticulos.Where(x => x.activo == true).ToListAsync();
        }

        async Task<IEnumerable<geneticas>> ObtenerGeneticasAsync(string wh)
        {            
            // return some scores ...
            return await db.geneticas.Where(x => x.clave.Contains(wh)).ToListAsync();
        }

        async Task<IEnumerable<onhand>> ObtenerOnHandAsync(string wh)
        {
            return await db.onhand.Where(x => x.subinv.Contains(wh + "10") || x.subinv.Contains(wh + "20")).ToListAsync();
        }

        async Task<IEnumerable<onhand>> ObtenerOnHandCedisAsync(string wh)
        {            
            return await db.onhand.Where(x => x.subinv.Contains("M1QE10")).ToListAsync();
        }
        
        async Task<IEnumerable<onhand>> ObtenerOnHandCalidadAsync(string wh)
        {            
            return await db.onhand.Where(x => x.subinv.Contains("M1QE87")).ToListAsync();
        }

        async Task<IEnumerable<familiasku>> ObtenerFamiliasAsync()
        {
            return await db.familiasku.ToListAsync();
        }

        public ActionResult ConsultaSkus(string wh)
        {
            ViewBag.WH = wh;
            
            try
            {
                List<PlaneacionViewModel> lista = new List<PlaneacionViewModel>();                
                var skus = db.masterarticulos.Where(x => x.activo == true).ToList();
                var onhand = db.onhand.Where(x => x.subinv.Contains(wh + "10") || x.subinv.Contains(wh + "20")).ToList();
                var onhandcediscalidad = db.onhand.Where(x => x.subinv.Contains("M1QE10") && x.subinv.Contains("M1QE87")).ToList();                
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
                        pl.posicion = (int)master.Where(x => x.sku == item.sku).FirstOrDefault().familiasku.posicion;                        
                        pl.multiplosurtido = master.Where(x => x.sku == item.sku).FirstOrDefault().multiplosurtido;
                        pl.wh = wh;
                        pl.qtyonhand = onhand.Where(x => x.item.Equals(item.sku)).Select(x => x.on_hand_qty - x.reserved_qty).Sum();
                        pl.qtycedis = onhandcediscalidad.Where(x => x.item.Equals(item.sku) && x.subinv.Contains("M1QE10")).Select(x => x.on_hand_qty - x.reserved_qty).Sum();
                        pl.qtycalidad = onhandcediscalidad.Where(x => x.item.Equals(item.sku) && x.subinv.Contains("M1QE87")).Select(x => x.on_hand_qty - x.reserved_qty).Sum();
                        lista.Add(pl);
                    }                                        
                }

                lista = lista.OrderBy(x => x.bloque).ThenBy(x => x.posicion).ToList();

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