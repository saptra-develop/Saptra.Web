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
    
    public partial class cTipoActividades
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public cTipoActividades()
        {
            this.dDetallePlanSemanal = new HashSet<dDetallePlanSemanal>();
        }
    
        public int TipoActividadId { get; set; }
        public System.DateTime FechaCreacion { get; set; }
        public int UsuarioCreacionId { get; set; }
        public int EstatusId { get; set; }
        public string NombreActividad { get; set; }
        public string DescripcionActividad { get; set; }
        public bool RequiereCheckIn { get; set; }
        public Nullable<bool> ActividadEspecial { get; set; }
        public string Mensaje { get; set; }
    
        public virtual cEstatus cEstatus { get; set; }
        public virtual mUsuarios mUsuarios { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dDetallePlanSemanal> dDetallePlanSemanal { get; set; }
    }
}
