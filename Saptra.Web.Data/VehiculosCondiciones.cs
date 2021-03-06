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
    
    public partial class VehiculosCondiciones
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public VehiculosCondiciones()
        {
            this.VehiculosCondicionesDetalles = new HashSet<VehiculosCondicionesDetalles>();
        }
    
        public int VehiculoCondicionId { get; set; }
        public Nullable<System.DateTime> Fecha { get; set; }
        public Nullable<int> VehiculoId { get; set; }
        public Nullable<System.DateTime> FechaCreacion { get; set; }
        public Nullable<System.DateTime> FechaActualizacion { get; set; }
        public Nullable<int> UsuarioModificoId { get; set; }
        public Nullable<int> UsuarioCreoId { get; set; }
        public Nullable<int> UsuarioId { get; set; }
    
        public virtual Vehiculos Vehiculos { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VehiculosCondicionesDetalles> VehiculosCondicionesDetalles { get; set; }
    }
}
