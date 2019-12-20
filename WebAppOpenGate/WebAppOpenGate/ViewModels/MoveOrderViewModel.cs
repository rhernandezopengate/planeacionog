using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebAppOpenGate.Models.Planeacion;

namespace WebAppOpenGate.ViewModels
{
    public class MoveOrderViewModel : moopen
    {
        public string Status { get; set; }

        public string DescripcionWH { get; set; }
    }
}