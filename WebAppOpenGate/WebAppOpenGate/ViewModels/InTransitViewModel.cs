using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebAppOpenGate.Models.Planeacion;

namespace WebAppOpenGate.ViewModels
{
    public class InTransitViewModel : hl_transit
    {
        public string wh { get; set; }

        public string llave { get; set; }

        public double qtypallets { get; set; }
    }
}