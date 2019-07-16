//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class Vehiculos
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Vehiculos()
        {
            this.VehiculosCondiciones = new HashSet<VehiculosCondiciones>();
            this.VehiculosSolicitudes = new HashSet<VehiculosSolicitudes>();
        }
    
        public int VehiculoId { get; set; }
        public string Matricula { get; set; }
        public string Modelo { get; set; }
        public int Anio { get; set; }
        public string Color { get; set; }
        public Nullable<byte> EstatusVehiculoId { get; set; }
        public Nullable<byte> VehiculoMarcaId { get; set; }
        public Nullable<byte> CoordinacionZonaId { get; set; }
        public Nullable<System.DateTime> FechaCreacion { get; set; }
        public Nullable<System.DateTime> FechaActualizacion { get; set; }
        public Nullable<int> UsuarioModificoId { get; set; }
        public Nullable<int> UsuarioCreoId { get; set; }
        public Nullable<int> UsuarioId { get; set; }
    
        public virtual VehiculosMarcas VehiculosMarcas { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VehiculosCondiciones> VehiculosCondiciones { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VehiculosSolicitudes> VehiculosSolicitudes { get; set; }
    }
}
