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
    
    public partial class moopen
    {
        public int id { get; set; }
        public Nullable<System.DateTime> FechaAlta { get; set; }
        public string Allocation { get; set; }
        public string Number { get; set; }
        public string type { get; set; }
        public Nullable<int> line { get; set; }
        public string transaction_type { get; set; }
        public string item { get; set; }
        public Nullable<int> rev { get; set; }
        public string source_subinv { get; set; }
        public string source_locator { get; set; }
        public string destination_subinv { get; set; }
        public string destination_locator { get; set; }
        public string destination_account { get; set; }
        public string lot_number { get; set; }
        public Nullable<System.DateTime> expiration_date { get; set; }
        public string serial_from { get; set; }
        public string serial_to { get; set; }
        public Nullable<int> unit_number { get; set; }
        public string uom { get; set; }
        public Nullable<int> transaction_qty { get; set; }
        public Nullable<int> requested_qty { get; set; }
        public Nullable<int> required_qty { get; set; }
        public Nullable<int> delivered_qty { get; set; }
        public Nullable<int> allocated_qty { get; set; }
        public string secondary_uom { get; set; }
        public Nullable<int> secondary_qty { get; set; }
        public Nullable<int> secondary_requested_qty { get; set; }
        public Nullable<int> secondary_required_qty { get; set; }
        public Nullable<int> secondary_delivered_qty { get; set; }
        public Nullable<int> secondary_allocated_qty { get; set; }
        public string grade { get; set; }
        public Nullable<System.DateTime> daterequired { get; set; }
        public string reason { get; set; }
        public string reference { get; set; }
        public string reference_number { get; set; }
        public string reference_type { get; set; }
        public string sales_order { get; set; }
        public string so_line { get; set; }
        public string ship_set { get; set; }
        public string project { get; set; }
        public string task { get; set; }
        public string line_status { get; set; }
        public Nullable<System.DateTime> status_date { get; set; }
        public string created_by { get; set; }
        public string ship_to_location { get; set; }
        public string ultimo { get; set; }
    }
}