///Propósito: Modelo de Permisos
///Fecha creación: 05/Febreo/2016
///Creador: David Galvan
///Fecha modifiacción: 
///Modificó:
///Dependencias de conexiones e interfaces: 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Saptra.Web.Models
{
    public class MenuGrupo
    {
        [Display(Name = "idMenuGrupo")]
        public int idMenuGrupo { get; set; }

        [Display(Name = "nombreGrupo")]
        public string nombreGrupo { get; set; }

        [Display(Name = "iconGrupo")]
        public string iconGrupo { get; set; }

        [Display(Name = "Lista Permisos")]
        public List<Permisos> lstPermisos { get; set; }
    }
}
