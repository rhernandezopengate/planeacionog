using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAppOpenGate.ViewModels
{
    public class PlaneacionViewModel
    {
        public string sku { get; set; }

        public int? multiplosurtido { get; set; } 

        public string wh { get; set; }

        public int? qtycalidad { get; set; }

        public int? qtycedis { get; set; }
        
        public int? qtyonhand { get; set; }
        
        public int bloque { get; internal set; }

        public int posicion { get; internal set; }
    }
}