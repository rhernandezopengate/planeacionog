//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebAppOpenGate.Models.Planeacion
{
    using System;
    using System.Collections.Generic;
    
    public partial class masterarticulos
    {
        public int id { get; set; }
        public string sku { get; set; }
        public Nullable<int> qtycaja { get; set; }
        public Nullable<int> qtypallet { get; set; }
        public Nullable<int> multiplosurtido { get; set; }
        public Nullable<int> cajaspallet { get; set; }
        public Nullable<decimal> kgcaja { get; set; }
        public Nullable<decimal> pesopallet { get; set; }
        public string descripcion { get; set; }
        public bool activo { get; set; }
        public Nullable<int> FamiliaSKU_Id { get; set; }
    
        public virtual familiasku familiasku { get; set; }
    }
}
