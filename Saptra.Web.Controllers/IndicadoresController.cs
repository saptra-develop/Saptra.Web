///Propósito:Controlador para la consulta de incidencias
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
using OfficeOpenXml.Style;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Text.RegularExpressions;
//using Spire.Xls;
//using Spire.Pdf;



using Saptra.Web.Data;
using Saptra.Web.Models;
using Saptra.Web.Utils;
//using Spire.Xls.Converter;

namespace Sispro.Web.Controllers
{
    public class IndicadoresController : Controller
    {
        private Inaeba_SaptraEntities db = new Inaeba_SaptraEntities();

        #region INDICADOR INCUMPLIMIENTO ACTIVIDADES POR PERIODO
        public ActionResult IncumplimientoAct()
        {
            return View();
        }

        [HttpGet]
        public JsonResult CargarIndRegion(string idPeriodo, string Region, string Zonas, string TipoActividad, int idUsuario)
        {
            try
            {
               
                if (Region == "")
                {
                    RegionesController regionesController = new RegionesController();
                    Region = regionesController.RegionesPorRol(idUsuario);
                }
                if (Zonas == "")
                {
                    ZonasController zonasController = new ZonasController();
                    Zonas = zonasController.ZonasPorRol(idUsuario);
                }

                List<udf_IncumplimientoActividadesPeriodoRegionList_Result> result = (from u in db.udf_IncumplimientoActividadesPeriodoRegionList(idPeriodo, Region, Zonas, TipoActividad) select u).ToList();

                var lstPlanSemanal = result.Select(C => new
                {
                    id = C.CoordinacionRegionId,
                    region = C.Region,
                    planeadastotal = C.PlaneadasTotal,
                    incumplidastotal = C.IncumplidasTotal,
                    pocentajepromotor = (C.PromotorPlanTotal == 0 ? 0 : (Convert.ToDecimal(C.PromotorIncumplio) / Convert.ToDecimal(C.PromotorPlanTotal))).ToString("0.00%"),
                    pocentajetecnico = (C.TecnicoPlan== 0 ? 0 : (Convert.ToDecimal(C.TecnicoIncumplio) / Convert.ToDecimal(C.TecnicoPlan))).ToString("0.00%"),
                    pocentajeformador = (C.FormadorPlan == 0 ? 0 : (Convert.ToDecimal(C.FormadorIncumplio) / Convert.ToDecimal(C.FormadorPlan))).ToString("0.00%"),
                });

                return Json(new { Success = true, datos = lstPlanSemanal }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = "Error al obtener la información" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult CargarIndRegionZona(string idPeriodo, string Region, string Zonas, string TipoActividad, int idUsuario)
        {
            try
            {
                if (Region == "")
                {
                    RegionesController regionesController = new RegionesController();
                    Region = regionesController.RegionesPorRol(idUsuario);
                }
                if (Zonas == "")
                {
                    ZonasController zonasController = new ZonasController();
                    Zonas = zonasController.ZonasPorRol(idUsuario);
                }
                List<udf_IncumplimientoActividadesPeriodoRegionZonaList_Result> result = (from u in db.udf_IncumplimientoActividadesPeriodoRegionZonaList(idPeriodo, Region, Zonas, TipoActividad) select u).ToList();

                var lstPlanSemanal = result.Select(C => new
                {
                    id = C.CoordinacionRegionId,
                    region = C.Region,
                    zona = C.Zona,
                    planeadastotal = C.PlaneadasTotal,
                    pocentajepromotor = (C.PromotorPlanTotal == 0 ? 0 : (Convert.ToDecimal(C.PromotorIncumplio) / Convert.ToDecimal(C.PromotorPlanTotal))).ToString("0.00%"),
                    pocentajetecnico = (C.TecnicoPlan == 0 ? 0 : (Convert.ToDecimal(C.TecnicoIncumplio) / Convert.ToDecimal(C.TecnicoPlan))).ToString("0.00%"),
                    pocentajeformador = (C.FormadorPlan == 0 ? 0 : (Convert.ToDecimal(C.FormadorIncumplio) / Convert.ToDecimal(C.FormadorPlan))).ToString("0.00%"),
                });

                return Json(new { Success = true, datos = lstPlanSemanal }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = "Error al obtener la información" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult CargarIndRegionZonaTipoActividad(string idPeriodo, string Region, string Zonas, string TipoActividad,int idUsuario)
        {
            try
            {
                if (Region == "")
                {
                    RegionesController regionesController = new RegionesController();
                    Region = regionesController.RegionesPorRol(idUsuario);
                }
                if (Zonas == "")
                {
                    ZonasController zonasController = new ZonasController();
                    Zonas = zonasController.ZonasPorRol(idUsuario);
                }
                string JSONString = string.Empty;
                string header = "<th>";
                string headerT = "Tipo Actividad";
                var dataConsult = (from u in db.udf_IncumplimientoActividadesPeriodoZonaActividadList(idPeriodo, Region, Zonas, TipoActividad)
                              group u by new { u.Actividad, u.Zona } into groupZona
                              select new
                              {
                                  actividad = groupZona.Key.Actividad,
                                  zona = groupZona.Key.Zona,
                                  planeadastotal = (groupZona.Sum(s => s.PlaneadasTotal) == 0 ? 0 : groupZona.Sum(s => s.PlaneadasTotal)),
                                  incumplidastotal = (groupZona.Sum(s => s.IncumplidasTotal) == 0 ? 0 : groupZona.Sum(s => s.IncumplidasTotal)),
                              }).ToList();

                var headerZona = (from pr in dataConsult
                                  select pr.zona).Distinct().ToArray();

                var rowActividad = (from s in dataConsult
                                       select s.actividad).Distinct().ToList();

                if (dataConsult.Count > 0)
                {
                    System.Data.DataTable resultData = new System.Data.DataTable();
                    resultData.Columns.Add("Tipo Actividad");
                    header += "Tipo Actividad</th>";
                    foreach (var itemP in headerZona)
                    {
                        resultData.Columns.Add(itemP);
                        header += "<th style='text-align:center;'>" + itemP + "</th>";
                        headerT += "," + itemP;
                    }

                    foreach (var row in rowActividad)
                    {
                        var result = (from u in dataConsult where u.actividad == row select u).ToList();
                        object[] dTabla = new object[resultData.Columns.Count];
                        dTabla[0] = row;
                        for (int i = 0; i < headerZona.Count(); i++)
                        {
                            var dato = (from d in result
                                        where d.zona == headerZona[i]
                                        select d).FirstOrDefault();
                            if (dato != null && dato.planeadastotal != 0)
                            {
                                dTabla[1 + i] = Math.Round((Convert.ToDecimal(dato.incumplidastotal) / Convert.ToDecimal(dato.planeadastotal)),2) * 100;
                            }
                            else
                                dTabla[1 + i] = 0;
                        }
                        resultData.Rows.Add(dTabla);
                    }

                    JSONString = JsonConvert.SerializeObject(resultData);
                }

                //    var lstPlanSemanal = result.Select(C => new
                //{
                //    id = C.CoordinacionRegionId,
                //    region = C.Region,
                //    zona = C.Zona,
                //    planeadastotal = C.PlaneadasTotal,
                //    pocentajepromotor = (C.PromotorPlanTotal == 0 ? 0 : (C.PromotorIncumplio / C.PromotorPlanTotal)).Value.ToString("0.00%"),
                //    pocentajetecnico = (C.TecnicoPlan == 0 ? 0 : (C.TecnicoIncumplio / C.TecnicoPlan)).Value.ToString("0.00%"),
                //    pocentajeformador = (C.FormadorPlan == 0 ? 0 : (C.FormadorIncumplio / C.TecnicoPlan)).Value.ToString("0.00%"),
                //});

                return Json(new { Success = true, datos = JSONString, header = header, hearderText = headerT }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = "Error al obtener la información" }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult CargarIndRegionZonaActividad(string idPeriodo, string Region, string Zonas, string TipoActividad, int idUsuario)
        {
            try
            {
                if (Region == "")
                {
                    RegionesController regionesController = new RegionesController();
                    Region = regionesController.RegionesPorRol(idUsuario);
                }
                if (Zonas == "")
                {
                    ZonasController zonasController = new ZonasController();
                    Zonas = zonasController.ZonasPorRol(idUsuario);
                }

                var dataConsult = (from u in db.udf_IncumplimientoActividadesPeriodoZonaActividadList(idPeriodo, Region, Zonas, TipoActividad)
                                   group u by new { u.Region, u.Zona, u.Actividad  } into groupZona
                                   select new
                                   {
                                       region = groupZona.Key.Region,
                                       zona = groupZona.Key.Zona,
                                       actividad = groupZona.Key.Actividad,
                                       promotorPlan = groupZona.Sum(s => s.PromotorPlanTotal) == 0 ? 0 : groupZona.Sum(s => s.PromotorPlanTotal),
                                       promotorIncumplio = groupZona.Sum(s => s.PromotorIncumplio) == 0 ? 0 : groupZona.Sum(s => s.PromotorIncumplio),
                                       tecnicoPlan = groupZona.Sum(s => s.TecnicoPlan) == 0 ? 0 : groupZona.Sum(s => s.TecnicoPlan),
                                       tecnicoIncumplio = groupZona.Sum(s => s.TecnicoIncumplio) == 0 ? 0 : groupZona.Sum(s => s.TecnicoIncumplio),
                                       formadorPlan = groupZona.Sum(s => s.FormadorPlan) == 0 ? 0 : groupZona.Sum(s => s.FormadorPlan),
                                       formadorIncumplio = groupZona.Sum(s => s.FormadorIncumplio) == 0 ? 0 : groupZona.Sum(s => s.FormadorIncumplio),
                                   }).ToList();

                var lstPlanSemanal = dataConsult.Select(C => new
                {
                    id = C.region + "-" + C.zona + "-" + C.actividad,
                    C.region,
                    C.zona,
                    C.actividad,
                    promotorIncumplio = (C.promotorPlan == 0 ? 0 : (Convert.ToDecimal(C.promotorIncumplio) / Convert.ToDecimal(C.promotorPlan))).ToString("0.00%"),
                    tecnicoIncumplio = (C.tecnicoPlan == 0 ? 0 : (Convert.ToDecimal(C.tecnicoIncumplio) / Convert.ToDecimal(C.tecnicoPlan))).ToString("0.00%"),
                    formadorIncumplio = (C.formadorPlan == 0 ? 0 : (Convert.ToDecimal(C.formadorIncumplio) / Convert.ToDecimal(C.formadorPlan))).ToString("0.00%"),
                });

                return Json(new { Success = true, datos = lstPlanSemanal }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = "Error al obtener la información" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult CargarIndRegionGrafico(string idPeriodo, string Region, string Zonas, string TipoActividad, int idUsuario)
        {
            try
            {
                if (Region == "")
                {
                    RegionesController regionesController = new RegionesController();
                    Region = regionesController.RegionesPorRol(idUsuario);
                }
                if (Zonas == "")
                {
                    ZonasController zonasController = new ZonasController();
                    Zonas = zonasController.ZonasPorRol(idUsuario);
                }

                var modelCRegion = new List<object>();
                var dataCFigura = new List<object>();

                var dataCPromotor = new List<object>();
                var dataCTecnico = new List<object>();
                var dataCFormador = new List<object>();


                var ltsGenFob = (from fg in db.udf_IncumplimientoActividadesPeriodoRegionList(idPeriodo, Region, Zonas, TipoActividad)
                                 select fg).OrderByDescending(a => a.Region).ToList();

                var lstPlanSemanal = ltsGenFob.Select(C => new
                {
                    id = C.CoordinacionRegionId,
                    region = C.Region,
                    planeadastotal = C.PlaneadasTotal,
                    incumplidastotal = C.IncumplidasTotal,
                    pocentajepromotor = (C.PromotorPlanTotal == 0 ? 0 : (Convert.ToDecimal(C.PromotorIncumplio) / Convert.ToDecimal(C.PromotorPlanTotal))).ToString("0.00%"),
                    pocentajetecnico = (C.TecnicoPlan == 0 ? 0 : (Convert.ToDecimal(C.TecnicoIncumplio) / Convert.ToDecimal(C.TecnicoPlan))).ToString("0.00%"),
                    pocentajeformador = (C.FormadorPlan == 0 ? 0 : (Convert.ToDecimal(C.FormadorIncumplio) / Convert.ToDecimal(C.FormadorPlan))).ToString("0.00%"),
                });
                if (ltsGenFob.Count > 0)
                {
                    foreach (var item in lstPlanSemanal)
                    {
                        modelCRegion.Add(new { label = item.region });
                        dataCPromotor.Add(new { value = item.pocentajepromotor });
                        dataCTecnico.Add(new { value = item.pocentajetecnico });
                        dataCFormador.Add(new { value = item.pocentajeformador });
                    }
                    dataCFigura.Add(new
                    {
                        seriesname = "Promotor",
                        data = dataCPromotor
                    });
                    dataCFigura.Add(new
                    {
                        seriesname = "Tecnico docente",
                        data = dataCTecnico
                    });
                    dataCFigura.Add(new
                    {
                        seriesname = "Formador",
                        data = dataCFormador
                    });
                  
                }
                return (Json(new { modelCRegion, dataCFigura }, JsonRequestBehavior.AllowGet));
            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = exp.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region INDICADOR FIGURAS REINCIDENTES VALIDACION
        public ActionResult FigurasReincidentes()
        {
            return View();
        }

        [HttpGet]
        public JsonResult CargarIndFigurasReincidentesRegion(string idPeriodo, string Region, string Zonas, int idUsuario)
        {
            try
            {

                if (Region == "")
                {
                    RegionesController regionesController = new RegionesController();
                    Region = regionesController.RegionesPorRol(idUsuario);
                }
                if (Zonas == "")
                {
                    ZonasController zonasController = new ZonasController();
                    Zonas = zonasController.ZonasPorRol(idUsuario);
                } 
                List<udf_RechazoValidacionRegionList_Result> result = (from u in db.udf_RechazoValidacionRegionList(idPeriodo, Region, Zonas, "") select u).ToList();

                var lstPlanSemanal = result.Select(C => new
                {
                    C.Region,
                    planeadastotal = C.capturadasTotal,
                    incumplidastotal = C.rechazadasTotal,
                    porcentajeRechazadasT = (C.capturadasTotal == 0 ? 0 : (Convert.ToDecimal(C.rechazadasTotal) / Convert.ToDecimal(C.capturadasTotal))).ToString("0.00%"),
                    pocentajePromotor = (C.capturadasPromotorTotal == 0 ? 0 : (Convert.ToDecimal(C.rechazadasPromotorTotal) / Convert.ToDecimal(C.capturadasPromotorTotal))).ToString("0.00%"),
                    pocentajeTecnico = (C.capturadasTecnicoTotal == 0 ? 0 : (Convert.ToDecimal(C.rechazadasTecnicoTotal) / Convert.ToDecimal(C.capturadasTecnicoTotal))).ToString("0.00%"),
                    pocentajeFormador = (C.capturadasFormadorTotal == 0 ? 0 : (Convert.ToDecimal(C.rechazadasFormadorTotal) / Convert.ToDecimal(C.capturadasFormadorTotal))).ToString("0.00%"),
                });

                return Json(new { Success = true, datos = lstPlanSemanal }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = "Error al obtener la información" }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        public JsonResult CargarIndFigurasReincidentesRegionZona(string idPeriodo, string Region, string Zonas, int idUsuario)
        {
            try
            {
                if (Region == "")
                {
                    RegionesController regionesController = new RegionesController();
                    Region = regionesController.RegionesPorRol(idUsuario);
                }
                if (Zonas == "")
                {
                    ZonasController zonasController = new ZonasController();
                    Zonas = zonasController.ZonasPorRol(idUsuario);
                }
                List<udf_RechazoValidacionZonaList_Result> result = (from u in db.udf_RechazoValidacionZonaList(idPeriodo, Region, Zonas, "") select u).ToList();

                var lstPlanSemanal = result.Select(C => new
                {
                    C.Region,
                    C.Zona,
                    planeadastotal = C.capturadasTotal,
                    incumplidastotal = C.rechazadasTotal,
                    porcentajeRechazadasT = (C.capturadasTotal == 0 ? 0 : (Convert.ToDecimal(C.rechazadasTotal) / Convert.ToDecimal(C.capturadasTotal))).ToString("0.00%"),
                    pocentajePromotor = (C.capturadasPromotorTotal == 0 ? 0 : (Convert.ToDecimal(C.rechazadasPromotorTotal) / Convert.ToDecimal(C.capturadasPromotorTotal))).ToString("0.00%"),
                    pocentajeTecnico = (C.capturadasTecnicoTotal == 0 ? 0 : (Convert.ToDecimal(C.rechazadasTecnicoTotal) / Convert.ToDecimal(C.capturadasTecnicoTotal))).ToString("0.00%"),
                    pocentajeFormador = (C.capturadasFormadorTotal == 0 ? 0 : (Convert.ToDecimal(C.rechazadasFormadorTotal) / Convert.ToDecimal(C.capturadasFormadorTotal))).ToString("0.00%"),
                });

                return Json(new { Success = true, datos = lstPlanSemanal }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = "Error al obtener la información" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult CargarIndFigurasReincidentesRegionZonaFigNom(string idPeriodo, string Region, string Zonas, int idUsuario)
        {
            try
            {
                if (Region == "")
                {
                    RegionesController regionesController = new RegionesController();
                    Region = regionesController.RegionesPorRol(idUsuario);
                }
                if (Zonas == "")
                {
                    ZonasController zonasController = new ZonasController();
                    Zonas = zonasController.ZonasPorRol(idUsuario);
                }
                List<udf_RechazoValidacionZonaFiguraNombreValidacion_Result> result = (from u in db.udf_RechazoValidacionZonaFiguraNombreValidacion(idPeriodo, Region, Zonas, "") select u).ToList();

                var lstPlanSemanal = result.Select(C => new
                {
                    C.Periodo,
                    C.Region,
                    C.Zona,
                    C.TipoFigura,
                    C.NombreFigura,
                    planeadastotal = C.capturadasTotal,
                    incumplidastotal = C.rechazadasTotal,
                    porcentajeRechazadasT = (C.capturadasTotal == 0 ? 0 : (Convert.ToDecimal(C.rechazadasTotal) / Convert.ToDecimal(C.capturadasTotal))).ToString("0.00%")
                    //porcentajeRechazoFigura = (C.TipoFiguraId == 1 ? 
                    //(C.capturadasPromotorTotal == 0 ? 0 : (Convert.ToDecimal(C.rechazadasPromotorTotal) / Convert.ToDecimal(C.capturadasPromotorTotal))).ToString("0.00%")
                    //: (C.TipoFiguraId == 2 ? 
                    //(C.capturadasTecnicoTotal == 0 ? 0 : (Convert.ToDecimal(C.rechazadasTecnicoTotal) / Convert.ToDecimal(C.capturadasTecnicoTotal))).ToString("0.00%")
                    // :
                    //(C.capturadasFormadorTotal == 0 ? 0 : (Convert.ToDecimal(C.rechazadasFormadorTotal) / Convert.ToDecimal(C.capturadasFormadorTotal))).ToString("0.00%")
                    //)
                    //)
                });

                return Json(new { Success = true, datos = lstPlanSemanal }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = "Error al obtener la información" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult CargarIndFigurasReincidentesRegionZonaFigNomAct(string idPeriodo, string Region, string Zonas, int idUsuario)
        {
            try
            {
                if (Region == "")
                {
                    RegionesController regionesController = new RegionesController();
                    Region = regionesController.RegionesPorRol(idUsuario);
                }
                if (Zonas == "")
                {
                    ZonasController zonasController = new ZonasController();
                    Zonas = zonasController.ZonasPorRol(idUsuario);
                }
                List<udf_RechazoValidacionZonaFiguraNombreValidacionAct_Result> result = (from u in db.udf_RechazoValidacionZonaFiguraNombreValidacionAct(idPeriodo, Region, Zonas, "") select u).ToList();

                var lstPlanSemanal = result.Select(C => new
                {
                    C.Periodo,
                    C.Region,
                    C.Zona,
                    C.TipoFigura,
                    C.NombreFigura,
                    C.DescripcionActividad,
                    C.ComentariosNoValidacion
                });

                return Json(new { Success = true, datos = lstPlanSemanal }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = "Error al obtener la información" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult CargarIndFigurasReincidentesRegionGrafico(string idPeriodo, string Region, string Zonas, string TipoActividad, int idUsuario)
        {
            try
            {
                if (Region == "")
                {
                    RegionesController regionesController = new RegionesController();
                    Region = regionesController.RegionesPorRol(idUsuario);
                }
                if (Zonas == "")
                {
                    ZonasController zonasController = new ZonasController();
                    Zonas = zonasController.ZonasPorRol(idUsuario);
                }

                var modelCRegion = new List<object>();
                var dataCFigura = new List<object>();

                var dataCPromotor = new List<object>();
                var dataCTecnico = new List<object>();
                var dataCFormador = new List<object>();


                var ltsGenFob = (from fg in db.udf_RechazoValidacionRegionList(idPeriodo, Region, Zonas, "")
                                 select fg).OrderByDescending(a => a.Region).ToList();

                var lstPlanSemanal = ltsGenFob.Select(C => new
                {
                    C.Region,
                    planeadastotal = C.capturadasTotal,
                    incumplidastotal = C.rechazadasTotal,
                    porcentajeRechazadasT = (C.capturadasTotal == 0 ? 0 : (Convert.ToDecimal(C.rechazadasTotal) / Convert.ToDecimal(C.capturadasTotal))).ToString("0.00%"),
                    pocentajePromotor = (C.capturadasPromotorTotal == 0 ? 0 : (Convert.ToDecimal(C.rechazadasPromotorTotal) / Convert.ToDecimal(C.capturadasPromotorTotal))).ToString("0.00%"),
                    pocentajeTecnico = (C.capturadasTecnicoTotal == 0 ? 0 : (Convert.ToDecimal(C.rechazadasTecnicoTotal) / Convert.ToDecimal(C.capturadasTecnicoTotal))).ToString("0.00%"),
                    pocentajeFormador = (C.capturadasFormadorTotal == 0 ? 0 : (Convert.ToDecimal(C.rechazadasFormadorTotal) / Convert.ToDecimal(C.capturadasFormadorTotal))).ToString("0.00%"),
                });

                if (ltsGenFob.Count > 0)
                {
                    foreach (var item in lstPlanSemanal)
                    {
                        modelCRegion.Add(new { label = item.Region });
                        dataCPromotor.Add(new { value = item.pocentajePromotor });
                        dataCTecnico.Add(new { value = item.pocentajeTecnico });
                        dataCFormador.Add(new { value = item.pocentajeFormador });
                    }
                    dataCFigura.Add(new
                    {
                        seriesname = "Promotor",
                        data = dataCPromotor
                    });
                    dataCFigura.Add(new
                    {
                        seriesname = "Tecnico docente",
                        data = dataCTecnico
                    });
                    dataCFigura.Add(new
                    {
                        seriesname = "Formador",
                        data = dataCFormador
                    });

                }
                return (Json(new { modelCRegion, dataCFigura }, JsonRequestBehavior.AllowGet));
            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = exp.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult CargarIndFigurasReincidentesRegionZonaGrafico(string idPeriodo, string Region, string Zonas, string TipoActividad, int idUsuario)
        {
            try
            {
                if (Region == "")
                {
                    RegionesController regionesController = new RegionesController();
                    Region = regionesController.RegionesPorRol(idUsuario);
                }
                if (Zonas == "")
                {
                    ZonasController zonasController = new ZonasController();
                    Zonas = zonasController.ZonasPorRol(idUsuario);
                }

                var modelCZona = new List<object>();
                var dataCFigura = new List<object>();

                var dataCPromotor = new List<object>();
                var dataCTecnico = new List<object>();
                var dataCFormador = new List<object>();


                var ltsGenFob = (from fg in db.udf_RechazoValidacionZonaList(idPeriodo, Region, Zonas, "")
                                 select fg).OrderByDescending(a => a.Zona).ToList();

                var lstPlanSemanal = ltsGenFob.Select(C => new
                {
                    C.Zona,
                    planeadastotal = C.capturadasTotal,
                    incumplidastotal = C.rechazadasTotal,
                    porcentajeRechazadasT = (C.capturadasTotal == 0 ? 0 : (Convert.ToDecimal(C.rechazadasTotal) / Convert.ToDecimal(C.capturadasTotal))).ToString("0.00%"),
                    pocentajePromotor = (C.capturadasPromotorTotal == 0 ? 0 : (Convert.ToDecimal(C.rechazadasPromotorTotal) / Convert.ToDecimal(C.capturadasPromotorTotal))).ToString("0.00%"),
                    pocentajeTecnico = (C.capturadasTecnicoTotal == 0 ? 0 : (Convert.ToDecimal(C.rechazadasTecnicoTotal) / Convert.ToDecimal(C.capturadasTecnicoTotal))).ToString("0.00%"),
                    pocentajeFormador = (C.capturadasFormadorTotal == 0 ? 0 : (Convert.ToDecimal(C.rechazadasFormadorTotal) / Convert.ToDecimal(C.capturadasFormadorTotal))).ToString("0.00%"),
                });

                if (ltsGenFob.Count > 0)
                {
                    foreach (var item in lstPlanSemanal)
                    {
                        modelCZona.Add(new { label = item.Zona });
                        dataCPromotor.Add(new { value = item.pocentajePromotor });
                        dataCTecnico.Add(new { value = item.pocentajeTecnico });
                        dataCFormador.Add(new { value = item.pocentajeFormador });
                    }
                    dataCFigura.Add(new
                    {
                        seriesname = "Promotor",
                        data = dataCPromotor
                    });
                    dataCFigura.Add(new
                    {
                        seriesname = "Tecnico docente",
                        data = dataCTecnico
                    });
                    dataCFigura.Add(new
                    {
                        seriesname = "Formador",
                        data = dataCFormador
                    });

                }
                return (Json(new { modelCZona, dataCFigura }, JsonRequestBehavior.AllowGet));
            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = exp.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region INDICADOR FIGURAS CONOBSERVACIONES APROBACION
        public ActionResult FigurasObservaciones()
        {
            return View();
        }

        [HttpGet]
        public JsonResult CargarIndFigurasObservacionesRegion(string idPeriodo, string Region, string Zonas, string TipoActividad, int idUsuario)
        {
            try
            {
                if (Region == "")
                {
                    RegionesController regionesController = new RegionesController();
                    Region = regionesController.RegionesPorRol(idUsuario);
                }
                if (Zonas == "")
                {
                    ZonasController zonasController = new ZonasController();
                    Zonas = zonasController.ZonasPorRol(idUsuario);
                }
                List<udf_RechazoAprobacionRegionList_Result> result = (from u in db.udf_RechazoAprobacionRegionList(idPeriodo, Region, Zonas, "") select u).ToList();

                var lstPlanSemanal = result.Select(C => new
                {
                    C.Region,
                    planeadastotal = C.capturadasTotal,
                    incumplidastotal = C.rechazadasTotal,
                    porcentajeRechazadasT = (C.capturadasTotal == 0 ? 0 : (Convert.ToDecimal(C.rechazadasTotal) / Convert.ToDecimal(C.capturadasTotal))).ToString("0.00%"),
                    pocentajePromotor = (C.capturadasPromotorTotal == 0 ? 0 : (Convert.ToDecimal(C.rechazadasPromotorTotal) / Convert.ToDecimal(C.capturadasPromotorTotal))).ToString("0.00%"),
                    pocentajeTecnico = (C.capturadasTecnicoTotal == 0 ? 0 : (Convert.ToDecimal(C.rechazadasTecnicoTotal) / Convert.ToDecimal(C.capturadasTecnicoTotal))).ToString("0.00%"),
                    pocentajeFormador = (C.capturadasFormadorTotal == 0 ? 0 : (Convert.ToDecimal(C.rechazadasFormadorTotal) / Convert.ToDecimal(C.capturadasFormadorTotal))).ToString("0.00%"),
                });

                return Json(new { Success = true, datos = lstPlanSemanal }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = "Error al obtener la información" }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        public JsonResult CargarIndFigurasObservacionesRegionZona(string idPeriodo, string Region, string Zonas, string TipoActividad, int idUsuario)
        {
            try
            {
                if (Region == "")
                {
                    RegionesController regionesController = new RegionesController();
                    Region = regionesController.RegionesPorRol(idUsuario);
                }
                if (Zonas == "")
                {
                    ZonasController zonasController = new ZonasController();
                    Zonas = zonasController.ZonasPorRol(idUsuario);
                }
                List<udf_RechazoAprobacionZonaList_Result> result = (from u in db.udf_RechazoAprobacionZonaList(idPeriodo, Region, Zonas, "") select u).ToList();

                var lstPlanSemanal = result.Select(C => new
                {
                    C.Region,
                    C.Zona,
                    planeadastotal = C.capturadasTotal,
                    incumplidastotal = C.rechazadasTotal,
                    porcentajeRechazadasT = (C.capturadasTotal == 0 ? 0 : (Convert.ToDecimal(C.rechazadasTotal) / Convert.ToDecimal(C.capturadasTotal))).ToString("0.00%"),
                    pocentajePromotor = (C.capturadasPromotorTotal == 0 ? 0 : (Convert.ToDecimal(C.rechazadasPromotorTotal) / Convert.ToDecimal(C.capturadasPromotorTotal))).ToString("0.00%"),
                    pocentajeTecnico = (C.capturadasTecnicoTotal == 0 ? 0 : (Convert.ToDecimal(C.rechazadasTecnicoTotal) / Convert.ToDecimal(C.capturadasTecnicoTotal))).ToString("0.00%"),
                    pocentajeFormador = (C.capturadasFormadorTotal == 0 ? 0 : (Convert.ToDecimal(C.rechazadasFormadorTotal) / Convert.ToDecimal(C.capturadasFormadorTotal))).ToString("0.00%"),
                });

                return Json(new { Success = true, datos = lstPlanSemanal }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = "Error al obtener la información" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult CargarIndFigurasObservacionesRegionZonaFigNom(string idPeriodo, string Region, string Zonas, string TipoActividad, int idUsuario)
        {
            try
            {
                if (Region == "")
                {
                    RegionesController regionesController = new RegionesController();
                    Region = regionesController.RegionesPorRol(idUsuario);
                }
                if (Zonas == "")
                {
                    ZonasController zonasController = new ZonasController();
                    Zonas = zonasController.ZonasPorRol(idUsuario);
                }
                List<udf_RechazoAprobacionZonaFiguraNombreAprobacionList_Result> result = (from u in db.udf_RechazoAprobacionZonaFiguraNombreAprobacionList(idPeriodo, Region, Zonas, "") select u).ToList();

                var lstPlanSemanal = result.Select(C => new
                {
                    C.Periodo,
                    C.Region,
                    C.Zona,
                    C.TipoFigura,
                    C.NombreFigura,
                    planeadastotal = C.capturadasTotal,
                    incumplidastotal = C.rechazadasTotal,
                    porcentajeRechazadasT = (C.capturadasTotal == 0 ? 0 : (Convert.ToDecimal(C.rechazadasTotal) / Convert.ToDecimal(C.capturadasTotal))).ToString("0.00%"),
                    //pocentajePromotor = (C.capturadasPromotorTotal == 0 ? 0 : (Convert.ToDecimal(C.rechazadasPromotorTotal) / Convert.ToDecimal(C.capturadasPromotorTotal))).ToString("0.00%"),
                    //pocentajeTecnico = (C.capturadasTecnicoTotal == 0 ? 0 : (Convert.ToDecimal(C.rechazadasTecnicoTotal) / Convert.ToDecimal(C.capturadasTecnicoTotal))).ToString("0.00%"),
                    //pocentajeFormador = (C.capturadasFormadorTotal == 0 ? 0 : (Convert.ToDecimal(C.rechazadasFormadorTotal) / Convert.ToDecimal(C.capturadasFormadorTotal))).ToString("0.00%"),
                });

                return Json(new { Success = true, datos = lstPlanSemanal }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = "Error al obtener la información" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult CargarIndFigurasObservacionesRegionZonaFigNomAct(string idPeriodo, string Region, string Zonas, string TipoActividad, int idUsuario)
        {
            try
            {
                if (Region == "")
                {
                    RegionesController regionesController = new RegionesController();
                    Region = regionesController.RegionesPorRol(idUsuario);
                }
                if (Zonas == "")
                {
                    ZonasController zonasController = new ZonasController();
                    Zonas = zonasController.ZonasPorRol(idUsuario);
                }
                List<udf_RechazoAprobacionZonaFiguraNombreAprobacionAct_Result> result = (from u in db.udf_RechazoAprobacionZonaFiguraNombreAprobacionAct(idPeriodo, Region, Zonas, "") select u).ToList();

                var lstPlanSemanal = result.Select(C => new
                {
                    C.Periodo,
                    C.Region,
                    C.Zona,
                    C.TipoFigura,
                    C.NombreFigura,
                    C.DescripcionActividad,
                    C.ComentariosRechazo
                });

                return Json(new { Success = true, datos = lstPlanSemanal }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = "Error al obtener la información" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult CargarIndFigurasObservacionesRegionGrafico(string idPeriodo, string Region, string Zonas, string TipoActividad, int idUsuario)
        {
            try
            {
                if (Region == "")
                {
                    RegionesController regionesController = new RegionesController();
                    Region = regionesController.RegionesPorRol(idUsuario);
                }
                if (Zonas == "")
                {
                    ZonasController zonasController = new ZonasController();
                    Zonas = zonasController.ZonasPorRol(idUsuario);
                }

                var modelCRegion = new List<object>();
                var dataCFigura = new List<object>();

                var dataCPromotor = new List<object>();
                var dataCTecnico = new List<object>();
                var dataCFormador = new List<object>();


                var ltsGenFob = (from fg in db.udf_RechazoAprobacionRegionList(idPeriodo, Region, Zonas, "")
                                 select fg).OrderByDescending(a => a.Region).ToList();

                var lstPlanSemanal = ltsGenFob.Select(C => new
                {
                    C.Region,
                    planeadastotal = C.capturadasTotal,
                    incumplidastotal = C.rechazadasTotal,
                    porcentajeRechazadasT = (C.capturadasTotal == 0 ? 0 : (Convert.ToDecimal(C.rechazadasTotal) / Convert.ToDecimal(C.capturadasTotal))).ToString("0.00%"),
                    pocentajePromotor = (C.capturadasPromotorTotal == 0 ? 0 : (Convert.ToDecimal(C.rechazadasPromotorTotal) / Convert.ToDecimal(C.capturadasPromotorTotal))).ToString("0.00%"),
                    pocentajeTecnico = (C.capturadasTecnicoTotal == 0 ? 0 : (Convert.ToDecimal(C.rechazadasTecnicoTotal) / Convert.ToDecimal(C.capturadasTecnicoTotal))).ToString("0.00%"),
                    pocentajeFormador = (C.capturadasFormadorTotal == 0 ? 0 : (Convert.ToDecimal(C.rechazadasFormadorTotal) / Convert.ToDecimal(C.capturadasFormadorTotal))).ToString("0.00%"),
                });

                if (ltsGenFob.Count > 0)
                {
                    foreach (var item in lstPlanSemanal)
                    {
                        modelCRegion.Add(new { label = item.Region });
                        dataCPromotor.Add(new { value = item.pocentajePromotor });
                        dataCTecnico.Add(new { value = item.pocentajeTecnico });
                        dataCFormador.Add(new { value = item.pocentajeFormador });
                    }
                    dataCFigura.Add(new
                    {
                        seriesname = "Promotor",
                        data = dataCPromotor
                    });
                    dataCFigura.Add(new
                    {
                        seriesname = "Tecnico docente",
                        data = dataCTecnico
                    });
                    dataCFigura.Add(new
                    {
                        seriesname = "Formador",
                        data = dataCFormador
                    });

                }
                return (Json(new { modelCRegion, dataCFigura }, JsonRequestBehavior.AllowGet));
            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = exp.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult CargarIndFigurasObservacionesRegionZonaGrafico(string idPeriodo, string Region, string Zonas, string TipoActividad, int idUsuario)
        {
            try
            {
                if (Region == "")
                {
                    RegionesController regionesController = new RegionesController();
                    Region = regionesController.RegionesPorRol(idUsuario);
                }
                if (Zonas == "")
                {
                    ZonasController zonasController = new ZonasController();
                    Zonas = zonasController.ZonasPorRol(idUsuario);
                }

                var modelCZona = new List<object>();
                var dataCFigura = new List<object>();

                var dataCPromotor = new List<object>();
                var dataCTecnico = new List<object>();
                var dataCFormador = new List<object>();


                var ltsGenFob = (from fg in db.udf_RechazoAprobacionZonaList(idPeriodo, Region, Zonas, "")
                                 select fg).OrderByDescending(a => a.Zona).ToList();

                var lstPlanSemanal = ltsGenFob.Select(C => new
                {
                    C.Zona,
                    planeadastotal = C.capturadasTotal,
                    incumplidastotal = C.rechazadasTotal,
                    porcentajeRechazadasT = (C.capturadasTotal == 0 ? 0 : (Convert.ToDecimal(C.rechazadasTotal) / Convert.ToDecimal(C.capturadasTotal))).ToString("0.00%"),
                    pocentajePromotor = (C.capturadasPromotorTotal == 0 ? 0 : (Convert.ToDecimal(C.rechazadasPromotorTotal) / Convert.ToDecimal(C.capturadasPromotorTotal))).ToString("0.00%"),
                    pocentajeTecnico = (C.capturadasTecnicoTotal == 0 ? 0 : (Convert.ToDecimal(C.rechazadasTecnicoTotal) / Convert.ToDecimal(C.capturadasTecnicoTotal))).ToString("0.00%"),
                    pocentajeFormador = (C.capturadasFormadorTotal == 0 ? 0 : (Convert.ToDecimal(C.rechazadasFormadorTotal) / Convert.ToDecimal(C.capturadasFormadorTotal))).ToString("0.00%"),
                });

                if (ltsGenFob.Count > 0)
                {
                    foreach (var item in lstPlanSemanal)
                    {
                        modelCZona.Add(new { label = item.Zona });
                        dataCPromotor.Add(new { value = item.pocentajePromotor });
                        dataCTecnico.Add(new { value = item.pocentajeTecnico });
                        dataCFormador.Add(new { value = item.pocentajeFormador });
                    }
                    dataCFigura.Add(new
                    {
                        seriesname = "Promotor",
                        data = dataCPromotor
                    });
                    dataCFigura.Add(new
                    {
                        seriesname = "Tecnico docente",
                        data = dataCTecnico
                    });
                    dataCFigura.Add(new
                    {
                        seriesname = "Formador",
                        data = dataCFormador
                    });

                }
                return (Json(new { modelCZona, dataCFigura }, JsonRequestBehavior.AllowGet));
            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = exp.Message }, JsonRequestBehavior.AllowGet);
            }
        }



        #endregion

        #region INDICADOR PREREGISTRADOS VS INCORPORADOS
        public ActionResult IncorporadosPreregistrados()
        {
            return View();
        }

        [HttpGet]
        public JsonResult CargarIndPreregistradosIncRegion(string idPeriodo, string Region, string Zonas, int idUsuario)
        {
            try
            {

                if (Region == "")
                {
                    RegionesController regionesController = new RegionesController();
                    Region = regionesController.RegionesPorRol(idUsuario);
                }
                if (Zonas == "")
                {
                    ZonasController zonasController = new ZonasController();
                    Zonas = zonasController.ZonasPorRol(idUsuario);
                }
                List<udf_PreregistrosIncorporadosBaseRegionList_Result> result = (from u in db.udf_PreregistrosIncorporadosBaseRegionList(idPeriodo, Region, Zonas, "") select u).ToList();

                var lstPlanSemanal = result.Select(C => new
                {
                    C.Region,
                    preregistrosTotal = C.prerregistradosTotal,
                    imcorporadostotal = C.incorporadosTotal,
                    porcentajeIncorporadosT = (C.prerregistradosTotal == 0 ? 0 : (Convert.ToDecimal(C.incorporadosTotal) / Convert.ToDecimal(C.prerregistradosTotal))).ToString("0.00%"),
                    pocentajePromotor = (C.prerregistradosTotal == 0 ? 0 : (Convert.ToDecimal(C.incPromotorTotal) / Convert.ToDecimal(C.prerregistradosTotal))).ToString("0.00%"),
                    pocentajeTecnico = (C.preTecnicoTotal == 0 ? 0 : (Convert.ToDecimal(C.incTecnicoTotal) / Convert.ToDecimal(C.preTecnicoTotal))).ToString("0.00%"),
                    pocentajeFormador = (C.preFormadorTotal == 0 ? 0 : (Convert.ToDecimal(C.incFormadorTotal) / Convert.ToDecimal(C.preFormadorTotal))).ToString("0.00%"),
                });

                return Json(new { Success = true, datos = lstPlanSemanal }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = "Error al obtener la información" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult CargarIndPreregistradosIncRegionGrafico(string idPeriodo, string Region, string Zonas, string TipoActividad, int idUsuario)
        {
            try
            {
                if (Region == "")
                {
                    RegionesController regionesController = new RegionesController();
                    Region = regionesController.RegionesPorRol(idUsuario);
                }
                if (Zonas == "")
                {
                    ZonasController zonasController = new ZonasController();
                    Zonas = zonasController.ZonasPorRol(idUsuario);
                }

                var modelCRegion = new List<object>();
                var dataCFigura = new List<object>();

                var dataCPromotor = new List<object>();
                var dataCTecnico = new List<object>();
                var dataCFormador = new List<object>();


                var ltsGenFob = (from fg in db.udf_PreregistrosIncorporadosBaseRegionList(idPeriodo, Region, Zonas, "")
                                 select fg).OrderByDescending(a => a.Region).ToList();

                var lstPlanSemanal = ltsGenFob.Select(C => new
                {
                    C.Region,
                    preregistrosTotal = C.prerregistradosTotal,
                    imcorporadostotal = C.incorporadosTotal,
                    porcentajeIncorporadosT = (C.prerregistradosTotal == 0 ? 0 : (Convert.ToDecimal(C.incorporadosTotal) / Convert.ToDecimal(C.prerregistradosTotal))).ToString("0.00%"),
                    pocentajePromotor = (C.prerregistradosTotal == 0 ? 0 : (Convert.ToDecimal(C.incPromotorTotal) / Convert.ToDecimal(C.prerregistradosTotal))).ToString("0.00%"),
                    pocentajeTecnico = (C.preTecnicoTotal == 0 ? 0 : (Convert.ToDecimal(C.incTecnicoTotal) / Convert.ToDecimal(C.preTecnicoTotal))).ToString("0.00%"),
                    pocentajeFormador = (C.preFormadorTotal == 0 ? 0 : (Convert.ToDecimal(C.incFormadorTotal) / Convert.ToDecimal(C.preFormadorTotal))).ToString("0.00%"),
                });

                if (ltsGenFob.Count > 0)
                {
                    foreach (var item in lstPlanSemanal)
                    {
                        modelCRegion.Add(new { label = item.Region });
                        dataCPromotor.Add(new { value = item.pocentajePromotor });
                        dataCTecnico.Add(new { value = item.pocentajeTecnico });
                        dataCFormador.Add(new { value = item.pocentajeFormador });
                    }
                    dataCFigura.Add(new
                    {
                        seriesname = "Promotor",
                        data = dataCPromotor
                    });
                    dataCFigura.Add(new
                    {
                        seriesname = "Tecnico docente",
                        data = dataCTecnico
                    });
                    dataCFigura.Add(new
                    {
                        seriesname = "Formador",
                        data = dataCFormador
                    });

                }
                return (Json(new { modelCRegion, dataCFigura }, JsonRequestBehavior.AllowGet));
            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = exp.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult CargarIndPreregistradosIncZona(string idPeriodo, string Region, string Zonas, int idUsuario)
        {
            try
            {

                if (Region == "")
                {
                    RegionesController regionesController = new RegionesController();
                    Region = regionesController.RegionesPorRol(idUsuario);
                }
                if (Zonas == "")
                {
                    ZonasController zonasController = new ZonasController();
                    Zonas = zonasController.ZonasPorRol(idUsuario);
                }
                List<udf_PreregistrosIncorporadosZonaList_Result> result = (from u in db.udf_PreregistrosIncorporadosZonaList(idPeriodo, Region, Zonas, "") select u).ToList();

                var lstPlanSemanal = result.Select(C => new
                {
                    C.Region,
                    C.Zona,
                    preregistrosTotal = C.prerregistradosTotal,
                    imcorporadostotal = C.incorporadosTotal,
                    porcentajeIncorporadosT = (C.prerregistradosTotal == 0 ? 0 : (Convert.ToDecimal(C.incorporadosTotal) / Convert.ToDecimal(C.prerregistradosTotal))).ToString("0.00%"),
                    pocentajePromotor = (C.prerregistradosTotal == 0 ? 0 : (Convert.ToDecimal(C.incPromotorTotal) / Convert.ToDecimal(C.prerregistradosTotal))).ToString("0.00%"),
                    pocentajeTecnico = (C.preTecnicoTotal == 0 ? 0 : (Convert.ToDecimal(C.incTecnicoTotal) / Convert.ToDecimal(C.preTecnicoTotal))).ToString("0.00%"),
                    pocentajeFormador = (C.preFormadorTotal == 0 ? 0 : (Convert.ToDecimal(C.incFormadorTotal) / Convert.ToDecimal(C.preFormadorTotal))).ToString("0.00%"),
                });

                return Json(new { Success = true, datos = lstPlanSemanal }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = "Error al obtener la información" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult CargarIndPreregistradosIncZonaGrafico(string idPeriodo, string Region, string Zonas, string TipoActividad, int idUsuario)
        {
            try
            {
                if (Region == "")
                {
                    RegionesController regionesController = new RegionesController();
                    Region = regionesController.RegionesPorRol(idUsuario);
                }
                if (Zonas == "")
                {
                    ZonasController zonasController = new ZonasController();
                    Zonas = zonasController.ZonasPorRol(idUsuario);
                }

                var modelCZona = new List<object>();
                var dataCFigura = new List<object>();

                var dataCPromotor = new List<object>();
                var dataCTecnico = new List<object>();
                var dataCFormador = new List<object>();


                var ltsGenFob = (from fg in db.udf_PreregistrosIncorporadosZonaList(idPeriodo, Region, Zonas, "")
                                 select fg).OrderByDescending(a => a.Zona).ToList();

                var lstPlanSemanal = ltsGenFob.Select(C => new
                {
                    C.Zona,
                    preregistrosTotal = C.prerregistradosTotal,
                    imcorporadostotal = C.incorporadosTotal,
                    porcentajeIncorporadosT = (C.prerregistradosTotal == 0 ? 0 : (Convert.ToDecimal(C.incorporadosTotal) / Convert.ToDecimal(C.prerregistradosTotal))).ToString("0.00%"),
                    pocentajePromotor = (C.prerregistradosTotal == 0 ? 0 : (Convert.ToDecimal(C.incPromotorTotal) / Convert.ToDecimal(C.prerregistradosTotal))).ToString("0.00%"),
                    pocentajeTecnico = (C.preTecnicoTotal == 0 ? 0 : (Convert.ToDecimal(C.incTecnicoTotal) / Convert.ToDecimal(C.preTecnicoTotal))).ToString("0.00%"),
                    pocentajeFormador = (C.preFormadorTotal == 0 ? 0 : (Convert.ToDecimal(C.incFormadorTotal) / Convert.ToDecimal(C.preFormadorTotal))).ToString("0.00%"),
                });

                if (ltsGenFob.Count > 0)
                {
                    foreach (var item in lstPlanSemanal)
                    {
                        modelCZona.Add(new { label = item.Zona });
                        dataCPromotor.Add(new { value = item.pocentajePromotor });
                        dataCTecnico.Add(new { value = item.pocentajeTecnico });
                        dataCFormador.Add(new { value = item.pocentajeFormador });
                    }
                    dataCFigura.Add(new
                    {
                        seriesname = "Promotor",
                        data = dataCPromotor
                    });
                    dataCFigura.Add(new
                    {
                        seriesname = "Tecnico docente",
                        data = dataCTecnico
                    });
                    dataCFigura.Add(new
                    {
                        seriesname = "Formador",
                        data = dataCFormador
                    });

                }
                return (Json(new { modelCZona, dataCFigura }, JsonRequestBehavior.AllowGet));
            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = exp.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult CargarIndPreregistradosIncZonaFigura(string idPeriodo, string Region, string Zonas, string TipoActividad, int idUsuario)
        {
            try
            {
                if (Region == "")
                {
                    RegionesController regionesController = new RegionesController();
                    Region = regionesController.RegionesPorRol(idUsuario);
                }
                if (Zonas == "")
                {
                    ZonasController zonasController = new ZonasController();
                    Zonas = zonasController.ZonasPorRol(idUsuario);
                }
                List<udf_PreregistrosIncorporadosPeriodoFiguraList_Result> result = (from u in db.udf_PreregistrosIncorporadosPeriodoFiguraList(idPeriodo, Region, Zonas, "") select u).ToList();

                var lstPlanSemanal = result.Select(C => new
                {
                    C.Periodo,
                    C.Region,
                    C.Zona,
                    C.DescripcionTipoFigura,
                    C.NombreUsuario,
                    preregistrosTotal = C.prerregistradosTotal,
                    imcorporadostotal = C.incorporadosTotal,
                    porcentajeIncorporadosT = (C.prerregistradosTotal == 0 ? 0 : (Convert.ToDecimal(C.incorporadosTotal) / Convert.ToDecimal(C.prerregistradosTotal))).ToString("0.00%")
                });

                return Json(new { Success = true, datos = lstPlanSemanal }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = "Error al obtener la información" }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        public JsonResult CargarIndPreregistradosIncZonaFiguraEducando(string idPeriodo, string Region, string Zonas, string TipoActividad, int idUsuario)
        {
            try
            {
                if (Region == "")
                {
                    RegionesController regionesController = new RegionesController();
                    Region = regionesController.RegionesPorRol(idUsuario);
                }
                if (Zonas == "")
                {
                    ZonasController zonasController = new ZonasController();
                    Zonas = zonasController.ZonasPorRol(idUsuario);
                }
                List<udf_PreregistrosIncorporadosPeriodoFiguraEducandoList_Result> result = (from u in db.udf_PreregistrosIncorporadosPeriodoFiguraEducandoList(idPeriodo, Region, Zonas, "") select u).ToList();

                var lstPlanSemanal = result.Select(C => new
                {
                    C.Periodo,
                    C.Region,
                    C.Zona,
                    C.DescripcionTipoFigura,
                    C.NombreUsuario,
                    C.NombrePreregistro,
                    C.FechaRegistro
                });

                return Json(new { Success = true, datos = lstPlanSemanal }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = "Error al obtener la información" }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion


        #region INDICADOR CERTIFICADOS PROGRAMADOS VS ENTREGADOS
        public ActionResult Certificados()
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