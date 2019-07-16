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
    public class IncidenciasController : Controller
    {
        private Inaeba_SaptraEntities db = new Inaeba_SaptraEntities();

        #region INCIDENCIAS
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult CargarIncidencias(int? idUsu, int? idEstatus, string Periodos, string TiposFigura, string NombresFigura, string TipoActividades)
        {
            try
            {
                List<udf_PlanSemanalIncidenciasList_Result> result = (from u in db.udf_PlanSemanalIncidenciasList(idUsu, Periodos, TiposFigura, NombresFigura, TipoActividades) select u) .ToList();

                var lstPlanSemanal = result.Select(C => new
                {
                    id = C.DetallePlanId,
                    periodo = C.Periodo.ToUpper(),
                    usuario = C.Usuario,
                    actividad = C.NombreActividad,
                    descripcion = C.DescripcionActividad,
                    fecha = C.FechaActividad.Value.ToString("dd/MM/yyyy"),
                    comentarios = C.ComentariosRechazo,
                    numeroEmp = C.NumeroEmpleado
                });

                return Json(new { Success = true, datos = lstPlanSemanal }, JsonRequestBehavior.AllowGet);
            }
#pragma warning disable CS0168 // La variable 'exp' se ha declarado pero nunca se usa
            catch (Exception exp)
#pragma warning restore CS0168 // La variable 'exp' se ha declarado pero nunca se usa
            {
                return Json(new { Success = false, Message = "Error al obtener la información" }, JsonRequestBehavior.AllowGet);
            }
        }

        public void ExportIncidenciasExcel(int? idUsu, int? idEstatus, string Periodos, string TiposFigura, string NombresFigura, string TipoActividades)
        {
            try
            {
                List<udf_PlanSemanalIncidenciasList_Result> result = (from u in db.udf_PlanSemanalIncidenciasList(idUsu, Periodos, TiposFigura, NombresFigura, TipoActividades) select u).ToList();

                var lstPlanSemanal = result.Select(C => new
                {
                    id = C.DetallePlanId,
                    periodo = C.Periodo.ToUpper(),
                    usuario = C.Usuario,
                    actividad = C.NombreActividad,
                    descripcion = C.DescripcionActividad,
                    fecha = C.FechaActividad.Value.ToString("dd/MM/yyyy"),
                    comentarios = C.ComentariosRechazo,
                    C.CoordinacionZona,
                    C.NumeroEmpleado
                });

                if (lstPlanSemanal.Count() > 0)
                {


                    using (ExcelPackage pkg = new ExcelPackage())
                    {
                        ExcelWorksheet ws = pkg.Workbook.Worksheets.Add("Reporte Incidencias");
                        Color colFromHex = System.Drawing.ColorTranslator.FromHtml("#D9D9D9");
                        ws.Cells["A8:L8"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;

                        //Imagen logo GTO
                        Bitmap image = new Bitmap(Server.MapPath("~/Content/images/gto_logo.png"));
                        var picture = ws.Drawings.AddPicture("gtologo", image);
                        picture.SetPosition(0 * 1, 0, 0, 0);

                        //Imagen logo soy inaeba
                        Bitmap imageInaeba = new Bitmap(Server.MapPath("~/Content/images/logo-soy-inaeba.png"));
                        var pictureInaeba = ws.Drawings.AddPicture("soyinaebalogo", imageInaeba);
                        pictureInaeba.SetPosition(2 * 1, 0, 11, 0);
                   
                        ws.Cells["B2:K2"].Merge = true;
                       
                        ws.Cells["B2:K2"].Value = "INSTITUTO DE ALFABETIZACIÓN Y EDUCACIÓN BÁSICA PARA ADULTOS";
                        ws.Cells["B2:K2"].Style.Font.Bold = true;
                        ws.Cells["B2:K2"].AutoFitColumns();
                        ws.Cells["B2:K2"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        ws.Cells["B3:K3"].Merge = true;
                        ws.Cells["B3:K3"].Value = "REPORTE DE INCIDENCIAS";
                        ws.Cells["B3:K3"].Style.Font.Bold = true;
                        ws.Cells["B3:K3"].AutoFitColumns();
                        ws.Cells["B3:K3"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        ws.Cells["A8"].Value = "CZ";
                        ws.Cells["A8"].Style.Font.Bold = true;
                        ws.Cells["A8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        ws.Cells["B8"].Value = "PERIODO";
                        ws.Cells["B8"].Style.Font.Bold = true;
                        ws.Cells["B8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        ws.Cells["C8"].Value = "NOMBRE EMPLEADO";
                        ws.Cells["C8"].Style.Font.Bold = true;
                        ws.Cells["C8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        ws.Cells["D8:E8"].Merge = true;
                        ws.Cells["D8:E8"].Value = "ACTIVIDAD";
                        ws.Cells["D8:E8"].Style.Font.Bold = true;
                        ws.Cells["D8:E8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        ws.Cells["F8:H8"].Merge = true;
                        ws.Cells["F8:H8"].Value = "DESCRIPCION";
                        ws.Cells["F8:H8"].Style.Font.Bold = true;
                        ws.Cells["F8:H8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        ws.Cells["I8"].Value = "FECHA";
                        ws.Cells["I8"].Style.Font.Bold = true;
                        ws.Cells["I8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        ws.Cells["J8:K8"].Merge = true;
                        ws.Cells["J8:K8"].Value = "COMENTARIOS CZ";
                        ws.Cells["J8:K8"].Style.Font.Bold = true;
                        ws.Cells["J8:K8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        ws.Cells["L8"].Value = "NÚMERO EMPLEADO";
                        ws.Cells["L8"].Style.Font.Bold = true;
                        ws.Cells["L8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        using (ExcelRange cell = ws.Cells["A8:L8"])
                        {
                            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                            cell.Style.Fill.BackgroundColor.SetColor(colFromHex);
                        }                      

                        int i = 9;
                        foreach (var item in lstPlanSemanal)
                        {
                            ws.Cells["A" + i.ToString()].Value = item.CoordinacionZona;
                            ws.Cells["A" + i.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            ws.Cells["B" + i.ToString()].Value = item.periodo;
                            ws.Cells["B" + i.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            ws.Cells["C" + i.ToString()].Value = item.usuario;
                            ws.Cells["C" + i.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            ws.Cells["D" + i.ToString() + ":E" + i.ToString()].Merge = true;
                            ws.Cells["D" + i.ToString() + ":E" + i.ToString()].Value = item.actividad;
                            ws.Cells["D" + i.ToString() + ":E" + i.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            ws.Cells["F" + i.ToString() + ":H" + i.ToString()].Merge = true;
                            ws.Cells["F" + i.ToString() + ":H" + i.ToString()].Value = item.descripcion;
                            ws.Cells["F" + i.ToString() + ":H" + i.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            ws.Cells["I" + i.ToString()].Value = item.fecha;
                            ws.Cells["I" + i.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            ws.Cells["J" + i.ToString() + ":K" + i.ToString()].Merge = true;
                            ws.Cells["J" + i.ToString() + ":K" + i.ToString()].Value = item.comentarios;
                            ws.Cells["J" + i.ToString() + ":K" + i.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            ws.Cells["L" + i.ToString()].Value = item.NumeroEmpleado;
                            ws.Cells["L" + i.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            i++;

                        }

                        ws.Cells[ws.Dimension.Address].AutoFitColumns();
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("content-disposition", "attachment;  filename=ReporteIncidencias" + ".xlsx");
                        Response.BinaryWrite(pkg.GetAsByteArray());


                    }


                }

            }
#pragma warning disable CS0168 // La variable 'exp' se ha declarado pero nunca se usa
            catch (Exception exp)
#pragma warning restore CS0168 // La variable 'exp' se ha declarado pero nunca se usa
            {
                //return  exp.Message;
            }

        }

        //public void ExportPdf()
        //{
        //    //Workbook workbook = new Workbook();
        //    //workbook.LoadFromFile("C:/Users/davidjasso.CHARLY/Downloads/ReporteIncidencias.xlsx", ExcelVersion.Version2010);
        //    //workbook.SaveToFile("result.pdf", Spire.Xls.FileFormat.PDF);

        //    Workbook workbook = new Workbook();
        //    workbook.LoadFromFile("C:/Users/davidjasso.CHARLY/Downloads/ReporteIncidencias.xlsx");

        //    PdfDocument pdfDocument = new PdfDocument();
        //    pdfDocument.PageSettings.Orientation = PdfPageOrientation.Landscape;
        //    pdfDocument.PageSettings.Width = 970;
        //    pdfDocument.PageSettings.Height = 850;

        //    PdfConverter pdfConverter = new PdfConverter(workbook);
        //    PdfConverterSettings settings = new PdfConverterSettings();
        //    settings.TemplateDocument = pdfDocument;
        //    //pdfDocument = pdfConverter.Convert().Convert(settings);

        //    pdfDocument.SaveToFile("result.pdf");
        //    System.Diagnostics.Process.Start("result.pdf");

        //}


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