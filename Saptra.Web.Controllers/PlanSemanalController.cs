///Propósito:Controlador para la administracion de planes semanles
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
    public class PlanSemanalController : Controller
    {
        private Inaeba_SaptraEntities db = new Inaeba_SaptraEntities();

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult CargarPlanes(int? idUsu, int? idEstatus)
        {
            try
            {
                var result = (from cat in db.mPlanSemanal
                              where cat.EstatusId == (idEstatus == null ? cat.EstatusId : idEstatus)
                                  && cat.UsuarioCreacionId == (idUsu == null ? cat.UsuarioCreacionId : idUsu)
                              select new
                              {
                                  id = cat.PlanSemanalId,
                                  descripcionPlan = cat.DescripcionPlaneacion,
                                  periodo = cat.cPeriodos.DecripcionPeriodo.ToUpper(),
                                  usuario = cat.mUsuarios.NombresUsuario + " " + cat.mUsuarios.ApellidosUsuario
                              }).ToList();



                return Json(new { Success = true, datos = result }, JsonRequestBehavior.AllowGet);
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
                              select new
                              {
                                  id = cat.DetallePlanId,
                                  actividad = cat.cTipoActividades.NombreActividad,
                                  descripcion = cat.DescripcionActividad,
                                  fecha = cat.FechaActividad.ToString(),
                                  hora = cat.HoraActividad.ToString(),
                                  checkin = cat.CantidadCheckIn,
                                  comentariosNoValidacion = cat.ComentariosNoValidacion
                              }).ToList();



                return Json(new { Success = true, datos = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = exp.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult NuevoDetalle(int id)
        {
            var objDetallePlan = new dDetallePlanSemanal();
            ViewBag.Titulo = "Nueva actividad";
            objDetallePlan.PlanSemanalId = id;
            return PartialView("_NuevoDetalle", objDetallePlan);
        }

        [HttpPost]
        public JsonResult NuevoDetalle(dDetallePlanSemanal pobjModelo)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    pobjModelo.FechaCreacion = DateTime.Now;
                    pobjModelo.EstatusId = 5;
                    db.dDetallePlanSemanal.Add(pobjModelo);
                    db.SaveChanges();

                    return Json(new { Success = true, id = pobjModelo.DetallePlanId, Message = "Se guardo correctamente la actividad " });
                }
                catch (Exception exp)
                {
                    return Json(new { Success = false, Message = exp.Message });
                }
            }

            return Json(new { Success = false, Message = "Informacion incompleta" });
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