using Newtonsoft.Json.Linq;
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


        public ActionResult CrearPlaneacion3(string error)
        {
            ViewBag.Error = error;
            return View();
        }

        public async Task<ActionResult> Cargas3(string wh) 
        {
            List<PlaneacionViewModel> lista = await PlaneacionAsync(wh);
            System.Web.HttpContext.Current.Session["listaGeneral"] = lista;
            return PartialView("Cargas3", lista);
        }        

        public ActionResult CrearPlaneacion(string error)
        {
            ViewBag.Error = error;
            return View();
        }

        public async Task<ActionResult> Cargas(string wh)
        {
            var geneticas = await db.geneticas.ToListAsync();
            
            var onhandcv = await  (from oh in db.onhand 
                                where oh.subinv.Contains(wh + "10") || oh.subinv.Contains(wh + "20")
                                select new { oh.on_hand_qty, oh.reserved_qty, oh.item }).ToListAsync();

            var onhandcedis = await (from oh in db.onhand
                                  where oh.subinv.Contains("M1QE10")
                                  select new { oh.on_hand_qty, oh.reserved_qty, oh.item }).ToListAsync();

            var onhandcalidad = await (from oh in db.onhand
                                     where oh.subinv.Contains("M1QE87")
                                     select new { oh.on_hand_qty, oh.reserved_qty, oh.item }).ToListAsync();

            var onhandmx3 = await (from oh in db.onhand
                                       where oh.subinv.Contains("M3TP10")
                                       select new { oh.on_hand_qty, oh.reserved_qty, oh.item }).ToListAsync();

            var promediogeneticas = await (from gen in db.geneticas
                                           where gen.organizacion.Contains("MX1") || gen.organizacion.Contains("MX4") || gen.organizacion.Contains("MX5")
                                           select new { gen.promedio, gen.sku }).ToListAsync();


            var master = await db.masterarticulos.Where(x => x.activo == true).ToListAsync();            
            var familias = await db.familiasku.ToListAsync();

            List<PlaneacionViewModel> lista = new List<PlaneacionViewModel>();

            foreach (var item in master)
            {
                PlaneacionViewModel pl = new PlaneacionViewModel();

                string clave = item.sku + "/" + wh;

                var gen = geneticas.Where(x => x.clave.Equals(clave)).FirstOrDefault();

                if (gen != null)
                {
                    pl.sku = item.sku;
                    pl.bloque = familias.Where(x => x.id == item.FamiliaSKU_Id).FirstOrDefault().id;
                    pl.multiplosurtido = item.multiplosurtido;
                    pl.clasificacion = gen.geneticafinal.Substring(0, 1);
                    pl.maximo = (decimal)gen.maximo;
                    pl.minimo = (decimal)gen.minimo;
                    pl.qtyonhandcv = onhandcv.Where(x => x.item.Equals(item.sku)).Select(x => x.on_hand_qty - x.reserved_qty).Sum();
                    pl.qtyonhandcedis = onhandcedis.Where(x => x.item.Equals(item.sku)).Select(x => x.on_hand_qty - x.reserved_qty).Sum();
                    pl.qtyonhandcalidad = onhandcalidad.Where(x => x.item.Equals(item.sku)).Select(x => x.on_hand_qty - x.reserved_qty).Sum();
                    pl.qtyonhandmx3 = onhandmx3.Where(x => x.item.Equals(item.sku)).Select(x => x.on_hand_qty - x.reserved_qty).Sum();
                    var consumopromedio = promediogeneticas.Where(x => x.sku.Equals(item.sku)).Select(x => x.promedio).Sum();
                    decimal coverageweeks = (decimal)(pl.qtyonhandcedis / consumopromedio);
                    pl.coverageweeks = Math.Round(coverageweeks, 1);

                    lista.Add(pl);
                }
            }

            lista = lista.OrderBy(x => x.bloque).ThenBy(x => x.posicion).ToList();

            return PartialView("Cargas", lista);
        }

        async Task<List<geneticas>> ObtenerGeneticas()
        {          

            return await db.geneticas.ToListAsync();
        }

        public async Task<ActionResult> Cargas2(string wh)
        {
            List<PlaneacionViewModel> lista = await PlaneacionAsync(wh);
            System.Web.HttpContext.Current.Session["listaGeneral"] = lista;
            return PartialView("_PlantillaPlaneacion", lista);
        }
        
        [HttpPost]
        public ActionResult UpdateCustomer(PlaneacionViewModel customer)
        {
            var pl = (List<PlaneacionViewModel>)System.Web.HttpContext.Current.Session["listaGeneral"];

            var skueditar = pl.Where(x => x.sku.Contains(customer.sku)).FirstOrDefault();
            bool respuesta = true;

            if (skueditar != null)
            {                
                skueditar.qtyonhandcalidad = customer.qtyonhandcalidad;               

                return Json(new { skueditar, res = respuesta }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { skueditar, res = false }, JsonRequestBehavior.AllowGet);
        }
        
        public async Task<ActionResult> saveuser(string wh, string id, string propertyName, string value)
        {
            var status = false;
            var message = "";            
            var listaPlaneacion = await PlaneacionAsync(wh);

            var skueditar = listaPlaneacion.Where(x => x.sku.Equals(id)).FirstOrDefault();

            if (skueditar != null)
            {                
                skueditar.multiplosurtido =int.Parse(value);
                skueditar.qtyonhandcedis = 0;
                skueditar.qtyonhandcalidad = 0;
                skueditar.qtyonhandcv = 0;
                skueditar.qtyonhandstring = String.Format("#,##0.00", skueditar.qtyonhandcv);

                status = true;
            }
            else
            {
                message = "Error!";
            }

            var response = new { value = value, status = status, message = message , skueditar};
            JObject o = JObject.FromObject(response);
            return Content(o.ToString());
        }

        async Task<List<PlaneacionViewModel>> PlaneacionAsync(string wh)
        {
            try
            {
                ViewBag.WH = wh;
                var master = await ObtenerMasterAsync();
                var onhand = await ObtenerOnHandAsync(wh);
                var onhandcedis = await ObtenerOnHandCedisAsync(wh);
                var onhandcalidad = await ObtenerOnHandCalidadAsync(wh);
                var onhandmx3 = await ObtenerOnHandMX3Async(wh);
                IEnumerable<geneticas> ListaGeneticas = await ObtenerGeneticasAsync(wh);
                IEnumerable<familiasku> ListaFamilias = await ObtenerFamiliasAsync();
                List<PlaneacionViewModel> lista = new List<PlaneacionViewModel>();

                int contador = 0;

                foreach (var item in master)
                {
                    PlaneacionViewModel pl = new PlaneacionViewModel();

                    string clave = item.sku + "/" + wh;

                    var gen = ListaGeneticas.Where(x => x.clave.Equals(clave)).FirstOrDefault();

                    var consumopromedio = await ObtenerConsumoPromedio(item.sku);

                   

                    if (gen != null)
                    {
                        contador++;

                        pl.sku = item.sku;
                        pl.multiplosurtido = item.multiplosurtido;
                        pl.clasificacion = gen.geneticafinal.Substring(0, 1);
                        pl.maximo = (decimal)gen.maximo;
                        pl.minimo = (decimal)gen.minimo;
                        pl.bloque = ListaFamilias.Where(x => x.id == item.FamiliaSKU_Id).FirstOrDefault().id;
                        pl.qtyonhandcv = onhand.Where(x => x.item.Equals(item.sku)).Select(x => x.on_hand_qty - x.reserved_qty).Sum();
                        pl.qtyonhandcedis = onhandcedis.Where(x => x.item.Equals(item.sku)).Select(x => x.on_hand_qty - x.reserved_qty).Sum();
                        pl.qtyonhandcalidad = onhandcalidad.Where(x => x.item.Equals(item.sku)).Select(x => x.on_hand_qty - x.reserved_qty).Sum();
                        pl.qtyonhandmx3 = onhandmx3.Where(x => x.item.Equals(item.sku)).Select(x => x.on_hand_qty - x.reserved_qty).Sum();

                        decimal coverageweeks = (decimal)(pl.qtyonhandcedis / consumopromedio);

                        pl.coverageweeks = Math.Round(coverageweeks, 1);
                        pl.folio = contador;
                        
                        lista.Add(pl);
                    }
                }

                lista = lista.OrderBy(x => x.bloque).ThenBy(x => x.posicion).ToList();

                return lista;
            }
            catch (Exception _ex)
            {
                return null;
            }
        }

        //Coverage Weeks
        async Task<decimal?> ObtenerConsumoPromedio(string sku)
        {
            // return some scores ...
            return await db.geneticas.Where(x => (x.organizacion.Contains("MX1") || x.organizacion.Contains("MX4") || x.organizacion.Contains("MX5")) && x.sku.Equals(sku)).SumAsync(x => x.promedio);
        }

        //On Hand CV
        async Task<IEnumerable<onhand>> ObtenerOnHandAsync(string wh)
        {
            return await db.onhand.Where(x => x.subinv.Contains(wh + "10") || x.subinv.Contains(wh + "20")).ToListAsync();
        }

        //On Hand CEDIS
        async Task<IEnumerable<onhand>> ObtenerOnHandCedisAsync(string wh)
        {
            return await db.onhand.Where(x => x.subinv.Contains("M1QE10")).ToListAsync();
        }

        //On Hand Calidad
        async Task<IEnumerable<onhand>> ObtenerOnHandCalidadAsync(string wh)
        {
            return await db.onhand.Where(x => x.subinv.Contains("M1QE87")).ToListAsync();
        }

        //On Hand MX3
        async Task<IEnumerable<onhand>> ObtenerOnHandMX3Async(string wh)
        {
            return await db.onhand.Where(x => x.subinv.Contains("M3TP10")).ToListAsync();
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
                        pl.qtyonhandcv = onhand.Where(x => x.item.Equals(item.sku)).Select(x => x.on_hand_qty - x.reserved_qty).Sum();
                        pl.qtyonhandcedis = onhandcediscalidad.Where(x => x.item.Equals(item.sku) && x.subinv.Contains("M1QE10")).Select(x => x.on_hand_qty - x.reserved_qty).Sum();
                        pl.qtyonhandcalidad = onhandcediscalidad.Where(x => x.item.Equals(item.sku) && x.subinv.Contains("M1QE87")).Select(x => x.on_hand_qty - x.reserved_qty).Sum();
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