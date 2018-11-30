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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;


using Saptra.Web.Data;
using Saptra.Web.Models;
using Saptra.Web.Utils;

namespace Sispro.Web.Controllers
{
    public class PlanSemanalController : Controller
    {
        private Inaeba_SaptraEntities db = new Inaeba_SaptraEntities();
        private SipaeEntities dbSipae = new SipaeEntities();

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult CargarPlanes(int? idUsu, int? idEstatus, int? idPeriodo)
        {
            try
            {
                string[] estatus = Regex.Split("7,8,10", ",");
                List<udf_PlanSemanalList_Result> result = (from u in db.udf_PlanSemanalList(idUsu) select u).Where(x => (idPeriodo == 0 ? 1==1 : x.PeriodoId == idPeriodo) && estatus.Contains(x.EstatusId.ToString())) .ToList();

                var lstPlanSemanal = result.Select(C => new
                {
                    id = C.PlnSemanalId,
                    descripcionPlan = C.DescripcionPlan,
                    periodo = C.Periodo.ToUpper(),
                    usuario = C.Usuario,
                    accion = "<button class='btn btn-xs btn-info accrowProdDetalle' id='accrowProdDetalle_" + C.PlnSemanalId + "' data-idplan='" + C.PlnSemanalId + "' data-idestatus='" + C.EstatusId + "' " + (C.EstatusId == 10 ? "disabled" : "") + "><i class='fa fa-plus  fa-lg fa-fw'></i></button>" +
                            " <button class='btn btn-xs " + (C.EstatusId == 10 ? "btn-success" : "btn-primary") + " btnEnvioValidacion' id='btnEnvioValidacion_" + C.PlnSemanalId + "' data-idplan='" + C.PlnSemanalId + "' data-idestatus='" + C.EstatusId + "' " + (C.TieneDetalle == 0 || C.EstatusId == 10 ? "disabled" : "") + "><i class='fa fa-paper-plane  fa-lg fa-fw'></i></button>",
                    actividades  = (C.TieneActividades == 0 ? "<i class='fa fa-times-circle-o fa-2x' style='color:#F74048;'></i> Sin actividades" : "<i class='fa fa-check-circle-o fa-2x' style='color:#2FFA5D;'></i> Actividades registradas")
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
                              select cat).ToList();

                var lstDetalle = result.Select(cat => new
                {
                    id = cat.DetallePlanId,
                    actividad = cat.cTipoActividades.NombreActividad + (cat.ComentariosNoValidacion != null ? "<input type='hidden' class='hdnPintaFila' />" : ""),
                    descripcion = cat.DescripcionActividad,
                    lugar = cat.LugarActividad,
                    fecha = cat.FechaActividad.ToString("MM/dd/yyyy"),
                    hora = cat.HoraActividad.ToString("hh':'mm"),
                    horaFin = cat.HoraFin == null ? "" : cat.HoraFin.Value.ToString("hh':'mm"),
                    checkin = (cat.CantidadCheckIn < 1 ? "N/A" : cat.CantidadCheckIn.ToString()),
                    comentariosNoValidacion = "<strong>" + cat.ComentariosNoValidacion + "</strong>",
                    placa = cat.mSolicitudesVehiculo.Count() > 0 ? cat.mSolicitudesVehiculo.FirstOrDefault().PlacaVehiculo : "",
                    accion = "<button class='btn btn-xs btn-warning btnEditarDetalle' style='border-radius: 21px;' id='btnEditarDetalle_" + cat.DetallePlanId + "' data-iddetalleplan='" + cat.DetallePlanId + "' " + (cat.mPlanSemanal.EstatusId == 7 || cat.mPlanSemanal.EstatusId == 8 ? "" : "disabled") + ">Editar</button>" +
                             (cat.cTipoActividades.RequiereCheckIn == true ? " <button class='btn btn-xs btn-primary btnVehiculo' id='btnVehiculo_" + cat.DetallePlanId + "' data-iddetalleplan='" + cat.DetallePlanId + "'" + (cat.mPlanSemanal.EstatusId == 7 || cat.mPlanSemanal.EstatusId == 8 ? "" : "disabled") + " ><i class='fa fa-car'></i></ button > " : "")
                });



                return Json(new { Success = true, datos = lstDetalle }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = "Error al obtener la información" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult Nuevo()
        {
            var objPlanSemanal = new mPlanSemanal();
            ViewBag.Titulo = "Nuevo plan semanal";
            return PartialView("_Nuevo", objPlanSemanal);
        }

        [HttpPost]
        public JsonResult Nuevo(int idUsuario)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    DateTime fecha = Convert.ToDateTime(DateTime.Now.ToShortDateString());

                    var result = (from cat in db.cPeriodos
                                  where cat.EstatusId == 5
                                  && (fecha >= cat.FechaInicio && fecha <= cat.FechaFin)
                                  select new
                                  {
                                      id = cat.PeriodoId,
                                      nombre = cat.DecripcionPeriodo,
                                  }).ToList();

                    int idPeriodoNew = result.FirstOrDefault().id;
                    var PeriodoValidaPromo = (from cat in db.cPeriodos
                                              where cat.PeriodoId < idPeriodoNew && cat.EstatusId == 5
                                              select cat).ToList();

                    var maxPeriodoId = PeriodoValidaPromo.Max(x => x.PeriodoId);

                    var planExistePromoSinAsignar = (from a in db.dDetallePlanSemanal
                                                     join b in db.mPlanSemanal on a.PlanSemanalId equals b.PlanSemanalId
                                                     where a.TipoActividadId == 8
                                                     && a.UsuarioCreacionId == idUsuario
                                                     && b.PeriodoId == maxPeriodoId
                                                     select a).ToList();
                    int countSinAsig = 0;
                    foreach (dDetallePlanSemanal tc in planExistePromoSinAsignar)
                    {
                        if (tc.InaebaPreregistros.Count() == 0)
                        {
                            if (tc.SinProspectos == null)
                            {
                                countSinAsig += 1;
                            }
                        } 
                    }
                    if (countSinAsig == 0)
                    {
                        if (result.Count() > 0)
                        {
                            int idPeriodo = result.FirstOrDefault().id;

                            var existePlan = (from ps in db.mPlanSemanal
                                              where ps.PeriodoId == idPeriodo
                                              && ps.UsuarioCreacionId == idUsuario
                                              select ps).ToList();
                            if (existePlan.Count() == 0)
                            {
                                var periodo = (from p in db.cPeriodos
                                               where p.PeriodoId == idPeriodo
                                               select p).FirstOrDefault();
                                mPlanSemanal plan = new mPlanSemanal();
                                plan.FechaCreacion = DateTime.Now;
                                plan.DescripcionPlaneacion = "Plan Semanal del " + periodo.DecripcionPeriodo;
                                plan.EstatusId = 7;
                                plan.PeriodoId = idPeriodo;
                                plan.UsuarioCreacionId = idUsuario;
                                db.mPlanSemanal.Add(plan);
                                db.SaveChanges();

                                return Json(new { Success = true, id = plan.PlanSemanalId, Message = "guardado correctamente " });
                            }
                            else
                            {
                                return Json(new { Success = false, Message = " No es posible crear mas de un plan por periodo." });
                            }
                        }
                        else
                        {
                            return Json(new { Success = false, Message = " No es posible crear el plan, no se encuentra periodo registtrado para la semana en curso." });
                        }
                    }
                    else
                    {
                        return Json(new { Success = false, Message = " No es posible crear el plan semanal, existen actividades de Promoción y difusión del plan anterior abiertas" });
                    }
                }
                catch (Exception exp)
                {
                    return Json(new { Success = false, Message = "Error al guardar la información" });
                }
            }

            return Json(new { Success = false, Message = "Informacion incompleta" });
        }

        [HttpGet]
        public ActionResult NuevoDetalle(int id)
        {
            var planPeriodo = (from pl in db.mPlanSemanal
                               where pl.PlanSemanalId == id
                               select pl).FirstOrDefault();

            var objDetallePlan = new dDetallePlanSemanal();
            ViewBag.Titulo = "Nueva actividad";
            objDetallePlan.PlanSemanalId = id;
            objDetallePlan.CantidadCheckIn = 1;
            ViewBag.FechaIni = planPeriodo.cPeriodos.FechaInicio.ToString("MM/dd/yyyy");
            ViewBag.FechaFin = planPeriodo.cPeriodos.FechaFin.ToString("MM/dd/yyyy");
            return PartialView("_NuevoDetalle", objDetallePlan);
        }

        [HttpGet]
        public ActionResult NuevoVehiculo(int id)
        {
            var vehiculo = (from v in db.mSolicitudesVehiculo
                            where v.DetallePlanId == id
                            select v).ToList();
           
            var objVehiculo = new mSolicitudesVehiculo();
            ViewBag.Titulo = "Asignar Vehiculo";
            objVehiculo.DetallePlanId = id;
            if (vehiculo.Count() > 0)
            {
                objVehiculo.VehiculoId = vehiculo.FirstOrDefault().VehiculoId;
            }
            return PartialView("_Automoviles", objVehiculo);
        }

        [HttpPost]
        public JsonResult NuevoVehiculo(mSolicitudesVehiculo pobjModelo)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var vehiculoAsignado = (from v in db.mSolicitudesVehiculo
                                    where v.DetallePlanId == pobjModelo.DetallePlanId
                                    select v).ToList();
                    var detalle = db.dDetallePlanSemanal.Find(pobjModelo.DetallePlanId);
                    var vehiculo = dbSipae.Vehiculos.Find(pobjModelo.VehiculoId);
                    if (vehiculoAsignado.Count > 0)
                    {
                        //Actualiza Vehiculo
                        var dbTemp = vehiculoAsignado.First();
                        dbTemp.VehiculoId = pobjModelo.VehiculoId;
                        dbTemp.FechaUso = detalle.FechaActividad;
                        dbTemp.PlacaVehiculo = vehiculo.Matricula;

                        db.SaveChanges();

                        return Json(new { Success = true, id = pobjModelo.SolicitudVehiculoId, Message = "asignado correctamente " });
                    }
                    else
                    {
                  
                        pobjModelo.FechaCreacion = DateTime.Now;
                        pobjModelo.VehiculoId = pobjModelo.VehiculoId;
                        pobjModelo.FechaUso = detalle.FechaActividad;
                        pobjModelo.PlacaVehiculo = vehiculo.Matricula;
                        db.mSolicitudesVehiculo.Add(pobjModelo);
                        db.SaveChanges();

                        return Json(new { Success = true, id = pobjModelo.SolicitudVehiculoId, Message = "asignado correctamente " });
                    }
                    
                }
                catch (Exception exp)
                {
                    return Json(new { Success = false, Message = "Error al guardar la información" });
                }
            }

