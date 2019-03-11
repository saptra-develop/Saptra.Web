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
using OfficeOpenXml;
using System.Threading.Tasks;
using OfficeOpenXml.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Text.RegularExpressions;
using OfficeOpenXml.Style;
using System.Globalization;

using Saptra.Web.Data;
using Saptra.Web.Models;
using Saptra.Web.Utils;

namespace Sispro.Web.Controllers
{
    public class PlanSemanalSeguimientoController : Controller
    {
        private Inaeba_SaptraEntities db = new Inaeba_SaptraEntities();
        private SipaeEntities dbSipae = new SipaeEntities();

        #region VALIDACION DE PLAN SEMANAL
        public ActionResult ValidacionPlan()
        {
            return View();
        }

        [HttpGet]
        public JsonResult CargarPlanes(int? idUsu, int? idEstatus, string Periodos, string TiposFigura, string NombresFigura)
        {
            try
            {
                List<udf_PlanSemanalValidarList_Result> result = (from u in db.udf_PlanSemanalValidarList(idUsu, Periodos, TiposFigura, NombresFigura) select u).ToList();

                var lstPlanSemanal = result.Select(C => new
                {
                    id = C.PlnSemanalId,
                    descripcionPlan = C.DescripcionPlan,
                    periodo = C.Periodo.ToUpper(),
                    usuario = C.Usuario,
                    tipoFigura = C.TipoFigura,
                    acciones = "<div class='btn-group btn-group-sm' role='group'>" +
                               "<button type='button' " + (C.EstatusId == 8 ? "disabled" : "") + " class='btn btn-success btnValidar' id='btnValidar_" + C.PlnSemanalId + "' data-idplan='" + C.PlnSemanalId + "'><i class='fa fa-check-circle fa-lg fa-fw'></i> Validar</button>" +
                               "<button type='button' " + (C.EstatusId == 8 ? "disabled" : "") + " class='btn btn-warning btnFeedback' id='btnFeedback_" + C.PlnSemanalId + "' data-idplan='" + C.PlnSemanalId + "'><i class='fa fa-comment fa-lg fa-fw'></i> Observaciones</button>" +
                               "</div>"
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
                    actividad = cat.cTipoActividades.NombreActividad + (cat.ComentariosNoValidacion != null ? "<input type='hidden' class='hdnPintaFila' />" : ""),
                    descripcion = cat.DescripcionActividad,
                    lugar = cat.LugarActividad,
                    fecha = cat.FechaActividad.ToString("MM/dd/yyyy"),
                    hora = cat.HoraActividad.ToString("hh':'mm"),
                    horaFin = cat.HoraFin == null ? "" : cat.HoraFin.Value.ToString("hh':'mm"),
                    checkin = (cat.CantidadCheckIn < 1 ? "N/A" : cat.CantidadCheckIn.ToString()),
                    comentariosNoValidacion = "<div class='row'>" +
                                            "<div class='col-md-2'><br><div class='material-switch'><input " + (cat.mPlanSemanal.EstatusId == 8 || cat.ComentariosNoValidacion != null ? "disabled" : "") + "  class='checkComents' " + (cat.ComentariosNoValidacion == null ? "" : "checked") + " id='checkComents_" + cat.DetallePlanId + "' type='checkbox' data-iddetalleplan='" + cat.DetallePlanId + "'/>" +
                                            "<label for='checkComents_" + cat.DetallePlanId + "' class='label-success'></label></div></div>" +
                                            "<div class='col-md-10'><textarea maxlength='100' rows='2'  " + (cat.mPlanSemanal.EstatusId == 8 ? "readonly" : "") + " class='form-control idComentariosCZ' id='idComentariosCZ_" + cat.DetallePlanId + "' data-idplan='" + cat.PlanSemanalId + "' data-iddetalleplan='" + cat.DetallePlanId + "'" + (cat.ComentariosNoValidacion == null ? "disabled" : "enabled") + ">" + cat.ComentariosNoValidacion + "</textarea></div>" + 
                                            "</div>"
                });



                return Json(new { Success = true, datos = lstDetalle }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = "Error al obtener la información" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult ValidarPlan(int idPlan, int idUsuario)
        {
            try
            {
                var result = (from ps in db.mPlanSemanal
                              where ps.PlanSemanalId == idPlan
                              select ps).ToList();

                //Actualiza a estatus Validado
                var dbTemp = result.First();
                dbTemp.EstatusId = 2;
                db.SaveChanges();

                //Actualiza a estatus Inactiva para la notificacion de validacion
                var notificacion = (from n in db.mNotificaciones
                              where n.PlanSemanalId == idPlan
                              && n.TipoNotificacionId == 1
                              && n.EstatusId == 3
                              select n).ToList();

                var dbTempNot = notificacion.First();
                dbTempNot.EstatusId = 4;
                db.SaveChanges();


                return Json(new { Success = true, idPlan = idPlan, Message = "validado correctamente " });

            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = "Error al guardar la información" });
            }
        }

        [HttpPost]
        public JsonResult FeedbackPlan(int idPlan, int idUsuario, List<ComentariosFeedback> jsCommentarios)
        {
            try
            {
                var result = (from ps in db.mPlanSemanal
                              where ps.PlanSemanalId == idPlan
                              select ps).FirstOrDefault();

                //Actualiza a estatus con comentarios
                var dbTemp = result;
                dbTemp.EstatusId = 8;
                db.SaveChanges();

                //Revisa coordinador de zona al que pertenece
                //var coordinador = (from co in db.mCoordinacionZonaUsuario
                //                   where co.UsuarioId == idUsuario
                //                   select co).FirstOrDefault();


                var coordinacionJefe = (from co in db.mCoordinacionZonaUsuario
                                        where co.UsuarioId == result.UsuarioCreacionId
                                        && co.JefeCoordinacionZona == false
                                        select co).FirstOrDefault();

                //Genera notificacion de con comentarios
                mNotificaciones objNotificaciones = new mNotificaciones();

                objNotificaciones.EstatusId = 3;
                objNotificaciones.TipoNotificacionId = 3;
                objNotificaciones.PlanSemanalId = idPlan;
                objNotificaciones.UsuarioId = result.UsuarioCreacionId;
                objNotificaciones.CoordinacionZonaUsuarioId = coordinacionJefe.CordinacionZonaUsuarioId;
                objNotificaciones.FechaCreacion = DateTime.Now;
                db.mNotificaciones.Add(objNotificaciones);
                db.SaveChanges();

                //Actualiza a estatus Inactiva para la notificacion de validacion
                var notificacion = (from n in db.mNotificaciones
                                    where n.PlanSemanalId == idPlan
                                    && n.TipoNotificacionId == 1
                                    && n.EstatusId == 3
                                    select n).ToList();

                var dbTempNot = notificacion.First();
                dbTempNot.EstatusId = 4;
                db.SaveChanges();

                if (jsCommentarios != null)
                {
                    foreach (ComentariosFeedback objComentarios in jsCommentarios)
                    {
                        var resultDetallePlan = (from ps in db.dDetallePlanSemanal
                                                 where ps.DetallePlanId == objComentarios.id
                                                 select ps).FirstOrDefault();
                  
                        //Actualiza a estatus con comentarios
                        var dbTempDetalle = resultDetallePlan;
                        string comentarioAnt = (dbTempDetalle.ComentariosNoValidacion == null ? "" : dbTempDetalle.ComentariosNoValidacion + ", ");
                        dbTempDetalle.ComentariosNoValidacion = comentarioAnt + objComentarios.comentario;
                        db.SaveChanges();
                    }
                }


                return Json(new { Success = true, idPlan = idPlan, Message = "enviado correctamente para revisión " });

            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = "Error al guardar la información" });
            }
        }

