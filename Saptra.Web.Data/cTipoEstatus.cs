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
    
    public partial class cTipoEstatus
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public cTipoEstatus()
        {
            this.cEstatus = new HashSet<cEstatus>();
        }
    
        public int TipoEstatusId { get; set; }
        public System.DateTime FechaCreacion { get; set; }
        public int UsuarioCreacionId { get; set; }
        public string nombreTipoEstatus { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<cEstatus> cEstatus { get; set; }
        public virtual mUsuarios mUsuarios { get; set; }
    }
}
