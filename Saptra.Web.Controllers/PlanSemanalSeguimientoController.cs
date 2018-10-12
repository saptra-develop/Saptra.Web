///Propósito:Controlador para la consulta de planes semanles
///Fecha creación: 25/Septiembre/18
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
    public class PlanSemanalSeguimientoController : Controller
    {
        private Inaeba_SaptraEntities db = new Inaeba_SaptraEntities();

        #region VALIDACION DE PLAN SEMANAL
        public ActionResult ValidacionPlan()
        {
            return View();
        }

        [HttpGet]
        public JsonResult CargarPlanes(int? idUsu, int? idEstatus, int? idPeriodo)
        {
            try
            {
                List<udf_PlanSemanalValidarList_Result> result = (from u in db.udf_PlanSemanalValidarList(idUsu) select u).Where(x => (idPeriodo == null ? 1 == 1 : x.PeriodoId == idPeriodo) && x.EstatusId == 10) .ToList();

                var lstPlanSemanal = result.Select(C => new
                {
                    id = C.PlnSemanalId,
                    descripcionPlan = C.DescripcionPlan,
                    periodo = C.Periodo.ToUpper(),
                    usuario = C.Usuario,
                    tipoFigura = C.TipoFigura,
                    acciones = "<button class='btn btn-xs btn-success btnValidar' style='border-radius: 21px;' id='btnValidar_" + C.PlnSemanalId + "' data-idplan='" + C.PlnSemanalId + "'><i class='fa fa-paper-plane  fa-lg fa-fw'></i> Validar</button>"
                });

                return Json(new { Success = true, datos = lstPlanSemanal }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = exp.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult CargarDetallePlan(int idPlanSemanal)
        {
            try
            {
                var result = (from cat in db.dDetallePlanSemanal
                              where cat.PlanSemanalId == idPlanSemanal
                              select cat).ToList();

                var lstDetalle = result.Select(cat => new
                {
                    id = cat.DetallePlanId,
                    actividad = cat.cTipoActividades.NombreActividad,
                    descripcion = cat.DescripcionActividad,
                    fecha = cat.FechaActividad.ToString("MM/dd/yyyy"),
                    hora = cat.HoraActividad.ToString("hh':'mm"),
                    checkin = (cat.CantidadCheckIn < 1 ? "N/A" : cat.CantidadCheckIn.ToString()),
                    comentariosNoValidacion = "<strong>" + cat.ComentariosNoValidacion + "</strong>"
                });



                return Json(new { Success = true, datos = lstDetalle }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = exp.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region APROBACION DE PLAN SEMANAL
        public ActionResult AprobacionPlan()
        {
            return View();
        }

        #endregion

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