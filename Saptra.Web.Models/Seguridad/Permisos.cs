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
    public class Permisos
    {
        [Display(Name = "idModulo")]
        public int idModulo { get; set; }

        [Display(Name = "nombreModulo")]
        public string nombreModulo { get; set; }

        [Display(Name = "urlModulo")]
        public string urlModulo { get; set; }

        [Display(Name = "Lectura")]
        public int lecturaPermisos { get; set; }

        [Display(Name = "Escritura")]
        public int escrituraPermisos { get; set; }

        [Display(Name = "Borrado")]
        public int borradoPermisos { get; set; }

        [Display(Name = "Clonado")]
        public int clonadoPermisos { get; set; }

        [Display(Name = "Seguridad")]
        public int seguridadPermisos { get; set; }

        public string permisosAsignados
        {
            get
            {
                return (lecturaPermisos.ToString() +
                        escrituraPermisos.ToString() +
                        borradoPermisos.ToString() +
                        clonadoPermisos.ToString() +
                        seguridadPermisos.ToString());
            }
        }


    }
}