            return Json(new { Success = false, Message = "Informacion incompleta" });
        }



        [HttpPost]
        public JsonResult NuevoDetalle(dDetallePlanSemanal pobjModelo)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var tipoAct = db.cTipoActividades.Find(pobjModelo.TipoActividadId);
                    if (tipoAct.RequiereCheckIn == false)
                    {
                        pobjModelo.CantidadCheckIn = 0;
                    }
                    pobjModelo.FechaCreacion = DateTime.Now;
                    pobjModelo.EstatusId = 11;
                    db.dDetallePlanSemanal.Add(pobjModelo);
                    db.SaveChanges();

                    return Json(new { Success = true, id = pobjModelo.DetallePlanId, Message = "registrada correctamente " });
                }
                catch (Exception exp)
                {
                    return Json(new { Success = false, Message = "Error al guardar la información" });
                }
            }

            return Json(new { Success = false, Message = "Informacion incompleta" });
        }

        [HttpPost]
        public JsonResult EnviarPlan(int idPlan, int idUsuario)
        {
            try
            {
                var result = (from ps in db.mPlanSemanal
                              where ps.PlanSemanalId == idPlan
                              select ps).ToList();

                //Actualiza a estatus Enviado
                var dbTemp = result.First();
                dbTemp.EstatusId = 10;
                db.SaveChanges();

                //Revisa coordinador de zona al que pertenece
                var coordinador = (from co in db.mCoordinacionZonaUsuario
                                   where co.UsuarioId == idUsuario
                                   select co).FirstOrDefault();

                var coordinacionJefe = (from co in db.mCoordinacionZonaUsuario
                                        where co.CoordinacionZonaId == coordinador.CoordinacionZonaId
                                        && co.JefeCoordinacionZona == true
                                        select co).FirstOrDefault();

                //Genera notificacion de validacion
                mNotificaciones objNotificaciones = new mNotificaciones();

                objNotificaciones.EstatusId = 3;
                objNotificaciones.TipoNotificacionId = 1;
                objNotificaciones.PlanSemanalId = idPlan;
                objNotificaciones.UsuarioId = idUsuario;
                objNotificaciones.CoordinacionZonaUsuarioId = coordinacionJefe.CordinacionZonaUsuarioId;
                objNotificaciones.FechaCreacion = DateTime.Now;
                db.mNotificaciones.Add(objNotificaciones);
                db.SaveChanges();

                //Actualiza a estatus Inactiva para la notificacion de con comentarios
                var notificacion = (from n in db.mNotificaciones
                                    where n.PlanSemanalId == idPlan
                                    && n.TipoNotificacionId == 3
                                    && n.EstatusId == 3
                                    select n).ToList();

                if (notificacion.Count() > 0)
                {
                    var dbTempNot = notificacion.First();
                    dbTempNot.EstatusId = 4;
                    db.SaveChanges();
                }

                return Json(new { Success = true, idPlan = idPlan, Message = "enviado correctamente para revisión " });
                
            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = "Error al enviar la información" });
            }
        }


        [HttpGet]
        public ActionResult ActualizaDetallePlan(int id)
        {
            var planPeriodo = (from pl in db.dDetallePlanSemanal
                               where pl.DetallePlanId == id
                               select pl).FirstOrDefault();

            var objDetallePlan = db.dDetallePlanSemanal.Find(id);
            

            TimeSpan hInicio = TimeSpan.Parse(objDetallePlan.HoraActividad.ToString("hh':'mm"));
            TimeSpan hFin= TimeSpan.Parse(objDetallePlan.HoraFin.Value.ToString("hh':'mm"));
            objDetallePlan.HoraActividad = hInicio;
            objDetallePlan.HoraFin= hFin;
            ViewBag.Titulo = "Actualizar actividad";
            ViewBag.FechaIni = planPeriodo.mPlanSemanal.cPeriodos.FechaInicio.ToString("MM/dd/yyyy");
            ViewBag.FechaFin = planPeriodo.mPlanSemanal.cPeriodos.FechaFin.ToString("MM/dd/yyyy");
            return PartialView("_ActualizaDetalle", objDetallePlan);
        }

        [HttpPost]
        public JsonResult ActualizaDetalle(dDetallePlanSemanal pobjModelo)
        {
            try
            {
                var result = (from ps in db.dDetallePlanSemanal
                              where ps.DetallePlanId == pobjModelo.DetallePlanId
                              select ps).ToList();

                var ValidaTipoAct = (from t in db.cTipoActividades
                                     where t.TipoActividadId == pobjModelo.TipoActividadId
                                     select t).First();

                if(ValidaTipoAct.TipoActividadId != 6)
                {
                    pobjModelo.CantidadCheckIn = 1;
                }

                //Actualiza Detalle Plan
                var dbTemp = result.First();
                dbTemp.TipoActividadId = pobjModelo.TipoActividadId;
                dbTemp.DescripcionActividad = pobjModelo.DescripcionActividad;
                dbTemp.LugarActividad = pobjModelo.LugarActividad;
                dbTemp.FechaActividad = pobjModelo.FechaActividad;
                dbTemp.HoraActividad = pobjModelo.HoraActividad;
                dbTemp.HoraFin = pobjModelo.HoraFin;
                dbTemp.CantidadCheckIn = pobjModelo.CantidadCheckIn;
                db.SaveChanges();


                return Json(new { Success = true, idDetallePlan = pobjModelo.DetallePlanId, Message = "actualizada correctamente " });

            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = "Error al guardar la información" });
            }
        }


        [HttpGet]
        public JsonResult ValidarActividadFecha(int idPlan, DateTime fecha, TimeSpan hInicio, TimeSpan hFin)
        {
            try
            {
                var ActividadPlaneada = (from c in db.dDetallePlanSemanal
                                         where c.PlanSemanalId == idPlan
                                         && c.FechaActividad == fecha
                                         && (c.HoraActividad >=  hInicio && c.HoraFin <= hFin)
                                         select c).ToList();

                if (ActividadPlaneada.Count() > 0)
                {
                    return (Json(new
                    {
                        Success = true,
                        Message = "Existen actividades planeadas en la fecha y hora seleccionada."
                    }, JsonRequestBehavior.AllowGet));
                }
                else
                {
                    return (Json(new
                    {
                        Success = false,
                        Message = "No existen actividades en fecha y hora seleccionadas."
                    }, JsonRequestBehavior.AllowGet));
                }

               
            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = "Error al guardar la información" });
            }
        }

        [HttpGet]
        public JsonResult ValidarActividadFechaActualizar(int idPlan, int idDetallePlan, int idActividad, DateTime fecha, TimeSpan hInicio, TimeSpan hFin)
        {
            try
            {
                var ActividadPlaneada = (from c in db.dDetallePlanSemanal
                                         where c.PlanSemanalId == idPlan
                                         && c.FechaActividad == fecha
                                         && c.DetallePlanId != idDetallePlan
                                         && (c.HoraActividad >= hInicio && c.HoraFin <= hFin)
                                         select c).ToList();

                if (ActividadPlaneada.Count() > 0)
                {
                    return (Json(new
                    {
                        Success = true,
                        Message = "Existen actividades planeadas en la fecha y hora seleccionada."
                    }, JsonRequestBehavior.AllowGet));
                }
                else
                {
                    return (Json(new
                    {
                        Success = false,
                        Message = "No existen actividades en fecha y hora seleccionadas."
                    }, JsonRequestBehavior.AllowGet));
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
                dbSipae.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}