//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebAppOpenGate.Models.Permisos
{
    using System;
    using System.Collections.Generic;
    
    public partial class roloperacion
    {
        public int id { get; set; }
        public int operaciones_Id { get; set; }
        public int rol_Id { get; set; }
    
        public virtual operaciones operaciones { get; set; }
        public virtual rol rol { get; set; }
    }
}
