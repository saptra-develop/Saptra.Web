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
    
    public partial class mSolicitudesVehiculo
    {
        public int SolicitudVehiculoId { get; set; }
        public System.DateTime FechaCreacion { get; set; }
        public int UsuarioCreacionId { get; set; }
        public int VehiculoId { get; set; }
        public System.DateTime FechaUso { get; set; }
        public Nullable<int> SolicitudSipaeId { get; set; }
        public int DetallePlanId { get; set; }
        public string PlacaVehiculo { get; set; }
    
        public virtual dDetallePlanSemanal dDetallePlanSemanal { get; set; }
        public virtual mUsuarios mUsuarios { get; set; }
    }
}
