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
    public class ListaPermisos
    {
        public int id { get; set; }
        public string nombreModulo { get; set; }
        public int idEstatus { get; set; }
        public int lecturaPermiso { get; set; }
        public int escrituraPermiso { get; set; }
        public int edicionPermiso { get; set; }
        public int clonadoPermiso { get; set; }
        public int borradoPermiso { get; set; }
    }
}