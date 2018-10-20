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
                List<udf_PlanSemanalValidarList_Result> result = (from u in db.udf_PlanSemanalValidarList(idUsu, Periodos, TiposFigura, NombresFigura) select u).Where(x => x.EstatusId == 10) .ToList();

                var lstPlanSemanal = result.Select(C => new
                {
                    id = C.PlnSemanalId,
                    descripcionPlan = C.DescripcionPlan,
                    periodo = C.Periodo.ToUpper(),
                    usuario = C.Usuario,
                    tipoFigura = C.TipoFigura,
                    acciones = "<div class='btn-group btn-group-sm' role='group'>" +
                               "<button type='button' class='btn btn-success btnValidar' id='btnValidar_" + C.PlnSemanalId + "' data-idplan='" + C.PlnSemanalId + "'><i class='fa fa-check-circle fa-lg fa-fw'></i> Validar</button>" +
                               "<button type='button' class='btn btn-warning btnFeedback' id='btnFeedback_" + C.PlnSemanalId + "' data-idplan='" + C.PlnSemanalId + "'><i class='fa fa-comment fa-lg fa-fw'></i> Feedback</button>" +
                               "</div>"
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
                    comentariosNoValidacion = "<div class='row'>" +
                                            "<div class='col-md-2'><br><div class='material-switch'><input class='checkComents' " + (cat.ComentariosNoValidacion == null ? "" : "checked") + " id='checkComents_" + cat.DetallePlanId + "' type='checkbox' data-iddetalleplan='" + cat.DetallePlanId + "'/>" +
                                            "<label for='checkComents_" + cat.DetallePlanId + "' class='label-success'></label></div></div>" +
                                            "<div class='col-md-10'><textarea rows='2' class='form-control idComentariosCZ' id='idComentariosCZ_" + cat.DetallePlanId + "' data-idplan='" + cat.PlanSemanalId + "' data-iddetalleplan='" + cat.DetallePlanId + "'" + (cat.ComentariosNoValidacion == null ? "disabled" : "enabled") + ">" + cat.ComentariosNoValidacion + "</textarea></div>" + 
                                            "</div>"
                });



                return Json(new { Success = true, datos = lstDetalle }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = exp.Message }, JsonRequestBehavior.AllowGet);
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
                return Json(new { Success = false, Message = exp.Message });
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
                var coordinador = (from co in db.mCoordinacionZonaUsuario
                                   where co.UsuarioId == idUsuario
                                   select co).FirstOrDefault();

                //Genera notificacion de con comentarios
                mNotificaciones objNotificaciones = new mNotificaciones();

                objNotificaciones.EstatusId = 3;
                objNotificaciones.TipoNotificacionId = 3;
                objNotificaciones.PlanSemanalId = idPlan;
                objNotificaciones.UsuarioId = result.UsuarioCreacionId;
                objNotificaciones.CoordinacionZonaUsuarioId = coordinador.CoordinacionZonaId;
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
                        dbTempDetalle.ComentariosNoValidacion = objComentarios.comentario;
                        db.SaveChanges();
                    }
                }


                return Json(new { Success = true, idPlan = idPlan, Message = "enviado correctamente para revisión " });

            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = exp.Message });
            }
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
                               "<button type='button' class='btn btn-success btnAprobar'" + (C.EstatusId == 9 ? "disabled" : "") + " id='btnAprobar_" + C.PlnSemanalId + "' data-idplan='" + C.PlnSemanalId + "'><i class='fa fa-check-circle fa-lg fa-fw'></i> " + (C.EstatusId == 9 ? " Aprobado" : " Aprobar") + "</button>" +
                               "<button type='button' class='btn btn-primary btnRptPlaneacion' id='btnRptPlaneacion_" + C.PlnSemanalId + "' data-idplan='" + C.PlnSemanalId + "' data-idusuarioplan='" + C.idUsuarioCreacion + "'><i class='fa fa-list fa-lg fa-fw'></i> Reporte Planeación</button>" +
                               "<button type='button' class='btn btn-warning btnRptActividades' id='btnRptActividades_" + C.PlnSemanalId + "' data-idplan='" + C.PlnSemanalId + "' data-idusuarioplan='" + C.idUsuarioCreacion + "'><i class='fa fa-list-alt fa-lg fa-fw'></i> Reporte Actividades</button>" +
                               "</div>"
                });

                return Json(new { Success = true, datos = lstPlanSemanal }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = exp.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult CargarDetallePlanAprobacion(int idPlanSemanal)
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
                    checkin = (cat.cTipoActividades.RequiereCheckIn == true ? (cat.mCheckIn.Count() == 0 ? "No realizado" : "Realizado") : "Realizado"),
                    incidencias = (cat.mCheckIn.Count() == 0 ? "" : cat.mCheckIn.FirstOrDefault().Incidencias),
                    gps = (cat.mSolicitudesVehiculo.Count() == 0 ? "<button type='button' class='btn btn-xs btn-primary btnGpsVehiculo' id='btnGpsVehiculo_" + cat.DetallePlanId + "' data-iddetalleplan='" + cat.DetallePlanId + "'><i class='fa fa-car fa-md fa-fw'></i> Reporte GPS</button>" : ""),
                    comentariosRechazo = "<div class='row'>" +
                                            "<div class='col-md-2'><br><div class='material-switch'><input " + (cat.mPlanSemanal.EstatusId == 9 ? "disabled" : "") + " class='checkComents' " + (cat.ComentariosRechazo == null ? "" : "checked") + " id='checkComents_" + cat.DetallePlanId + "' type='checkbox' data-iddetalleplan='" + cat.DetallePlanId + "'/>" +
                                            "<label for='checkComents_" + cat.DetallePlanId + "' class='label-success'></label></div></div>" +
                                            "<div class='col-md-10'><textarea rows='2' class='form-control idComentariosCZ' id='idComentariosCZ_" + cat.DetallePlanId + "' data-idplan='" + cat.PlanSemanalId + "' data-iddetalleplan='" + cat.DetallePlanId + "'" + (cat.ComentariosRechazo == null ? "disabled" : "enabled") + ">" + cat.ComentariosRechazo + "</textarea></div>" +
                                            "</div>"
                });



                return Json(new { Success = true, datos = lstDetalle }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = exp.Message }, JsonRequestBehavior.AllowGet);
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
                        db.SaveChanges();
                    }
                }

                return Json(new { Success = true, idPlan = idPlan, Message = "aprobado correctamente " });

            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = exp.Message });
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
                    anio = C.anio,
                    mes = C.mes
                });

                if (lstPlanSemanal.Count() > 0)
                {


                    using (ExcelPackage pkg = new ExcelPackage())
                    {
                        string usuarioPlan = "";
                        ExcelWorksheet ws = pkg.Workbook.Worksheets.Add("Reporte Planeación Semanal");
                        Color colFromHex = System.Drawing.ColorTranslator.FromHtml("#D9D9D9");
                        ws.Cells["A8"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        ws.Cells["D8"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        ws.Cells["F8"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        ws.Cells["H8"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        ws.Cells["J8"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        ws.Cells["A10:K10"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;

                        //Imagen logo GTO
                        Bitmap image = new Bitmap(Server.MapPath("~/Content/images/gto_logo.png"));
                        var picture = ws.Drawings.AddPicture("gtologo", image);
                        picture.SetPosition(0 * 1, 0, 0, 0);

                        //Imagen logo soy inaeba
                        Bitmap imageInaeba = new Bitmap(Server.MapPath("~/Content/images/logo-soy-inaeba.png"));
                        var pictureInaeba = ws.Drawings.AddPicture("soyinaebalogo", imageInaeba);
                        pictureInaeba.SetPosition(2 * 1, 0, 9, 0);

                        ws.Cells["B2:I2"].Merge = true;
                        ws.Cells["B2:I2"].Value = "INSTITUTO DE ALFABETIZACIÓN Y EDUCACIÓN BÁSICA PARA ADULTOS";
                        ws.Cells["B2:I2"].Style.Font.Bold = true;
                        ws.Cells["B2:I2"].AutoFitColumns();
                        ws.Cells["B2:I2"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        ws.Cells["B3:I3"].Merge = true;
                        ws.Cells["B3:I3"].Value = "PLAN DE ACTIVIDADES SEMANAL";
                        ws.Cells["B3:I3"].Style.Font.Bold = true;
                        ws.Cells["B3:I3"].AutoFitColumns();
                        ws.Cells["B3:I3"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        ws.Cells["D4:F4"].Merge = true;
                        ws.Cells["D4:F4"].Value = "COORDINACIÓN DE ZONA:";
                        ws.Cells["D4:F4"].Style.Font.Bold = true;

                        ws.Cells["A8"].Value = "PERSONAL OPERATIVO";
                        ws.Cells["A8"].Style.Font.Bold = true;
                        ws.Cells["A8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        ws.Cells["B8:E8"].Merge = true;
                        ws.Cells["B8:E8"].Value = lstPlanSemanal.FirstOrDefault().usuario;
                        ws.Cells["F8"].Value = "PERIODO";
                        ws.Cells["F8"].Style.Font.Bold = true;
                        ws.Cells["F8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        ws.Cells["G8"].Value = lstPlanSemanal.FirstOrDefault().periodo;
                        ws.Cells["H8"].Value = "MES";
                        ws.Cells["H8"].Style.Font.Bold = true;
                        ws.Cells["H8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        ws.Cells["I8"].Value = lstPlanSemanal.FirstOrDefault().mes;
                        ws.Cells["J8"].Value = "AÑO";
                        ws.Cells["J8"].Style.Font.Bold = true;
                        ws.Cells["J8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        ws.Cells["K8"].Value = lstPlanSemanal.FirstOrDefault().anio;
                        ws.Cells["A8:K8"].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Black);

                        usuarioPlan = lstPlanSemanal.FirstOrDefault().usuario;

                        ws.Cells["A8"].Style.Fill.BackgroundColor.SetColor(colFromHex);
                        ws.Cells["D8"].Style.Fill.BackgroundColor.SetColor(colFromHex);
                        ws.Cells["F8"].Style.Fill.BackgroundColor.SetColor(colFromHex);
                        ws.Cells["H8"].Style.Fill.BackgroundColor.SetColor(colFromHex);
                        ws.Cells["J8"].Style.Fill.BackgroundColor.SetColor(colFromHex);

                        ws.Cells["A10"].Value = "FECHA";
                        ws.Cells["A10"].Style.Font.Bold = true;
                        ws.Cells["A10"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        ws.Cells["B10"].Value = "HORA";
                        ws.Cells["B10"].Style.Font.Bold = true;
                        ws.Cells["B10"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        ws.Cells["C10:F10"].Merge = true;
                        ws.Cells["C10:F10"].Value = "ACTIVIDADES A REALIZAR (ESPECIFICAR)";
                        ws.Cells["C10:F10"].Style.Font.Bold = true;
                        ws.Cells["C10:F10"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        ws.Cells["G10:K10"].Merge = true;
                        ws.Cells["G10:K10"].Value = "OBJETIVO (RESULTADO)";
                        ws.Cells["G10:K10"].Style.Font.Bold = true;
                        ws.Cells["G10:K10"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        ws.Cells["A10:K10"].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Black);



                        using (ExcelRange cell = ws.Cells["A10:K10"])
                        {
                            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                            cell.Style.Fill.BackgroundColor.SetColor(colFromHex);
                        }
                        var resultDetalle = (from cat in db.dDetallePlanSemanal
                                      where cat.PlanSemanalId == idPlan
                                      select cat).ToList();

                        var lstDetalle = resultDetalle.Select(cat => new
                        {
                            id = cat.DetallePlanId,
                            actividad = cat.cTipoActividades.NombreActividad,
                            descripcion = cat.DescripcionActividad,
                            fecha = cat.FechaActividad.ToString("MM/dd/yyyy"),
                            hora = cat.HoraActividad.ToString("hh':'mm"),
                            checkin = (cat.cTipoActividades.RequiereCheckIn == true ? (cat.mCheckIn.Count() == 0 ? "No realizado" : "Realizado") : "Realizado"),
                            incidencias = (cat.mCheckIn.Count() == 0 ? "" : cat.mCheckIn.FirstOrDefault().Incidencias)
                        });

                        int i = 11;
                        foreach (var item in lstDetalle)
                        {
                            ws.Cells["A" + i.ToString()].Value = item.fecha;
                            ws.Cells["A" + i.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            ws.Cells["B" + i.ToString()].Value = item.hora;
                            ws.Cells["B" + i.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            ws.Cells["C" + i.ToString() + ":F" + i.ToString()].Merge = true;
                            ws.Cells["C" + i.ToString() + ":F" + i.ToString()].Value = item.actividad;
                            ws.Cells["C" + i.ToString() + ":F" + i.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            ws.Cells["G" + i.ToString() + ":K" + i.ToString()].Merge = true;
                            ws.Cells["G" + i.ToString() + ":K" + i.ToString()].Value = item.descripcion;
                            ws.Cells["G" + i.ToString() + ":K" + i.ToString()].AutoFitColumns();
                            ws.Cells["G" + i.ToString() + ":K" + i.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

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
                    mes = C.FechaInicioPeriodo.Value.ToString("MMMM", new CultureInfo("es-ES")),
                    C.dia
                });

                if (lstPlanSemanal.Count() > 0)
                {


                    using (ExcelPackage pkg = new ExcelPackage())
                    {
                        string usuarioPlan = "";
                        ExcelWorksheet ws = pkg.Workbook.Worksheets.Add("Reporte Actvidades Realizadas");
                        Color colFromHex = System.Drawing.ColorTranslator.FromHtml("#D9D9D9");
                        Color colorEncabezado = System.Drawing.ColorTranslator.FromHtml("#9CC3E6");
                        ws.Cells["B2"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        ws.Cells["E2:H2"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        ws.Cells["B4:D4"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        ws.Cells["F4"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        ws.Cells["H4"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        ws.Cells["A6:H6"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;

                        ws.Cells["A2"].Value = "CZ";
                        ws.Cells["A2"].Style.Font.Bold = true;
                        ws.Cells["A2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                        ws.Cells["B2"].Value = lstPlanSemanal.FirstOrDefault().cz;
                        ws.Cells["B2"].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Black);
                        ws.Cells["C2:D2"].Merge = true;
                        ws.Cells["C2:D2"].Value = "PERSONAL OPERATIVO";
                        ws.Cells["C2:D2"].Style.Font.Bold = true;
                        ws.Cells["C2:D2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        ws.Cells["E2:H2"].Merge = true;
                        ws.Cells["E2:H2"].Value = lstPlanSemanal.FirstOrDefault().usuario;
                        ws.Cells["E2:H2"].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Black);

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
                        ws.Cells["H4"].Value = lstPlanSemanal.FirstOrDefault().anio;
                        ws.Cells["H4"].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Black);
                        ws.Cells["H4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;

                        usuarioPlan = lstPlanSemanal.FirstOrDefault().usuario;

                        ws.Cells["B2"].Style.Fill.BackgroundColor.SetColor(colorEncabezado);
                        ws.Cells["E2:H2"].Style.Fill.BackgroundColor.SetColor(colorEncabezado);
                        ws.Cells["B4:D4"].Style.Fill.BackgroundColor.SetColor(colorEncabezado);
                        ws.Cells["F4"].Style.Fill.BackgroundColor.SetColor(colorEncabezado);
                        ws.Cells["H4"].Style.Fill.BackgroundColor.SetColor(colorEncabezado);

                        ws.Cells["A6"].Value = "DIA";
                        ws.Cells["A6"].Style.Font.Bold = true;
                        ws.Cells["A6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        ws.Cells["B6"].Value = "FECHA";
                        ws.Cells["B6"].Style.Font.Bold = true;
                        ws.Cells["B6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        ws.Cells["C6"].Value = "HORA";
                        ws.Cells["C6"].Style.Font.Bold = true;
                        ws.Cells["C6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        ws.Cells["D6"].Value = "ACTIVIDAD";
                        ws.Cells["D6"].Style.Font.Bold = true;
                        ws.Cells["D6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        ws.Cells["E6"].Value = "DESCRIPCIÓN ACTIVIDAD";
                        ws.Cells["E6"].Style.Font.Bold = true;
                        ws.Cells["E6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        ws.Cells["F6"].Value = "INCIDENCIA";
                        ws.Cells["F6"].Style.Font.Bold = true;
                        ws.Cells["F6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        ws.Cells["G6"].Value = "ESTATUS ACTIVIDAD";
                        ws.Cells["G6"].Style.Font.Bold = true;
                        ws.Cells["G6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        ws.Cells["H6"].Value = "COMENTARIOS CZ";
                        ws.Cells["H6"].Style.Font.Bold = true;
                        ws.Cells["H6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        using (ExcelRange cell = ws.Cells["A6:H6"])
                        {
                            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                            cell.Style.Fill.BackgroundColor.SetColor(colorEncabezado);
                        }

                        var resultDetalle = (from cat in db.dDetallePlanSemanal
                                             where cat.PlanSemanalId == idPlan
                                             select cat).ToList();

                        var lstDetalle = resultDetalle.Select(cat => new
                        {
                            id = cat.DetallePlanId,
                            actividad = cat.cTipoActividades.NombreActividad,
                            descripcion = cat.DescripcionActividad,
                            fecha = cat.FechaActividad.ToString("MM/dd/yyyy"),
                            hora = cat.HoraActividad.ToString("hh':'mm"),
                            checkin = (cat.cTipoActividades.RequiereCheckIn == true ? (cat.mCheckIn.Count() == 0 ? "No realizado" : "Realizado") : "Realizado"),
                            incidencias = (cat.mCheckIn.Count() == 0 ? "" : cat.mCheckIn.FirstOrDefault().Incidencias),
                            comentarios = cat.ComentariosRechazo,
                            dia = cat.FechaActividad.ToString("dddd", new CultureInfo("es-ES"))
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
                            ws.Cells["D" + i.ToString()].Value = item.actividad;
                            ws.Cells["D" + i.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            ws.Cells["E" + i.ToString()].Value = item.descripcion;
                            ws.Cells["E" + i.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            ws.Cells["F" + i.ToString()].Value = item.incidencias;
                            ws.Cells["F" + i.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            ws.Cells["G" + i.ToString()].Value = item.checkin;
                            ws.Cells["G" + i.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            ws.Cells["H" + i.ToString()].Value = item.comentarios;
                            ws.Cells["H" + i.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                            i++;

                        }

                        ws.Cells[ws.Dimension.Address].AutoFitColumns();
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("content-disposition", "attachment;  filename=ActvidadesRealizadas_" + usuarioPlan + ".xlsx");
                        Response.BinaryWrite(pkg.GetAsByteArray());

                    }


                }

            }
            catch (Exception exp)
            {
                //return  exp.Message;
            }

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