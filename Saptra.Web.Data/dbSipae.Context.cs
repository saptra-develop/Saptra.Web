﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Saptra.Web.Data
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class SipaeEntities : DbContext
    {
        public SipaeEntities()
            : base("name=SipaeEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Vehiculos> Vehiculos { get; set; }
        public virtual DbSet<VehiculosCondiciones> VehiculosCondiciones { get; set; }
        public virtual DbSet<VehiculosCondicionesDetalles> VehiculosCondicionesDetalles { get; set; }
        public virtual DbSet<VehiculosCondicionesTipos> VehiculosCondicionesTipos { get; set; }
        public virtual DbSet<VehiculosMarcas> VehiculosMarcas { get; set; }
        public virtual DbSet<VehiculosSolicitudes> VehiculosSolicitudes { get; set; }
    }
}
