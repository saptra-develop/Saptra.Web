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
    
    public partial class mCoordinacionRegionZonaUsuario
    {
        public int CoordinacionRegionZonaId { get; set; }
        public int CoordinacionRegionId { get; set; }
        public int CoordinacionZonaId { get; set; }
        public int UsuarioJefeRegionId { get; set; }
    
        public virtual cCoordinacionesRegion cCoordinacionesRegion { get; set; }
        public virtual cCoordinacionesZona cCoordinacionesZona { get; set; }
        public virtual mUsuarios mUsuarios { get; set; }
    }
}
