﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class DB_A3F19C_PruebasEntities : DbContext
    {
        public DB_A3F19C_PruebasEntities()
            : base("name=DB_A3F19C_PruebasEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<onhand> onhand { get; set; }
        public virtual DbSet<masterarticulos> masterarticulos { get; set; }
        public virtual DbSet<wharehouse> wharehouse { get; set; }
        public virtual DbSet<familiasku> familiasku { get; set; }
        public virtual DbSet<geneticas> geneticas { get; set; }
        public virtual DbSet<moopen> moopen { get; set; }
        public virtual DbSet<hl_transit> hl_transit { get; set; }
        public virtual DbSet<asignacionwhintransit> asignacionwhintransit { get; set; }
    }
}
