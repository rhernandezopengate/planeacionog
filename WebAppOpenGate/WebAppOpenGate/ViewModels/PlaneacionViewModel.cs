using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAppOpenGate.ViewModels
{
    public class PlaneacionViewModel
    {
        public int folio { get; set; }

        public string sku { get; set; }
        
        public string clasificacion { get; set; }

        public int? multiplosurtido { get; set; } 

        public string wh { get; set; }

        public int? qtyonhandcalidad { get; set; }

        public int? qtyonhandcedis { get; set; }
        
        public int? qtyonhandcv { get; set; }

        public int? qtyonhandmx3 { get; set; }

        public string qtyonhandstring { get; set; }

        public int bloque { get; internal set; }

        public int posicion { get; internal set; }

        public decimal minimo { get; internal set; }

        public decimal maximo { get; internal set; }

        public decimal? coverageweeks { get; internal set; }
    }
}