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
using System.Data;
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

                var dataConsult = (from u in db.udf_IncumplimientoActividadesPeriodoRegionList(idPeriodo, Region, Zonas, TipoActividad)
                                   group u by new { u.Region } into groupZona
                                   select new
                                   {
                                       Region = groupZona.Key.Region,
                                       PlaneadasTotal = (groupZona.Sum(s => s.PlaneadasTotal) == 0 ? 0 : groupZona.Sum(s => s.PlaneadasTotal)),
                                       IncumplidasTotal = (groupZona.Sum(s => s.IncumplidasTotal) == 0 ? 0 : groupZona.Sum(s => s.IncumplidasTotal)),
                                       PromotorPlanTotal = (groupZona.Sum(s => s.PromotorPlanTotal) == 0 ? 0 : groupZona.Sum(s => s.PromotorPlanTotal)),
                                       PromotorIncumplio = (groupZona.Sum(s => s.PromotorIncumplio) == 0 ? 0 : groupZona.Sum(s => s.PromotorIncumplio)),
                                       TecnicoPlan = (groupZona.Sum(s => s.TecnicoPlan) == 0 ? 0 : groupZona.Sum(s => s.TecnicoPlan)),
                                       TecnicoIncumplio = (groupZona.Sum(s => s.TecnicoIncumplio) == 0 ? 0 : groupZona.Sum(s => s.TecnicoIncumplio)),
                                       FormadorPlan = (groupZona.Sum(s => s.FormadorPlan) == 0 ? 0 : groupZona.Sum(s => s.FormadorPlan)),
                                       FormadorIncumplio = (groupZona.Sum(s => s.FormadorIncumplio) == 0 ? 0 : groupZona.Sum(s => s.FormadorIncumplio)),

                                   }).ToList();
              
                var lstPlanSemanal = dataConsult.Select(C => new
                {
                    //id = C.CoordinacionRegionId,
                    region = C.Region,
                    planeadastotal = C.PlaneadasTotal,
                    incumplidastotal = C.IncumplidasTotal,
                    pocentajeTotal = (C.PlaneadasTotal == 0 ? 0 : (Convert.ToDecimal(C.IncumplidasTotal) / Convert.ToDecimal(C.PlaneadasTotal))).ToString("0.00%"),
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

                var dataConsult = (from u in db.udf_IncumplimientoActividadesPeriodoRegionZonaList(idPeriodo, Region, Zonas, TipoActividad)
                                   group u by new { u.Region, u.Zona } into groupZona
                                   select new
                                   {
                                       Region = groupZona.Key.Region,
                                       Zona = groupZona.Key.Zona,
                                       PlaneadasTotal = (groupZona.Sum(s => s.PlaneadasTotal) == 0 ? 0 : groupZona.Sum(s => s.PlaneadasTotal)),
                                       IncumplidasTotal = (groupZona.Sum(s => s.IncumplidasTotal) == 0 ? 0 : groupZona.Sum(s => s.IncumplidasTotal)),
                                       PromotorPlanTotal = (groupZona.Sum(s => s.PromotorPlanTotal) == 0 ? 0 : groupZona.Sum(s => s.PromotorPlanTotal)),
                                       PromotorIncumplio = (groupZona.Sum(s => s.PromotorIncumplio) == 0 ? 0 : groupZona.Sum(s => s.PromotorIncumplio)),
                                       TecnicoPlan = (groupZona.Sum(s => s.TecnicoPlan) == 0 ? 0 : groupZona.Sum(s => s.TecnicoPlan)),
                                       TecnicoIncumplio = (groupZona.Sum(s => s.TecnicoIncumplio) == 0 ? 0 : groupZona.Sum(s => s.TecnicoIncumplio)),
                                       FormadorPlan = (groupZona.Sum(s => s.FormadorPlan) == 0 ? 0 : groupZona.Sum(s => s.FormadorPlan)),
                                       FormadorIncumplio = (groupZona.Sum(s => s.FormadorIncumplio) == 0 ? 0 : groupZona.Sum(s => s.FormadorIncumplio)),

                                   }).ToList();

                var lstPlanSemanal = dataConsult.Select(C => new
                {
                    region = C.Region,
                    zona = C.Zona,
                    planeadastotal = C.PlaneadasTotal,
                    incumplidastotal = C.IncumplidasTotal,
                    pocentajeTotal = (C.PlaneadasTotal == 0 ? 0 : (Convert.ToDecimal(C.IncumplidasTotal) / Convert.ToDecimal(C.PlaneadasTotal))).ToString("0.00%"),
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
                    promotorIncumplio = ((C.promotorPlan == 0 ? 0 : (Convert.ToDecimal(C.promotorIncumplio) / Convert.ToDecimal(C.promotorPlan))) * 100).ToString("0.00"),
                    tecnicoIncumplio = ((C.tecnicoPlan == 0 ? 0 : (Convert.ToDecimal(C.tecnicoIncumplio) / Convert.ToDecimal(C.tecnicoPlan))) * 100).ToString("0.00"),
                    formadorIncumplio = ((C.formadorPlan == 0 ? 0 : (Convert.ToDecimal(C.formadorIncumplio) / Convert.ToDecimal(C.formadorPlan))) * 100).ToString("0.00"),
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


                var dataConsult = (from u in db.udf_IncumplimientoActividadesPeriodoRegionList(idPeriodo, Region, Zonas, TipoActividad)
                                   group u by new { u.Region } into groupZona
                                   select new
                                   {
                                       Region = groupZona.Key.Region,
                                       PlaneadasTotal = (groupZona.Sum(s => s.PlaneadasTotal) == 0 ? 0 : groupZona.Sum(s => s.PlaneadasTotal)),
                                       IncumplidasTotal = (groupZona.Sum(s => s.IncumplidasTotal) == 0 ? 0 : groupZona.Sum(s => s.IncumplidasTotal)),
                                       PromotorPlanTotal = (groupZona.Sum(s => s.PromotorPlanTotal) == 0 ? 0 : groupZona.Sum(s => s.PromotorPlanTotal)),
                                       PromotorIncumplio = (groupZona.Sum(s => s.PromotorIncumplio) == 0 ? 0 : groupZona.Sum(s => s.PromotorIncumplio)),
                                       TecnicoPlan = (groupZona.Sum(s => s.TecnicoPlan) == 0 ? 0 : groupZona.Sum(s => s.TecnicoPlan)),
                                       TecnicoIncumplio = (groupZona.Sum(s => s.TecnicoIncumplio) == 0 ? 0 : groupZona.Sum(s => s.TecnicoIncumplio)),
                                       FormadorPlan = (groupZona.Sum(s => s.FormadorPlan) == 0 ? 0 : groupZona.Sum(s => s.FormadorPlan)),
                                       FormadorIncumplio = (groupZona.Sum(s => s.FormadorIncumplio) == 0 ? 0 : groupZona.Sum(s => s.FormadorIncumplio)),

                                   }).ToList();

                var lstPlanSemanal = dataConsult.Select(C => new
                {
                    //id = C.CoordinacionRegionId,
                    region = C.Region,
                    planeadastotal = C.PlaneadasTotal,
                    incumplidastotal = C.IncumplidasTotal,
                    pocentajeTotal = (C.PlaneadasTotal == 0 ? 0 : (Convert.ToDecimal(C.IncumplidasTotal) / Convert.ToDecimal(C.PlaneadasTotal))).ToString("0.00%"),
                    pocentajepromotor = (C.PromotorPlanTotal == 0 ? 0 : (Convert.ToDecimal(C.PromotorIncumplio) / Convert.ToDecimal(C.PromotorPlanTotal))).ToString("0.00%"),
                    pocentajetecnico = (C.TecnicoPlan == 0 ? 0 : (Convert.ToDecimal(C.TecnicoIncumplio) / Convert.ToDecimal(C.TecnicoPlan))).ToString("0.00%"),
                    pocentajeformador = (C.FormadorPlan == 0 ? 0 : (Convert.ToDecimal(C.FormadorIncumplio) / Convert.ToDecimal(C.FormadorPlan))).ToString("0.00%"),
                });
                if (dataConsult.Count > 0)
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

        [HttpGet]
        public JsonResult CargarIndIncumplimintoZonaGrafico(string idPeriodo, string Region, string Zonas, string TipoActividad, int idUsuario)
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

                var ltsGenFob = (from u in db.udf_IncumplimientoActividadesPeriodoRegionZonaList(idPeriodo, Region, Zonas, TipoActividad)
                                   group u by new { u.Region, u.Zona } into groupZona
                                   select new
                                   {
                                       Region = groupZona.Key.Region,
                                       Zona = groupZona.Key.Zona,
                                       PlaneadasTotal = (groupZona.Sum(s => s.PlaneadasTotal) == 0 ? 0 : groupZona.Sum(s => s.PlaneadasTotal)),
                                       IncumplidasTotal = (groupZona.Sum(s => s.IncumplidasTotal) == 0 ? 0 : groupZona.Sum(s => s.IncumplidasTotal)),
                                       PromotorPlanTotal = (groupZona.Sum(s => s.PromotorPlanTotal) == 0 ? 0 : groupZona.Sum(s => s.PromotorPlanTotal)),
                                       PromotorIncumplio = (groupZona.Sum(s => s.PromotorIncumplio) == 0 ? 0 : groupZona.Sum(s => s.PromotorIncumplio)),
                                       TecnicoPlan = (groupZona.Sum(s => s.TecnicoPlan) == 0 ? 0 : groupZona.Sum(s => s.TecnicoPlan)),
                                       TecnicoIncumplio = (groupZona.Sum(s => s.TecnicoIncumplio) == 0 ? 0 : groupZona.Sum(s => s.TecnicoIncumplio)),
                                       FormadorPlan = (groupZona.Sum(s => s.FormadorPlan) == 0 ? 0 : groupZona.Sum(s => s.FormadorPlan)),
                                       FormadorIncumplio = (groupZona.Sum(s => s.FormadorIncumplio) == 0 ? 0 : groupZona.Sum(s => s.FormadorIncumplio)),

                                   }).ToList();

                var lstPlanSemanal = ltsGenFob.Select(C => new
                {
                    region = C.Region,
                    zona = C.Zona,
                    planeadastotal = C.PlaneadasTotal,
                    pocentajepromotor = (C.PromotorPlanTotal == 0 ? 0 : (Convert.ToDecimal(C.PromotorIncumplio) / Convert.ToDecimal(C.PromotorPlanTotal))).ToString("0.00%"),
                    pocentajetecnico = (C.TecnicoPlan == 0 ? 0 : (Convert.ToDecimal(C.TecnicoIncumplio) / Convert.ToDecimal(C.TecnicoPlan))).ToString("0.00%"),
                    pocentajeformador = (C.FormadorPlan == 0 ? 0 : (Convert.ToDecimal(C.FormadorIncumplio) / Convert.ToDecimal(C.FormadorPlan))).ToString("0.00%"),
                });

                if (ltsGenFob.Count > 0)
                {
                    foreach (var item in lstPlanSemanal)
                    {
                        modelCZona.Add(new { label = item.zona });
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
                return (Json(new { modelCZona, dataCFigura }, JsonRequestBehavior.AllowGet));
            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = exp.Message }, JsonRequestBehavior.AllowGet);
            }
        }



        public void ExportIndIncumplimientoExcel(string idPeriodo, string Region, string Zonas, string TipoActividad, int idUsuario)
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

                //Region
                List<udf_IncumplimientoActividadesPeriodoRegionList_Result> result = (from u in db.udf_IncumplimientoActividadesPeriodoRegionList(idPeriodo, Region, Zonas, TipoActividad) select u).ToList();

                var lstPlanSemanal = result.Select(C => new
                {
                    id = C.CoordinacionRegionId,
                    periodo = C.DecripcionPeriodo,
                    region = C.Region,
                    planeadastotal = C.PlaneadasTotal,
                    incumplidastotal = C.IncumplidasTotal,
                    porcentajeTotal = (C.PlaneadasTotal == 0 ? 0 : (Convert.ToDecimal(C.IncumplidasTotal) / Convert.ToDecimal(C.PlaneadasTotal))).ToString("0.00%"),
                    pocentajepromotor = (C.PromotorPlanTotal == 0 ? 0 : (Convert.ToDecimal(C.PromotorIncumplio) / Convert.ToDecimal(C.PromotorPlanTotal))).ToString("0.00%"),
                    pocentajetecnico = (C.TecnicoPlan == 0 ? 0 : (Convert.ToDecimal(C.TecnicoIncumplio) / Convert.ToDecimal(C.TecnicoPlan))).ToString("0.00%"),
                    pocentajeformador = (C.FormadorPlan == 0 ? 0 : (Convert.ToDecimal(C.FormadorIncumplio) / Convert.ToDecimal(C.FormadorPlan))).ToString("0.00%"),
                });

                //Zona
                List<udf_IncumplimientoActividadesPeriodoRegionZonaList_Result> resultZona = (from u in db.udf_IncumplimientoActividadesPeriodoRegionZonaList(idPeriodo, Region, Zonas, TipoActividad) select u).ToList();

                var lstPlanSemanalZona = resultZona.Select(C => new
                {
                    id = C.CoordinacionRegionId,
                    periodo = C.DecripcionPeriodo,
                    region = C.Region,
                    zona = C.Zona,
                    planeadastotal = C.PlaneadasTotal,
                    incumplidastotal = C.IncumplidasTotal,
                    porcentajeTotal = (C.PlaneadasTotal == 0 ? 0 : (Convert.ToDecimal(C.IncumplidasTotal) / Convert.ToDecimal(C.PlaneadasTotal))).ToString("0.00%"),
                    pocentajepromotor = (C.PromotorPlanTotal == 0 ? 0 : (Convert.ToDecimal(C.PromotorIncumplio) / Convert.ToDecimal(C.PromotorPlanTotal))).ToString("0.00%"),
                    pocentajetecnico = (C.TecnicoPlan == 0 ? 0 : (Convert.ToDecimal(C.TecnicoIncumplio) / Convert.ToDecimal(C.TecnicoPlan))).ToString("0.00%"),
                    pocentajeformador = (C.FormadorPlan == 0 ? 0 : (Convert.ToDecimal(C.FormadorIncumplio) / Convert.ToDecimal(C.FormadorPlan))).ToString("0.00%"),
                });

                //Tipo Actividad
                var dataConsult = (from u in db.udf_IncumplimientoActividadesPeriodoZonaActividadList(idPeriodo, Region, Zonas, TipoActividad)
                                   group u by new { u.Actividad, u.Zona} into groupZona
                                   select new
                                   {
                                       actividad = groupZona.Key.Actividad,
                                       zona = groupZona.Key.Zona,
                                       planeadastotal = (groupZona.Sum(s => s.PlaneadasTotal) == null ? 0 : groupZona.Sum(s => s.PlaneadasTotal)),
                                       incumplidastotal = (groupZona.Sum(s => s.IncumplidasTotal) == null ? 0 : groupZona.Sum(s => s.IncumplidasTotal)),
                                   }).ToList();

                var headerZona = (from pr in dataConsult
                                  select pr.zona).Distinct().ToArray();

                var rowActividad = (from s in dataConsult
                                    select s.actividad).Distinct().ToList();
                System.Data.DataTable resultData = new System.Data.DataTable();
                if (dataConsult.Count > 0)
                {
                    
                    resultData.Columns.Add("Tipo Actividad");

                    foreach (var itemP in headerZona)
                    {
                        resultData.Columns.Add(itemP);
                    }

                    foreach (var row in rowActividad)
                    {
                        var resultTipoAct = (from u in dataConsult where u.actividad == row select u).ToList();
                        object[] dTabla = new object[resultData.Columns.Count];
                        dTabla[0] = row;
                        for (int i = 0; i < headerZona.Count(); i++)
                        {
                            var dato = (from d in resultTipoAct
                                        where d.zona == headerZona[i]
                                        select d).FirstOrDefault();
                            if (dato != null && dato.planeadastotal != 0)
                            {
                                dTabla[1 + i] = dato.incumplidastotal == 0 ? 0 : Math.Round((Convert.ToDecimal(dato.incumplidastotal) / Convert.ToDecimal(dato.planeadastotal)), 2) * 100;
                            }
                            else
                                dTabla[1 + i] = 0;
                        }
                        resultData.Rows.Add(dTabla);
                    }
                }

                //Tipo Figura
                var dataConsultFigura = (from u in db.udf_IncumplimientoActividadesPeriodoZonaActividadList(idPeriodo, Region, Zonas, TipoActividad)
                                   group u by new { u.Region, u.Zona, u.Actividad, u.DecripcionPeriodo } into groupZona
                                   select new
                                   {
                                       region = groupZona.Key.Region,
                                       zona = groupZona.Key.Zona,
                                       actividad = groupZona.Key.Actividad,
                                       periodo = groupZona.Key.DecripcionPeriodo,
                                       promotorPlan = groupZona.Sum(s => s.PromotorPlanTotal) == 0 ? 0 : groupZona.Sum(s => s.PromotorPlanTotal),
                                       promotorIncumplio = groupZona.Sum(s => s.PromotorIncumplio) == 0 ? 0 : groupZona.Sum(s => s.PromotorIncumplio),
                                       tecnicoPlan = groupZona.Sum(s => s.TecnicoPlan) == 0 ? 0 : groupZona.Sum(s => s.TecnicoPlan),
                                       tecnicoIncumplio = groupZona.Sum(s => s.TecnicoIncumplio) == 0 ? 0 : groupZona.Sum(s => s.TecnicoIncumplio),
                                       formadorPlan = groupZona.Sum(s => s.FormadorPlan) == 0 ? 0 : groupZona.Sum(s => s.FormadorPlan),
                                       formadorIncumplio = groupZona.Sum(s => s.FormadorIncumplio) == 0 ? 0 : groupZona.Sum(s => s.FormadorIncumplio),
                                   }).ToList();

                var lstPlanSemanalFigura = dataConsultFigura.Select(C => new
                {
                    id = C.region + "-" + C.zona + "-" + C.actividad,
                    C.periodo,
                    C.region,
                    C.zona,
                    C.actividad,
                    promotorIncumplio = (C.promotorPlan == 0 ? 0 : (Convert.ToDecimal(C.promotorIncumplio) / Convert.ToDecimal(C.promotorPlan))).ToString("0.00%"),
                    tecnicoIncumplio = (C.tecnicoPlan == 0 ? 0 : (Convert.ToDecimal(C.tecnicoIncumplio) / Convert.ToDecimal(C.tecnicoPlan))).ToString("0.00%"),
                    formadorIncumplio = (C.formadorPlan == 0 ? 0 : (Convert.ToDecimal(C.formadorIncumplio) / Convert.ToDecimal(C.formadorPlan))).ToString("0.00%"),
                });

                if (lstPlanSemanal.Count() > 0)
                {


                    using (ExcelPackage pkg = new ExcelPackage())
                    {
                        ExcelWorksheet ws = pkg.Workbook.Worksheets.Add("Región");
                        Color colFromHex = System.Drawing.ColorTranslator.FromHtml("#D9D9D9");
                        ws.Cells["A1:H1"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;

                        ws.Cells["A1"].Value = "Periodo";
                        ws.Cells["A1"].Style.Font.Bold = true;
                        ws.Cells["A1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        ws.Cells["B1"].Value = "Región";
                        ws.Cells["B1"].Style.Font.Bold = true;
                        ws.Cells["B1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        ws.Cells["C1"].Value = "Total Actividades planeadas";
                        ws.Cells["C1"].Style.Font.Bold = true;
                        ws.Cells["C1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        ws.Cells["D1"].Value = "Incumplidas";
                        ws.Cells["D1"].Style.Font.Bold = true;
                        ws.Cells["D1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        ws.Cells["E1"].Value = "%Incumplimiento";
                        ws.Cells["E1"].Style.Font.Bold = true;
                        ws.Cells["E1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        ws.Cells["F1"].Value = "%Incumplimiento Promotor";
                        ws.Cells["F1"].Style.Font.Bold = true;
                        ws.Cells["F1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        ws.Cells["G1"].Value = "%Incumplimiento Tecnico docente";
                        ws.Cells["G1"].Style.Font.Bold = true;
                        ws.Cells["G1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        ws.Cells["H1"].Value = "%Incumplimiento Formador";
                        ws.Cells["H1"].Style.Font.Bold = true;
                        ws.Cells["H1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        using (ExcelRange cell = ws.Cells["A1:H1"])
                        {
                            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                            cell.Style.Fill.BackgroundColor.SetColor(colFromHex);
                        }

                        int i = 2;
                        foreach (var item in lstPlanSemanal)
                        {
                            ws.Cells["A" + i.ToString()].Value = item.periodo;
                            ws.Cells["A" + i.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            ws.Cells["B" + i.ToString()].Value = item.region;
                            ws.Cells["B" + i.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            ws.Cells["C" + i.ToString()].Value = item.planeadastotal;
                            ws.Cells["C" + i.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            ws.Cells["D" + i.ToString()].Value = item.incumplidastotal;
                            ws.Cells["D" + i.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            ws.Cells["E" + i.ToString()].Value = item.porcentajeTotal;
                            ws.Cells["E" + i.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            ws.Cells["F" + i.ToString()].Value = item.pocentajepromotor;
                            ws.Cells["F" + i.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            ws.Cells["G" + i.ToString()].Value = item.pocentajetecnico;
                            ws.Cells["G" + i.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            ws.Cells["H" + i.ToString()].Value = item.pocentajeformador;
                            ws.Cells["H" + i.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            i++;

                        }

                        //Hoja Zona

                        ExcelWorksheet wsZona = pkg.Workbook.Worksheets.Add("Zona");
                        wsZona.Cells["A1:I1"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;

                        wsZona.Cells["A1"].Value = "Periodo";
                        wsZona.Cells["A1"].Style.Font.Bold = true;
                        wsZona.Cells["A1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsZona.Cells["B1"].Value = "Región";
                        wsZona.Cells["B1"].Style.Font.Bold = true;
                        wsZona.Cells["B1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsZona.Cells["C1"].Value = "Zona";
                        wsZona.Cells["C1"].Style.Font.Bold = true;
                        wsZona.Cells["C1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsZona.Cells["D1"].Value = "Total Actividades planeadas";
                        wsZona.Cells["D1"].Style.Font.Bold = true;
                        wsZona.Cells["D1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsZona.Cells["E1"].Value = "Incumplidas";
                        wsZona.Cells["E1"].Style.Font.Bold = true;
                        wsZona.Cells["E1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsZona.Cells["F1"].Value = "%Incumplimiento";
                        wsZona.Cells["F1"].Style.Font.Bold = true;
                        wsZona.Cells["F1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsZona.Cells["G1"].Value = "%Incumplimiento Promotor";
                        wsZona.Cells["G1"].Style.Font.Bold = true;
                        wsZona.Cells["G1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsZona.Cells["H1"].Value = "%Incumplimiento Tecnico docente";
                        wsZona.Cells["H1"].Style.Font.Bold = true;
                        wsZona.Cells["H1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsZona.Cells["I1"].Value = "%Incumplimiento Formador";
                        wsZona.Cells["I1"].Style.Font.Bold = true;
                        wsZona.Cells["I1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        using (ExcelRange cell = wsZona.Cells["A1:I1"])
                        {
                            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                            cell.Style.Fill.BackgroundColor.SetColor(colFromHex);
                        }
                        int iZona = 2;
                        foreach (var item in lstPlanSemanalZona)
                        {
                            wsZona.Cells["A" + iZona.ToString()].Value = item.periodo;
                            wsZona.Cells["A" + iZona.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsZona.Cells["B" + iZona.ToString()].Value = item.region;
                            wsZona.Cells["B" + iZona.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsZona.Cells["C" + iZona.ToString()].Value = item.zona;
                            wsZona.Cells["C" + iZona.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsZona.Cells["D" + iZona.ToString()].Value = item.planeadastotal;
                            wsZona.Cells["D" + iZona.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsZona.Cells["E" + iZona.ToString()].Value = item.incumplidastotal;
                            wsZona.Cells["E" + iZona.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsZona.Cells["F" + iZona.ToString()].Value = item.porcentajeTotal;
                            wsZona.Cells["F" + iZona.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsZona.Cells["G" + iZona.ToString()].Value = item.pocentajepromotor;
                            wsZona.Cells["G" + iZona.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsZona.Cells["H" + iZona.ToString()].Value = item.pocentajetecnico;
                            wsZona.Cells["H" + iZona.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsZona.Cells["I" + iZona.ToString()].Value = item.pocentajeformador;
                            wsZona.Cells["I" + iZona.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            iZona++;
                        }

                        // Hoja Tipo actividad

                        ExcelWorksheet wsTipoAct = pkg.Workbook.Worksheets.Add("Tipo Actividad");

                        char positionHeader = 'B', positionValues = 'A';
                        int l = 1, m = 2;
                        wsTipoAct.Cells["A1"].Value = "Tipo Actividad";
                        foreach (var item in headerZona)
                        {
                            wsTipoAct.Cells[positionHeader.ToString() + l.ToString()].Value = item;
                            positionHeader++;
                        }
                        positionHeader--;
                        wsTipoAct.Cells["A1:" + positionHeader.ToString() + "1"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        wsTipoAct.Cells["A1:" + positionHeader.ToString() + "1"].Style.Font.Bold = true;
                        wsTipoAct.Cells["A1:" + positionHeader.ToString() + "1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        wsTipoAct.Cells["A1:" + positionHeader.ToString() + "1"].Style.Fill.BackgroundColor.SetColor(colFromHex);
                        wsTipoAct.Cells["A1:" + positionHeader.ToString() + "1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        char ultimaPosicion = 'A';
                        foreach (DataRow dr in resultData.Rows)
                        {
                            int o = 0;
                            foreach (DataColumn dc in resultData.Columns)
                            {
                                wsTipoAct.Cells[positionValues.ToString() + m.ToString()].Value = dr[dc].ToString();
                                wsTipoAct.Cells[positionValues.ToString() + m.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                positionValues++;
                                o++;
                                ultimaPosicion = positionValues;
                            }
                            m++;
                            positionValues = 'A';
                        }


                        // Hoja Tipo Figura
                        ExcelWorksheet wsTipoFigura = pkg.Workbook.Worksheets.Add("Tipo Figura");
                        wsTipoFigura.Cells["A1:G1"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;

                        wsTipoFigura.Cells["A1"].Value = "Periodo";
                        wsTipoFigura.Cells["A1"].Style.Font.Bold = true;
                        wsTipoFigura.Cells["A1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsTipoFigura.Cells["B1"].Value = "Región";
                        wsTipoFigura.Cells["B1"].Style.Font.Bold = true;
                        wsTipoFigura.Cells["B1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsTipoFigura.Cells["C1"].Value = "Zona";
                        wsTipoFigura.Cells["C1"].Style.Font.Bold = true;
                        wsTipoFigura.Cells["C1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsTipoFigura.Cells["D1"].Value = "Actividad";
                        wsTipoFigura.Cells["D1"].Style.Font.Bold = true;
                        wsTipoFigura.Cells["D1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsTipoFigura.Cells["E1"].Value = "%Promotor";
                        wsTipoFigura.Cells["E1"].Style.Font.Bold = true;
                        wsTipoFigura.Cells["E1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsTipoFigura.Cells["F1"].Value = "%Tecnico docente";
                        wsTipoFigura.Cells["F1"].Style.Font.Bold = true;
                        wsTipoFigura.Cells["F1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsTipoFigura.Cells["G1"].Value = "%Formador";
                        wsTipoFigura.Cells["G1"].Style.Font.Bold = true;
                        wsTipoFigura.Cells["G1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        using (ExcelRange cell = wsTipoFigura.Cells["A1:G1"])
                        {
                            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                            cell.Style.Fill.BackgroundColor.SetColor(colFromHex);
                        }
                        int iFig = 2;
                        foreach (var item in lstPlanSemanalFigura)
                        {
                            wsTipoFigura.Cells["A" + iFig.ToString()].Value = item.periodo;
                            wsTipoFigura.Cells["A" + iFig.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsTipoFigura.Cells["B" + iFig.ToString()].Value = item.region;
                            wsTipoFigura.Cells["B" + iFig.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsTipoFigura.Cells["C" + iFig.ToString()].Value = item.zona;
                            wsTipoFigura.Cells["C" + iFig.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsTipoFigura.Cells["D" + iFig.ToString()].Value = item.actividad;
                            wsTipoFigura.Cells["D" + iFig.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsTipoFigura.Cells["E" + iFig.ToString()].Value = item.promotorIncumplio;
                            wsTipoFigura.Cells["E" + iFig.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsTipoFigura.Cells["F" + iFig.ToString()].Value = item.tecnicoIncumplio;
                            wsTipoFigura.Cells["F" + iFig.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsTipoFigura.Cells["G" + iFig.ToString()].Value = item.formadorIncumplio;
                            wsTipoFigura.Cells["G" + iFig.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            iFig++;
                        }

                        ws.Cells[ws.Dimension.Address].AutoFitColumns();
                        wsZona.Cells[wsZona.Dimension.Address].AutoFitColumns();
                        wsTipoAct.Cells[wsTipoAct.Dimension.Address].AutoFitColumns();
                        wsTipoFigura.Cells[wsTipoFigura.Dimension.Address].AutoFitColumns();
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("content-disposition", "attachment;  filename=Incumplimiento de actividades por periodo" + ".xlsx");
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
                List<udf_RechazoValidacionZonaFiguraNombreValidacion_Result> result = (from u in db.udf_RechazoValidacionZonaFiguraNombreValidacion(idPeriodo, Region, Zonas, "") select u).Where(x => x.rechazadasTotal > 0).ToList();

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
                }).OrderBy(x =>  x.NombreFigura).ThenBy(x => x.Periodo);

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
                }).OrderBy(x => x.NombreFigura).ThenBy(x => x.Periodo);

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

        public void ExportIndReincidentesExcel(string idPeriodo, string Region, string Zonas, int idUsuario)
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

                //Region
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

                //Zona
                List<udf_RechazoValidacionZonaList_Result> resultZona = (from u in db.udf_RechazoValidacionZonaList(idPeriodo, Region, Zonas, "") select u).ToList();

                var lstPlanSemanalZona = resultZona.Select(C => new
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

                //Figura
                List<udf_RechazoValidacionZonaFiguraNombreValidacion_Result> resultFigura = (from u in db.udf_RechazoValidacionZonaFiguraNombreValidacion(idPeriodo, Region, Zonas, "") select u).Where(x => x.rechazadasTotal > 0).ToList();

                var lstPlanSemanalFigura = resultFigura.Select(C => new
                {
                    C.Periodo,
                    C.Region,
                    C.Zona,
                    C.TipoFigura,
                    C.NombreFigura,
                    planeadastotal = C.capturadasTotal,
                    incumplidastotal = C.rechazadasTotal,
                    porcentajeRechazadasT = (C.capturadasTotal == 0 ? 0 : (Convert.ToDecimal(C.rechazadasTotal) / Convert.ToDecimal(C.capturadasTotal))).ToString("0.00%")
                }).OrderBy(x => x.NombreFigura).ThenBy(x => x.Periodo);

                //Detalle Figura
                List<udf_RechazoValidacionZonaFiguraNombreValidacionAct_Result> resultDetFigura = (from u in db.udf_RechazoValidacionZonaFiguraNombreValidacionAct(idPeriodo, Region, Zonas, "") select u).ToList();

                var lstPlanSemanalDetFigura = resultDetFigura.Select(C => new
                {
                    C.Periodo,
                    C.Region,
                    C.Zona,
                    C.TipoFigura,
                    C.NombreFigura,
                    C.DescripcionActividad,
                    C.ComentariosNoValidacion
                }).OrderBy(x => x.NombreFigura).ThenBy(x => x.Periodo);

                if (lstPlanSemanal.Count() > 0)
                {


                    using (ExcelPackage pkg = new ExcelPackage())
                    {
                        ExcelWorksheet ws = pkg.Workbook.Worksheets.Add("Región");
                        Color colFromHex = System.Drawing.ColorTranslator.FromHtml("#D9D9D9");
                        ws.Cells["A1:G1"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;

                        ws.Cells["A1"].Value = "Región";
                        ws.Cells["A1"].Style.Font.Bold = true;
                        ws.Cells["A1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        ws.Cells["B1"].Value = "Actividades capturadas";
                        ws.Cells["B1"].Style.Font.Bold = true;
                        ws.Cells["B1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        ws.Cells["C1"].Value = "Actividades rechazadas";
                        ws.Cells["C1"].Style.Font.Bold = true;
                        ws.Cells["C1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        ws.Cells["D1"].Value = "%Actividades rechazadas";
                        ws.Cells["D1"].Style.Font.Bold = true;
                        ws.Cells["D1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        ws.Cells["E1"].Value = "%Promotor";
                        ws.Cells["E1"].Style.Font.Bold = true;
                        ws.Cells["E1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        ws.Cells["F1"].Value = "%Técnico docente";
                        ws.Cells["F1"].Style.Font.Bold = true;
                        ws.Cells["F1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        ws.Cells["G1"].Value = "%Formador";
                        ws.Cells["G1"].Style.Font.Bold = true;
                        ws.Cells["G1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        using (ExcelRange cell = ws.Cells["A1:G1"])
                        {
                            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                            cell.Style.Fill.BackgroundColor.SetColor(colFromHex);
                        }

                        int i = 2;
                        foreach (var item in lstPlanSemanal)
                        {
                            ws.Cells["A" + i.ToString()].Value = item.Region;
                            ws.Cells["A" + i.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            ws.Cells["B" + i.ToString()].Value = item.planeadastotal;
                            ws.Cells["B" + i.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            ws.Cells["C" + i.ToString()].Value = item.incumplidastotal;
                            ws.Cells["C" + i.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            ws.Cells["D" + i.ToString()].Value = item.porcentajeRechazadasT;
                            ws.Cells["D" + i.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            ws.Cells["E" + i.ToString()].Value = item.pocentajePromotor;
                            ws.Cells["E" + i.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            ws.Cells["F" + i.ToString()].Value = item.pocentajeTecnico;
                            ws.Cells["F" + i.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            ws.Cells["G" + i.ToString()].Value = item.pocentajeFormador;
                            ws.Cells["G" + i.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            i++;

                        }

                        //Hoja Zona

                        ExcelWorksheet wsZona = pkg.Workbook.Worksheets.Add("Zona");
                        wsZona.Cells["A1:H1"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;

                        wsZona.Cells["A1"].Value = "Región";
                        wsZona.Cells["A1"].Style.Font.Bold = true;
                        wsZona.Cells["A1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsZona.Cells["B1"].Value = "Zona";
                        wsZona.Cells["B1"].Style.Font.Bold = true;
                        wsZona.Cells["B1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsZona.Cells["C1"].Value = "Actividades capturadas";
                        wsZona.Cells["C1"].Style.Font.Bold = true;
                        wsZona.Cells["C1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsZona.Cells["D1"].Value = "Actividades rechazadas";
                        wsZona.Cells["D1"].Style.Font.Bold = true;
                        wsZona.Cells["D1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsZona.Cells["E1"].Value = "%Actividades rechazadas";
                        wsZona.Cells["E1"].Style.Font.Bold = true;
                        wsZona.Cells["E1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsZona.Cells["F1"].Value = "%Promotor";
                        wsZona.Cells["F1"].Style.Font.Bold = true;
                        wsZona.Cells["F1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsZona.Cells["G1"].Value = "%Técnico docente";
                        wsZona.Cells["G1"].Style.Font.Bold = true;
                        wsZona.Cells["G1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsZona.Cells["H1"].Value = "%Formador";
                        wsZona.Cells["H1"].Style.Font.Bold = true;
                        wsZona.Cells["H1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        using (ExcelRange cell = wsZona.Cells["A1:H1"])
                        {
                            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                            cell.Style.Fill.BackgroundColor.SetColor(colFromHex);
                        }
                        int iZona = 2;
                        foreach (var item in lstPlanSemanalZona)
                        {
                            wsZona.Cells["A" + iZona.ToString()].Value = item.Region;
                            wsZona.Cells["A" + iZona.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsZona.Cells["B" + iZona.ToString()].Value = item.Zona;
                            wsZona.Cells["B" + iZona.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsZona.Cells["C" + iZona.ToString()].Value = item.planeadastotal;
                            wsZona.Cells["C" + iZona.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsZona.Cells["D" + iZona.ToString()].Value = item.incumplidastotal;
                            wsZona.Cells["D" + iZona.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsZona.Cells["E" + iZona.ToString()].Value = item.porcentajeRechazadasT;
                            wsZona.Cells["E" + iZona.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsZona.Cells["F" + iZona.ToString()].Value = item.pocentajePromotor;
                            wsZona.Cells["F" + iZona.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsZona.Cells["G" + iZona.ToString()].Value = item.pocentajeTecnico;
                            wsZona.Cells["G" + iZona.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsZona.Cells["H" + iZona.ToString()].Value = item.pocentajeFormador;
                            wsZona.Cells["H" + iZona.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            iZona++;
                        }

                        // Hoja Figura

                        ExcelWorksheet wsUsuEducando = pkg.Workbook.Worksheets.Add("Por Figura durante Validación");
                        wsUsuEducando.Cells["A1:H1"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;

                        wsUsuEducando.Cells["A1"].Value = "Periodo";
                        wsUsuEducando.Cells["A1"].Style.Font.Bold = true;
                        wsUsuEducando.Cells["A1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsUsuEducando.Cells["B1"].Value = "Region";
                        wsUsuEducando.Cells["B1"].Style.Font.Bold = true;
                        wsUsuEducando.Cells["B1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsUsuEducando.Cells["C1"].Value = "Zona";
                        wsUsuEducando.Cells["C1"].Style.Font.Bold = true;
                        wsUsuEducando.Cells["C1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsUsuEducando.Cells["D1"].Value = "Tipo Figura";
                        wsUsuEducando.Cells["D1"].Style.Font.Bold = true;
                        wsUsuEducando.Cells["D1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsUsuEducando.Cells["E1"].Value = "Nombre Figura";
                        wsUsuEducando.Cells["E1"].Style.Font.Bold = true;
                        wsUsuEducando.Cells["E1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsUsuEducando.Cells["F1"].Value = "Actividades capturadas";
                        wsUsuEducando.Cells["F1"].Style.Font.Bold = true;
                        wsUsuEducando.Cells["F1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsUsuEducando.Cells["G1"].Value = "Actividades rechazadas";
                        wsUsuEducando.Cells["G1"].Style.Font.Bold = true;
                        wsUsuEducando.Cells["G1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsUsuEducando.Cells["H1"].Value = "%Actividades rechazadas";
                        wsUsuEducando.Cells["H1"].Style.Font.Bold = true;
                        wsUsuEducando.Cells["H1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        using (ExcelRange cell = wsUsuEducando.Cells["A1:H1"])
                        {
                            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                            cell.Style.Fill.BackgroundColor.SetColor(colFromHex);
                        }
                        int iUsuEducando = 2;
                        foreach (var item in lstPlanSemanalFigura)
                        {
                            wsUsuEducando.Cells["A" + iUsuEducando.ToString()].Value = item.Periodo;
                            wsUsuEducando.Cells["A" + iUsuEducando.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsUsuEducando.Cells["B" + iUsuEducando.ToString()].Value = item.Region;
                            wsUsuEducando.Cells["B" + iUsuEducando.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsUsuEducando.Cells["C" + iUsuEducando.ToString()].Value = item.Zona;
                            wsUsuEducando.Cells["C" + iUsuEducando.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsUsuEducando.Cells["D" + iUsuEducando.ToString()].Value = item.TipoFigura;
                            wsUsuEducando.Cells["D" + iUsuEducando.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsUsuEducando.Cells["E" + iUsuEducando.ToString()].Value = item.NombreFigura;
                            wsUsuEducando.Cells["E" + iUsuEducando.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsUsuEducando.Cells["F" + iUsuEducando.ToString()].Value = item.planeadastotal;
                            wsUsuEducando.Cells["F" + iUsuEducando.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsUsuEducando.Cells["G" + iUsuEducando.ToString()].Value = item.incumplidastotal;
                            wsUsuEducando.Cells["G" + iUsuEducando.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsUsuEducando.Cells["H" + iUsuEducando.ToString()].Value = item.porcentajeRechazadasT;
                            wsUsuEducando.Cells["H" + iUsuEducando.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            iUsuEducando++;
                        }
                        if(lstPlanSemanalFigura.Count() == 0)
                        {
                            wsUsuEducando.Cells["A2:H2"].Merge = true;
                            wsUsuEducando.Cells["A2:H2"].Value = "Sin información encontrada";
                            wsUsuEducando.Cells["A2:H2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                            wsUsuEducando.Cells["A2:H2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        }

                        // Hoja Detalle Figura
                        ExcelWorksheet wsEducando = pkg.Workbook.Worksheets.Add("Detalle por Figura");
                        wsEducando.Cells["A1:G1"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;

                        wsEducando.Cells["A1"].Value = "Periodo";
                        wsEducando.Cells["A1"].Style.Font.Bold = true;
                        wsEducando.Cells["A1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsEducando.Cells["B1"].Value = "Region";
                        wsEducando.Cells["B1"].Style.Font.Bold = true;
                        wsEducando.Cells["B1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsEducando.Cells["C1"].Value = "Zona";
                        wsEducando.Cells["C1"].Style.Font.Bold = true;
                        wsEducando.Cells["C1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsEducando.Cells["D1"].Value = "Tipo Figura";
                        wsEducando.Cells["D1"].Style.Font.Bold = true;
                        wsEducando.Cells["D1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsEducando.Cells["E1"].Value = "Nombre Figura";
                        wsEducando.Cells["E1"].Style.Font.Bold = true;
                        wsEducando.Cells["E1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsEducando.Cells["F1"].Value = "Tipo de Actividad";
                        wsEducando.Cells["F1"].Style.Font.Bold = true;
                        wsEducando.Cells["F1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsEducando.Cells["G1"].Value = "Comentarios CZ";
                        wsEducando.Cells["G1"].Style.Font.Bold = true;
                        wsEducando.Cells["G1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        using (ExcelRange cell = wsEducando.Cells["A1:G1"])
                        {
                            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                            cell.Style.Fill.BackgroundColor.SetColor(colFromHex);
                        }
                        int iFig = 2;
                        foreach (var item in lstPlanSemanalDetFigura)
                        {
                            wsEducando.Cells["A" + iFig.ToString()].Value = item.Periodo;
                            wsEducando.Cells["A" + iFig.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsEducando.Cells["B" + iFig.ToString()].Value = item.Region;
                            wsEducando.Cells["B" + iFig.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsEducando.Cells["C" + iFig.ToString()].Value = item.Zona;
                            wsEducando.Cells["C" + iFig.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsEducando.Cells["D" + iFig.ToString()].Value = item.TipoFigura;
                            wsEducando.Cells["D" + iFig.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsEducando.Cells["E" + iFig.ToString()].Value = item.NombreFigura;
                            wsEducando.Cells["E" + iFig.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsEducando.Cells["F" + iFig.ToString()].Value = item.DescripcionActividad;
                            wsEducando.Cells["F" + iFig.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsEducando.Cells["G" + iFig.ToString()].Value = item.ComentariosNoValidacion;
                            wsEducando.Cells["G" + iFig.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            iFig++;
                        }
                        if (lstPlanSemanalDetFigura.Count() == 0)
                        {
                            wsEducando.Cells["A2:G2"].Merge = true;
                            wsEducando.Cells["A2:G2"].Value = "Sin información encontrada";
                            wsEducando.Cells["A2:G2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                            wsEducando.Cells["A2:G2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        }

                        ws.Cells[ws.Dimension.Address].AutoFitColumns();
                        wsZona.Cells[wsZona.Dimension.Address].AutoFitColumns();
                        wsUsuEducando.Cells[wsUsuEducando.Dimension.Address].AutoFitColumns();
                        wsEducando.Cells[wsEducando.Dimension.Address].AutoFitColumns();
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("content-disposition", "attachment;  filename=Figuras reincidentes en rechazo de plan semanal" + ".xlsx");
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
                List<udf_RechazoAprobacionZonaFiguraNombreAprobacionList_Result> result = (from u in db.udf_RechazoAprobacionZonaFiguraNombreAprobacionList(idPeriodo, Region, Zonas, "") select u).Where(x => x.rechazadasTotal > 0).ToList();

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

        public void ExportIndObservacionesExcel(string idPeriodo, string Region, string Zonas, int idUsuario)
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

                //Region
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

                //Zona
                List<udf_RechazoAprobacionZonaList_Result> resultZona = (from u in db.udf_RechazoAprobacionZonaList(idPeriodo, Region, Zonas, "") select u).ToList();

                var lstPlanSemanalZona = resultZona.Select(C => new
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

                //Figura
                List<udf_RechazoAprobacionZonaFiguraNombreAprobacionList_Result> resultFigura = (from u in db.udf_RechazoAprobacionZonaFiguraNombreAprobacionList(idPeriodo, Region, Zonas, "") select u).ToList();

                var lstPlanSemanalFigura = resultFigura.Select(C => new
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

                //Detalle Figura
                List<udf_RechazoAprobacionZonaFiguraNombreAprobacionAct_Result> resultDetFigura = (from u in db.udf_RechazoAprobacionZonaFiguraNombreAprobacionAct(idPeriodo, Region, Zonas, "") select u).ToList();

                var lstPlanSemanalDetFigura = resultDetFigura.Select(C => new
                {
                    C.Periodo,
                    C.Region,
                    C.Zona,
                    C.TipoFigura,
                    C.NombreFigura,
                    C.DescripcionActividad,
                    C.ComentariosRechazo
                });

                if (lstPlanSemanal.Count() > 0)
                {


                    using (ExcelPackage pkg = new ExcelPackage())
                    {
                        ExcelWorksheet ws = pkg.Workbook.Worksheets.Add("Región");
                        Color colFromHex = System.Drawing.ColorTranslator.FromHtml("#D9D9D9");
                        ws.Cells["A1:G1"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;

                        ws.Cells["A1"].Value = "Región";
                        ws.Cells["A1"].Style.Font.Bold = true;
                        ws.Cells["A1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        ws.Cells["B1"].Value = "Actividades aprobadas";
                        ws.Cells["B1"].Style.Font.Bold = true;
                        ws.Cells["B1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        ws.Cells["C1"].Value = "Actividades con observaciones";
                        ws.Cells["C1"].Style.Font.Bold = true;
                        ws.Cells["C1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        ws.Cells["D1"].Value = "%Observaciones";
                        ws.Cells["D1"].Style.Font.Bold = true;
                        ws.Cells["D1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        ws.Cells["E1"].Value = "%Promotor";
                        ws.Cells["E1"].Style.Font.Bold = true;
                        ws.Cells["E1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        ws.Cells["F1"].Value = "%Técnico docente";
                        ws.Cells["F1"].Style.Font.Bold = true;
                        ws.Cells["F1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        ws.Cells["G1"].Value = "%Formador";
                        ws.Cells["G1"].Style.Font.Bold = true;
                        ws.Cells["G1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        using (ExcelRange cell = ws.Cells["A1:G1"])
                        {
                            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                            cell.Style.Fill.BackgroundColor.SetColor(colFromHex);
                        }

                        int i = 2;
                        foreach (var item in lstPlanSemanal)
                        {
                            ws.Cells["A" + i.ToString()].Value = item.Region;
                            ws.Cells["A" + i.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            ws.Cells["B" + i.ToString()].Value = item.planeadastotal;
                            ws.Cells["B" + i.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            ws.Cells["C" + i.ToString()].Value = item.incumplidastotal;
                            ws.Cells["C" + i.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            ws.Cells["D" + i.ToString()].Value = item.porcentajeRechazadasT;
                            ws.Cells["D" + i.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            ws.Cells["E" + i.ToString()].Value = item.pocentajePromotor;
                            ws.Cells["E" + i.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            ws.Cells["F" + i.ToString()].Value = item.pocentajeTecnico;
                            ws.Cells["F" + i.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            ws.Cells["G" + i.ToString()].Value = item.pocentajeFormador;
                            ws.Cells["G" + i.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            i++;

                        }

                        //Hoja Zona

                        ExcelWorksheet wsZona = pkg.Workbook.Worksheets.Add("Zona");
                        wsZona.Cells["A1:H1"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;

                        wsZona.Cells["A1"].Value = "Región";
                        wsZona.Cells["A1"].Style.Font.Bold = true;
                        wsZona.Cells["A1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsZona.Cells["B1"].Value = "Zona";
                        wsZona.Cells["B1"].Style.Font.Bold = true;
                        wsZona.Cells["B1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsZona.Cells["C1"].Value = "Actividades aprobadas";
                        wsZona.Cells["C1"].Style.Font.Bold = true;
                        wsZona.Cells["C1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsZona.Cells["D1"].Value = "Actividades con observaciones";
                        wsZona.Cells["D1"].Style.Font.Bold = true;
                        wsZona.Cells["D1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsZona.Cells["E1"].Value = "%Observaciones";
                        wsZona.Cells["E1"].Style.Font.Bold = true;
                        wsZona.Cells["E1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsZona.Cells["F1"].Value = "%Promotor";
                        wsZona.Cells["F1"].Style.Font.Bold = true;
                        wsZona.Cells["F1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsZona.Cells["G1"].Value = "%Técnico docente";
                        wsZona.Cells["G1"].Style.Font.Bold = true;
                        wsZona.Cells["G1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsZona.Cells["H1"].Value = "%Formador";
                        wsZona.Cells["H1"].Style.Font.Bold = true;
                        wsZona.Cells["H1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        using (ExcelRange cell = wsZona.Cells["A1:H1"])
                        {
                            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                            cell.Style.Fill.BackgroundColor.SetColor(colFromHex);
                        }
                        int iZona = 2;
                        foreach (var item in lstPlanSemanalZona)
                        {
                            wsZona.Cells["A" + iZona.ToString()].Value = item.Region;
                            wsZona.Cells["A" + iZona.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsZona.Cells["B" + iZona.ToString()].Value = item.Zona;
                            wsZona.Cells["B" + iZona.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsZona.Cells["C" + iZona.ToString()].Value = item.planeadastotal;
                            wsZona.Cells["C" + iZona.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsZona.Cells["D" + iZona.ToString()].Value = item.incumplidastotal;
                            wsZona.Cells["D" + iZona.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsZona.Cells["E" + iZona.ToString()].Value = item.porcentajeRechazadasT;
                            wsZona.Cells["E" + iZona.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsZona.Cells["F" + iZona.ToString()].Value = item.pocentajePromotor;
                            wsZona.Cells["F" + iZona.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsZona.Cells["G" + iZona.ToString()].Value = item.pocentajeTecnico;
                            wsZona.Cells["G" + iZona.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsZona.Cells["H" + iZona.ToString()].Value = item.pocentajeFormador;
                            wsZona.Cells["H" + iZona.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            iZona++;
                        }

                        // Hoja Figura

                        ExcelWorksheet wsUsuEducando = pkg.Workbook.Worksheets.Add("Por Figura durante Validación");
                        wsUsuEducando.Cells["A1:H1"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;

                        wsUsuEducando.Cells["A1"].Value = "Periodo";
                        wsUsuEducando.Cells["A1"].Style.Font.Bold = true;
                        wsUsuEducando.Cells["A1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsUsuEducando.Cells["B1"].Value = "Region";
                        wsUsuEducando.Cells["B1"].Style.Font.Bold = true;
                        wsUsuEducando.Cells["B1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsUsuEducando.Cells["C1"].Value = "Zona";
                        wsUsuEducando.Cells["C1"].Style.Font.Bold = true;
                        wsUsuEducando.Cells["C1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsUsuEducando.Cells["D1"].Value = "Tipo Figura";
                        wsUsuEducando.Cells["D1"].Style.Font.Bold = true;
                        wsUsuEducando.Cells["D1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsUsuEducando.Cells["E1"].Value = "Nombre Figura";
                        wsUsuEducando.Cells["E1"].Style.Font.Bold = true;
                        wsUsuEducando.Cells["E1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsUsuEducando.Cells["F1"].Value = "Actividades aprobadas";
                        wsUsuEducando.Cells["F1"].Style.Font.Bold = true;
                        wsUsuEducando.Cells["F1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsUsuEducando.Cells["G1"].Value = "Actividades observadas";
                        wsUsuEducando.Cells["G1"].Style.Font.Bold = true;
                        wsUsuEducando.Cells["G1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsUsuEducando.Cells["H1"].Value = "%Observaciones";
                        wsUsuEducando.Cells["H1"].Style.Font.Bold = true;
                        wsUsuEducando.Cells["H1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        using (ExcelRange cell = wsUsuEducando.Cells["A1:H1"])
                        {
                            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                            cell.Style.Fill.BackgroundColor.SetColor(colFromHex);
                        }
                        int iUsuEducando = 2;
                        foreach (var item in lstPlanSemanalFigura)
                        {
                            wsUsuEducando.Cells["A" + iUsuEducando.ToString()].Value = item.Periodo;
                            wsUsuEducando.Cells["A" + iUsuEducando.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsUsuEducando.Cells["B" + iUsuEducando.ToString()].Value = item.Region;
                            wsUsuEducando.Cells["B" + iUsuEducando.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsUsuEducando.Cells["C" + iUsuEducando.ToString()].Value = item.Zona;
                            wsUsuEducando.Cells["C" + iUsuEducando.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsUsuEducando.Cells["D" + iUsuEducando.ToString()].Value = item.TipoFigura;
                            wsUsuEducando.Cells["D" + iUsuEducando.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsUsuEducando.Cells["E" + iUsuEducando.ToString()].Value = item.NombreFigura;
                            wsUsuEducando.Cells["E" + iUsuEducando.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsUsuEducando.Cells["F" + iUsuEducando.ToString()].Value = item.planeadastotal;
                            wsUsuEducando.Cells["F" + iUsuEducando.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsUsuEducando.Cells["G" + iUsuEducando.ToString()].Value = item.incumplidastotal;
                            wsUsuEducando.Cells["G" + iUsuEducando.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsUsuEducando.Cells["H" + iUsuEducando.ToString()].Value = item.porcentajeRechazadasT;
                            wsUsuEducando.Cells["H" + iUsuEducando.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            iUsuEducando++;
                        }
                        if (lstPlanSemanalFigura.Count() == 0)
                        {
                            wsUsuEducando.Cells["A2:H2"].Merge = true;
                            wsUsuEducando.Cells["A2:H2"].Value = "Sin información encontrada";
                            wsUsuEducando.Cells["A2:H2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                            wsUsuEducando.Cells["A2:H2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        }

                        // Hoja Detalle Figura
                        ExcelWorksheet wsEducando = pkg.Workbook.Worksheets.Add("Detalle por Figura");
                        wsEducando.Cells["A1:G1"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;

                        wsEducando.Cells["A1"].Value = "Periodo";
                        wsEducando.Cells["A1"].Style.Font.Bold = true;
                        wsEducando.Cells["A1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsEducando.Cells["B1"].Value = "Region";
                        wsEducando.Cells["B1"].Style.Font.Bold = true;
                        wsEducando.Cells["B1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsEducando.Cells["C1"].Value = "Zona";
                        wsEducando.Cells["C1"].Style.Font.Bold = true;
                        wsEducando.Cells["C1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsEducando.Cells["D1"].Value = "Tipo Figura";
                        wsEducando.Cells["D1"].Style.Font.Bold = true;
                        wsEducando.Cells["D1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsEducando.Cells["E1"].Value = "Nombre Figura";
                        wsEducando.Cells["E1"].Style.Font.Bold = true;
                        wsEducando.Cells["E1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsEducando.Cells["F1"].Value = "Tipo de Actividad";
                        wsEducando.Cells["F1"].Style.Font.Bold = true;
                        wsEducando.Cells["F1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsEducando.Cells["G1"].Value = "Comentarios CZ";
                        wsEducando.Cells["G1"].Style.Font.Bold = true;
                        wsEducando.Cells["G1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        using (ExcelRange cell = wsEducando.Cells["A1:G1"])
                        {
                            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                            cell.Style.Fill.BackgroundColor.SetColor(colFromHex);
                        }
                        int iFig = 2;
                        foreach (var item in lstPlanSemanalDetFigura)
                        {
                            wsEducando.Cells["A" + iFig.ToString()].Value = item.Periodo;
                            wsEducando.Cells["A" + iFig.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsEducando.Cells["B" + iFig.ToString()].Value = item.Region;
                            wsEducando.Cells["B" + iFig.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsEducando.Cells["C" + iFig.ToString()].Value = item.Zona;
                            wsEducando.Cells["C" + iFig.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsEducando.Cells["D" + iFig.ToString()].Value = item.TipoFigura;
                            wsEducando.Cells["D" + iFig.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsEducando.Cells["E" + iFig.ToString()].Value = item.NombreFigura;
                            wsEducando.Cells["E" + iFig.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsEducando.Cells["F" + iFig.ToString()].Value = item.DescripcionActividad;
                            wsEducando.Cells["F" + iFig.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsEducando.Cells["G" + iFig.ToString()].Value = item.ComentariosRechazo;
                            wsEducando.Cells["G" + iFig.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            iFig++;
                        }
                        if (lstPlanSemanalDetFigura.Count() == 0)
                        {
                            wsEducando.Cells["A2:G2"].Merge = true;
                            wsEducando.Cells["A2:G2"].Value = "Sin información encontrada";
                            wsEducando.Cells["A2:G2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                            wsEducando.Cells["A2:G2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        }

                        ws.Cells[ws.Dimension.Address].AutoFitColumns();
                        wsZona.Cells[wsZona.Dimension.Address].AutoFitColumns();
                        wsUsuEducando.Cells[wsUsuEducando.Dimension.Address].AutoFitColumns();
                        wsEducando.Cells[wsEducando.Dimension.Address].AutoFitColumns();
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("content-disposition", "attachment;  filename=Figuras con observaciones en la aprobación" + ".xlsx");
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
                    C.FechaPreregistro,
                    C.FechaRegistro
                });

                return Json(new { Success = true, datos = lstPlanSemanal }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = "Error al obtener la información" }, JsonRequestBehavior.AllowGet);
            }
        }

        public void ExportIndPreregistradosExcel(string idPeriodo, string Region, string Zonas, string TipoActividad, int idUsuario)
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

                //Region
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

                //Zona
                List<udf_PreregistrosIncorporadosZonaList_Result> resultZona = (from u in db.udf_PreregistrosIncorporadosZonaList(idPeriodo, Region, Zonas, "") select u).ToList();

                var lstPlanSemanalZona = resultZona.Select(C => new
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

                //Figura
                List<udf_PreregistrosIncorporadosPeriodoFiguraList_Result> resultFigura = (from u in db.udf_PreregistrosIncorporadosPeriodoFiguraList(idPeriodo, Region, Zonas, "") select u).ToList();

                var lstPlanSemanalFigura = resultFigura.Select(C => new
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

                //Educando
                List<udf_PreregistrosIncorporadosPeriodoFiguraEducandoList_Result> resultEducando = (from u in db.udf_PreregistrosIncorporadosPeriodoFiguraEducandoList(idPeriodo, Region, Zonas, "") select u).ToList();

                var lstPlanSemanalEducando = resultEducando.Select(C => new
                {
                    C.Periodo,
                    C.Region,
                    C.Zona,
                    C.DescripcionTipoFigura,
                    C.NombreUsuario,
                    C.NombrePreregistro,
                    C.FechaPreregistro,
                    C.FechaRegistro
                });

                if (lstPlanSemanal.Count() > 0)
                {


                    using (ExcelPackage pkg = new ExcelPackage())
                    {
                        ExcelWorksheet ws = pkg.Workbook.Worksheets.Add("Región");
                        Color colFromHex = System.Drawing.ColorTranslator.FromHtml("#D9D9D9");
                        ws.Cells["A1:G1"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;

                        ws.Cells["A1"].Value = "Región";
                        ws.Cells["A1"].Style.Font.Bold = true;
                        ws.Cells["A1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        ws.Cells["B1"].Value = "Educandos pre-registrados";
                        ws.Cells["B1"].Style.Font.Bold = true;
                        ws.Cells["B1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        ws.Cells["C1"].Value = "Educandos registrados";
                        ws.Cells["C1"].Style.Font.Bold = true;
                        ws.Cells["C1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        ws.Cells["D1"].Value = "%Registrados";
                        ws.Cells["D1"].Style.Font.Bold = true;
                        ws.Cells["D1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        ws.Cells["E1"].Value = "%Promotor";
                        ws.Cells["E1"].Style.Font.Bold = true;
                        ws.Cells["E1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        ws.Cells["F1"].Value = "%Técnicos docentes";
                        ws.Cells["F1"].Style.Font.Bold = true;
                        ws.Cells["F1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        ws.Cells["G1"].Value = "%Formadores";
                        ws.Cells["G1"].Style.Font.Bold = true;
                        ws.Cells["G1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        using (ExcelRange cell = ws.Cells["A1:G1"])
                        {
                            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                            cell.Style.Fill.BackgroundColor.SetColor(colFromHex);
                        }

                        int i = 2;
                        foreach (var item in lstPlanSemanal)
                        {
                            ws.Cells["A" + i.ToString()].Value = item.Region;
                            ws.Cells["A" + i.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            ws.Cells["B" + i.ToString()].Value = item.preregistrosTotal;
                            ws.Cells["B" + i.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            ws.Cells["C" + i.ToString()].Value = item.imcorporadostotal;
                            ws.Cells["C" + i.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            ws.Cells["D" + i.ToString()].Value = item.porcentajeIncorporadosT;
                            ws.Cells["D" + i.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            ws.Cells["E" + i.ToString()].Value = item.pocentajePromotor;
                            ws.Cells["E" + i.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            ws.Cells["F" + i.ToString()].Value = item.pocentajeTecnico;
                            ws.Cells["F" + i.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            ws.Cells["G" + i.ToString()].Value = item.pocentajeFormador;
                            ws.Cells["G" + i.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            i++;

                        }

                        //Hoja Zona

                        ExcelWorksheet wsZona = pkg.Workbook.Worksheets.Add("Zona");
                        wsZona.Cells["A1:H1"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;

                        wsZona.Cells["A1"].Value = "Región";
                        wsZona.Cells["A1"].Style.Font.Bold = true;
                        wsZona.Cells["A1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsZona.Cells["B1"].Value = "Zona";
                        wsZona.Cells["B1"].Style.Font.Bold = true;
                        wsZona.Cells["B1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsZona.Cells["C1"].Value = "Educandos pre-registrados";
                        wsZona.Cells["C1"].Style.Font.Bold = true;
                        wsZona.Cells["C1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsZona.Cells["D1"].Value = "Educandos registrados";
                        wsZona.Cells["D1"].Style.Font.Bold = true;
                        wsZona.Cells["D1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsZona.Cells["E1"].Value = "%Registrados";
                        wsZona.Cells["E1"].Style.Font.Bold = true;
                        wsZona.Cells["E1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsZona.Cells["F1"].Value = "%Promotor";
                        wsZona.Cells["F1"].Style.Font.Bold = true;
                        wsZona.Cells["F1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsZona.Cells["G1"].Value = "%Técnicos docentes";
                        wsZona.Cells["G1"].Style.Font.Bold = true;
                        wsZona.Cells["G1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsZona.Cells["H1"].Value = "%Formadores";
                        wsZona.Cells["H1"].Style.Font.Bold = true;
                        wsZona.Cells["H1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        using (ExcelRange cell = wsZona.Cells["A1:H1"])
                        {
                            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                            cell.Style.Fill.BackgroundColor.SetColor(colFromHex);
                        }
                        int iZona = 2;
                        foreach (var item in lstPlanSemanalZona)
                        {
                            wsZona.Cells["A" + iZona.ToString()].Value = item.Region;
                            wsZona.Cells["A" + iZona.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsZona.Cells["B" + iZona.ToString()].Value = item.Zona;
                            wsZona.Cells["B" + iZona.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsZona.Cells["C" + iZona.ToString()].Value = item.preregistrosTotal;
                            wsZona.Cells["C" + iZona.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsZona.Cells["D" + iZona.ToString()].Value = item.imcorporadostotal;
                            wsZona.Cells["D" + iZona.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsZona.Cells["E" + iZona.ToString()].Value = item.porcentajeIncorporadosT;
                            wsZona.Cells["E" + iZona.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsZona.Cells["F" + iZona.ToString()].Value = item.pocentajePromotor;
                            wsZona.Cells["F" + iZona.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsZona.Cells["G" + iZona.ToString()].Value = item.pocentajeTecnico;
                            wsZona.Cells["G" + iZona.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsZona.Cells["H" + iZona.ToString()].Value = item.pocentajeFormador;
                            wsZona.Cells["H" + iZona.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            iZona++;
                        }

                        // Hoja Usuario de Educandos

                        ExcelWorksheet wsUsuEducando = pkg.Workbook.Worksheets.Add("Usuario de Educandos");
                        wsUsuEducando.Cells["A1:H1"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;

                        wsUsuEducando.Cells["A1"].Value = "Periodo";
                        wsUsuEducando.Cells["A1"].Style.Font.Bold = true;
                        wsUsuEducando.Cells["A1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsUsuEducando.Cells["B1"].Value = "Region";
                        wsUsuEducando.Cells["B1"].Style.Font.Bold = true;
                        wsUsuEducando.Cells["B1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsUsuEducando.Cells["C1"].Value = "Zona";
                        wsUsuEducando.Cells["C1"].Style.Font.Bold = true;
                        wsUsuEducando.Cells["C1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsUsuEducando.Cells["D1"].Value = "Tipo Figura";
                        wsUsuEducando.Cells["D1"].Style.Font.Bold = true;
                        wsUsuEducando.Cells["D1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsUsuEducando.Cells["E1"].Value = "Nombre Figura";
                        wsUsuEducando.Cells["E1"].Style.Font.Bold = true;
                        wsUsuEducando.Cells["E1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsUsuEducando.Cells["F1"].Value = "Educandos pre-registrados";
                        wsUsuEducando.Cells["F1"].Style.Font.Bold = true;
                        wsUsuEducando.Cells["F1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsUsuEducando.Cells["G1"].Value = "Educandos registrados";
                        wsUsuEducando.Cells["G1"].Style.Font.Bold = true;
                        wsUsuEducando.Cells["G1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsUsuEducando.Cells["H1"].Value = "%Registrados";
                        wsUsuEducando.Cells["H1"].Style.Font.Bold = true;
                        wsUsuEducando.Cells["H1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        using (ExcelRange cell = wsUsuEducando.Cells["A1:H1"])
                        {
                            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                            cell.Style.Fill.BackgroundColor.SetColor(colFromHex);
                        }
                        int iUsuEducando = 2;
                        foreach (var item in lstPlanSemanalFigura)
                        {
                            wsUsuEducando.Cells["A" + iUsuEducando.ToString()].Value = item.Periodo;
                            wsUsuEducando.Cells["A" + iUsuEducando.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsUsuEducando.Cells["B" + iUsuEducando.ToString()].Value = item.Region;
                            wsUsuEducando.Cells["B" + iUsuEducando.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsUsuEducando.Cells["C" + iUsuEducando.ToString()].Value = item.Zona;
                            wsUsuEducando.Cells["C" + iUsuEducando.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsUsuEducando.Cells["D" + iUsuEducando.ToString()].Value = item.DescripcionTipoFigura;
                            wsUsuEducando.Cells["D" + iUsuEducando.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsUsuEducando.Cells["E" + iUsuEducando.ToString()].Value = item.NombreUsuario;
                            wsUsuEducando.Cells["E" + iUsuEducando.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsUsuEducando.Cells["F" + iUsuEducando.ToString()].Value = item.preregistrosTotal;
                            wsUsuEducando.Cells["F" + iUsuEducando.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsUsuEducando.Cells["G" + iUsuEducando.ToString()].Value = item.imcorporadostotal;
                            wsUsuEducando.Cells["G" + iUsuEducando.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsUsuEducando.Cells["H" + iUsuEducando.ToString()].Value = item.porcentajeIncorporadosT;
                            wsUsuEducando.Cells["H" + iUsuEducando.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            iUsuEducando++;
                        }

                        // Hoja Educandos
                        ExcelWorksheet wsEducando = pkg.Workbook.Worksheets.Add("Tipo Figura");
                        wsEducando.Cells["A1:H1"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;

                        wsEducando.Cells["A1"].Value = "Periodo";
                        wsEducando.Cells["A1"].Style.Font.Bold = true;
                        wsEducando.Cells["A1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsEducando.Cells["B1"].Value = "Region";
                        wsEducando.Cells["B1"].Style.Font.Bold = true;
                        wsEducando.Cells["B1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsEducando.Cells["C1"].Value = "Zona";
                        wsEducando.Cells["C1"].Style.Font.Bold = true;
                        wsEducando.Cells["C1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsEducando.Cells["D1"].Value = "Tipo Figura";
                        wsEducando.Cells["D1"].Style.Font.Bold = true;
                        wsEducando.Cells["D1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsEducando.Cells["E1"].Value = "Nombre Figura";
                        wsEducando.Cells["E1"].Style.Font.Bold = true;
                        wsEducando.Cells["E1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsEducando.Cells["F1"].Value = "Nombre del educando";
                        wsEducando.Cells["F1"].Style.Font.Bold = true;
                        wsEducando.Cells["F1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsEducando.Cells["G1"].Value = "Fecha Pre-registro";
                        wsEducando.Cells["G1"].Style.Font.Bold = true;
                        wsEducando.Cells["G1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsEducando.Cells["H1"].Value = "Fecha Registro";
                        wsEducando.Cells["H1"].Style.Font.Bold = true;
                        wsEducando.Cells["H1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        using (ExcelRange cell = wsEducando.Cells["A1:H1"])
                        {
                            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                            cell.Style.Fill.BackgroundColor.SetColor(colFromHex);
                        }
                        int iFig = 2;
                        foreach (var item in lstPlanSemanalEducando)
                        {
                            wsEducando.Cells["A" + iFig.ToString()].Value = item.Periodo;
                            wsEducando.Cells["A" + iFig.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsEducando.Cells["B" + iFig.ToString()].Value = item.Region;
                            wsEducando.Cells["B" + iFig.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsEducando.Cells["C" + iFig.ToString()].Value = item.Zona;
                            wsEducando.Cells["C" + iFig.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsEducando.Cells["D" + iFig.ToString()].Value = item.DescripcionTipoFigura;
                            wsEducando.Cells["D" + iFig.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsEducando.Cells["E" + iFig.ToString()].Value = item.NombreUsuario;
                            wsEducando.Cells["E" + iFig.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsEducando.Cells["F" + iFig.ToString()].Value = item.NombrePreregistro;
                            wsEducando.Cells["F" + iFig.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsEducando.Cells["G" + iFig.ToString()].Value = item.FechaPreregistro;
                            wsEducando.Cells["G" + iFig.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsEducando.Cells["H" + iFig.ToString()].Value = item.FechaRegistro;
                            wsEducando.Cells["H" + iFig.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            iFig++;
                        }

                        ws.Cells[ws.Dimension.Address].AutoFitColumns();
                        wsZona.Cells[wsZona.Dimension.Address].AutoFitColumns();
                        wsUsuEducando.Cells[wsUsuEducando.Dimension.Address].AutoFitColumns();
                        wsEducando.Cells[wsEducando.Dimension.Address].AutoFitColumns();
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("content-disposition", "attachment;  filename=Educandos Preregistrados vs Registrados" + ".xlsx");
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
        
        #region INDICADOR CERTIFICADOS PROGRAMADOS VS ENTREGADOS
        public ActionResult Certificados()
        {
            return View();
        }

        [HttpGet]
        public JsonResult CargarIndCertificadosRegion(string idPeriodo, string Region, string Zonas, int idUsuario)
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
                List<udf_CertificadosInaebaRegionList_Result> result = (from u in db.udf_CertificadosInaebaRegionList(idPeriodo, Region, Zonas, "") select u).Where(x => x.Region != null).ToList();

                var lstPlanSemanal = result.Select(C => new
                {
                    C.Region,
                    certificadosProgramadosTotal = C.certificadosProgramados,
                    certificadosEntregadosTotal = C.certificadosEntregados,
                    porcentajeCumplimientoT = (C.certificadosProgramados == 0 ? 0 : (Convert.ToDecimal(C.certificadosEntregados) / Convert.ToDecimal(C.certificadosProgramados))).ToString("0.00%"),
                    pocentajePromotor = (C.ProgramadosPromotorTotal == 0 ? 0 : (Convert.ToDecimal(C.EntregadosPromotorTotal) / Convert.ToDecimal(C.ProgramadosPromotorTotal))).ToString("0.00%"),
                    pocentajeTecnico = (C.ProgramadosTecnicoTotal == 0 ? 0 : (Convert.ToDecimal(C.EntregadosTecnicoTotal) / Convert.ToDecimal(C.ProgramadosTecnicoTotal))).ToString("0.00%"),
                    pocentajeFormador = (C.ProgramadosFromadorTotal == 0 ? 0 : (Convert.ToDecimal(C.EntregadosFormadorTotal) / Convert.ToDecimal(C.ProgramadosFromadorTotal))).ToString("0.00%"),
                });

                return Json(new { Success = true, datos = lstPlanSemanal }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = "Error al obtener la información" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult CargarIndCertificadosRegionGrafico(string idPeriodo, string Region, string Zonas, int idUsuario)
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


                var ltsGenFob = (from fg in db.udf_CertificadosInaebaRegionList(idPeriodo, Region, Zonas, "")
                                 select fg).Where(x => x.Region != null).OrderByDescending(a => a.Region).ToList();

                var lstPlanSemanal = ltsGenFob.Select(C => new
                {
                    C.Region,
                    certificadosProgramadosTotal = C.certificadosProgramados,
                    certificadosEntregadosTotal = C.certificadosEntregados,
                    porcentajeCumplimientoT = (C.certificadosProgramados == 0 ? 0 : (Convert.ToDecimal(C.certificadosEntregados) / Convert.ToDecimal(C.certificadosProgramados))).ToString("0.00%"),
                    pocentajePromotor = (C.ProgramadosPromotorTotal == 0 ? 0 : (Convert.ToDecimal(C.EntregadosPromotorTotal) / Convert.ToDecimal(C.ProgramadosPromotorTotal))).ToString("0.00%"),
                    pocentajeTecnico = (C.ProgramadosTecnicoTotal == 0 ? 0 : (Convert.ToDecimal(C.EntregadosTecnicoTotal) / Convert.ToDecimal(C.ProgramadosTecnicoTotal))).ToString("0.00%"),
                    pocentajeFormador = (C.ProgramadosFromadorTotal == 0 ? 0 : (Convert.ToDecimal(C.EntregadosFormadorTotal) / Convert.ToDecimal(C.ProgramadosFromadorTotal))).ToString("0.00%"),
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
        public JsonResult CargarIndCertificadosZona(string idPeriodo, string Region, string Zonas, int idUsuario)
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
                List<udf_CertificadosInaebaZonaList_Result> result = (from u in db.udf_CertificadosInaebaZonaList(idPeriodo, Region, Zonas, "") select u).Where(x => x.Region != null).ToList();

                var lstPlanSemanal = result.Select(C => new
                {
                    C.Region,
                    C.Zona,
                    certificadosProgramadosTotal = C.certificadosProgramados,
                    certificadosEntregadosTotal = C.certificadosEntregados,
                    porcentajeCumplimientoT = (C.certificadosProgramados == 0 ? 0 : (Convert.ToDecimal(C.certificadosEntregados) / Convert.ToDecimal(C.certificadosProgramados))).ToString("0.00%"),
                    pocentajePromotor = (C.ProgramadosPromotorTotal == 0 ? 0 : (Convert.ToDecimal(C.EntregadosPromotorTotal) / Convert.ToDecimal(C.ProgramadosPromotorTotal))).ToString("0.00%"),
                    pocentajeTecnico = (C.ProgramadosTecnicoTotal == 0 ? 0 : (Convert.ToDecimal(C.EntregadosTecnicoTotal) / Convert.ToDecimal(C.ProgramadosTecnicoTotal))).ToString("0.00%"),
                    pocentajeFormador = (C.ProgramadosFromadorTotal == 0 ? 0 : (Convert.ToDecimal(C.EntregadosFormadorTotal) / Convert.ToDecimal(C.ProgramadosFromadorTotal))).ToString("0.00%"),
                });

                return Json(new { Success = true, datos = lstPlanSemanal }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = "Error al obtener la información" }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult CargarIndCertificadosZonaGrafico(string idPeriodo, string Region, string Zonas, int idUsuario)
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


                var ltsGenFob = (from fg in db.udf_CertificadosInaebaZonaList(idPeriodo, Region, Zonas, "")
                                 select fg).Where(x => x.Region != null).OrderByDescending(a => a.Zona).ToList();

                var lstPlanSemanal = ltsGenFob.Select(C => new
                {
                    C.Zona,
                    certificadosProgramadosTotal = C.certificadosProgramados,
                    certificadosEntregadosTotal = C.certificadosEntregados,
                    porcentajeCumplimientoT = (C.certificadosProgramados == 0 ? 0 : (Convert.ToDecimal(C.certificadosEntregados) / Convert.ToDecimal(C.certificadosProgramados))).ToString("0.00%"),
                    pocentajePromotor = (C.ProgramadosPromotorTotal == 0 ? 0 : (Convert.ToDecimal(C.EntregadosPromotorTotal) / Convert.ToDecimal(C.ProgramadosPromotorTotal))).ToString("0.00%"),
                    pocentajeTecnico = (C.ProgramadosTecnicoTotal == 0 ? 0 : (Convert.ToDecimal(C.EntregadosTecnicoTotal) / Convert.ToDecimal(C.ProgramadosTecnicoTotal))).ToString("0.00%"),
                    pocentajeFormador = (C.ProgramadosFromadorTotal == 0 ? 0 : (Convert.ToDecimal(C.EntregadosFormadorTotal) / Convert.ToDecimal(C.ProgramadosFromadorTotal))).ToString("0.00%"),
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
        public JsonResult CargarIndCertificadosZonaFigura(string idPeriodo, string Region, string Zonas, int idUsuario)
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
                List<udf_CertificadosInaebaZonaFiguraList_Result> result = (from u in db.udf_CertificadosInaebaZonaFiguraList(idPeriodo, Region, Zonas, "") select u).Where(x => x.Periodo != null).ToList();

                var lstPlanSemanal = result.Select(C => new
                {
                    C.Periodo,
                    C.Region,
                    C.Zona,
                    C.DescripcionTipoFigura,
                    C.NombreUsuario,
                    certificadosProgramadosTotal = C.certificadosProgramados,
                    certificadosEntregadosTotal = C.certificadosEntregados,
                    porcentajeCumplimientoT = (C.certificadosProgramados == 0 ? 0 : (Convert.ToDecimal(C.certificadosEntregados) / Convert.ToDecimal(C.certificadosProgramados))).ToString("0.00%"),
                });

                return Json(new { Success = true, datos = lstPlanSemanal }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = "Error al obtener la información" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult CargarIndCertificadosFiguraEducando(string idPeriodo, string Region, string Zonas, int idUsuario)
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
                List<udf_CertificadosInaebaFiguraEducandoList_Result> result = (from u in db.udf_CertificadosInaebaFiguraEducandoList(idPeriodo, Region, Zonas, "") select u).Where(x => x.Periodo != null).ToList();

                var lstPlanSemanal = result.Select(C => new
                {
                    C.Periodo,
                    C.Region,
                    C.Zona,
                    C.DescripcionTipoFigura,
                    C.NombreUsuario,
                    Educando = C.mc_nombre,
                    Curp = C.mc_curp,
                    FolioCertificado = C.mc_foliocert,
                    Acuse = C.mc_matrnu,
                    EstatusCert = C.st_desc,
                    FechaEmision = C.mc_fconclusion,
                    FechaPlan = C.FechaActividad,
                    FechaCheck = C.FechaCreacion,
                });

                return Json(new { Success = true, datos = lstPlanSemanal }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = "Error al obtener la información" }, JsonRequestBehavior.AllowGet);
            }
        }

        public void ExportIndCertificadosExcel(string idPeriodo, string Region, string Zonas, int idUsuario)
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

                //Region
                List<udf_CertificadosInaebaRegionList_Result> result = (from u in db.udf_CertificadosInaebaRegionList(idPeriodo, Region, Zonas, "") select u).Where(x => x.Region != null).ToList();

                var lstPlanSemanal = result.Select(C => new
                {
                    C.Region,
                    certificadosProgramadosTotal = C.certificadosProgramados,
                    certificadosEntregadosTotal = C.certificadosEntregados,
                    porcentajeCumplimientoT = (C.certificadosProgramados == 0 ? 0 : (Convert.ToDecimal(C.certificadosEntregados) / Convert.ToDecimal(C.certificadosProgramados))).ToString("0.00%"),
                    pocentajePromotor = (C.ProgramadosPromotorTotal == 0 ? 0 : (Convert.ToDecimal(C.EntregadosPromotorTotal) / Convert.ToDecimal(C.ProgramadosPromotorTotal))).ToString("0.00%"),
                    pocentajeTecnico = (C.ProgramadosTecnicoTotal == 0 ? 0 : (Convert.ToDecimal(C.EntregadosTecnicoTotal) / Convert.ToDecimal(C.ProgramadosTecnicoTotal))).ToString("0.00%"),
                    pocentajeFormador = (C.ProgramadosFromadorTotal == 0 ? 0 : (Convert.ToDecimal(C.EntregadosFormadorTotal) / Convert.ToDecimal(C.ProgramadosFromadorTotal))).ToString("0.00%"),
                });

                //Zona
                List<udf_CertificadosInaebaZonaList_Result> resultZona = (from u in db.udf_CertificadosInaebaZonaList(idPeriodo, Region, Zonas, "") select u).Where(x => x.Region != null).ToList();

                var lstPlanSemanalZona = resultZona.Select(C => new
                {
                    C.Region,
                    C.Zona,
                    certificadosProgramadosTotal = C.certificadosProgramados,
                    certificadosEntregadosTotal = C.certificadosEntregados,
                    porcentajeCumplimientoT = (C.certificadosProgramados == 0 ? 0 : (Convert.ToDecimal(C.certificadosEntregados) / Convert.ToDecimal(C.certificadosProgramados))).ToString("0.00%"),
                    pocentajePromotor = (C.ProgramadosPromotorTotal == 0 ? 0 : (Convert.ToDecimal(C.EntregadosPromotorTotal) / Convert.ToDecimal(C.ProgramadosPromotorTotal))).ToString("0.00%"),
                    pocentajeTecnico = (C.ProgramadosTecnicoTotal == 0 ? 0 : (Convert.ToDecimal(C.EntregadosTecnicoTotal) / Convert.ToDecimal(C.ProgramadosTecnicoTotal))).ToString("0.00%"),
                    pocentajeFormador = (C.ProgramadosFromadorTotal == 0 ? 0 : (Convert.ToDecimal(C.EntregadosFormadorTotal) / Convert.ToDecimal(C.ProgramadosFromadorTotal))).ToString("0.00%"),
                });

                //Zona Figura
                List<udf_CertificadosInaebaZonaFiguraList_Result> resultZFigura = (from u in db.udf_CertificadosInaebaZonaFiguraList(idPeriodo, Region, Zonas, "") select u).Where(x => x.Periodo != null).ToList();

                var lstPlanSemanalZFigura = resultZFigura.Select(C => new
                {
                    C.Periodo,
                    C.Region,
                    C.Zona,
                    C.DescripcionTipoFigura,
                    C.NombreUsuario,
                    certificadosProgramadosTotal = C.certificadosProgramados,
                    certificadosEntregadosTotal = C.certificadosEntregados,
                    porcentajeCumplimientoT = (C.certificadosProgramados == 0 ? 0 : (Convert.ToDecimal(C.certificadosEntregados) / Convert.ToDecimal(C.certificadosProgramados))).ToString("0.00%"),
                });

                //Figura Educando
                List<udf_CertificadosInaebaFiguraEducandoList_Result> resultFEducando = (from u in db.udf_CertificadosInaebaFiguraEducandoList(idPeriodo, Region, Zonas, "") select u).Where(x => x.Periodo != null).ToList();

                var lstPlanSemanalFEducando = resultFEducando.Select(C => new
                {
                    C.Periodo,
                    C.Region,
                    C.Zona,
                    C.DescripcionTipoFigura,
                    C.NombreUsuario,
                    Educando = C.mc_nombre,
                    Curp = C.mc_curp,
                    FolioCertificado = C.mc_foliocert,
                    Acuse = C.mc_matrnu,
                    EstatusCert = C.st_desc,
                    FechaEmision = C.mc_fconclusion,
                    FechaPlan = C.FechaActividad,
                    FechaCheck = C.FechaCreacion,
                });

                if (lstPlanSemanal.Count() > 0)
                {


                    using (ExcelPackage pkg = new ExcelPackage())
                    {
                        ExcelWorksheet ws = pkg.Workbook.Worksheets.Add("Región");
                        Color colFromHex = System.Drawing.ColorTranslator.FromHtml("#D9D9D9");
                        ws.Cells["A1:G1"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;

                        ws.Cells["A1"].Value = "Región";
                        ws.Cells["A1"].Style.Font.Bold = true;
                        ws.Cells["A1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        ws.Cells["B1"].Value = "Certificados programados";
                        ws.Cells["B1"].Style.Font.Bold = true;
                        ws.Cells["B1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        ws.Cells["C1"].Value = "Certificados entregados";
                        ws.Cells["C1"].Style.Font.Bold = true;
                        ws.Cells["C1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        ws.Cells["D1"].Value = "%Cumplimiento";
                        ws.Cells["D1"].Style.Font.Bold = true;
                        ws.Cells["D1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        ws.Cells["E1"].Value = "%Promotor";
                        ws.Cells["E1"].Style.Font.Bold = true;
                        ws.Cells["E1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        ws.Cells["F1"].Value = "%Técnicos docentes";
                        ws.Cells["F1"].Style.Font.Bold = true;
                        ws.Cells["F1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        ws.Cells["G1"].Value = "%Formadores";
                        ws.Cells["G1"].Style.Font.Bold = true;
                        ws.Cells["G1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        using (ExcelRange cell = ws.Cells["A1:G1"])
                        {
                            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                            cell.Style.Fill.BackgroundColor.SetColor(colFromHex);
                        }

                        int i = 2;
                        foreach (var item in lstPlanSemanal)
                        {
                            ws.Cells["A" + i.ToString()].Value = item.Region;
                            ws.Cells["A" + i.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            ws.Cells["B" + i.ToString()].Value = item.certificadosProgramadosTotal;
                            ws.Cells["B" + i.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            ws.Cells["C" + i.ToString()].Value = item.certificadosEntregadosTotal;
                            ws.Cells["C" + i.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            ws.Cells["D" + i.ToString()].Value = item.porcentajeCumplimientoT;
                            ws.Cells["D" + i.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            ws.Cells["E" + i.ToString()].Value = item.pocentajePromotor;
                            ws.Cells["E" + i.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            ws.Cells["F" + i.ToString()].Value = item.pocentajeTecnico;
                            ws.Cells["F" + i.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            ws.Cells["G" + i.ToString()].Value = item.pocentajeFormador;
                            ws.Cells["G" + i.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            i++;

                        }

                        //Hoja Zona

                        ExcelWorksheet wsZona = pkg.Workbook.Worksheets.Add("Zona");
                        wsZona.Cells["A1:H1"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;

                        wsZona.Cells["A1"].Value = "Región";
                        wsZona.Cells["A1"].Style.Font.Bold = true;
                        wsZona.Cells["A1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsZona.Cells["B1"].Value = "Zona";
                        wsZona.Cells["B1"].Style.Font.Bold = true;
                        wsZona.Cells["B1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsZona.Cells["C1"].Value = "Certificados programados";
                        wsZona.Cells["C1"].Style.Font.Bold = true;
                        wsZona.Cells["C1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsZona.Cells["D1"].Value = "Certificados entregados";
                        wsZona.Cells["D1"].Style.Font.Bold = true;
                        wsZona.Cells["D1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsZona.Cells["E1"].Value = "%Cumplimiento";
                        wsZona.Cells["E1"].Style.Font.Bold = true;
                        wsZona.Cells["E1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsZona.Cells["F1"].Value = "%Promotor";
                        wsZona.Cells["F1"].Style.Font.Bold = true;
                        wsZona.Cells["F1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsZona.Cells["G1"].Value = "%Técnicos docentes";
                        wsZona.Cells["G1"].Style.Font.Bold = true;
                        wsZona.Cells["G1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsZona.Cells["H1"].Value = "%Formadores";
                        wsZona.Cells["H1"].Style.Font.Bold = true;
                        wsZona.Cells["H1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        using (ExcelRange cell = wsZona.Cells["A1:H1"])
                        {
                            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                            cell.Style.Fill.BackgroundColor.SetColor(colFromHex);
                        }
                        int iZona = 2;
                        foreach (var item in lstPlanSemanalZona)
                        {
                            wsZona.Cells["A" + iZona.ToString()].Value = item.Region;
                            wsZona.Cells["A" + iZona.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsZona.Cells["B" + iZona.ToString()].Value = item.Zona;
                            wsZona.Cells["B" + iZona.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsZona.Cells["C" + iZona.ToString()].Value = item.certificadosProgramadosTotal;
                            wsZona.Cells["C" + iZona.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsZona.Cells["D" + iZona.ToString()].Value = item.certificadosEntregadosTotal;
                            wsZona.Cells["D" + iZona.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsZona.Cells["E" + iZona.ToString()].Value = item.porcentajeCumplimientoT;
                            wsZona.Cells["E" + iZona.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsZona.Cells["F" + iZona.ToString()].Value = item.pocentajePromotor;
                            wsZona.Cells["F" + iZona.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsZona.Cells["G" + iZona.ToString()].Value = item.pocentajeTecnico;
                            wsZona.Cells["G" + iZona.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsZona.Cells["H" + iZona.ToString()].Value = item.pocentajeFormador;
                            wsZona.Cells["H" + iZona.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            iZona++;
                        }

                        // Hoja Zona Figura

                        ExcelWorksheet wsUsuEducando = pkg.Workbook.Worksheets.Add("Detalle programado vs entregado");
                        wsUsuEducando.Cells["A1:H1"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;

                        wsUsuEducando.Cells["A1"].Value = "Periodo";
                        wsUsuEducando.Cells["A1"].Style.Font.Bold = true;
                        wsUsuEducando.Cells["A1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsUsuEducando.Cells["B1"].Value = "Region";
                        wsUsuEducando.Cells["B1"].Style.Font.Bold = true;
                        wsUsuEducando.Cells["B1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsUsuEducando.Cells["C1"].Value = "Zona";
                        wsUsuEducando.Cells["C1"].Style.Font.Bold = true;
                        wsUsuEducando.Cells["C1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsUsuEducando.Cells["D1"].Value = "Tipo Figura";
                        wsUsuEducando.Cells["D1"].Style.Font.Bold = true;
                        wsUsuEducando.Cells["D1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsUsuEducando.Cells["E1"].Value = "Nombre Figura";
                        wsUsuEducando.Cells["E1"].Style.Font.Bold = true;
                        wsUsuEducando.Cells["E1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsUsuEducando.Cells["F1"].Value = "Certificados programados";
                        wsUsuEducando.Cells["F1"].Style.Font.Bold = true;
                        wsUsuEducando.Cells["F1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsUsuEducando.Cells["G1"].Value = "Certificados entregados";
                        wsUsuEducando.Cells["G1"].Style.Font.Bold = true;
                        wsUsuEducando.Cells["G1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsUsuEducando.Cells["H1"].Value = "%Cumplimiento";
                        wsUsuEducando.Cells["H1"].Style.Font.Bold = true;
                        wsUsuEducando.Cells["H1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        using (ExcelRange cell = wsUsuEducando.Cells["A1:H1"])
                        {
                            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                            cell.Style.Fill.BackgroundColor.SetColor(colFromHex);
                        }
                        int iUsuEducando = 2;
                        foreach (var item in lstPlanSemanalZFigura)
                        {
                            wsUsuEducando.Cells["A" + iUsuEducando.ToString()].Value = item.Periodo;
                            wsUsuEducando.Cells["A" + iUsuEducando.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsUsuEducando.Cells["B" + iUsuEducando.ToString()].Value = item.Region;
                            wsUsuEducando.Cells["B" + iUsuEducando.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsUsuEducando.Cells["C" + iUsuEducando.ToString()].Value = item.Zona;
                            wsUsuEducando.Cells["C" + iUsuEducando.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsUsuEducando.Cells["D" + iUsuEducando.ToString()].Value = item.DescripcionTipoFigura;
                            wsUsuEducando.Cells["D" + iUsuEducando.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsUsuEducando.Cells["E" + iUsuEducando.ToString()].Value = item.NombreUsuario;
                            wsUsuEducando.Cells["E" + iUsuEducando.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsUsuEducando.Cells["F" + iUsuEducando.ToString()].Value = item.certificadosProgramadosTotal;
                            wsUsuEducando.Cells["F" + iUsuEducando.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsUsuEducando.Cells["G" + iUsuEducando.ToString()].Value = item.certificadosEntregadosTotal;
                            wsUsuEducando.Cells["G" + iUsuEducando.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsUsuEducando.Cells["H" + iUsuEducando.ToString()].Value = item.porcentajeCumplimientoT;
                            wsUsuEducando.Cells["H" + iUsuEducando.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            iUsuEducando++;
                        }

                        // Hoja Figura Educando
                        ExcelWorksheet wsEducando = pkg.Workbook.Worksheets.Add("Consulta por educando de certificados");
                        wsEducando.Cells["A1:M1"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;

                        wsEducando.Cells["A1"].Value = "Periodo";
                        wsEducando.Cells["A1"].Style.Font.Bold = true;
                        wsEducando.Cells["A1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsEducando.Cells["B1"].Value = "Region";
                        wsEducando.Cells["B1"].Style.Font.Bold = true;
                        wsEducando.Cells["B1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsEducando.Cells["C1"].Value = "Zona";
                        wsEducando.Cells["C1"].Style.Font.Bold = true;
                        wsEducando.Cells["C1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsEducando.Cells["D1"].Value = "Tipo Figura";
                        wsEducando.Cells["D1"].Style.Font.Bold = true;
                        wsEducando.Cells["D1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsEducando.Cells["E1"].Value = "Nombre Figura";
                        wsEducando.Cells["E1"].Style.Font.Bold = true;
                        wsEducando.Cells["E1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsEducando.Cells["F1"].Value = "Nombre del educando";
                        wsEducando.Cells["F1"].Style.Font.Bold = true;
                        wsEducando.Cells["F1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsEducando.Cells["G1"].Value = "CURP del educando";
                        wsEducando.Cells["G1"].Style.Font.Bold = true;
                        wsEducando.Cells["G1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsEducando.Cells["H1"].Value = "Folio del certificado";
                        wsEducando.Cells["H1"].Style.Font.Bold = true;
                        wsEducando.Cells["H1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsEducando.Cells["I1"].Value = "Acuse de recibo";
                        wsEducando.Cells["I1"].Style.Font.Bold = true;
                        wsEducando.Cells["I1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsEducando.Cells["J1"].Value = "Estatus del certificado";
                        wsEducando.Cells["J1"].Style.Font.Bold = true;
                        wsEducando.Cells["J1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsEducando.Cells["K1"].Value = "Fecha emisión";
                        wsEducando.Cells["K1"].Style.Font.Bold = true;
                        wsEducando.Cells["K1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsEducando.Cells["L1"].Value = "Fecha programada";
                        wsEducando.Cells["L1"].Style.Font.Bold = true;
                        wsEducando.Cells["L1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        wsEducando.Cells["M1"].Value = "Fecha entregado";
                        wsEducando.Cells["M1"].Style.Font.Bold = true;
                        wsEducando.Cells["M1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        using (ExcelRange cell = wsEducando.Cells["A1:M1"])
                        {
                            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                            cell.Style.Fill.BackgroundColor.SetColor(colFromHex);
                        }
                        int iFig = 2;
                        foreach (var item in lstPlanSemanalFEducando)
                        {
                            wsEducando.Cells["A" + iFig.ToString()].Value = item.Periodo;
                            wsEducando.Cells["A" + iFig.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsEducando.Cells["B" + iFig.ToString()].Value = item.Region;
                            wsEducando.Cells["B" + iFig.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsEducando.Cells["C" + iFig.ToString()].Value = item.Zona;
                            wsEducando.Cells["C" + iFig.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsEducando.Cells["D" + iFig.ToString()].Value = item.DescripcionTipoFigura;
                            wsEducando.Cells["D" + iFig.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsEducando.Cells["E" + iFig.ToString()].Value = item.NombreUsuario;
                            wsEducando.Cells["E" + iFig.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsEducando.Cells["F" + iFig.ToString()].Value = item.Educando;
                            wsEducando.Cells["F" + iFig.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsEducando.Cells["G" + iFig.ToString()].Value = item.Curp;
                            wsEducando.Cells["G" + iFig.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsEducando.Cells["H" + iFig.ToString()].Value = item.FolioCertificado;
                            wsEducando.Cells["H" + iFig.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsEducando.Cells["I" + iFig.ToString()].Value = item.Acuse;
                            wsEducando.Cells["I" + iFig.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsEducando.Cells["J" + iFig.ToString()].Value = item.EstatusCert;
                            wsEducando.Cells["J" + iFig.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsEducando.Cells["K" + iFig.ToString()].Value = item.FechaEmision;
                            wsEducando.Cells["K" + iFig.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsEducando.Cells["L" + iFig.ToString()].Value = item.FechaPlan;
                            wsEducando.Cells["L" + iFig.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            wsEducando.Cells["M" + iFig.ToString()].Value = item.FechaCheck;
                            wsEducando.Cells["M" + iFig.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            iFig++;
                        }

                        ws.Cells[ws.Dimension.Address].AutoFitColumns();
                        wsZona.Cells[wsZona.Dimension.Address].AutoFitColumns();
                        wsUsuEducando.Cells[wsUsuEducando.Dimension.Address].AutoFitColumns();
                        wsEducando.Cells[wsEducando.Dimension.Address].AutoFitColumns();
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("content-disposition", "attachment;  filename=Cantidad de certificados entregados VS programados" + ".xlsx");
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