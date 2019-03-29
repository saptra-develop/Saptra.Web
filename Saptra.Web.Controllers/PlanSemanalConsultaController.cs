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
    public class PlanSemanalConsultaController : Controller
    {
        private Inaeba_SaptraEntities db = new Inaeba_SaptraEntities();

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult CargarPlanes(int? idUsu, int? idEstatus, int? idPeriodo)
        {
            try
            {
                int[] estatus = { 2, 7, 9, 10 };
           
                List<udf_PlanSemanalList_Result> result = (from u in db.udf_PlanSemanalList(idUsu) select u).Where(x => (idPeriodo == 0 ? 1 == 1 : x.PeriodoId == idPeriodo) && estatus.Contains(x.EstatusId.Value)) .ToList();

                var lstPlanSemanal = result.Select(C => new
                {
                    id = C.PlnSemanalId,
                    descripcionPlan = C.DescripcionPlan,
                    periodo = C.Periodo.ToUpper(),
                    usuario = C.Usuario
                });

                return Json(new { Success = true, datos = lstPlanSemanal }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = "Error al obtener la información" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult CargarDetallePlan(int idPlanSemanal)
        {
            try
            {
                var result = (from cat in db.dDetallePlanSemanal
                              where cat.PlanSemanalId == idPlanSemanal
                              select cat).OrderBy(x => new { x.FechaActividad, x.HoraActividad }).ToList();

                var lstDetalle = result.Select(cat => new
                {
                    id = cat.DetallePlanId,
                    actividad = cat.cTipoActividades.NombreActividad + (cat.TipoActividadId == 8 ? (cat.SinProspectos == 1 ? "<br><span style='color:green'><strong>Sin prospectos</strong></span>" : "<br><button type = 'button' class='btn btn-xs btn-primary btnAgregarEducandos' " + (cat.mCheckIn.Count() > 0 ? "" : "disabled")  +" id='btnAgregarEducandos_" + cat.DetallePlanId + "' data-iddetalleplan='" + cat.DetallePlanId + "' " + (cat.SinProspectos == 1 ? "disabled" : "") + "><i class='fa fa-address-card-o  fa-lg fa-fw'></i> Vincular pre-registros</button>") : "") + (cat.InaebaPreregistros.Count() > 0 ? "<br> <a href='javascript:;' class='hrefVerEducandos' data-iddetalleplan='" +cat.DetallePlanId + "'>Ver Pre-registros Asignados</a>" : ""),
                    descripcion = cat.DescripcionActividad,
                    lugar = cat.LugarActividad,
                    fecha = cat.FechaActividad.ToString("dd/MM/yyyy"),
                    hora = cat.HoraActividad.ToString("hh':'mm"),
                    horaFin = cat.HoraFin == null ? "" : cat.HoraFin.Value.ToString("hh':'mm"),
                    checkin = (cat.CantidadCheckIn < 1 ? "N/A" : cat.CantidadCheckIn.ToString()),
                    placa = cat.mSolicitudesVehiculo.Count() > 0 ? cat.mSolicitudesVehiculo.FirstOrDefault().PlacaVehiculo : "",
                    comentariosNoValidacion = "<strong>" + cat.ComentariosNoValidacion + "</strong>"
                });



                return Json(new { Success = true, datos = lstDetalle }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = "Error al obtener la información" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult NuevoPreregistros(int id)
        {
            var objDetallePlan = new dDetallePlanSemanal();

            var TienePre = (from p in db.InaebaPreregistros
                            where p.num_activ_saptra == id
                            select p).ToList();

            objDetallePlan.DetallePlanId = id;
            ViewBag.TieneEducandos = (TienePre.Count > 0 ? 1 : 0);
            return PartialView("_Educandos", objDetallePlan);

        }
        [HttpGet]
        public ActionResult VerEducandos()
        {
            return PartialView("_VerEducandos");

        }
        [HttpGet]
        public JsonResult CargarEducandos(int idUsuario)
        {
            try
            {
                var usuario = (from u in db.mUsuarios
                               where u.UsuarioId == idUsuario
                               select u).First();

                var result = (from cat in db.InaebaPreregistros
                              where cat.mwu_rfc == usuario.RFCUsuario
                              && cat.num_activ_saptra == null
                              select cat).ToList();

                var lstDetalle = result.Select(cat => new
                {
                    id = cat.PreregistroId,
                    Campocheck = "<input type='checkbox' id='" + cat.PreregistroId + "-Campocheck' data-idpreregistro='" + cat.PreregistroId + "' class='chkRenglonGridPre'/>",
                    nombre = cat.ben_nom + " " + cat.ben_app + " " + cat.ben_apm,
                    curp = cat.ben_curp,
                    fecha = cat.ben_nacf
                });



                return Json(new { Success = true, datos = lstDetalle }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = "Error al obtener la información" }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult CargarVerEducandos(int idDetallePlan)
        {
            try
            {
                var result = (from cat in db.InaebaPreregistros
                              where cat.num_activ_saptra == idDetallePlan
                              select cat).ToList();

                var lstDetalle = result.Select(cat => new
                {
                    id = cat.PreregistroId,
                    nombre = cat.ben_nom + " " + cat.ben_app + " " + cat.ben_apm,
                    curp = cat.ben_curp,
                    fecha = cat.ben_nacf
                });



                return Json(new { Success = true, datos = lstDetalle }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = "Error al obtener la información" }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult ActualizaSinProspectos(dDetallePlanSemanal pobjModelo)
        {
            try
            {
                var result = (from ps in db.dDetallePlanSemanal
                              where ps.DetallePlanId == pobjModelo.DetallePlanId
                              select ps).ToList();

                //Actualiza Detalle Plan
                var dbTemp = result.First();
                dbTemp.SinProspectos = 1;
                db.SaveChanges();


                return Json(new { Success = true, idDetallePlan = pobjModelo.DetallePlanId, Message = "actualizada correctamente " });

            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = "Error al guardar la información" });
            }
        }

        [HttpPost]
        public JsonResult ActualizaEducandos(List<Educandos> jsEducandos)
        {
            try
            {
                if (jsEducandos != null)
                {
                    foreach (Educandos objEducandos in jsEducandos)
                    {
                        var resultEducandos = (from ip in db.InaebaPreregistros
                                                 where ip.PreregistroId == objEducandos.idPreregistro
                                                 select ip).FirstOrDefault();

                        //Actualiza a idDetallePlan en Educando
                        var dbTemp = resultEducandos;
                        dbTemp.num_activ_saptra = objEducandos.idDetallePlan;
                        db.SaveChanges();
                    }
                    return Json(new { Success = true, Message = "vinculados correctamente. " });
                }
                else
                {
                    return Json(new { Success = false, Message = "Sin información para guardar. " });
                }

              

            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = "Error al guardar la información" });
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