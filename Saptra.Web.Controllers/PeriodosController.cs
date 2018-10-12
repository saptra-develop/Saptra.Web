///Propósito:Controlador para la administracion de periodos
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
    public class PeriodosController : Controller
    {
        private Inaeba_SaptraEntities db = new Inaeba_SaptraEntities();

        [HttpGet]
        public JsonResult CargarPeriodos(int? id, int? idEstatus)
        {
            try
            {
                DateTime fecha = Convert.ToDateTime(DateTime.Now.ToShortDateString());
                var result = (from cat in db.cPeriodos
                              where cat.EstatusId == (idEstatus == null ? cat.EstatusId : idEstatus)
                              select new
                              {
                                  id = cat.PeriodoId,
                                  nombre = cat.DecripcionPeriodo,
                              })
                              .OrderBy(cat => cat.id)
                              .ToList();

                return (Json(result, JsonRequestBehavior.AllowGet));

            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = exp.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult CargarPeriodoActual(int? id, int? idEstatus)
        {
            try
            {
                DateTime fecha = Convert.ToDateTime(DateTime.Now.ToShortDateString());
                var result = (from cat in db.cPeriodos
                              where cat.EstatusId == (idEstatus == null ? cat.EstatusId : idEstatus)
                              && (fecha >= cat.FechaInicio && fecha <= cat.FechaFin)
                              select new
                              {
                                  id = cat.PeriodoId,
                                  nombre = cat.DecripcionPeriodo,
                              })
                              .OrderBy(cat => cat.id)
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