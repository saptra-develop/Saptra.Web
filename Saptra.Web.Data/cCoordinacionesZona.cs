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
    
    public partial class cCoordinacionesZona
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public cCoordinacionesZona()
        {
            this.mCoordinacionZonaUsuario = new HashSet<mCoordinacionZonaUsuario>();
        }
    
        public int CoordinacionZonaId { get; set; }
        public System.DateTime FechaCreacion { get; set; }
        public int UsuarioCreacionId { get; set; }
        public string DescripcionCoordinacionZona { get; set; }
        public int EstatusId { get; set; }
        public Nullable<int> CoordinacionRegionId { get; set; }
        public Nullable<int> CZonaSauId { get; set; }
    
        public virtual cCoordinacionesRegion cCoordinacionesRegion { get; set; }
        public virtual cEstatus cEstatus { get; set; }
        public virtual mUsuarios mUsuarios { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<mCoordinacionZonaUsuario> mCoordinacionZonaUsuario { get; set; }
    }
}
