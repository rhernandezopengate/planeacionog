using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WebAppOpenGate.Models.Planeacion;

namespace WebAppOpenGate.ViewModels
{
    public class OnHandViewModels : onhand
    {        
        public double qtypallets { get; set; }

        public string wh { get; set; }

        public string llave { get; set; }
    }
    
}