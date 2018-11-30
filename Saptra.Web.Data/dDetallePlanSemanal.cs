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
    
    public partial class dDetallePlanSemanal
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public dDetallePlanSemanal()
        {
            this.mEducandosCaptados = new HashSet<mEducandosCaptados>();
            this.InaebaPreregistros = new HashSet<InaebaPreregistros>();
            this.mCheckIn = new HashSet<mCheckIn>();
            this.mSolicitudesVehiculo = new HashSet<mSolicitudesVehiculo>();
        }
    
        public int DetallePlanId { get; set; }
        public int PlanSemanalId { get; set; }
        public System.DateTime FechaCreacion { get; set; }
        public int UsuarioCreacionId { get; set; }
        public int EstatusId { get; set; }
        public int TipoActividadId { get; set; }
        public System.DateTime FechaActividad { get; set; }
        public System.TimeSpan HoraActividad { get; set; }
        public Nullable<System.TimeSpan> HoraFin { get; set; }
        public int CantidadCheckIn { get; set; }
        public string DescripcionActividad { get; set; }
        public string LugarActividad { get; set; }
        public string ComentariosNoValidacion { get; set; }
        public string ComentariosRechazo { get; set; }
        public Nullable<bool> ActividadRechazada { get; set; }
        public Nullable<int> SinProspectos { get; set; }
    
        public virtual cEstatus cEstatus { get; set; }
        public virtual cTipoActividades cTipoActividades { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<mEducandosCaptados> mEducandosCaptados { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InaebaPreregistros> InaebaPreregistros { get; set; }
        public virtual mPlanSemanal mPlanSemanal { get; set; }
        public virtual mUsuarios mUsuarios { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<mCheckIn> mCheckIn { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<mSolicitudesVehiculo> mSolicitudesVehiculo { get; set; }
    }
}
