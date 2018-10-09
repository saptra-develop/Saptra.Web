///Propósito:Controlador para la administracion de tipos de actividades
///Fecha creación: 03/Octubre/18
///Creador: David Jasso
///Fecha modifiacción: 
///Modificó:
///Dependencias de conexiones e interfaces: 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Configuration;
using Newtonsoft.Json;


using Saptra.Web.Data;
using Saptra.Web.Models;
using Saptra.Web.Utils;

namespace Sispro.Web.Controllers
{
    public class TipoActividadesController : Controller
    {
        private Inaeba_SaptraEntities db = new Inaeba_SaptraEntities();


        [HttpGet]
        public JsonResult CargarTipoActividades(int? id, int? idEstatus)
        {
            try
            {
                var listaActvidades = new List<cTipoActividades>();

                var result = (from cat in db.cTipoActividades
                              where cat.EstatusId == (idEstatus == null ? cat.EstatusId : idEstatus)
                              select new
                              {
                                  id = cat.TipoActividadId,
                                  nombre = cat.NombreActividad.ToUpper()
                              })
                              .OrderBy(cat => cat.nombre)
                              .ToList();

                return (Json(result, JsonRequestBehavior.AllowGet));

            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = exp.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}