        [HttpGet]
        public ActionResult Mapas(int id)
        {
            var coordenadas = (from c in db.mCheckIn
                               where c.DetallePlanId == id
                               select c).FirstOrDefault();
            
            string[] coor = Regex.Split(coordenadas.Coordenadas, ",");
            ViewBag.Coordenadas = coor[1] + "," + coor[0];
            return PartialView("_Mapa");
        }

        #endregion

        #region APROBACION DE PLAN SEMANAL
        public ActionResult AprobacionPlan()
        {
            return View();
        }

        [HttpGet]
        public JsonResult CargarPlanesAprobacion(int? idUsu, int? idEstatus, string Periodos, string TiposFigura, string NombresFigura)
        {
            try
            {
                List<udf_PlanSemanalAprobarList_Result> result = (from u in db.udf_PlanSemanalAprobarList(idUsu, Periodos, TiposFigura, NombresFigura) select u).ToList();
            
                var lstPlanSemanal = result.Select(C => new
                {
                    id = C.PlnSemanalId,
                    descripcionPlan = C.DescripcionPlan,
                    periodo = C.Periodo.ToUpper(),
                    usuario = C.Usuario,
                    tipoFigura = C.TipoFigura,
                    acciones = "<div class='btn-group btn-group-sm' role='group'>" +
                               "<button type='button' class='btn btn-success btnAprobar'" + (C.EstatusId == 9 || C.Aprobar == 0 ? "disabled" : "") + " id='btnAprobar_" + C.PlnSemanalId + "' data-idplan='" + C.PlnSemanalId + "'><i class='fa fa-check-circle fa-lg fa-fw'></i> " + (C.EstatusId == 9 ? " Aprobado" : " Aprobar") + "</button>" +
                               "<button type='button' class='btn btn-primary btnRptPlaneacion' id='btnRptPlaneacion_" + C.PlnSemanalId + "' data-idplan='" + C.PlnSemanalId + "' data-idusuarioplan='" + C.idUsuarioCreacion + "'><i class='fa fa-list fa-lg fa-fw'></i> Reporte Planeación</button>" +
                               "<button type='button' class='btn btn-warning btnRptActividades' id='btnRptActividades_" + C.PlnSemanalId + "' data-idplan='" + C.PlnSemanalId + "' data-idusuarioplan='" + C.idUsuarioCreacion + "'><i class='fa fa-list-alt fa-lg fa-fw'></i> Reporte Actividades</button>" +
                               "</div>"
                });

                return Json(new { Success = true, datos = lstPlanSemanal }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = "Error al obtener la información" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult CargarDetallePlanAprobacion(int idPlanSemanal)
        {
            try
            {
                var result = (from cat in db.dDetallePlanSemanal
                              where cat.PlanSemanalId == idPlanSemanal
                              select cat).OrderBy(x => new { x.FechaActividad, x.HoraActividad }).ToList();
                int Comparacion = db.Database.SqlQuery<int>("SELECT dbo.udf_ComparaFechas(@p0)", result.First().mPlanSemanal.PeriodoId).FirstOrDefault();

                var lstDetalle = result.Select(cat => new
                {
                    id = cat.DetallePlanId,
                    actividad = cat.cTipoActividades.NombreActividad + (cat.TipoActividadId == 8 && cat.SinProspectos == 1 ? "<br><span style='color:green'><strong>Sin prospectos</strong></span>" : (cat.InaebaPreregistros.Count() > 0 ? "<br> <a href='javascript:;' class='hrefVerEducandos' data-iddetalleplan='" + cat.DetallePlanId + "'>Ver Pre-registros Asignados</a>" : "")),
                    descripcion = cat.DescripcionActividad,
                    lugar = cat.LugarActividad,
                    fecha = cat.FechaActividad.ToString("MM/dd/yyyy"),
                    hora = cat.HoraActividad.ToString("hh':'mm"),
                    horaFin = cat.HoraFin == null ? "" : cat.HoraFin.Value.ToString("hh':'mm"),
                    checkin = (cat.cTipoActividades.RequiereCheckIn == true ? (cat.mCheckIn.Count() == 0 ? "No realizado" : "Realizado" + (cat.TipoActividadId  == 6 ? "<br><a href='javascript:;' class='btnCertificados' data-iddetalleplan='" + cat.DetallePlanId + "' style='text-decoration: none; color: #FC540A;'><strong>Ver Certificados</strong></a> " : ("<br> <button type='button' class='btn btn-xs btnCoordenadas' data-iddetalleplan='" + cat.DetallePlanId +"'><i class='fa fa-map-marker fa-lg fa-fw' style='color:red;'></i></button>" + (cat.mCheckIn.First().FotoIncidencia == null ? "" : "  <button type='button' class='btn btn-xs btnImagen' data-src='" + cat.mCheckIn.First().FotoIncidencia + "'><i class='fa fa-picture-o fa-lg fa-fw' style='color:green;'></i></button>")))) : "Realizado"),
                    incidencias = (cat.mCheckIn.Count() == 0 ? "" : cat.mCheckIn.FirstOrDefault().Incidencias),
                    gps = (cat.mSolicitudesVehiculo.Count() > 0 ? "<button type='button' class='btn btn-xs btn-primary btnGpsVehiculo' id='btnGpsVehiculo_" + cat.DetallePlanId + "' data-iddetalleplan='" + cat.DetallePlanId + "'><i class='fa fa-car fa-md fa-fw'></i> Reporte GPS</button><br>"  + "Matricula: <strong>" + cat.mSolicitudesVehiculo.FirstOrDefault().PlacaVehiculo + "</strong>" : ""),
                    comentariosRechazo = "<div class='row'>" +
                                            "<div class='col-md-2'><br><div class='material-switch'><input " + (cat.mPlanSemanal.EstatusId == 9 || Comparacion == 0 ? "disabled" : "") + " class='checkComents' " + (cat.ComentariosRechazo == null ? "" : "checked") + " id='checkComents_" + cat.DetallePlanId + "' type='checkbox' data-iddetalleplan='" + cat.DetallePlanId + "'/>" +
                                            "<label for='checkComents_" + cat.DetallePlanId + "' class='label-success'></label></div></div>" +
                                            "<div class='col-md-10'><textarea maxlength='100' rows='2' class='form-control idComentariosCZ' id='idComentariosCZ_" + cat.DetallePlanId + "' data-idplan='" + cat.PlanSemanalId + "' data-iddetalleplan='" + cat.DetallePlanId + "'" + (cat.ComentariosRechazo == null || cat.mPlanSemanal.EstatusId == 9 ? "disabled" : "enabled") + ">" + cat.ComentariosRechazo + "</textarea></div>" +
                                            "</div>"
                });



                return Json(new { Success = true, datos = lstDetalle }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = "Error al obtener la información" }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult AprobarPlan(int idPlan, int idUsuario, List<ComentariosFeedback> jsCommentarios)
        {
            try
            {
                var result = (from ps in db.mPlanSemanal
                              where ps.PlanSemanalId == idPlan
                              select ps).ToList();

                //Actualiza a estatus Validado
                var dbTemp = result.First();
                dbTemp.EstatusId = 9;
                db.SaveChanges();

                if (jsCommentarios != null)
                {
                    foreach (ComentariosFeedback objComentarios in jsCommentarios)
                    {
                        var resultDetallePlan = (from ps in db.dDetallePlanSemanal
                                                 where ps.DetallePlanId == objComentarios.id
                                                 select ps).FirstOrDefault();

                        //Actualiza a estatus con comentarios
                        var dbTempDetalle = resultDetallePlan;
                        dbTempDetalle.ComentariosRechazo = objComentarios.comentario;
                        dbTempDetalle.ActividadRechazada = true;
                        db.SaveChanges();
                    }
                }

                return Json(new { Success = true, idPlan = idPlan, Message = "aprobado correctamente " });

            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = "Error al guardar la información" });
            }
        }

        public void ExportPlanSemanal(int? idUsu, int? idEstatus, string Periodos, string TiposFigura, string NombresFigura, int idPlan)
        {
            try
            {
                List<udf_PlanSemanalAprobarList_Result> result = (from u in db.udf_PlanSemanalAprobarList(idUsu, Periodos, TiposFigura, NombresFigura) select u).Where(x => x.PlnSemanalId == idPlan).ToList();

                var lstPlanSemanal = result.Select(C => new
                {
                    id = C.PlnSemanalId,
                    descripcionPlan = C.DescripcionPlan,
                    periodo = C.Periodo.ToUpper(),
                    usuario = C.Usuario,
                    tipoFigura = C.TipoFigura,
                    cz = C.CoordinacionZona,
                    C.anio,
                    mes = C.FechaInicioPeriodo.Value.ToString("MMMM", new CultureInfo("es-ES")).ToUpper()
                });

                if (lstPlanSemanal.Count() > 0)
                {


                    using (ExcelPackage pkg = new ExcelPackage())
                    {
                        string usuarioPlan = "";
                        ExcelWorksheet ws = pkg.Workbook.Worksheets.Add("Reporte Planeación Semanal");
                        Color colFromHex = System.Drawing.ColorTranslator.FromHtml("#D9D9D9");
                        ws.Cells["A8"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        ws.Cells["F8:G8"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        ws.Cells["J8:K8"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        ws.Cells["N8"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        ws.Cells["A10:P10"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;

                        //Imagen logo GTO
                        Bitmap image = new Bitmap(Server.MapPath("~/Content/images/gto_logo.png"));
                        var picture = ws.Drawings.AddPicture("gtologo", image);
                        picture.SetPosition(0 * 1, 0, 0, 0);

                        //Imagen logo soy inaeba
                        Bitmap imageInaeba = new Bitmap(Server.MapPath("~/Content/images/logo-soy-inaeba.png"));
                        var pictureInaeba = ws.Drawings.AddPicture("soyinaebalogo", imageInaeba);
                        pictureInaeba.SetPosition(2 * 1, 0, 13, 0);

                        ws.Cells["B2:L2"].Merge = true;
                        ws.Cells["B2:L2"].Value = "INSTITUTO DE ALFABETIZACIÓN Y EDUCACIÓN BÁSICA PARA ADULTOS";
                        ws.Cells["B2:L2"].Style.Font.Bold = true;
                        ws.Cells["B2:L2"].AutoFitColumns();
                        ws.Cells["B2:L2"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        ws.Cells["B3:L3"].Merge = true;
                        ws.Cells["B3:L3"].Value = "PLAN DE ACTIVIDADES SEMANAL";
                        ws.Cells["B3:L3"].Style.Font.Bold = true;
                        ws.Cells["B3:L3"].AutoFitColumns();
                        ws.Cells["B3:L3"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        ws.Cells["E4:G4"].Merge = true;
                        ws.Cells["E4:G4"].Value = "COORDINACIÓN DE ZONA:";
                        ws.Cells["E4:G4"].Style.Font.Bold = true;

                        ws.Cells["H4:J4"].Merge = true;
                        ws.Cells["H4:J4"].Value = lstPlanSemanal.FirstOrDefault().cz;
                        ws.Cells["H4:J4"].Style.Font.Bold = true;

                        ws.Cells["A8"].Value = "PERSONAL OPERATIVO";
                        ws.Cells["A8"].Style.Font.Bold = true;
                        ws.Cells["A8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        ws.Cells["B8:E8"].Merge = true;
                        ws.Cells["B8:E8"].Value = lstPlanSemanal.FirstOrDefault().usuario;
                        ws.Cells["F8:G8"].Merge = true;
                        ws.Cells["F8:G8"].Value = "PERIODO";
                        ws.Cells["F8:G8"].Style.Font.Bold = true;
                        ws.Cells["F8:G8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        ws.Cells["H8:I8"].Merge = true;
                        ws.Cells["H8:I8"].Value = lstPlanSemanal.FirstOrDefault().periodo;
                        ws.Cells["J8:K8"].Merge = true;
                        ws.Cells["J8:K8"].Value = "MES";
                        ws.Cells["J8:K8"].Style.Font.Bold = true;
                        ws.Cells["J8:K8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        ws.Cells["L8:M8"].Merge = true;
                        ws.Cells["L8:M8"].Value = lstPlanSemanal.FirstOrDefault().mes;
                        ws.Cells["N8"].Value = "AÑO";
                        ws.Cells["N8"].Style.Font.Bold = true;
                        ws.Cells["N8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        ws.Cells["O8:P8"].Merge = true;
                        ws.Cells["O8:P8"].Value = lstPlanSemanal.FirstOrDefault().anio;
                        ws.Cells["A8:P8"].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Black);

                        usuarioPlan = lstPlanSemanal.FirstOrDefault().usuario;

                        ws.Cells["A8"].Style.Fill.BackgroundColor.SetColor(colFromHex);
                        ws.Cells["F8:G8"].Style.Fill.BackgroundColor.SetColor(colFromHex);
                        ws.Cells["J8:K8"].Style.Fill.BackgroundColor.SetColor(colFromHex);
                        ws.Cells["N8"].Style.Fill.BackgroundColor.SetColor(colFromHex);

                        ws.Cells["A10"].Value = "FECHA";
                        ws.Cells["A10"].Style.Font.Bold = true;
                        ws.Cells["A10"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        ws.Cells["B10"].Value = "LUGAR A VISITAR";
                        ws.Cells["B10"].Style.Font.Bold = true;
                        ws.Cells["B10"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        ws.Cells["C10"].Value = "HORA INICIO";
                        ws.Cells["C10"].Style.Font.Bold = true;
                        ws.Cells["C10"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        ws.Cells["D10"].Value = "HORA FIN";
                        ws.Cells["D10"].Style.Font.Bold = true;
                        ws.Cells["D10"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        ws.Cells["E10:H10"].Merge = true;
                        ws.Cells["E10:H10"].Value = "ACTIVIDADES A REALIZAR (ESPECIFICAR)";
                        ws.Cells["E10:H10"].Style.Font.Bold = true;
                        ws.Cells["E10:H10"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        ws.Cells["I10:M10"].Merge = true;
                        ws.Cells["I10:M10"].Value = "OBJETIVO (RESULTADO)";
                        ws.Cells["I10:M10"].Style.Font.Bold = true;
                        ws.Cells["I10:M10"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        ws.Cells["N10:P10"].Merge = true;
                        ws.Cells["N10:P10"].Value = "COMENTARIOS CZ";
                        ws.Cells["N10:P10"].Style.Font.Bold = true;
                        ws.Cells["N10:P10"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        ws.Cells["A10:P10"].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Black);



                        using (ExcelRange cell = ws.Cells["A10:P10"])
                        {
                            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                            cell.Style.Fill.BackgroundColor.SetColor(colFromHex);
                        }
                        var resultDetalle = (from cat in db.dDetallePlanSemanal
                                      where cat.PlanSemanalId == idPlan
                                      select cat).OrderBy(x => new { x.FechaActividad, x.HoraActividad }).ToList();

                        var lstDetalle = resultDetalle.Select(cat => new
                        {
                            id = cat.DetallePlanId,
                            actividad = cat.cTipoActividades.NombreActividad,
                            descripcion = cat.DescripcionActividad,
                            fecha = cat.FechaActividad.ToString("MM/dd/yyyy"),
                            hora = cat.HoraActividad.ToString("hh':'mm"),
                            horaFin = cat.HoraFin.Value.ToString("hh':'mm"),
                            checkin = (cat.cTipoActividades.RequiereCheckIn == true ? (cat.mCheckIn.Count() == 0 ? "No realizado" : "Realizado") : "Realizado"),
                            incidencias = (cat.mCheckIn.Count() == 0 ? "" : cat.mCheckIn.FirstOrDefault().Incidencias),
                            lugar = cat.LugarActividad,
                            comentarios = (cat.ComentariosNoValidacion == null ? "" :  "Validación: " + cat.ComentariosNoValidacion) + (cat.ComentariosRechazo == null ? "" : ", Aprobación: " + cat.ComentariosRechazo)
                        });

                        int i = 11;
                        foreach (var item in lstDetalle)
                        {
                            ws.Cells["A" + i.ToString()].Value = item.fecha;
                            ws.Cells["A" + i.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            ws.Cells["B" + i.ToString()].Value = item.lugar;
                            ws.Cells["B" + i.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            ws.Cells["C" + i.ToString()].Value = item.hora;
                            ws.Cells["C" + i.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            ws.Cells["D" + i.ToString()].Value = item.horaFin;
                            ws.Cells["D" + i.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            ws.Cells["E" + i.ToString() + ":H" + i.ToString()].Merge = true;
                            ws.Cells["E" + i.ToString() + ":H" + i.ToString()].Value = item.actividad;
                            ws.Cells["E" + i.ToString() + ":H" + i.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            ws.Cells["I" + i.ToString() + ":M" + i.ToString()].Merge = true;
                            ws.Cells["I" + i.ToString() + ":M" + i.ToString()].Value = item.incidencias;
                            ws.Cells["I" + i.ToString() + ":M" + i.ToString()].AutoFitColumns();
                            ws.Cells["I" + i.ToString() + ":M" + i.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            ws.Cells["N" + i.ToString() + ":P" + i.ToString()].Merge = true;
                            ws.Cells["N" + i.ToString() + ":P" + i.ToString()].Value = item.comentarios;
                            ws.Cells["N" + i.ToString() + ":P" + i.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                            i++;

                        }

                        ws.Cells[ws.Dimension.Address].AutoFitColumns();
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("content-disposition", "attachment;  filename=PlanSemanal_" + usuarioPlan + ".xlsx");
                        Response.BinaryWrite(pkg.GetAsByteArray());

                    }


                }

            }
            catch (Exception exp)
            {
                //return  exp.Message;
            }

        }

        public void ExportActividadesRealizadas(int? idUsu, int? idEstatus, string Periodos, string TiposFigura, string NombresFigura, int idPlan)
        {
            try
            {
                List<udf_PlanSemanalAprobarList_Result> result = (from u in db.udf_PlanSemanalAprobarList(idUsu, Periodos, TiposFigura, NombresFigura) select u).Where(x => x.PlnSemanalId == idPlan).ToList();

                var lstPlanSemanal = result.Select(C => new
                {
                    id = C.PlnSemanalId,
                    descripcionPlan = C.DescripcionPlan,
                    periodo = C.Periodo.ToUpper(),
                    usuario = C.Usuario,
                    tipoFigura = C.TipoFigura,
                    cz = C.CoordinacionZona,
                    C.anio,
                    mes = C.FechaInicioPeriodo.Value.ToString("MMMM", new CultureInfo("es-ES")).ToUpper(),
                    C.dia
                });

                if (lstPlanSemanal.Count() > 0)
                {


                    using (ExcelPackage pkg = new ExcelPackage())
                    {
                        string usuarioPlan = "";
                        ExcelWorksheet ws = pkg.Workbook.Worksheets.Add("Reporte Actividades Realizadas");
                        Color colFromHex = System.Drawing.ColorTranslator.FromHtml("#D9D9D9");
                        Color colorEncabezado = System.Drawing.ColorTranslator.FromHtml("#9CC3E6");
                        ws.Cells["B2"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        ws.Cells["E2:I2"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        ws.Cells["B4:D4"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        ws.Cells["F4"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        ws.Cells["H4:I4"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        ws.Cells["A6:I6"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;

                        ws.Cells["A2"].Value = "CZ";
                        ws.Cells["A2"].Style.Font.Bold = true;
                        ws.Cells["A2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                        ws.Cells["B2"].Value = lstPlanSemanal.FirstOrDefault().cz;
                        ws.Cells["B2"].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Black);
                        ws.Cells["C2:D2"].Merge = true;
                        ws.Cells["C2:D2"].Value = "PERSONAL OPERATIVO";
                        ws.Cells["C2:D2"].Style.Font.Bold = true;
                        ws.Cells["C2:D2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        ws.Cells["E2:I2"].Merge = true;
                        ws.Cells["E2:I2"].Value = lstPlanSemanal.FirstOrDefault().usuario;
                        ws.Cells["E2:I2"].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Black);

                        ws.Cells["A4"].Value = "PERIODO";
                        ws.Cells["A4"].Style.Font.Bold = true;
                        ws.Cells["A4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                        ws.Cells["B4:D4"].Merge = true;
                        ws.Cells["B4:D4"].Value = lstPlanSemanal.FirstOrDefault().periodo;
                        ws.Cells["B4:D4"].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Black);
                        ws.Cells["B4:D4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                        ws.Cells["E4"].Value = "MES";
                        ws.Cells["E4"].Style.Font.Bold = true;
                        ws.Cells["E4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        ws.Cells["F4"].Value = lstPlanSemanal.FirstOrDefault().mes;
                        ws.Cells["F4"].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Black);
                        ws.Cells["F4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                        ws.Cells["G4"].Value = "AÑO";
                        ws.Cells["G4"].Style.Font.Bold = true;
                        ws.Cells["G4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        ws.Cells["H4:I4"].Merge = true;
                        ws.Cells["H4:I4"].Value = lstPlanSemanal.FirstOrDefault().anio;
                        ws.Cells["H4:I4"].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Black);
                        ws.Cells["H4:I4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;

                        usuarioPlan = lstPlanSemanal.FirstOrDefault().usuario;

                        ws.Cells["B2"].Style.Fill.BackgroundColor.SetColor(colorEncabezado);
                        ws.Cells["E2:I2"].Style.Fill.BackgroundColor.SetColor(colorEncabezado);
                        ws.Cells["B4:D4"].Style.Fill.BackgroundColor.SetColor(colorEncabezado);
                        ws.Cells["F4"].Style.Fill.BackgroundColor.SetColor(colorEncabezado);
                        ws.Cells["H4:I4"].Style.Fill.BackgroundColor.SetColor(colorEncabezado);

                        ws.Cells["A6"].Value = "DIA";
                        ws.Cells["A6"].Style.Font.Bold = true;
                        ws.Cells["A6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        ws.Cells["B6"].Value = "FECHA";
                        ws.Cells["B6"].Style.Font.Bold = true;
                        ws.Cells["B6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        ws.Cells["C6"].Value = "HORA";
                        ws.Cells["C6"].Style.Font.Bold = true;
                        ws.Cells["C6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        ws.Cells["D6"].Value = "HORA";
                        ws.Cells["D6"].Style.Font.Bold = true;
                        ws.Cells["D6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        ws.Cells["E6"].Value = "ACTIVIDAD";
                        ws.Cells["E6"].Style.Font.Bold = true;
                        ws.Cells["E6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        ws.Cells["F6"].Value = "DESCRIPCIÓN ACTIVIDAD";
                        ws.Cells["F6"].Style.Font.Bold = true;
                        ws.Cells["F6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        ws.Cells["G6"].Value = "OBSERVACIONES";
                        ws.Cells["G6"].Style.Font.Bold = true;
                        ws.Cells["G6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        ws.Cells["H6"].Value = "ESTATUS ACTIVIDAD";
                        ws.Cells["H6"].Style.Font.Bold = true;
                        ws.Cells["H6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        ws.Cells["I6"].Value = "COMENTARIOS CZ";
                        ws.Cells["I6"].Style.Font.Bold = true;
                        ws.Cells["I6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        using (ExcelRange cell = ws.Cells["A6:I6"])
                        {
                            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                            cell.Style.Fill.BackgroundColor.SetColor(colorEncabezado);
                        }

                        var resultDetalle = (from cat in db.dDetallePlanSemanal
                                             where cat.PlanSemanalId == idPlan
                                             select cat).OrderBy(x => new { x.FechaActividad, x.HoraActividad }).ToList();

                        var lstDetalle = resultDetalle.Select(cat => new
                        {
                            id = cat.DetallePlanId,
                            actividad = cat.cTipoActividades.NombreActividad,
                            descripcion = cat.DescripcionActividad,
                            fecha = cat.FechaActividad.ToString("MM/dd/yyyy"),
                            hora = cat.HoraActividad.ToString("hh':'mm"),
                            horaFin = cat.HoraFin.Value.ToString("hh':'mm"),
                            checkin = (cat.cTipoActividades.RequiereCheckIn == true ? (cat.mCheckIn.Count() == 0 ? "No realizado" : "Realizado") : "Realizado"),
                            incidencias = (cat.mCheckIn.Count() == 0 ? "" : cat.mCheckIn.FirstOrDefault().Incidencias),
                            comentarios = cat.ComentariosRechazo,
                            dia = cat.FechaActividad.ToString("dddd", new CultureInfo("es-ES")).ToUpper()
                        });

                        int i = 7;
                        foreach (var item in lstDetalle)
                        {
                            ws.Cells["A" + i.ToString()].Value = item.dia;
                            ws.Cells["A" + i.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            ws.Cells["B" + i.ToString()].Value = item.fecha;
                            ws.Cells["B" + i.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            ws.Cells["C" + i.ToString()].Value = item.hora;
                            ws.Cells["C" + i.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            ws.Cells["D" + i.ToString()].Value = item.horaFin;
                            ws.Cells["D" + i.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            ws.Cells["E" + i.ToString()].Value = item.actividad;
                            ws.Cells["E" + i.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            ws.Cells["F" + i.ToString()].Value = item.descripcion;
                            ws.Cells["F" + i.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            ws.Cells["G" + i.ToString()].Value = item.incidencias;
                            ws.Cells["G" + i.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            ws.Cells["H" + i.ToString()].Value = item.checkin;
                            ws.Cells["H" + i.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            ws.Cells["I" + i.ToString()].Value = item.comentarios;
                            ws.Cells["I" + i.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                            i++;

                        }

                        ws.Cells[ws.Dimension.Address].AutoFitColumns();
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("content-disposition", "attachment;  filename=ActividadesRealizadas_" + usuarioPlan + ".xlsx");
                        Response.BinaryWrite(pkg.GetAsByteArray());

                    }


                }

            }
            catch (Exception exp)
            {
                //return  exp.Message;
            }

        }

        [HttpGet]
        public JsonResult ObtenerVehiculo(int idDetallePlan)
        {
            try
            {
                var detalleV = (from dv in db.mSolicitudesVehiculo
                              where dv.DetallePlanId == idDetallePlan
                                select dv).ToList();

                var detalleP = (from dp in db.dDetallePlanSemanal
                                where dp.DetallePlanId == idDetallePlan
                                select dp).FirstOrDefault();
                int idVehiculo = detalleV.FirstOrDefault().VehiculoId;
                if (detalleV.Count() > 0)
                {
                    var vehiculoSipae = (from v in dbSipae.Vehiculos
                                         where v.VehiculoId == idVehiculo
                                         select v).FirstOrDefault();
                    return (Json(new
                    {
                        Success = true,
                        placa = vehiculoSipae.Matricula,
                        fecha = detalleV.FirstOrDefault().FechaUso.ToString("yyyy-MM-dd"),
                        horaInicio = detalleP.HoraActividad.ToString(@"hh\:mm"),
                        horaFin = detalleP.HoraFin.Value.ToString(@"hh\:mm")
                    }, JsonRequestBehavior.AllowGet));
                }
                else
                {
                    return Json(new { Success = false, Message = "No contiene vehiculo asignado" }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = exp.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult Certificados()
        {

            return PartialView("_Certificados");
        }

        [HttpGet]
        public JsonResult CargarCertificados(int id)
        {
            try
            {
                var result = (from v in db.mLecturaCertificados
                              where v.mCheckIn.DetallePlanId == id
                              select v).ToList();

                var lstDetalle = result.Select(cat => new
                {
                    id = cat.LecturaCertificadoId,
                    folio = cat.FolioCertificado,
                    accion = ("<button type = 'button' class='btn btn-xs btnCoordenadas' data-iddetalleplan='" + cat.mCheckIn.DetallePlanId + "'><i class='fa fa-map-marker fa-lg fa-fw' style='color:red;'></i></button>" + (cat.mCheckIn.FotoIncidencia == null ? "" : "  <button type = 'button' class='btn btn-xs btnImagen' data-src='" + cat.mCheckIn.FotoIncidencia + "'><i class='fa fa-picture-o fa-lg fa-fw' style='color:green;'></i></button>"))
                });



                return Json(new { Success = true, datos = lstDetalle }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = "Error al obtener la información" }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

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