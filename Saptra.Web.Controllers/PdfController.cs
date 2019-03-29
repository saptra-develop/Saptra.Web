using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using Saptra.Web.Models;
using Saptra.Web.Data;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Globalization;
//importamos 
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using System.IO;
using System.Web;
using System.Drawing.Drawing2D;
using RazorEngine;
using iTextSharp.text.html.simpleparser;

namespace Sispro.Web.Controllers

{
    public class PdfController : Controller
    {
        private Inaeba_SaptraEntities db = new Inaeba_SaptraEntities();
        public void ExportIncidenciasPDF(int? idUsu, string Periodos, string TiposFigura, string NombresFigura, string TipoActividades)
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

                // creacion del objeto documento
                Document DocumentoPDF = new Document(PageSize.A4.Rotate());
                PdfWriter writer = PdfWriter.GetInstance(DocumentoPDF, HttpContext.Response.OutputStream);

                ITextEvents evPdf = new ITextEvents();
                evPdf.Title = "INSTITUTO DE ALFABETIZACIÓN Y EDUCACIÓN BÁSICA PARA ADULTOS";
                evPdf.TipoReporte = "REPORTE DE INCIDENCIAS";
                evPdf.Count = lstPlanSemanal.Count();
                writer.PageEvent = evPdf;

                // Le colocamos el título y el autor
                // **Nota: Esto no será visible en el documento
                DocumentoPDF.AddTitle("Incidencias");
                DocumentoPDF.AddCreator("Inaeba");


                // Abrimos el archivo
                DocumentoPDF.Open();


                // Creamos el tipo de Font que vamos utilizar
                iTextSharp.text.Font _standardFont = new Font(Font.FontFamily.HELVETICA, 8, Font.NORMAL, BaseColor.BLACK);
                iTextSharp.text.Font _standardFontBold = new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.BLACK);


                //Crear tabla pdf
                PdfPTable TblReport = new PdfPTable(8);
                TblReport.WidthPercentage = 100;

                //Crear Celdas para el PDF
                PdfPCell cellCZ = new PdfPCell(new Phrase("CZ", _standardFontBold));
                //cellPeriodo.BorderWidth = 1;
                //cellPeriodo.BorderWidthBottom = 0.75f;
                cellCZ.BackgroundColor = BaseColor.LIGHT_GRAY;
                cellCZ.HorizontalAlignment = Element.ALIGN_CENTER;

                //Crear Celdas para el PDF
                PdfPCell cellPeriodo = new PdfPCell(new Phrase("PERIODO", _standardFontBold));
                //cellPeriodo.BorderWidth = 1;
                //cellPeriodo.BorderWidthBottom = 0.75f;
                cellPeriodo.BackgroundColor = BaseColor.LIGHT_GRAY;
                cellPeriodo.HorizontalAlignment = Element.ALIGN_CENTER;


                PdfPCell cellUsuario = new PdfPCell(new Phrase("NOMBRE EMPLEADO", _standardFontBold));
                //cellUsuario.BorderWidth = 1;
                //cellUsuario.BorderWidthBottom = 0.75f;
                cellUsuario.BackgroundColor = BaseColor.LIGHT_GRAY;
                cellUsuario.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellActividad = new PdfPCell(new Phrase("ACTIVIDAD", _standardFontBold));
                //cellActividad.BorderWidth = 1;
                //cellActividad.BorderWidthBottom = 0.75f;
                cellActividad.BackgroundColor = BaseColor.LIGHT_GRAY;
                cellActividad.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellDescripcion = new PdfPCell(new Phrase("DESCRIPCIÓN", _standardFontBold));
                //cellDescripcion.BorderWidth = 1;
                //cellDescripcion.BorderWidthBottom = 0.75f;
                cellDescripcion.BackgroundColor = BaseColor.LIGHT_GRAY;
                cellDescripcion.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellFecha= new PdfPCell(new Phrase("FECHA", _standardFontBold));
                //cellFecha.BorderWidth = 1;
                //cellFecha.BorderWidthBottom = 0.75f;
                cellFecha.BackgroundColor = BaseColor.LIGHT_GRAY;
                cellFecha.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellComentarios = new PdfPCell(new Phrase("COMENTARIOS CZ", _standardFontBold));
                //cellComentarios.BorderWidth = 1;
                //cellComentarios.BorderWidthBottom = 0.75f;
                cellComentarios.BackgroundColor = BaseColor.LIGHT_GRAY;
                cellComentarios.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellNumeroEmp = new PdfPCell(new Phrase("NÚMERO EMPLEADO", _standardFontBold));
                //cellComentarios.BorderWidth = 1;
                //cellComentarios.BorderWidthBottom = 0.75f;
                cellNumeroEmp.BackgroundColor = BaseColor.LIGHT_GRAY;
                cellNumeroEmp.HorizontalAlignment = Element.ALIGN_CENTER;

                //añadir las celdas a la tabla
                TblReport.AddCell(cellCZ);
                TblReport.AddCell(cellPeriodo);
                TblReport.AddCell(cellUsuario);
                TblReport.AddCell(cellActividad);
                TblReport.AddCell(cellDescripcion);
                TblReport.AddCell(cellFecha);
                TblReport.AddCell(cellComentarios);
                TblReport.AddCell(cellNumeroEmp);

                //Llenado de la tabla con la informacion
                foreach (var item in lstPlanSemanal)
                {
                    cellCZ = new PdfPCell(new Phrase(item.CoordinacionZona, _standardFont));
                    cellCZ.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellPeriodo = new PdfPCell(new Phrase(item.periodo, _standardFont));
                    cellPeriodo.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellUsuario = new PdfPCell(new Phrase(item.usuario, _standardFont));
                    cellUsuario.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellActividad = new PdfPCell(new Phrase(item.actividad, _standardFont));
                    cellActividad.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellDescripcion = new PdfPCell(new Phrase(item.descripcion, _standardFont));
                    cellDescripcion.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellFecha = new PdfPCell(new Phrase(item.fecha, _standardFont));
                    cellFecha.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellComentarios = new PdfPCell(new Phrase(item.comentarios, _standardFont));
                    cellComentarios.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellNumeroEmp = new PdfPCell(new Phrase((item.NumeroEmpleado == null ? "" : item.NumeroEmpleado.Value.ToString()), _standardFont));
                    cellNumeroEmp.HorizontalAlignment = Element.ALIGN_CENTER;


                    //Añadimos las celdas a la tabla
                    TblReport.AddCell(cellCZ);
                    TblReport.AddCell(cellPeriodo);
                    TblReport.AddCell(cellUsuario);
                    TblReport.AddCell(cellActividad);
                    TblReport.AddCell(cellDescripcion);
                    TblReport.AddCell(cellFecha);
                    TblReport.AddCell(cellComentarios);
                    TblReport.AddCell(cellNumeroEmp);
                }

                //Añadimos la tabla al documento PDF
                DocumentoPDF.Add(TblReport);

                DocumentoPDF.Close();
                writer.Close();


                HttpContext.Response.ContentType = "pdf/application";
                HttpContext.Response.AddHeader("content-disposition", "attachment;" +
               "filename=Incidencias" + "_" + DateTime.Now + ".pdf");
                HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                HttpContext.Response.Write(DocumentoPDF);
                HttpContext.Response.End();


            } // if lstProcesoOrd.Count()

        }

        public void ExportPlaneacionSemanalPDF(int? idUsu, int? idEstatus, string Periodos, string TiposFigura, string NombresFigura, int idPlan)
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

            var resultDetalle = (from cat in db.dDetallePlanSemanal
                                 where cat.PlanSemanalId == idPlan
                                 select cat).OrderBy(x => new { x.FechaActividad, x.HoraActividad }).ToList();
            
            var lstDetalle = resultDetalle.Select(cat => new
            {
                id = cat.DetallePlanId,
                actividad = cat.cTipoActividades.NombreActividad,
                descripcion = cat.DescripcionActividad,
                fecha = cat.FechaActividad.ToString("dd/MM/yyyy"),
                hora = cat.HoraActividad.ToString("hh':'mm"),
                horaFin = cat.HoraFin.Value.ToString("hh':'mm"),
                checkin = (cat.cTipoActividades.RequiereCheckIn == true ? (cat.mCheckIn.Count() == 0 ? "No realizado" : "Realizado") : "Realizado"),
                incidencias = (cat.mCheckIn.Count() == 0 ? "" : cat.mCheckIn.FirstOrDefault().Incidencias),
                lugar = cat.LugarActividad,
                comentarios = (cat.ComentariosNoValidacion == null ? "" : "Validación: " + cat.ComentariosNoValidacion) + (cat.ComentariosRechazo == null ? "" : ", Aprobación: " + cat.ComentariosRechazo)
            });

            if (lstPlanSemanal.Count() > 0)
            {

                // creacion del objeto documento
                Document DocumentoPDF = new Document(PageSize.A4.Rotate());
                PdfWriter writer = PdfWriter.GetInstance(DocumentoPDF, HttpContext.Response.OutputStream);

                ITextEvents evPdf = new ITextEvents();
                evPdf.Title = "INSTITUTO DE ALFABETIZACIÓN Y EDUCACIÓN BÁSICA PARA ADULTOS";
                evPdf.TipoReporte = "PLAN DE ACTIVIDADES SEMANAL";
                evPdf.Coordinacion = "COORDINACIÓN DE ZONA: " + lstPlanSemanal.FirstOrDefault().cz;
                evPdf.Count = lstDetalle.Count();
                writer.PageEvent = evPdf;

                // Le colocamos el título y el autor
                // **Nota: Esto no será visible en el documento
                DocumentoPDF.AddTitle("Planeacion Semanal");
                DocumentoPDF.AddCreator("Inaeba");


                // Abrimos el archivo
                DocumentoPDF.Open();


                // Creamos el tipo de Font que vamos utilizar
                iTextSharp.text.Font _standardFont = new Font(Font.FontFamily.HELVETICA, 8, Font.NORMAL, BaseColor.BLACK);
                iTextSharp.text.Font _standardFontBold = new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.BLACK);


                //Crear tabla pdf
                PdfPTable TblReport = new PdfPTable(16);
                TblReport.WidthPercentage = 100;
                //Crear Celdas encabezado
                PdfPCell cellPersonaOp = new PdfPCell(new Phrase("PERSONAL OPERATIVO", _standardFontBold));
                //cellPersonaOp.BorderWidth = 0;
                //cellPersonaOp.BorderWidthBottom = 0.75f;
                cellPersonaOp.BackgroundColor = BaseColor.LIGHT_GRAY;
                cellPersonaOp.HorizontalAlignment = Element.ALIGN_LEFT;
                cellPersonaOp.Colspan = 3;

                PdfPCell cellPersonal = new PdfPCell(new Phrase(lstPlanSemanal.FirstOrDefault().usuario, _standardFontBold));
                //cellPersonal.BorderWidth = 0;
                //cellPersonal.BorderWidthBottom = 0.75f;
                cellPersonal.BackgroundColor = BaseColor.WHITE;
                cellPersonal.HorizontalAlignment = Element.ALIGN_LEFT;
                cellPersonal.Colspan = 2;

                PdfPCell cellPeriodo = new PdfPCell(new Phrase("PERIODO", _standardFontBold));
                //cellPeriodo.BorderWidth = 0;
                //cellPeriodo.BorderWidthBottom = 0.75f;
                cellPeriodo.BackgroundColor = BaseColor.LIGHT_GRAY;
                cellPeriodo.HorizontalAlignment = Element.ALIGN_LEFT;
                cellPeriodo.Colspan = 2;

                PdfPCell cellPeriodoNom = new PdfPCell(new Phrase(lstPlanSemanal.FirstOrDefault().periodo, _standardFontBold));
                //cellPeriodoNom.BorderWidth = 0;
                //cellPeriodoNom.BorderWidthBottom = 0.75f;
                cellPeriodoNom.BackgroundColor = BaseColor.WHITE;
                cellPeriodoNom.HorizontalAlignment = Element.ALIGN_LEFT;
                cellPeriodoNom.Colspan = 2;

                PdfPCell cellMes = new PdfPCell(new Phrase("MES", _standardFontBold));
                //cellMes.BorderWidth = 0;
                //cellMes.BorderWidthBottom = 0.75f;
                cellMes.BackgroundColor = BaseColor.LIGHT_GRAY;
                cellMes.HorizontalAlignment = Element.ALIGN_LEFT;
                cellMes.Colspan = 2;

                PdfPCell cellMesNom = new PdfPCell(new Phrase(lstPlanSemanal.FirstOrDefault().mes, _standardFont));
                //cellMesNom.BorderWidth = 0;
                //cellMesNom.BorderWidthBottom = 0.75f;
                cellMesNom.BackgroundColor = BaseColor.WHITE;
                cellMesNom.HorizontalAlignment = Element.ALIGN_LEFT;
                cellMesNom.Colspan = 2;

                PdfPCell cellAnio = new PdfPCell(new Phrase("AÑO", _standardFontBold));
                //cellAnio.BorderWidth = 0;
                //cellAnio.BorderWidthBottom = 0.75f;
                cellAnio.BackgroundColor = BaseColor.LIGHT_GRAY;
                cellAnio.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell cellAnioNom = new PdfPCell(new Phrase(lstPlanSemanal.FirstOrDefault().anio.ToString(), _standardFontBold));
                //cellAnioNom.BorderWidth = 0;
                //cellAnioNom.BorderWidthBottom = 0.75f;
                cellAnioNom.BackgroundColor = BaseColor.WHITE;
                cellAnioNom.HorizontalAlignment = Element.ALIGN_LEFT;
                cellAnioNom.Colspan = 2;

                //añadir las celdas de tabla encabezado
                TblReport.AddCell(cellPersonaOp);
                TblReport.AddCell(cellPersonal);
                TblReport.AddCell(cellPeriodo);
                TblReport.AddCell(cellPeriodoNom);
                TblReport.AddCell(cellMes);
                TblReport.AddCell(cellMesNom);
                TblReport.AddCell(cellAnio);
                TblReport.AddCell(cellAnioNom);

                //Espacio en blanco salto
                PdfPCell cellSalto = new PdfPCell(new Phrase(" ", _standardFont));
                cellSalto.BorderWidth = 0;
                cellSalto.BackgroundColor = BaseColor.WHITE;
                cellSalto.Colspan = 16;
                TblReport.AddCell(cellSalto);

                //DocumentoPDF.Add(new Chunk("\n"));
                //Chunk c = new Chunk("White text on red background", _standardFont);
                //Paragraph p = new Paragraph(c);
                //DocumentoPDF.Add(p);
                //DocumentoPDF.Add(new Paragraph());
                //DocumentoPDF.Add(new Paragraph());
                //DocumentoPDF.Add(Chunk.NEWLINE);
                //DocumentoPDF.Add(Chunk.NEWLINE);
                //DocumentoPDF.Add(Chunk.NEWLINE);

                //Crear Celdas para el PDF
                PdfPCell cellFecha = new PdfPCell(new Phrase("FECHA", _standardFontBold));
                //cellFecha.BorderWidth = 0;
                cellFecha.BorderWidthBottom = 0.75f;
                cellFecha.BackgroundColor = BaseColor.LIGHT_GRAY;
                cellFecha.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellLugar = new PdfPCell(new Phrase("LUGAR A VISITAR", _standardFontBold));
                //cellHora.BorderWidth = 0;
                cellLugar.BorderWidthBottom = 0.75f;
                cellLugar.BackgroundColor = BaseColor.LIGHT_GRAY;
                cellLugar.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellHora = new PdfPCell(new Phrase("HORA INICIO", _standardFontBold));
                //cellHora.BorderWidth = 0;
                cellHora.BorderWidthBottom = 0.75f;
                cellHora.BackgroundColor = BaseColor.LIGHT_GRAY;
                cellHora.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellHoraFin = new PdfPCell(new Phrase("HORA FIN", _standardFontBold));
                //cellHora.BorderWidth = 0;
                cellHoraFin.BorderWidthBottom = 0.75f;
                cellHoraFin.BackgroundColor = BaseColor.LIGHT_GRAY;
                cellHoraFin.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellActividad = new PdfPCell(new Phrase("ACTIVIDADES A REALIZAR (ESPECIFICAR)", _standardFontBold));
                //cellActividad.BorderWidth = 0;
                cellActividad.BorderWidthBottom = 0.75f;
                cellActividad.BackgroundColor = BaseColor.LIGHT_GRAY;
                cellActividad.HorizontalAlignment = Element.ALIGN_CENTER;
                cellActividad.Colspan = 4;

                PdfPCell cellObjetivo = new PdfPCell(new Phrase("OBJETIVO (RESULTADO)", _standardFontBold));
                //cellObjetivo.BorderWidth = 0;
                cellObjetivo.BorderWidthBottom = 0.75f;
                cellObjetivo.BackgroundColor = BaseColor.LIGHT_GRAY;
                cellObjetivo.HorizontalAlignment = Element.ALIGN_CENTER;
                cellObjetivo.Colspan = 5;

                PdfPCell cellComentarios = new PdfPCell(new Phrase("COMENTARIOS CZ", _standardFontBold));
                //cellObjetivo.BorderWidth = 0;
                cellComentarios.BorderWidthBottom = 0.75f;
                cellComentarios.BackgroundColor = BaseColor.LIGHT_GRAY;
                cellComentarios.HorizontalAlignment = Element.ALIGN_CENTER;
                cellComentarios.Colspan = 3;

                //añadir las celdas a la tabla
                TblReport.AddCell(cellFecha);
                TblReport.AddCell(cellLugar);
                TblReport.AddCell(cellHora);
                TblReport.AddCell(cellHoraFin);
                TblReport.AddCell(cellActividad);
                TblReport.AddCell(cellObjetivo);
                TblReport.AddCell(cellComentarios);

                //Llenado de la tabla con la informacion
                foreach (var item in lstDetalle)
                {
                    cellFecha = new PdfPCell(new Phrase(item.fecha, _standardFont));
                    cellFecha.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellLugar = new PdfPCell(new Phrase(item.lugar, _standardFont));
                    cellLugar.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellHora = new PdfPCell(new Phrase(item.hora, _standardFont));
                    cellHora.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellHoraFin = new PdfPCell(new Phrase(item.horaFin, _standardFont));
                    cellHoraFin.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellActividad = new PdfPCell(new Phrase(item.actividad, _standardFont));
                    cellActividad.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellActividad.Colspan = 4;
                    cellObjetivo = new PdfPCell(new Phrase(item.incidencias, _standardFont));
                    cellObjetivo.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellObjetivo.Colspan = 5;
                    cellComentarios = new PdfPCell(new Phrase(item.comentarios, _standardFont));
                    cellComentarios.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellComentarios.Colspan = 3;


                    //Añadimos las celdas a la tabla
                    TblReport.AddCell(cellFecha);
                    TblReport.AddCell(cellLugar);
                    TblReport.AddCell(cellHora);
                    TblReport.AddCell(cellHoraFin);
                    TblReport.AddCell(cellActividad);
                    TblReport.AddCell(cellObjetivo);
                    TblReport.AddCell(cellComentarios);
                }

                //Añadimos la tabla al documento PDF
                DocumentoPDF.Add(TblReport);

                DocumentoPDF.Close();
                writer.Close();


                HttpContext.Response.ContentType = "pdf/application";
                HttpContext.Response.AddHeader("content-disposition", "attachment;" +
               "filename=PlaneacionSemanal" + "_" + DateTime.Now + ".pdf");
                HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                HttpContext.Response.Write(DocumentoPDF);
                HttpContext.Response.End();


            } // if lstProcesoOrd.Count()

        }

        public void ExportActividadesRealizadasPDF(int? idUsu, int? idEstatus, string Periodos, string TiposFigura, string NombresFigura, int idPlan)
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

            var resultDetalle = (from cat in db.dDetallePlanSemanal
                                 where cat.PlanSemanalId == idPlan
                                 select cat).OrderBy(x => new { x.FechaActividad, x.HoraActividad }).ToList();

            var lstDetalle = resultDetalle.Select(cat => new
            {
                id = cat.DetallePlanId,
                actividad = cat.cTipoActividades.NombreActividad,
                descripcion = cat.DescripcionActividad,
                fecha = cat.FechaActividad.ToString("dd/MM/yyyy"),
                hora = cat.HoraActividad.ToString("hh':'mm"),
                horaFin = cat.HoraFin.Value.ToString("hh':'mm"),
                checkin = (cat.cTipoActividades.RequiereCheckIn == true ? (cat.mCheckIn.Count() == 0 ? "No realizado" : "Realizado") : "Realizado"),
                incidencias = (cat.mCheckIn.Count() == 0 ? "" : cat.mCheckIn.FirstOrDefault().Incidencias),
                comentarios = cat.ComentariosRechazo,
                dia = cat.FechaActividad.ToString("dddd", new CultureInfo("es-ES")).ToUpper()
            });

            if (lstPlanSemanal.Count() > 0)
            {

                // creacion del objeto documento
                Document DocumentoPDF = new Document(PageSize.A4.Rotate());
                PdfWriter writer = PdfWriter.GetInstance(DocumentoPDF, HttpContext.Response.OutputStream);

                ITextEvents evPdf = new ITextEvents();
                evPdf.Title = "INSTITUTO DE ALFABETIZACIÓN Y EDUCACIÓN BÁSICA PARA ADULTOS";
                evPdf.TipoReporte = "ACTIVIDADES REALIZADAS";
                evPdf.Coordinacion = "COORDINACIÓN DE ZONA: " + lstPlanSemanal.FirstOrDefault().cz;
                evPdf.Count = lstDetalle.Count();
                writer.PageEvent = evPdf;

                // Le colocamos el título y el autor
                // **Nota: Esto no será visible en el documento
                DocumentoPDF.AddTitle("Actividades realizadas");
                DocumentoPDF.AddCreator("Inaeba");


                // Abrimos el archivo
                DocumentoPDF.Open();


                // Creamos el tipo de Font que vamos utilizar
                iTextSharp.text.Font _standardFont = new Font(Font.FontFamily.HELVETICA, 8, Font.NORMAL, BaseColor.BLACK);
                iTextSharp.text.Font _standardFontBold = new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.BLACK);


                //Crear tabla pdf
                PdfPTable TblReport = new PdfPTable(9);
                TblReport.WidthPercentage = 100;

                //Crear Celdas encabezado
                PdfPCell cellCZ = new PdfPCell(new Phrase("CZ", _standardFont));
                cellCZ.BorderWidth = 0;
                cellCZ.BackgroundColor = BaseColor.WHITE;
                cellCZ.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellCZNom = new PdfPCell(new Phrase(lstPlanSemanal.FirstOrDefault().cz, _standardFontBold));
                cellCZNom.BorderWidth = 1;
                cellCZNom.BackgroundColor = new BaseColor(156, 195, 230);
                cellCZNom.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell cellPersonaOp = new PdfPCell(new Phrase("PERSONAL OPERATIVO", _standardFont));
                cellPersonaOp.BorderWidth = 0;
                cellPersonaOp.BackgroundColor = BaseColor.WHITE;
                cellPersonaOp.HorizontalAlignment = Element.ALIGN_RIGHT;
                cellPersonaOp.Colspan = 2;

                PdfPCell cellPersonal = new PdfPCell(new Phrase(lstPlanSemanal.FirstOrDefault().usuario, _standardFontBold));
                cellPersonal.BorderWidth = 1;
                cellPersonal.BackgroundColor = new BaseColor(156, 195, 230);
                cellPersonal.HorizontalAlignment = Element.ALIGN_LEFT;
                cellPersonal.Colspan = 5;

                TblReport.AddCell(cellCZ);
                TblReport.AddCell(cellCZNom);
                TblReport.AddCell(cellPersonaOp);
                TblReport.AddCell(cellPersonal);

                //Espacio en blanco salto
                PdfPCell cellSalto = new PdfPCell(new Phrase(" ", _standardFontBold));
                cellSalto.BorderWidth = 0;
                cellSalto.BackgroundColor = BaseColor.WHITE;
                cellSalto.Colspan = 9;
                TblReport.AddCell(cellSalto);

                PdfPCell cellPeriodo = new PdfPCell(new Phrase("PERIODO", _standardFont));
                cellPeriodo.BorderWidth = 0;
                cellPeriodo.BackgroundColor = BaseColor.WHITE;
                cellPeriodo.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellPeriodoNom = new PdfPCell(new Phrase(lstPlanSemanal.FirstOrDefault().periodo, _standardFontBold));
                cellPeriodoNom.BorderWidth = 1;
                cellPeriodoNom.BackgroundColor = new BaseColor(156, 195, 230);
                cellPeriodoNom.HorizontalAlignment = Element.ALIGN_CENTER;
                cellPeriodoNom.Colspan = 3;

                PdfPCell cellMes = new PdfPCell(new Phrase("MES", _standardFont));
                cellMes.BorderWidth = 0;
                cellMes.BackgroundColor = BaseColor.WHITE;
                cellMes.HorizontalAlignment = Element.ALIGN_RIGHT;

                PdfPCell cellMesNom = new PdfPCell(new Phrase(lstPlanSemanal.FirstOrDefault().mes, _standardFontBold));
                cellMesNom.BorderWidth = 1;
                cellMesNom.BackgroundColor = new BaseColor(156, 195, 230);
                cellMesNom.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellAnio = new PdfPCell(new Phrase("AÑO", _standardFont));
                cellAnio.BorderWidth = 0;
                cellAnio.BackgroundColor = BaseColor.WHITE;
                cellAnio.HorizontalAlignment = Element.ALIGN_RIGHT;

                PdfPCell cellAnioNom = new PdfPCell(new Phrase(lstPlanSemanal.FirstOrDefault().anio.ToString(), _standardFontBold));
                cellAnioNom.BorderWidth = 1;
                cellAnioNom.BackgroundColor = new BaseColor(156, 195, 230);
                cellAnioNom.HorizontalAlignment = Element.ALIGN_CENTER;
                cellAnioNom.Colspan = 2;

                //añadir las celdas de tabla encabezado
                TblReport.AddCell(cellPeriodo);
                TblReport.AddCell(cellPeriodoNom);
                TblReport.AddCell(cellMes);
                TblReport.AddCell(cellMesNom);
                TblReport.AddCell(cellAnio);
                TblReport.AddCell(cellAnioNom);

                //Espacio en blanco salto
                //PdfPCell cellSalto1 = new PdfPCell(new Phrase(" ", _standardFont));
                //cellSalto1.BorderWidth = 0;
                //cellSalto1.BackgroundColor = BaseColor.WHITE;
                //cellSalto1.Colspan = 8;
                TblReport.AddCell(cellSalto);

                //Crear Celdas para el PDF
                PdfPCell cellDia = new PdfPCell(new Phrase("DIA", _standardFontBold));
                //cellFecha.BorderWidth = 0;
                cellDia.BackgroundColor = new BaseColor(156, 195, 230);
                cellDia.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellFecha = new PdfPCell(new Phrase("FECHA", _standardFontBold));
                //cellHora.BorderWidth = 0;
                cellFecha.BorderWidthBottom = 0.75f;
                cellFecha.BackgroundColor = new BaseColor(156, 195, 230);
                cellFecha.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellHora = new PdfPCell(new Phrase("HORA INICIO", _standardFontBold));
                //cellActividad.BorderWidth = 0;
                cellHora.BackgroundColor = new BaseColor(156, 195, 230);
                cellHora.HorizontalAlignment = Element.ALIGN_CENTER;
                
                PdfPCell cellHoraFin = new PdfPCell(new Phrase("HORA FIN", _standardFontBold));
                //cellActividad.BorderWidth = 0;
                cellHoraFin.BackgroundColor = new BaseColor(156, 195, 230);
                cellHoraFin.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellActividad = new PdfPCell(new Phrase("ACTIVIDAD", _standardFontBold));
                //cellObjetivo.BorderWidth = 0;
                cellActividad.BorderWidthBottom = 0.75f;
                cellActividad.BackgroundColor = new BaseColor(156, 195, 230);
                cellActividad.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellDescActividad = new PdfPCell(new Phrase("DESCRIPCIÓN ACTIVIDAD", _standardFontBold));
                //cellObjetivo.BorderWidth = 0;
                cellDescActividad.BackgroundColor = new BaseColor(156, 195, 230);
                cellDescActividad.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellIncidencia = new PdfPCell(new Phrase("OBSERVACIONES", _standardFontBold));
                //cellObjetivo.BorderWidth = 0;
                cellIncidencia.BackgroundColor = new BaseColor(156, 195, 230);
                cellIncidencia.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellEstatus = new PdfPCell(new Phrase("ESTATUS ACTIVIDAD", _standardFontBold));
                //cellObjetivo.BorderWidth = 0;
                cellEstatus.BackgroundColor = new BaseColor(156, 195, 230);
                cellEstatus.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellComentarios = new PdfPCell(new Phrase("COMENTARIOS CZ", _standardFontBold));
                //cellObjetivo.BorderWidth = 0;
                cellComentarios.BackgroundColor = new BaseColor(156, 195, 230);
                cellComentarios.HorizontalAlignment = Element.ALIGN_CENTER;

                //añadir las celdas a la tabla
                TblReport.AddCell(cellDia);
                TblReport.AddCell(cellFecha);
                TblReport.AddCell(cellHora);
                TblReport.AddCell(cellHoraFin);
                TblReport.AddCell(cellActividad);
                TblReport.AddCell(cellDescActividad);
                TblReport.AddCell(cellIncidencia);
                TblReport.AddCell(cellEstatus);
                TblReport.AddCell(cellComentarios);

                //Llenado de la tabla con la informacion
                foreach (var item in lstDetalle)
                {
                    cellDia = new PdfPCell(new Phrase(item.dia, _standardFont));
                    cellDia.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellFecha = new PdfPCell(new Phrase(item.fecha, _standardFont));
                    cellFecha.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellHora = new PdfPCell(new Phrase(item.hora, _standardFont));
                    cellHora.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellHoraFin = new PdfPCell(new Phrase(item.horaFin, _standardFont));
                    cellHoraFin.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellActividad = new PdfPCell(new Phrase(item.actividad, _standardFont));
                    cellActividad.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellDescActividad = new PdfPCell(new Phrase(item.descripcion, _standardFont));
                    cellDescActividad.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellIncidencia = new PdfPCell(new Phrase(item.incidencias, _standardFont));
                    cellIncidencia.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellEstatus = new PdfPCell(new Phrase(item.checkin, _standardFont));
                    cellEstatus.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellComentarios= new PdfPCell(new Phrase(item.comentarios, _standardFont));
                    cellComentarios.HorizontalAlignment = Element.ALIGN_CENTER;
                    
                    //Añadimos las celdas a la tabla
                    TblReport.AddCell(cellDia);
                    TblReport.AddCell(cellFecha);
                    TblReport.AddCell(cellHora);
                    TblReport.AddCell(cellHoraFin);
                    TblReport.AddCell(cellActividad);
                    TblReport.AddCell(cellDescActividad);
                    TblReport.AddCell(cellIncidencia);
                    TblReport.AddCell(cellEstatus);
                    TblReport.AddCell(cellComentarios);
                }

                //Añadimos la tabla al documento PDF
                DocumentoPDF.Add(TblReport);

                DocumentoPDF.Close();
                writer.Close();


                HttpContext.Response.ContentType = "pdf/application";
                HttpContext.Response.AddHeader("content-disposition", "attachment;" +
               "filename=ActividadesRealizadas" + "_" + lstPlanSemanal.FirstOrDefault().usuario + ".pdf");
                HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                HttpContext.Response.Write(DocumentoPDF);
                HttpContext.Response.End();


            } // if lstProcesoOrd.Count()

        }

        public void ExportIndicadorCertificadosPDF(int idUsuario, string idPeriodo, string Region, string Zonas)
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

            List<udf_CertificadosInaebaZonaFiguraList_Result> resultZonaFigura = (from u in db.udf_CertificadosInaebaZonaFiguraList(idPeriodo, Region, Zonas, "") select u).Where(x => x.Periodo != null).ToList();

            var lstPlanSemanalZonaFigura = resultZonaFigura.Select(C => new
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

            List<udf_CertificadosInaebaFiguraEducandoList_Result> resultEducando = (from u in db.udf_CertificadosInaebaFiguraEducandoList(idPeriodo, Region, Zonas, "") select u).Where(x => x.Periodo != null).ToList();

            var lstPlanSemanalEducando = resultEducando.Select(C => new
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

                // creacion del objeto documento
                Document DocumentoPDF = new Document(PageSize.A4.Rotate());
                PdfWriter writer = PdfWriter.GetInstance(DocumentoPDF, HttpContext.Response.OutputStream);

                ITextEvents evPdf = new ITextEvents();
                evPdf.Title = "INSTITUTO DE ALFABETIZACIÓN Y EDUCACIÓN BÁSICA PARA ADULTOS";
                evPdf.TipoReporte = "INDICADOR CERTIFICADOS ENTREGADOS VS PROGRAMADOS";
                evPdf.Coordinacion = "";
                evPdf.Count = lstPlanSemanal.Count();
                writer.PageEvent = evPdf;

                // Le colocamos el título y el autor
                // **Nota: Esto no será visible en el documento
                DocumentoPDF.AddTitle("INDICADOR CERTIFICADOS ENTREGADOS VS PROGRAMADOS");
                DocumentoPDF.AddCreator("Inaeba");


                // Abrimos el archivo
                DocumentoPDF.Open();


                // Creamos el tipo de Font que vamos utilizar
                iTextSharp.text.Font _standardFont = new Font(Font.FontFamily.HELVETICA, 8, Font.NORMAL, BaseColor.BLACK);
                iTextSharp.text.Font _standardFontBold = new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.BLACK);


                //Crear tabla pdf
                PdfPTable TblReport = new PdfPTable(7);
                TblReport.WidthPercentage = 100;

                PdfPTable TblReportZona = new PdfPTable(8);
                TblReportZona.WidthPercentage = 100;

                PdfPTable TblReportZonaFigura = new PdfPTable(8);
                TblReportZonaFigura.WidthPercentage = 100;

                PdfPTable TblReportEducando= new PdfPTable(13);
                TblReportEducando.WidthPercentage = 100;

                //Titulo
                PdfPCell cellTitulo = new PdfPCell(new Phrase("REGIÓN", _standardFontBold));
                cellTitulo.BackgroundColor = BaseColor.WHITE;
                cellTitulo.Colspan = 7;
                cellTitulo.HorizontalAlignment = Element.ALIGN_CENTER;
                TblReport.AddCell(cellTitulo);

                //Crear Celdas para el PDF
                PdfPCell cellRegion = new PdfPCell(new Phrase("Región", _standardFontBold));
                cellRegion.BackgroundColor = new BaseColor(156, 195, 230);
                cellRegion.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellCertProg = new PdfPCell(new Phrase("Certificados programados", _standardFontBold));
                cellCertProg.BackgroundColor = new BaseColor(156, 195, 230);
                cellCertProg.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellCertEnt = new PdfPCell(new Phrase("Certificados entregados", _standardFontBold));
                cellCertEnt.BackgroundColor = new BaseColor(156, 195, 230);
                cellCertEnt.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellCumplimiento = new PdfPCell(new Phrase("%Cumplimiento", _standardFontBold));
                cellCumplimiento.BackgroundColor = new BaseColor(156, 195, 230);
                cellCumplimiento.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellPromotor = new PdfPCell(new Phrase("%Promotor", _standardFontBold));
                cellPromotor.BackgroundColor = new BaseColor(156, 195, 230);
                cellPromotor.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellTecnico = new PdfPCell(new Phrase("%Técnicos docentes", _standardFontBold));
                cellTecnico.BackgroundColor = new BaseColor(156, 195, 230);
                cellTecnico.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellFormador = new PdfPCell(new Phrase("%Formadores", _standardFontBold));
                cellFormador.BackgroundColor = new BaseColor(156, 195, 230);
                cellFormador.HorizontalAlignment = Element.ALIGN_CENTER;

                //añadir las celdas a la tabla
                TblReport.AddCell(cellRegion);
                TblReport.AddCell(cellCertProg);
                TblReport.AddCell(cellCertEnt);
                TblReport.AddCell(cellCumplimiento);
                TblReport.AddCell(cellPromotor);
                TblReport.AddCell(cellTecnico);
                TblReport.AddCell(cellFormador);


                //Llenado de la tabla con la informacion
                foreach (var item in lstPlanSemanal)
                {
                    string certProg = (item.certificadosProgramadosTotal == null ? 0 : item.certificadosProgramadosTotal.Value).ToString();
                    string certEntre = (item.certificadosEntregadosTotal == null ? 0 : item.certificadosEntregadosTotal.Value).ToString();
                    cellRegion = new PdfPCell(new Phrase(item.Region, _standardFont));
                    cellRegion.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellCertProg = new PdfPCell(new Phrase(certProg, _standardFont));
                    cellCertProg.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellCertEnt = new PdfPCell(new Phrase(certEntre, _standardFont));
                    cellCertEnt.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellCumplimiento = new PdfPCell(new Phrase(item.porcentajeCumplimientoT, _standardFont));
                    cellCumplimiento.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellPromotor = new PdfPCell(new Phrase(item.pocentajePromotor, _standardFont));
                    cellPromotor.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellTecnico = new PdfPCell(new Phrase(item.pocentajeTecnico, _standardFont));
                    cellTecnico.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellFormador = new PdfPCell(new Phrase(item.pocentajeFormador, _standardFont));
                    cellFormador.HorizontalAlignment = Element.ALIGN_CENTER;

                    //Añadimos las celdas a la tabla
                    TblReport.AddCell(cellRegion);
                    TblReport.AddCell(cellCertProg);
                    TblReport.AddCell(cellCertEnt);
                    TblReport.AddCell(cellCumplimiento);
                    TblReport.AddCell(cellPromotor);
                    TblReport.AddCell(cellTecnico);
                    TblReport.AddCell(cellFormador);
                }

                //Añadimos la tabla al documento PDF
                DocumentoPDF.Add(TblReport);

                //Espacio en blanco salto
                PdfPCell cellSalto = new PdfPCell(new Phrase(" ", _standardFontBold));
                cellSalto.BorderWidth = 0;
                cellSalto.BackgroundColor = BaseColor.WHITE;
                cellSalto.Colspan = 8;
                TblReportZona.AddCell(cellSalto);

                //Titulo
                PdfPCell cellTituloZona = new PdfPCell(new Phrase("ZONA", _standardFontBold));
                cellTituloZona.BackgroundColor = BaseColor.WHITE;
                cellTituloZona.Colspan = 8;
                cellTituloZona.HorizontalAlignment = Element.ALIGN_CENTER;
                TblReportZona.AddCell(cellTituloZona);

                //Crear Celdas para el PDF ZONA
                PdfPCell cellRegionZona = new PdfPCell(new Phrase("Región", _standardFontBold));
                cellRegionZona.BackgroundColor = new BaseColor(156, 195, 230);
                cellRegionZona.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellZona = new PdfPCell(new Phrase("Zona", _standardFontBold));
                cellZona.BackgroundColor = new BaseColor(156, 195, 230);
                cellZona.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellCertProgZona = new PdfPCell(new Phrase("Certificados programados", _standardFontBold));
                cellCertProgZona.BackgroundColor = new BaseColor(156, 195, 230);
                cellCertProgZona.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellCertEntZona = new PdfPCell(new Phrase("Certificados entregados", _standardFontBold));
                cellCertEntZona.BackgroundColor = new BaseColor(156, 195, 230);
                cellCertEntZona.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellCumplimientoZona = new PdfPCell(new Phrase("%Cumplimiento", _standardFontBold));
                cellCumplimientoZona.BackgroundColor = new BaseColor(156, 195, 230);
                cellCumplimientoZona.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellPromotorZona = new PdfPCell(new Phrase("%Promotor", _standardFontBold));
                cellPromotorZona.BackgroundColor = new BaseColor(156, 195, 230);
                cellPromotorZona.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellTecnicoZona = new PdfPCell(new Phrase("%Técnicos docentes", _standardFontBold));
                cellTecnicoZona.BackgroundColor = new BaseColor(156, 195, 230);
                cellTecnicoZona.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellFormadorZona = new PdfPCell(new Phrase("%Formadores", _standardFontBold));
                cellFormadorZona.BackgroundColor = new BaseColor(156, 195, 230);
                cellFormadorZona.HorizontalAlignment = Element.ALIGN_CENTER;

                //añadir las celdas a la tabla
                TblReportZona.AddCell(cellRegionZona);
                TblReportZona.AddCell(cellZona);
                TblReportZona.AddCell(cellCertProgZona);
                TblReportZona.AddCell(cellCertEntZona);
                TblReportZona.AddCell(cellCumplimientoZona);
                TblReportZona.AddCell(cellPromotorZona);
                TblReportZona.AddCell(cellTecnicoZona);
                TblReportZona.AddCell(cellFormadorZona);


                //Llenado de la tabla con la informacion
                foreach (var item in lstPlanSemanalZona)
                {
                    string certProg = (item.certificadosProgramadosTotal == null ? 0 : item.certificadosProgramadosTotal.Value).ToString();
                    string certEntre = (item.certificadosEntregadosTotal == null ? 0 : item.certificadosEntregadosTotal.Value).ToString();
                    cellRegionZona = new PdfPCell(new Phrase(item.Region, _standardFont));
                    cellRegionZona.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellZona = new PdfPCell(new Phrase(item.Zona, _standardFont));
                    cellZona.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellCertProgZona = new PdfPCell(new Phrase(certProg, _standardFont));
                    cellCertProgZona.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellCertEntZona = new PdfPCell(new Phrase(certEntre, _standardFont));
                    cellCertEntZona.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellCumplimientoZona = new PdfPCell(new Phrase(item.porcentajeCumplimientoT, _standardFont));
                    cellCumplimientoZona.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellPromotorZona = new PdfPCell(new Phrase(item.pocentajePromotor, _standardFont));
                    cellPromotorZona.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellTecnicoZona = new PdfPCell(new Phrase(item.pocentajeTecnico, _standardFont));
                    cellTecnicoZona.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellFormadorZona = new PdfPCell(new Phrase(item.pocentajeFormador, _standardFont));
                    cellFormadorZona.HorizontalAlignment = Element.ALIGN_CENTER;

                    //Añadimos las celdas a la tabla
                    TblReportZona.AddCell(cellRegionZona);
                    TblReportZona.AddCell(cellZona);
                    TblReportZona.AddCell(cellCertProgZona);
                    TblReportZona.AddCell(cellCertEntZona);
                    TblReportZona.AddCell(cellCumplimientoZona);
                    TblReportZona.AddCell(cellPromotorZona);
                    TblReportZona.AddCell(cellTecnicoZona);
                    TblReportZona.AddCell(cellFormadorZona);
                }

                //Añadimos la tabla al documento PDF
                DocumentoPDF.Add(TblReportZona);

                //Espacio en blanco salto
                PdfPCell cellSaltoZFigura = new PdfPCell(new Phrase(" ", _standardFontBold));
                cellSaltoZFigura.BorderWidth = 0;
                cellSaltoZFigura.BackgroundColor = BaseColor.WHITE;
                cellSaltoZFigura.Colspan = 8;
                TblReportZonaFigura.AddCell(cellSaltoZFigura);

                //Titulo
                PdfPCell cellTituloZonaFig = new PdfPCell(new Phrase("Usuario (Detalle programado vs entregado)", _standardFontBold));
                cellTituloZonaFig.BackgroundColor = BaseColor.WHITE;
                cellTituloZonaFig.Colspan = 8;
                cellTituloZonaFig.HorizontalAlignment = Element.ALIGN_CENTER;
                TblReportZonaFigura.AddCell(cellTituloZonaFig);

                //Crear Celdas para el PDF ZONA
                PdfPCell cellPeriodoZonaFig = new PdfPCell(new Phrase("Periodo", _standardFontBold));
                cellPeriodoZonaFig.BackgroundColor = new BaseColor(156, 195, 230);
                cellPeriodoZonaFig.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellRegionZonaFig = new PdfPCell(new Phrase("Región", _standardFontBold));
                cellRegionZonaFig.BackgroundColor = new BaseColor(156, 195, 230);
                cellRegionZonaFig.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellZonaFigura = new PdfPCell(new Phrase("Zona", _standardFontBold));
                cellZonaFigura.BackgroundColor = new BaseColor(156, 195, 230);
                cellZonaFigura.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellTipoFiZonaFig = new PdfPCell(new Phrase("Tipo Figura", _standardFontBold));
                cellTipoFiZonaFig.BackgroundColor = new BaseColor(156, 195, 230);
                cellTipoFiZonaFig.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellNomFiZonaFig = new PdfPCell(new Phrase("Nombre Figura", _standardFontBold));
                cellNomFiZonaFig.BackgroundColor = new BaseColor(156, 195, 230);
                cellNomFiZonaFig.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellCertProgZonaFig = new PdfPCell(new Phrase("Certificados programados", _standardFontBold));
                cellCertProgZonaFig.BackgroundColor = new BaseColor(156, 195, 230);
                cellCertProgZonaFig.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellCertEntZonaFig = new PdfPCell(new Phrase("Certificados entregados", _standardFontBold));
                cellCertEntZonaFig.BackgroundColor = new BaseColor(156, 195, 230);
                cellCertEntZonaFig.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellCumplimientoZonaFig = new PdfPCell(new Phrase("%Cumplimiento", _standardFontBold));
                cellCumplimientoZonaFig.BackgroundColor = new BaseColor(156, 195, 230);
                cellCumplimientoZonaFig.HorizontalAlignment = Element.ALIGN_CENTER;

                //añadir las celdas a la tabla
                TblReportZonaFigura.AddCell(cellPeriodoZonaFig);
                TblReportZonaFigura.AddCell(cellRegionZonaFig);
                TblReportZonaFigura.AddCell(cellZonaFigura);
                TblReportZonaFigura.AddCell(cellTipoFiZonaFig);
                TblReportZonaFigura.AddCell(cellNomFiZonaFig);
                TblReportZonaFigura.AddCell(cellCertProgZonaFig);
                TblReportZonaFigura.AddCell(cellCertEntZonaFig);
                TblReportZonaFigura.AddCell(cellCumplimientoZonaFig);


                //Llenado de la tabla con la informacion
                foreach (var item in lstPlanSemanalZonaFigura)
                {
                    string certProg = (item.certificadosProgramadosTotal == null ? 0 : item.certificadosProgramadosTotal.Value).ToString();
                    string certEntre = (item.certificadosEntregadosTotal == null ? 0 : item.certificadosEntregadosTotal.Value).ToString();
                    cellPeriodoZonaFig = new PdfPCell(new Phrase(item.Periodo, _standardFont));
                    cellPeriodoZonaFig.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellRegionZonaFig = new PdfPCell(new Phrase(item.Region, _standardFont));
                    cellRegionZonaFig.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellZonaFigura = new PdfPCell(new Phrase(item.Zona, _standardFont));
                    cellZonaFigura.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellTipoFiZonaFig = new PdfPCell(new Phrase(item.DescripcionTipoFigura, _standardFont));
                    cellTipoFiZonaFig.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellNomFiZonaFig = new PdfPCell(new Phrase(item.NombreUsuario, _standardFont));
                    cellNomFiZonaFig.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellCertProgZonaFig = new PdfPCell(new Phrase(certProg, _standardFont));
                    cellCertProgZonaFig.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellCertEntZonaFig = new PdfPCell(new Phrase(certEntre, _standardFont));
                    cellCertEntZonaFig.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellCumplimientoZonaFig = new PdfPCell(new Phrase(item.porcentajeCumplimientoT, _standardFont));
                    cellCumplimientoZonaFig.HorizontalAlignment = Element.ALIGN_CENTER;

                    //Añadimos las celdas a la tabla
                    TblReportZonaFigura.AddCell(cellPeriodoZonaFig);
                    TblReportZonaFigura.AddCell(cellRegionZonaFig);
                    TblReportZonaFigura.AddCell(cellZonaFigura);
                    TblReportZonaFigura.AddCell(cellTipoFiZonaFig);
                    TblReportZonaFigura.AddCell(cellNomFiZonaFig);
                    TblReportZonaFigura.AddCell(cellCertProgZonaFig);
                    TblReportZonaFigura.AddCell(cellCertEntZonaFig);
                    TblReportZonaFigura.AddCell(cellCumplimientoZonaFig);
                }

                //Añadimos la tabla al documento PDF
                DocumentoPDF.Add(TblReportZonaFigura);

                //Espacio en blanco salto
                PdfPCell cellSaltoEducando = new PdfPCell(new Phrase(" ", _standardFontBold));
                cellSaltoEducando.BorderWidth = 0;
                cellSaltoEducando.BackgroundColor = BaseColor.WHITE;
                cellSaltoEducando.Colspan = 13;
                TblReportEducando.AddCell(cellSaltoEducando);

                //Titulo
                PdfPCell cellTituloEducando = new PdfPCell(new Phrase("Consulta por educando de certificados", _standardFontBold));
                cellTituloEducando.BackgroundColor = BaseColor.WHITE;
                cellTituloEducando.Colspan = 13;
                cellTituloEducando.HorizontalAlignment = Element.ALIGN_CENTER;
                TblReportEducando.AddCell(cellTituloEducando);

                //Crear Celdas para el PDF ZONA
                PdfPCell cellPeriodoEdu = new PdfPCell(new Phrase("Periodo", _standardFontBold));
                cellPeriodoEdu.BackgroundColor = new BaseColor(156, 195, 230);
                cellPeriodoEdu.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellRegionEdu = new PdfPCell(new Phrase("Región", _standardFontBold));
                cellRegionEdu.BackgroundColor = new BaseColor(156, 195, 230);
                cellRegionEdu.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellZonaEdu = new PdfPCell(new Phrase("Zona", _standardFontBold));
                cellZonaEdu.BackgroundColor = new BaseColor(156, 195, 230);
                cellZonaEdu.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellTipoFigEdu= new PdfPCell(new Phrase("Tipo Figura", _standardFontBold));
                cellTipoFigEdu.BackgroundColor = new BaseColor(156, 195, 230);
                cellTipoFigEdu.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellNomFigEdu = new PdfPCell(new Phrase("Nombre Figura", _standardFontBold));
                cellNomFigEdu.BackgroundColor = new BaseColor(156, 195, 230);
                cellNomFigEdu.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellNomEdu = new PdfPCell(new Phrase("Nombre del educando", _standardFontBold));
                cellNomEdu.BackgroundColor = new BaseColor(156, 195, 230);
                cellNomEdu.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellCurpEdu = new PdfPCell(new Phrase("CURP del educando", _standardFontBold));
                cellCurpEdu.BackgroundColor = new BaseColor(156, 195, 230);
                cellCurpEdu.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellFolioCert = new PdfPCell(new Phrase("Folio del certificado", _standardFontBold));
                cellFolioCert.BackgroundColor = new BaseColor(156, 195, 230);
                cellFolioCert.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellAcuseCert = new PdfPCell(new Phrase("Acuse de recibo", _standardFontBold));
                cellAcuseCert.BackgroundColor = new BaseColor(156, 195, 230);
                cellAcuseCert.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellEstCert = new PdfPCell(new Phrase("Estatus del certificado", _standardFontBold));
                cellEstCert.BackgroundColor = new BaseColor(156, 195, 230);
                cellEstCert.HorizontalAlignment = Element.ALIGN_CENTER;
                
                PdfPCell cellFechaCert = new PdfPCell(new Phrase("Fecha emisión", _standardFontBold));
                cellFechaCert.BackgroundColor = new BaseColor(156, 195, 230);
                cellFechaCert.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellFechaProCert = new PdfPCell(new Phrase("Fecha programada", _standardFontBold));
                cellFechaProCert.BackgroundColor = new BaseColor(156, 195, 230);
                cellFechaProCert.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellFechaEntCert = new PdfPCell(new Phrase("Fecha entregado", _standardFontBold));
                cellFechaEntCert.BackgroundColor = new BaseColor(156, 195, 230);
                cellFechaEntCert.HorizontalAlignment = Element.ALIGN_CENTER;

                //añadir las celdas a la tabla
                TblReportEducando.AddCell(cellPeriodoEdu);
                TblReportEducando.AddCell(cellRegionEdu);
                TblReportEducando.AddCell(cellZonaEdu);
                TblReportEducando.AddCell(cellTipoFigEdu);
                TblReportEducando.AddCell(cellNomFigEdu);
                TblReportEducando.AddCell(cellNomEdu);
                TblReportEducando.AddCell(cellCurpEdu);
                TblReportEducando.AddCell(cellFolioCert);
                TblReportEducando.AddCell(cellAcuseCert);
                TblReportEducando.AddCell(cellEstCert);
                TblReportEducando.AddCell(cellFechaCert);
                TblReportEducando.AddCell(cellFechaProCert);
                TblReportEducando.AddCell(cellFechaEntCert);

                //Llenado de la tabla con la informacion
                foreach (var item in lstPlanSemanalEducando)
                {
                    cellPeriodoEdu = new PdfPCell(new Phrase(item.Periodo, _standardFont));
                    cellPeriodoEdu.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellRegionEdu = new PdfPCell(new Phrase(item.Region, _standardFont));
                    cellRegionEdu.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellZonaEdu = new PdfPCell(new Phrase(item.Zona, _standardFont));
                    cellZonaEdu.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellTipoFigEdu = new PdfPCell(new Phrase(item.DescripcionTipoFigura, _standardFont));
                    cellTipoFigEdu.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellNomFigEdu = new PdfPCell(new Phrase(item.NombreUsuario, _standardFont));
                    cellNomFigEdu.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellNomEdu = new PdfPCell(new Phrase(item.Educando, _standardFont));
                    cellNomEdu.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellCurpEdu = new PdfPCell(new Phrase(item.Curp, _standardFont));
                    cellCurpEdu.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellFolioCert = new PdfPCell(new Phrase(item.FolioCertificado, _standardFont));
                    cellFolioCert.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellAcuseCert = new PdfPCell(new Phrase(item.Acuse, _standardFont));
                    cellAcuseCert.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellEstCert = new PdfPCell(new Phrase(item.EstatusCert, _standardFont));
                    cellEstCert.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellFechaCert = new PdfPCell(new Phrase(item.FechaEmision, _standardFont));
                    cellFechaCert.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellFechaProCert = new PdfPCell(new Phrase(item.FechaPlan, _standardFont));
                    cellFechaProCert.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellFechaEntCert = new PdfPCell(new Phrase(item.FechaCheck, _standardFont));
                    cellFechaEntCert.HorizontalAlignment = Element.ALIGN_CENTER;

                    //Añadimos las celdas a la tabla
                    TblReportEducando.AddCell(cellPeriodoEdu);
                    TblReportEducando.AddCell(cellRegionEdu);
                    TblReportEducando.AddCell(cellZonaEdu);
                    TblReportEducando.AddCell(cellTipoFigEdu);
                    TblReportEducando.AddCell(cellNomFigEdu);
                    TblReportEducando.AddCell(cellNomEdu);
                    TblReportEducando.AddCell(cellCurpEdu);
                    TblReportEducando.AddCell(cellFolioCert);
                    TblReportEducando.AddCell(cellAcuseCert);
                    TblReportEducando.AddCell(cellEstCert);
                    TblReportEducando.AddCell(cellFechaCert);
                    TblReportEducando.AddCell(cellFechaProCert);
                    TblReportEducando.AddCell(cellFechaEntCert);
                }

                //Añadimos la tabla al documento PDF
                DocumentoPDF.Add(TblReportEducando);

                DocumentoPDF.Close();
                writer.Close();


                HttpContext.Response.ContentType = "pdf/application";
                HttpContext.Response.AddHeader("content-disposition", "attachment;" +
               "filename=IndicadorCertificados" + "_" + DateTime.Now.Date + ".pdf");
                HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                HttpContext.Response.Write(DocumentoPDF);
                HttpContext.Response.End();


            } // if lstProcesoOrd.Count()

        }

        public void ExportIndicadorPreregistrosPDF(int idUsuario, string idPeriodo, string Region, string Zonas)
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

                // creacion del objeto documento
                Document DocumentoPDF = new Document(PageSize.A4.Rotate());
                PdfWriter writer = PdfWriter.GetInstance(DocumentoPDF, HttpContext.Response.OutputStream);

                ITextEvents evPdf = new ITextEvents();
                evPdf.Title = "INSTITUTO DE ALFABETIZACIÓN Y EDUCACIÓN BÁSICA PARA ADULTOS";
                evPdf.TipoReporte = "INDICADOR EDUCANDOS PRE-REGISTRADOS VS REGISTRADOS";
                evPdf.Coordinacion = "";
                evPdf.Count = lstPlanSemanal.Count();
                writer.PageEvent = evPdf;

                // Le colocamos el título y el autor
                // **Nota: Esto no será visible en el documento
                DocumentoPDF.AddTitle("INDICADOR EDUCANDOS PRE-REGISTRADOS VS REGISTRADOS");
                DocumentoPDF.AddCreator("Inaeba");


                // Abrimos el archivo
                DocumentoPDF.Open();


                // Creamos el tipo de Font que vamos utilizar
                iTextSharp.text.Font _standardFont = new Font(Font.FontFamily.HELVETICA, 8, Font.NORMAL, BaseColor.BLACK);
                iTextSharp.text.Font _standardFontBold = new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.BLACK);


                //Crear tabla pdf
                PdfPTable TblReport = new PdfPTable(7);
                TblReport.WidthPercentage = 100;

                PdfPTable TblReportZona = new PdfPTable(8);
                TblReportZona.WidthPercentage = 100;

                PdfPTable TblReportZonaFigura = new PdfPTable(8);
                TblReportZonaFigura.WidthPercentage = 100;

                PdfPTable TblReportEducando = new PdfPTable(8);
                TblReportEducando.WidthPercentage = 100;

                //Titulo
                PdfPCell cellTitulo = new PdfPCell(new Phrase("REGIÓN", _standardFontBold));
                cellTitulo.BackgroundColor = BaseColor.WHITE;
                cellTitulo.Colspan = 7;
                cellTitulo.HorizontalAlignment = Element.ALIGN_CENTER;
                TblReport.AddCell(cellTitulo);

                //Crear Celdas para el PDF
                PdfPCell cellRegion = new PdfPCell(new Phrase("Región", _standardFontBold));
                cellRegion.BackgroundColor = new BaseColor(156, 195, 230);
                cellRegion.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellEduPre = new PdfPCell(new Phrase("Educandos pre-registrados", _standardFontBold));
                cellEduPre.BackgroundColor = new BaseColor(156, 195, 230);
                cellEduPre.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellEduInc = new PdfPCell(new Phrase("Educandos registrados", _standardFontBold));
                cellEduInc.BackgroundColor = new BaseColor(156, 195, 230);
                cellEduInc.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellPorcentajeEduInc = new PdfPCell(new Phrase("%Registrados", _standardFontBold));
                cellPorcentajeEduInc.BackgroundColor = new BaseColor(156, 195, 230);
                cellPorcentajeEduInc.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellPromotor = new PdfPCell(new Phrase("%Promotor", _standardFontBold));
                cellPromotor.BackgroundColor = new BaseColor(156, 195, 230);
                cellPromotor.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellTecnico = new PdfPCell(new Phrase("%Técnicos docentes", _standardFontBold));
                cellTecnico.BackgroundColor = new BaseColor(156, 195, 230);
                cellTecnico.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellFormador = new PdfPCell(new Phrase("%Formadores", _standardFontBold));
                cellFormador.BackgroundColor = new BaseColor(156, 195, 230);
                cellFormador.HorizontalAlignment = Element.ALIGN_CENTER;

                //añadir las celdas a la tabla
                TblReport.AddCell(cellRegion);
                TblReport.AddCell(cellEduPre);
                TblReport.AddCell(cellEduInc);
                TblReport.AddCell(cellPorcentajeEduInc);
                TblReport.AddCell(cellPromotor);
                TblReport.AddCell(cellTecnico);
                TblReport.AddCell(cellFormador);


                //Llenado de la tabla con la informacion
                foreach (var item in lstPlanSemanal)
                {
                    string eduPre = (item.preregistrosTotal == null ? 0 : item.preregistrosTotal.Value).ToString();
                    string eduInc = (item.imcorporadostotal == null ? 0 : item.imcorporadostotal.Value).ToString();
                    cellRegion = new PdfPCell(new Phrase(item.Region, _standardFont));
                    cellRegion.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellEduPre = new PdfPCell(new Phrase(eduPre, _standardFont));
                    cellEduPre.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellEduInc = new PdfPCell(new Phrase(eduInc, _standardFont));
                    cellEduInc.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellPorcentajeEduInc = new PdfPCell(new Phrase(item.porcentajeIncorporadosT, _standardFont));
                    cellPorcentajeEduInc.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellPromotor = new PdfPCell(new Phrase(item.pocentajePromotor, _standardFont));
                    cellPromotor.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellTecnico = new PdfPCell(new Phrase(item.pocentajeTecnico, _standardFont));
                    cellTecnico.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellFormador = new PdfPCell(new Phrase(item.pocentajeFormador, _standardFont));
                    cellFormador.HorizontalAlignment = Element.ALIGN_CENTER;

                    //Añadimos las celdas a la tabla
                    TblReport.AddCell(cellRegion);
                    TblReport.AddCell(cellEduPre);
                    TblReport.AddCell(cellEduInc);
                    TblReport.AddCell(cellPorcentajeEduInc);
                    TblReport.AddCell(cellPromotor);
                    TblReport.AddCell(cellTecnico);
                    TblReport.AddCell(cellFormador);
                }

                //Añadimos la tabla al documento PDF
                DocumentoPDF.Add(TblReport);

                //Espacio en blanco salto
                PdfPCell cellSalto = new PdfPCell(new Phrase(" ", _standardFontBold));
                cellSalto.BorderWidth = 0;
                cellSalto.BackgroundColor = BaseColor.WHITE;
                cellSalto.Colspan = 8;
                TblReportZona.AddCell(cellSalto);

                //Titulo
                PdfPCell cellTituloZona = new PdfPCell(new Phrase("ZONA", _standardFontBold));
                cellTituloZona.BackgroundColor = BaseColor.WHITE;
                cellTituloZona.Colspan = 8;
                cellTituloZona.HorizontalAlignment = Element.ALIGN_CENTER;
                TblReportZona.AddCell(cellTituloZona);

                //Crear Celdas para el PDF ZONA
                PdfPCell cellRegionZona = new PdfPCell(new Phrase("Región", _standardFontBold));
                cellRegionZona.BackgroundColor = new BaseColor(156, 195, 230);
                cellRegionZona.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellZona = new PdfPCell(new Phrase("Zona", _standardFontBold));
                cellZona.BackgroundColor = new BaseColor(156, 195, 230);
                cellZona.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellEduPreZona = new PdfPCell(new Phrase("Educandos pre-registrados", _standardFontBold));
                cellEduPreZona.BackgroundColor = new BaseColor(156, 195, 230);
                cellEduPreZona.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellEduIncZona = new PdfPCell(new Phrase("Educandos registrados", _standardFontBold));
                cellEduIncZona.BackgroundColor = new BaseColor(156, 195, 230);
                cellEduIncZona.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellPorcentajeEduIncZona = new PdfPCell(new Phrase("%Registrados", _standardFontBold));
                cellPorcentajeEduIncZona.BackgroundColor = new BaseColor(156, 195, 230);
                cellPorcentajeEduIncZona.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellPromotorZona = new PdfPCell(new Phrase("%Promotor", _standardFontBold));
                cellPromotorZona.BackgroundColor = new BaseColor(156, 195, 230);
                cellPromotorZona.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellTecnicoZona = new PdfPCell(new Phrase("%Técnicos docentes", _standardFontBold));
                cellTecnicoZona.BackgroundColor = new BaseColor(156, 195, 230);
                cellTecnicoZona.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellFormadorZona = new PdfPCell(new Phrase("%Formadores", _standardFontBold));
                cellFormadorZona.BackgroundColor = new BaseColor(156, 195, 230);
                cellFormadorZona.HorizontalAlignment = Element.ALIGN_CENTER;

                //añadir las celdas a la tabla
                TblReportZona.AddCell(cellRegionZona);
                TblReportZona.AddCell(cellZona);
                TblReportZona.AddCell(cellEduPreZona);
                TblReportZona.AddCell(cellEduIncZona);
                TblReportZona.AddCell(cellPorcentajeEduIncZona);
                TblReportZona.AddCell(cellPromotorZona);
                TblReportZona.AddCell(cellTecnicoZona);
                TblReportZona.AddCell(cellFormadorZona);


                //Llenado de la tabla con la informacion
                foreach (var item in lstPlanSemanalZona)
                {
                    string eduPre = (item.preregistrosTotal == null ? 0 : item.preregistrosTotal.Value).ToString();
                    string eduInc = (item.imcorporadostotal == null ? 0 : item.imcorporadostotal.Value).ToString();
                    cellRegionZona = new PdfPCell(new Phrase(item.Region, _standardFont));
                    cellRegionZona.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellZona = new PdfPCell(new Phrase(item.Zona, _standardFont));
                    cellZona.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellEduPreZona = new PdfPCell(new Phrase(eduPre, _standardFont));
                    cellEduPreZona.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellEduIncZona = new PdfPCell(new Phrase(eduInc, _standardFont));
                    cellEduIncZona.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellPorcentajeEduIncZona = new PdfPCell(new Phrase(item.porcentajeIncorporadosT, _standardFont));
                    cellPorcentajeEduIncZona.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellPromotorZona = new PdfPCell(new Phrase(item.pocentajePromotor, _standardFont));
                    cellPromotorZona.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellTecnicoZona = new PdfPCell(new Phrase(item.pocentajeTecnico, _standardFont));
                    cellTecnicoZona.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellFormadorZona = new PdfPCell(new Phrase(item.pocentajeFormador, _standardFont));
                    cellFormadorZona.HorizontalAlignment = Element.ALIGN_CENTER;

                    //Añadimos las celdas a la tabla
                    TblReportZona.AddCell(cellRegionZona);
                    TblReportZona.AddCell(cellZona);
                    TblReportZona.AddCell(cellEduPreZona);
                    TblReportZona.AddCell(cellEduIncZona);
                    TblReportZona.AddCell(cellPorcentajeEduIncZona);
                    TblReportZona.AddCell(cellPromotorZona);
                    TblReportZona.AddCell(cellTecnicoZona);
                    TblReportZona.AddCell(cellFormadorZona);
                }

                //Añadimos la tabla al documento PDF
                DocumentoPDF.Add(TblReportZona);

                //Espacio en blanco salto
                PdfPCell cellSaltoZFigura = new PdfPCell(new Phrase(" ", _standardFontBold));
                cellSaltoZFigura.BorderWidth = 0;
                cellSaltoZFigura.BackgroundColor = BaseColor.WHITE;
                cellSaltoZFigura.Colspan = 8;
                TblReportZonaFigura.AddCell(cellSaltoZFigura);

                //Titulo
                PdfPCell cellTituloZonaFig = new PdfPCell(new Phrase("Consulta por usuario de educandos", _standardFontBold));
                cellTituloZonaFig.BackgroundColor = BaseColor.WHITE;
                cellTituloZonaFig.Colspan = 8;
                cellTituloZonaFig.HorizontalAlignment = Element.ALIGN_CENTER;
                TblReportZonaFigura.AddCell(cellTituloZonaFig);

                //Crear Celdas para el PDF ZONA
                PdfPCell cellPeriodoZonaFig = new PdfPCell(new Phrase("Periodo", _standardFontBold));
                cellPeriodoZonaFig.BackgroundColor = new BaseColor(156, 195, 230);
                cellPeriodoZonaFig.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellRegionZonaFig = new PdfPCell(new Phrase("Región", _standardFontBold));
                cellRegionZonaFig.BackgroundColor = new BaseColor(156, 195, 230);
                cellRegionZonaFig.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellZonaFigura = new PdfPCell(new Phrase("Zona", _standardFontBold));
                cellZonaFigura.BackgroundColor = new BaseColor(156, 195, 230);
                cellZonaFigura.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellTipoFiZonaFig = new PdfPCell(new Phrase("Tipo Figura", _standardFontBold));
                cellTipoFiZonaFig.BackgroundColor = new BaseColor(156, 195, 230);
                cellTipoFiZonaFig.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellNomFiZonaFig = new PdfPCell(new Phrase("Nombre Figura", _standardFontBold));
                cellNomFiZonaFig.BackgroundColor = new BaseColor(156, 195, 230);
                cellNomFiZonaFig.HorizontalAlignment = Element.ALIGN_CENTER;
                
                PdfPCell cellEduPreZonaFig = new PdfPCell(new Phrase("Educandos pre-registrados", _standardFontBold));
                cellEduPreZonaFig.BackgroundColor = new BaseColor(156, 195, 230);
                cellEduPreZonaFig.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellEduIncZonaFig = new PdfPCell(new Phrase("Educandos registrados", _standardFontBold));
                cellEduIncZonaFig.BackgroundColor = new BaseColor(156, 195, 230);
                cellEduIncZonaFig.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellPorcentajeEduIncZonaFig = new PdfPCell(new Phrase("%Registrados", _standardFontBold));
                cellPorcentajeEduIncZonaFig.BackgroundColor = new BaseColor(156, 195, 230);
                cellPorcentajeEduIncZonaFig.HorizontalAlignment = Element.ALIGN_CENTER;

                //añadir las celdas a la tabla
                TblReportZonaFigura.AddCell(cellPeriodoZonaFig);
                TblReportZonaFigura.AddCell(cellRegionZonaFig);
                TblReportZonaFigura.AddCell(cellZonaFigura);
                TblReportZonaFigura.AddCell(cellTipoFiZonaFig);
                TblReportZonaFigura.AddCell(cellNomFiZonaFig);
                TblReportZonaFigura.AddCell(cellEduPreZonaFig);
                TblReportZonaFigura.AddCell(cellEduIncZonaFig);
                TblReportZonaFigura.AddCell(cellPorcentajeEduIncZonaFig);


                //Llenado de la tabla con la informacion
                foreach (var item in lstPlanSemanalFigura)
                {
                    string eduPre = (item.preregistrosTotal == null ? 0 : item.preregistrosTotal.Value).ToString();
                    string eduInc = (item.imcorporadostotal == null ? 0 : item.imcorporadostotal.Value).ToString();
                    cellPeriodoZonaFig = new PdfPCell(new Phrase(item.Periodo, _standardFont));
                    cellPeriodoZonaFig.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellRegionZonaFig = new PdfPCell(new Phrase(item.Region, _standardFont));
                    cellRegionZonaFig.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellZonaFigura = new PdfPCell(new Phrase(item.Zona, _standardFont));
                    cellZonaFigura.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellTipoFiZonaFig = new PdfPCell(new Phrase(item.DescripcionTipoFigura, _standardFont));
                    cellTipoFiZonaFig.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellNomFiZonaFig = new PdfPCell(new Phrase(item.NombreUsuario, _standardFont));
                    cellNomFiZonaFig.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellEduPreZonaFig = new PdfPCell(new Phrase(eduPre, _standardFont));
                    cellEduPreZonaFig.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellEduIncZonaFig = new PdfPCell(new Phrase(eduInc, _standardFont));
                    cellEduIncZonaFig.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellPorcentajeEduIncZonaFig = new PdfPCell(new Phrase(item.porcentajeIncorporadosT, _standardFont));
                    cellPorcentajeEduIncZonaFig.HorizontalAlignment = Element.ALIGN_CENTER;

                    //Añadimos las celdas a la tabla
                    TblReportZonaFigura.AddCell(cellPeriodoZonaFig);
                    TblReportZonaFigura.AddCell(cellRegionZonaFig);
                    TblReportZonaFigura.AddCell(cellZonaFigura);
                    TblReportZonaFigura.AddCell(cellTipoFiZonaFig);
                    TblReportZonaFigura.AddCell(cellNomFiZonaFig);
                    TblReportZonaFigura.AddCell(cellEduPreZonaFig);
                    TblReportZonaFigura.AddCell(cellEduIncZonaFig);
                    TblReportZonaFigura.AddCell(cellPorcentajeEduIncZonaFig);
                }

                //Añadimos la tabla al documento PDF
                DocumentoPDF.Add(TblReportZonaFigura);

                //Espacio en blanco salto
                PdfPCell cellSaltoEducando = new PdfPCell(new Phrase(" ", _standardFontBold));
                cellSaltoEducando.BorderWidth = 0;
                cellSaltoEducando.BackgroundColor = BaseColor.WHITE;
                cellSaltoEducando.Colspan = 8;
                TblReportEducando.AddCell(cellSaltoEducando);

                //Titulo
                PdfPCell cellTituloEducando = new PdfPCell(new Phrase("Consulta por educandos", _standardFontBold));
                cellTituloEducando.BackgroundColor = BaseColor.WHITE;
                cellTituloEducando.Colspan = 8;
                cellTituloEducando.HorizontalAlignment = Element.ALIGN_CENTER;
                TblReportEducando.AddCell(cellTituloEducando);

                //Crear Celdas para el PDF ZONA
                PdfPCell cellPeriodoEdu = new PdfPCell(new Phrase("Periodo", _standardFontBold));
                cellPeriodoEdu.BackgroundColor = new BaseColor(156, 195, 230);
                cellPeriodoEdu.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellRegionEdu = new PdfPCell(new Phrase("Región", _standardFontBold));
                cellRegionEdu.BackgroundColor = new BaseColor(156, 195, 230);
                cellRegionEdu.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellZonaEdu = new PdfPCell(new Phrase("Zona", _standardFontBold));
                cellZonaEdu.BackgroundColor = new BaseColor(156, 195, 230);
                cellZonaEdu.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellTipoFigEdu = new PdfPCell(new Phrase("Tipo Figura", _standardFontBold));
                cellTipoFigEdu.BackgroundColor = new BaseColor(156, 195, 230);
                cellTipoFigEdu.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellNomFigEdu = new PdfPCell(new Phrase("Nombre Figura", _standardFontBold));
                cellNomFigEdu.BackgroundColor = new BaseColor(156, 195, 230);
                cellNomFigEdu.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellNomEdu = new PdfPCell(new Phrase("Nombre del educando", _standardFontBold));
                cellNomEdu.BackgroundColor = new BaseColor(156, 195, 230);
                cellNomEdu.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellFechaEdu = new PdfPCell(new Phrase("Fecha Pre-registro", _standardFontBold));
                cellFechaEdu.BackgroundColor = new BaseColor(156, 195, 230);
                cellFechaEdu.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellFechaReg = new PdfPCell(new Phrase("Fecha Registro", _standardFontBold));
                cellFechaReg.BackgroundColor = new BaseColor(156, 195, 230);
                cellFechaReg.HorizontalAlignment = Element.ALIGN_CENTER;

                //añadir las celdas a la tabla
                TblReportEducando.AddCell(cellPeriodoEdu);
                TblReportEducando.AddCell(cellRegionEdu);
                TblReportEducando.AddCell(cellZonaEdu);
                TblReportEducando.AddCell(cellTipoFigEdu);
                TblReportEducando.AddCell(cellNomFigEdu);
                TblReportEducando.AddCell(cellNomEdu);
                TblReportEducando.AddCell(cellFechaEdu);
                TblReportEducando.AddCell(cellFechaReg);

                //Llenado de la tabla con la informacion
                foreach (var item in lstPlanSemanalEducando)
                {
                    cellPeriodoEdu = new PdfPCell(new Phrase(item.Periodo, _standardFont));
                    cellPeriodoEdu.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellRegionEdu = new PdfPCell(new Phrase(item.Region, _standardFont));
                    cellRegionEdu.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellZonaEdu = new PdfPCell(new Phrase(item.Zona, _standardFont));
                    cellZonaEdu.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellTipoFigEdu = new PdfPCell(new Phrase(item.DescripcionTipoFigura, _standardFont));
                    cellTipoFigEdu.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellNomFigEdu = new PdfPCell(new Phrase(item.NombreUsuario, _standardFont));
                    cellNomFigEdu.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellNomEdu = new PdfPCell(new Phrase(item.NombrePreregistro, _standardFont));
                    cellNomEdu.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellFechaEdu = new PdfPCell(new Phrase(item.FechaPreregistro, _standardFont));
                    cellFechaEdu.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellFechaReg = new PdfPCell(new Phrase(item.FechaRegistro, _standardFont));
                    cellFechaReg.HorizontalAlignment = Element.ALIGN_CENTER;

                    //Añadimos las celdas a la tabla
                    TblReportEducando.AddCell(cellPeriodoEdu);
                    TblReportEducando.AddCell(cellRegionEdu);
                    TblReportEducando.AddCell(cellZonaEdu);
                    TblReportEducando.AddCell(cellTipoFigEdu);
                    TblReportEducando.AddCell(cellNomFigEdu);
                    TblReportEducando.AddCell(cellNomEdu);
                    TblReportEducando.AddCell(cellFechaEdu);
                    TblReportEducando.AddCell(cellFechaReg);
                }

                //Añadimos la tabla al documento PDF
                DocumentoPDF.Add(TblReportEducando);

                DocumentoPDF.Close();
                writer.Close();


                HttpContext.Response.ContentType = "pdf/application";
                HttpContext.Response.AddHeader("content-disposition", "attachment;" +
               "filename=IndicadorPreregistros" + "_" + DateTime.Now.Date + ".pdf");
                HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                HttpContext.Response.Write(DocumentoPDF);
                HttpContext.Response.End();


            } // if lstProcesoOrd.Count()

        }

        public void ExportIndicadorValidacionPDF(int idUsuario, string idPeriodo, string Region, string Zonas)
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

                // creacion del objeto documento
                Document DocumentoPDF = new Document(PageSize.A4.Rotate());
                PdfWriter writer = PdfWriter.GetInstance(DocumentoPDF, HttpContext.Response.OutputStream);

                ITextEvents evPdf = new ITextEvents();
                evPdf.Title = "INSTITUTO DE ALFABETIZACIÓN Y EDUCACIÓN BÁSICA PARA ADULTOS";
                evPdf.TipoReporte = "INDICADOR FIGURAS REINCIDENTES EN RECHAZO DE PLAN SEMANAL";
                evPdf.Coordinacion = "";
                evPdf.Count = lstPlanSemanal.Count();
                writer.PageEvent = evPdf;

                // Le colocamos el título y el autor
                // **Nota: Esto no será visible en el documento
                DocumentoPDF.AddTitle("INDICADOR FIGURAS REINCIDENTES EN RECHAZO DE PLAN SEMANAL");
                DocumentoPDF.AddCreator("Inaeba");


                // Abrimos el archivo
                DocumentoPDF.Open();


                // Creamos el tipo de Font que vamos utilizar
                iTextSharp.text.Font _standardFont = new Font(Font.FontFamily.HELVETICA, 8, Font.NORMAL, BaseColor.BLACK);
                iTextSharp.text.Font _standardFontBold = new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.BLACK);


                //Crear tabla pdf
                PdfPTable TblReport = new PdfPTable(7);
                TblReport.WidthPercentage = 100;

                PdfPTable TblReportZona = new PdfPTable(8);
                TblReportZona.WidthPercentage = 100;

                PdfPTable TblReportZonaFigura = new PdfPTable(8);
                TblReportZonaFigura.WidthPercentage = 100;

                PdfPTable TblReportEducando = new PdfPTable(7);
                TblReportEducando.WidthPercentage = 100;

                //Titulo
                PdfPCell cellTitulo = new PdfPCell(new Phrase("REGIÓN", _standardFontBold));
                cellTitulo.BackgroundColor = BaseColor.WHITE;
                cellTitulo.Colspan = 7;
                cellTitulo.HorizontalAlignment = Element.ALIGN_CENTER;
                TblReport.AddCell(cellTitulo);

                //Crear Celdas para el PDF
                PdfPCell cellRegion = new PdfPCell(new Phrase("Región", _standardFontBold));
                cellRegion.BackgroundColor = new BaseColor(156, 195, 230);
                cellRegion.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellActCap = new PdfPCell(new Phrase("Actividades capturadas", _standardFontBold));
                cellActCap.BackgroundColor = new BaseColor(156, 195, 230);
                cellActCap.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellActRech = new PdfPCell(new Phrase("Actividades rechazadas", _standardFontBold));
                cellActRech.BackgroundColor = new BaseColor(156, 195, 230);
                cellActRech.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellPorcentajActRech = new PdfPCell(new Phrase("%Actividades rechazadas", _standardFontBold));
                cellPorcentajActRech.BackgroundColor = new BaseColor(156, 195, 230);
                cellPorcentajActRech.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellPromotor = new PdfPCell(new Phrase("%Promotor", _standardFontBold));
                cellPromotor.BackgroundColor = new BaseColor(156, 195, 230);
                cellPromotor.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellTecnico = new PdfPCell(new Phrase("%Técnicos docentes", _standardFontBold));
                cellTecnico.BackgroundColor = new BaseColor(156, 195, 230);
                cellTecnico.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellFormador = new PdfPCell(new Phrase("%Formadores", _standardFontBold));
                cellFormador.BackgroundColor = new BaseColor(156, 195, 230);
                cellFormador.HorizontalAlignment = Element.ALIGN_CENTER;

                //añadir las celdas a la tabla
                TblReport.AddCell(cellRegion);
                TblReport.AddCell(cellActCap);
                TblReport.AddCell(cellActRech);
                TblReport.AddCell(cellPorcentajActRech);
                TblReport.AddCell(cellPromotor);
                TblReport.AddCell(cellTecnico);
                TblReport.AddCell(cellFormador);


                //Llenado de la tabla con la informacion
                foreach (var item in lstPlanSemanal)
                {
                    string actCap = (item.planeadastotal == null ? 0 : item.planeadastotal.Value).ToString();
                    string actRech = (item.incumplidastotal == null ? 0 : item.incumplidastotal.Value).ToString();
                    cellRegion = new PdfPCell(new Phrase(item.Region, _standardFont));
                    cellRegion.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellActCap = new PdfPCell(new Phrase(actCap, _standardFont));
                    cellActCap.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellActRech = new PdfPCell(new Phrase(actRech, _standardFont));
                    cellActRech.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellPorcentajActRech = new PdfPCell(new Phrase(item.porcentajeRechazadasT, _standardFont));
                    cellPorcentajActRech.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellPromotor = new PdfPCell(new Phrase(item.pocentajePromotor, _standardFont));
                    cellPromotor.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellTecnico = new PdfPCell(new Phrase(item.pocentajeTecnico, _standardFont));
                    cellTecnico.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellFormador = new PdfPCell(new Phrase(item.pocentajeFormador, _standardFont));
                    cellFormador.HorizontalAlignment = Element.ALIGN_CENTER;

                    //Añadimos las celdas a la tabla
                    TblReport.AddCell(cellRegion);
                    TblReport.AddCell(cellActCap);
                    TblReport.AddCell(cellActRech);
                    TblReport.AddCell(cellPorcentajActRech);
                    TblReport.AddCell(cellPromotor);
                    TblReport.AddCell(cellTecnico);
                    TblReport.AddCell(cellFormador);
                }

                //Añadimos la tabla al documento PDF
                DocumentoPDF.Add(TblReport);

                //Espacio en blanco salto
                PdfPCell cellSalto = new PdfPCell(new Phrase(" ", _standardFontBold));
                cellSalto.BorderWidth = 0;
                cellSalto.BackgroundColor = BaseColor.WHITE;
                cellSalto.Colspan = 8;
                TblReportZona.AddCell(cellSalto);

                //Titulo
                PdfPCell cellTituloZona = new PdfPCell(new Phrase("ZONA", _standardFontBold));
                cellTituloZona.BackgroundColor = BaseColor.WHITE;
                cellTituloZona.Colspan = 8;
                cellTituloZona.HorizontalAlignment = Element.ALIGN_CENTER;
                TblReportZona.AddCell(cellTituloZona);

                //Crear Celdas para el PDF ZONA
                PdfPCell cellRegionZona = new PdfPCell(new Phrase("Región", _standardFontBold));
                cellRegionZona.BackgroundColor = new BaseColor(156, 195, 230);
                cellRegionZona.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellZona = new PdfPCell(new Phrase("Zona", _standardFontBold));
                cellZona.BackgroundColor = new BaseColor(156, 195, 230);
                cellZona.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellEduPreZona = new PdfPCell(new Phrase("Actividades capturadas", _standardFontBold));
                cellEduPreZona.BackgroundColor = new BaseColor(156, 195, 230);
                cellEduPreZona.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellEduIncZona = new PdfPCell(new Phrase("Actividades capturadas", _standardFontBold));
                cellEduIncZona.BackgroundColor = new BaseColor(156, 195, 230);
                cellEduIncZona.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellPorcentajeEduIncZona = new PdfPCell(new Phrase("%Actividades rechazadas", _standardFontBold));
                cellPorcentajeEduIncZona.BackgroundColor = new BaseColor(156, 195, 230);
                cellPorcentajeEduIncZona.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellPromotorZona = new PdfPCell(new Phrase("%Promotor", _standardFontBold));
                cellPromotorZona.BackgroundColor = new BaseColor(156, 195, 230);
                cellPromotorZona.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellTecnicoZona = new PdfPCell(new Phrase("%Técnicos docentes", _standardFontBold));
                cellTecnicoZona.BackgroundColor = new BaseColor(156, 195, 230);
                cellTecnicoZona.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellFormadorZona = new PdfPCell(new Phrase("%Formadores", _standardFontBold));
                cellFormadorZona.BackgroundColor = new BaseColor(156, 195, 230);
                cellFormadorZona.HorizontalAlignment = Element.ALIGN_CENTER;

                //añadir las celdas a la tabla
                TblReportZona.AddCell(cellRegionZona);
                TblReportZona.AddCell(cellZona);
                TblReportZona.AddCell(cellEduPreZona);
                TblReportZona.AddCell(cellEduIncZona);
                TblReportZona.AddCell(cellPorcentajeEduIncZona);
                TblReportZona.AddCell(cellPromotorZona);
                TblReportZona.AddCell(cellTecnicoZona);
                TblReportZona.AddCell(cellFormadorZona);


                //Llenado de la tabla con la informacion
                foreach (var item in lstPlanSemanalZona)
                {
                    string actCap = (item.planeadastotal == null ? 0 : item.planeadastotal.Value).ToString();
                    string actRech = (item.incumplidastotal == null ? 0 : item.incumplidastotal.Value).ToString();
                    cellRegionZona = new PdfPCell(new Phrase(item.Region, _standardFont));
                    cellRegionZona.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellZona = new PdfPCell(new Phrase(item.Zona, _standardFont));
                    cellZona.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellEduPreZona = new PdfPCell(new Phrase(actCap, _standardFont));
                    cellEduPreZona.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellEduIncZona = new PdfPCell(new Phrase(actRech, _standardFont));
                    cellEduIncZona.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellPorcentajeEduIncZona = new PdfPCell(new Phrase(item.porcentajeRechazadasT, _standardFont));
                    cellPorcentajeEduIncZona.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellPromotorZona = new PdfPCell(new Phrase(item.pocentajePromotor, _standardFont));
                    cellPromotorZona.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellTecnicoZona = new PdfPCell(new Phrase(item.pocentajeTecnico, _standardFont));
                    cellTecnicoZona.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellFormadorZona = new PdfPCell(new Phrase(item.pocentajeFormador, _standardFont));
                    cellFormadorZona.HorizontalAlignment = Element.ALIGN_CENTER;

                    //Añadimos las celdas a la tabla
                    TblReportZona.AddCell(cellRegionZona);
                    TblReportZona.AddCell(cellZona);
                    TblReportZona.AddCell(cellEduPreZona);
                    TblReportZona.AddCell(cellEduIncZona);
                    TblReportZona.AddCell(cellPorcentajeEduIncZona);
                    TblReportZona.AddCell(cellPromotorZona);
                    TblReportZona.AddCell(cellTecnicoZona);
                    TblReportZona.AddCell(cellFormadorZona);
                }

                //Añadimos la tabla al documento PDF
                DocumentoPDF.Add(TblReportZona);

                //Espacio en blanco salto
                PdfPCell cellSaltoZFigura = new PdfPCell(new Phrase(" ", _standardFontBold));
                cellSaltoZFigura.BorderWidth = 0;
                cellSaltoZFigura.BackgroundColor = BaseColor.WHITE;
                cellSaltoZFigura.Colspan = 8;
                TblReportZonaFigura.AddCell(cellSaltoZFigura);

                //Titulo
                PdfPCell cellTituloZonaFig = new PdfPCell(new Phrase("Por Figura durante Validación", _standardFontBold));
                cellTituloZonaFig.BackgroundColor = BaseColor.WHITE;
                cellTituloZonaFig.Colspan = 8;
                cellTituloZonaFig.HorizontalAlignment = Element.ALIGN_CENTER;
                TblReportZonaFigura.AddCell(cellTituloZonaFig);

                //Crear Celdas para el PDF ZONA
                PdfPCell cellPeriodoZonaFig = new PdfPCell(new Phrase("Periodo", _standardFontBold));
                cellPeriodoZonaFig.BackgroundColor = new BaseColor(156, 195, 230);
                cellPeriodoZonaFig.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellRegionZonaFig = new PdfPCell(new Phrase("Región", _standardFontBold));
                cellRegionZonaFig.BackgroundColor = new BaseColor(156, 195, 230);
                cellRegionZonaFig.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellZonaFigura = new PdfPCell(new Phrase("Zona", _standardFontBold));
                cellZonaFigura.BackgroundColor = new BaseColor(156, 195, 230);
                cellZonaFigura.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellTipoFiZonaFig = new PdfPCell(new Phrase("Tipo Figura", _standardFontBold));
                cellTipoFiZonaFig.BackgroundColor = new BaseColor(156, 195, 230);
                cellTipoFiZonaFig.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellNomFiZonaFig = new PdfPCell(new Phrase("Nombre Figura", _standardFontBold));
                cellNomFiZonaFig.BackgroundColor = new BaseColor(156, 195, 230);
                cellNomFiZonaFig.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellEduPreZonaFig = new PdfPCell(new Phrase("Actividades capturadas", _standardFontBold));
                cellEduPreZonaFig.BackgroundColor = new BaseColor(156, 195, 230);
                cellEduPreZonaFig.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellEduIncZonaFig = new PdfPCell(new Phrase("Actividades rechazadas", _standardFontBold));
                cellEduIncZonaFig.BackgroundColor = new BaseColor(156, 195, 230);
                cellEduIncZonaFig.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellPorcentajeEduIncZonaFig = new PdfPCell(new Phrase("%Actividades rechazadas", _standardFontBold));
                cellPorcentajeEduIncZonaFig.BackgroundColor = new BaseColor(156, 195, 230);
                cellPorcentajeEduIncZonaFig.HorizontalAlignment = Element.ALIGN_CENTER;

                //añadir las celdas a la tabla
                TblReportZonaFigura.AddCell(cellPeriodoZonaFig);
                TblReportZonaFigura.AddCell(cellRegionZonaFig);
                TblReportZonaFigura.AddCell(cellZonaFigura);
                TblReportZonaFigura.AddCell(cellTipoFiZonaFig);
                TblReportZonaFigura.AddCell(cellNomFiZonaFig);
                TblReportZonaFigura.AddCell(cellEduPreZonaFig);
                TblReportZonaFigura.AddCell(cellEduIncZonaFig);
                TblReportZonaFigura.AddCell(cellPorcentajeEduIncZonaFig);


                //Llenado de la tabla con la informacion
                foreach (var item in lstPlanSemanalFigura)
                {
                    string actCap = (item.planeadastotal == null ? 0 : item.planeadastotal.Value).ToString();
                    string actRech = (item.incumplidastotal == null ? 0 : item.incumplidastotal.Value).ToString();
                    cellPeriodoZonaFig = new PdfPCell(new Phrase(item.Periodo, _standardFont));
                    cellPeriodoZonaFig.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellRegionZonaFig = new PdfPCell(new Phrase(item.Region, _standardFont));
                    cellRegionZonaFig.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellZonaFigura = new PdfPCell(new Phrase(item.Zona, _standardFont));
                    cellZonaFigura.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellTipoFiZonaFig = new PdfPCell(new Phrase(item.TipoFigura, _standardFont));
                    cellTipoFiZonaFig.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellNomFiZonaFig = new PdfPCell(new Phrase(item.NombreFigura, _standardFont));
                    cellNomFiZonaFig.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellEduPreZonaFig = new PdfPCell(new Phrase(actCap, _standardFont));
                    cellEduPreZonaFig.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellEduIncZonaFig = new PdfPCell(new Phrase(actRech, _standardFont));
                    cellEduIncZonaFig.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellPorcentajeEduIncZonaFig = new PdfPCell(new Phrase(item.porcentajeRechazadasT, _standardFont));
                    cellPorcentajeEduIncZonaFig.HorizontalAlignment = Element.ALIGN_CENTER;

                    //Añadimos las celdas a la tabla
                    TblReportZonaFigura.AddCell(cellPeriodoZonaFig);
                    TblReportZonaFigura.AddCell(cellRegionZonaFig);
                    TblReportZonaFigura.AddCell(cellZonaFigura);
                    TblReportZonaFigura.AddCell(cellTipoFiZonaFig);
                    TblReportZonaFigura.AddCell(cellNomFiZonaFig);
                    TblReportZonaFigura.AddCell(cellEduPreZonaFig);
                    TblReportZonaFigura.AddCell(cellEduIncZonaFig);
                    TblReportZonaFigura.AddCell(cellPorcentajeEduIncZonaFig);
                }

                //Añadimos la tabla al documento PDF
                DocumentoPDF.Add(TblReportZonaFigura);

                //Espacio en blanco salto
                PdfPCell cellSaltoEducando = new PdfPCell(new Phrase(" ", _standardFontBold));
                cellSaltoEducando.BorderWidth = 0;
                cellSaltoEducando.BackgroundColor = BaseColor.WHITE;
                cellSaltoEducando.Colspan = 7;
                TblReportEducando.AddCell(cellSaltoEducando);

                //Titulo
                PdfPCell cellTituloEducando = new PdfPCell(new Phrase("Detalle por Figura", _standardFontBold));
                cellTituloEducando.BackgroundColor = BaseColor.WHITE;
                cellTituloEducando.Colspan = 7;
                cellTituloEducando.HorizontalAlignment = Element.ALIGN_CENTER;
                TblReportEducando.AddCell(cellTituloEducando);

                //Crear Celdas para el PDF ZONA
                PdfPCell cellPeriodoEdu = new PdfPCell(new Phrase("Periodo", _standardFontBold));
                cellPeriodoEdu.BackgroundColor = new BaseColor(156, 195, 230);
                cellPeriodoEdu.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellRegionEdu = new PdfPCell(new Phrase("Región", _standardFontBold));
                cellRegionEdu.BackgroundColor = new BaseColor(156, 195, 230);
                cellRegionEdu.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellZonaEdu = new PdfPCell(new Phrase("Zona", _standardFontBold));
                cellZonaEdu.BackgroundColor = new BaseColor(156, 195, 230);
                cellZonaEdu.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellTipoFigEdu = new PdfPCell(new Phrase("Tipo Figura", _standardFontBold));
                cellTipoFigEdu.BackgroundColor = new BaseColor(156, 195, 230);
                cellTipoFigEdu.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellNomFigEdu = new PdfPCell(new Phrase("Nombre Figura", _standardFontBold));
                cellNomFigEdu.BackgroundColor = new BaseColor(156, 195, 230);
                cellNomFigEdu.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellNomEdu = new PdfPCell(new Phrase("Tipo de Actividad", _standardFontBold));
                cellNomEdu.BackgroundColor = new BaseColor(156, 195, 230);
                cellNomEdu.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellFechaEdu = new PdfPCell(new Phrase("Comentarios CZ", _standardFontBold));
                cellFechaEdu.BackgroundColor = new BaseColor(156, 195, 230);
                cellFechaEdu.HorizontalAlignment = Element.ALIGN_CENTER;

                //añadir las celdas a la tabla
                TblReportEducando.AddCell(cellPeriodoEdu);
                TblReportEducando.AddCell(cellRegionEdu);
                TblReportEducando.AddCell(cellZonaEdu);
                TblReportEducando.AddCell(cellTipoFigEdu);
                TblReportEducando.AddCell(cellNomFigEdu);
                TblReportEducando.AddCell(cellNomEdu);
                TblReportEducando.AddCell(cellFechaEdu);

                //Llenado de la tabla con la informacion
                foreach (var item in lstPlanSemanalDetFigura)
                {
                    cellPeriodoEdu = new PdfPCell(new Phrase(item.Periodo, _standardFont));
                    cellPeriodoEdu.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellRegionEdu = new PdfPCell(new Phrase(item.Region, _standardFont));
                    cellRegionEdu.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellZonaEdu = new PdfPCell(new Phrase(item.Zona, _standardFont));
                    cellZonaEdu.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellTipoFigEdu = new PdfPCell(new Phrase(item.TipoFigura, _standardFont));
                    cellTipoFigEdu.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellNomFigEdu = new PdfPCell(new Phrase(item.NombreFigura, _standardFont));
                    cellNomFigEdu.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellNomEdu = new PdfPCell(new Phrase(item.DescripcionActividad, _standardFont));
                    cellNomEdu.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellFechaEdu = new PdfPCell(new Phrase(item.ComentariosNoValidacion, _standardFont));
                    cellFechaEdu.HorizontalAlignment = Element.ALIGN_CENTER;

                    //Añadimos las celdas a la tabla
                    TblReportEducando.AddCell(cellPeriodoEdu);
                    TblReportEducando.AddCell(cellRegionEdu);
                    TblReportEducando.AddCell(cellZonaEdu);
                    TblReportEducando.AddCell(cellTipoFigEdu);
                    TblReportEducando.AddCell(cellNomFigEdu);
                    TblReportEducando.AddCell(cellNomEdu);
                    TblReportEducando.AddCell(cellFechaEdu);
                }

                //Añadimos la tabla al documento PDF
                DocumentoPDF.Add(TblReportEducando);

                DocumentoPDF.Close();
                writer.Close();


                HttpContext.Response.ContentType = "pdf/application";
                HttpContext.Response.AddHeader("content-disposition", "attachment;" +
               "filename=IndicadorFigurasReincidentes" + "_" + DateTime.Now.Date + ".pdf");
                HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                HttpContext.Response.Write(DocumentoPDF);
                HttpContext.Response.End();


            } // if lstProcesoOrd.Count()

        }

        public void ExportIndicadorAprobacionPDF(int idUsuario, string idPeriodo, string Region, string Zonas)
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

            List<udf_RechazoAprobacionZonaFiguraNombreAprobacionList_Result> resultFigura = (from u in db.udf_RechazoAprobacionZonaFiguraNombreAprobacionList(idPeriodo, Region, Zonas, "") select u).Where(x => x.rechazadasTotal > 0).ToList();

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
            });

            List<udf_RechazoAprobacionZonaFiguraNombreAprobacionAct_Result> resultDetFigura = (from u in db.udf_RechazoAprobacionZonaFiguraNombreAprobacionAct(idPeriodo, Region, Zonas, "") select u).ToList();

            var lstPlanSemanalDetFigura= resultDetFigura.Select(C => new
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

                // creacion del objeto documento
                Document DocumentoPDF = new Document(PageSize.A4.Rotate());
                PdfWriter writer = PdfWriter.GetInstance(DocumentoPDF, HttpContext.Response.OutputStream);

                ITextEvents evPdf = new ITextEvents();
                evPdf.Title = "INSTITUTO DE ALFABETIZACIÓN Y EDUCACIÓN BÁSICA PARA ADULTOS";
                evPdf.TipoReporte = "INDICADOR FIGURAS CON OBSERVACIONES EN LA APROBACIÓN";
                evPdf.Coordinacion = "";
                evPdf.Count = lstPlanSemanal.Count();
                writer.PageEvent = evPdf;

                // Le colocamos el título y el autor
                // **Nota: Esto no será visible en el documento
                DocumentoPDF.AddTitle("INDICADOR FIGURAS CON OBSERVACIONES EN LA APROBACIÓN");
                DocumentoPDF.AddCreator("Inaeba");


                // Abrimos el archivo
                DocumentoPDF.Open();


                // Creamos el tipo de Font que vamos utilizar
                iTextSharp.text.Font _standardFont = new Font(Font.FontFamily.HELVETICA, 8, Font.NORMAL, BaseColor.BLACK);
                iTextSharp.text.Font _standardFontBold = new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.BLACK);


                //Crear tabla pdf
                PdfPTable TblReport = new PdfPTable(7);
                TblReport.WidthPercentage = 100;

                PdfPTable TblReportZona = new PdfPTable(8);
                TblReportZona.WidthPercentage = 100;

                PdfPTable TblReportZonaFigura = new PdfPTable(8);
                TblReportZonaFigura.WidthPercentage = 100;

                PdfPTable TblReportEducando = new PdfPTable(7);
                TblReportEducando.WidthPercentage = 100;

                //Titulo
                PdfPCell cellTitulo = new PdfPCell(new Phrase("REGIÓN", _standardFontBold));
                cellTitulo.BackgroundColor = BaseColor.WHITE;
                cellTitulo.Colspan = 7;
                cellTitulo.HorizontalAlignment = Element.ALIGN_CENTER;
                TblReport.AddCell(cellTitulo);

                //Crear Celdas para el PDF
                PdfPCell cellRegion = new PdfPCell(new Phrase("Región", _standardFontBold));
                cellRegion.BackgroundColor = new BaseColor(156, 195, 230);
                cellRegion.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellActCap = new PdfPCell(new Phrase("Actividades aprobadas", _standardFontBold));
                cellActCap.BackgroundColor = new BaseColor(156, 195, 230);
                cellActCap.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellActRech = new PdfPCell(new Phrase("Actividades con observaciones", _standardFontBold));
                cellActRech.BackgroundColor = new BaseColor(156, 195, 230);
                cellActRech.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellPorcentajActRech = new PdfPCell(new Phrase("%Observaciones", _standardFontBold));
                cellPorcentajActRech.BackgroundColor = new BaseColor(156, 195, 230);
                cellPorcentajActRech.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellPromotor = new PdfPCell(new Phrase("%Promotor", _standardFontBold));
                cellPromotor.BackgroundColor = new BaseColor(156, 195, 230);
                cellPromotor.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellTecnico = new PdfPCell(new Phrase("%Técnicos docentes", _standardFontBold));
                cellTecnico.BackgroundColor = new BaseColor(156, 195, 230);
                cellTecnico.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellFormador = new PdfPCell(new Phrase("%Formadores", _standardFontBold));
                cellFormador.BackgroundColor = new BaseColor(156, 195, 230);
                cellFormador.HorizontalAlignment = Element.ALIGN_CENTER;

                //añadir las celdas a la tabla
                TblReport.AddCell(cellRegion);
                TblReport.AddCell(cellActCap);
                TblReport.AddCell(cellActRech);
                TblReport.AddCell(cellPorcentajActRech);
                TblReport.AddCell(cellPromotor);
                TblReport.AddCell(cellTecnico);
                TblReport.AddCell(cellFormador);


                //Llenado de la tabla con la informacion
                foreach (var item in lstPlanSemanal)
                {
                    string actCap = (item.planeadastotal == null ? 0 : item.planeadastotal.Value).ToString();
                    string actRech = (item.incumplidastotal == null ? 0 : item.incumplidastotal.Value).ToString();
                    cellRegion = new PdfPCell(new Phrase(item.Region, _standardFont));
                    cellRegion.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellActCap = new PdfPCell(new Phrase(actCap, _standardFont));
                    cellActCap.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellActRech = new PdfPCell(new Phrase(actRech, _standardFont));
                    cellActRech.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellPorcentajActRech = new PdfPCell(new Phrase(item.porcentajeRechazadasT, _standardFont));
                    cellPorcentajActRech.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellPromotor = new PdfPCell(new Phrase(item.pocentajePromotor, _standardFont));
                    cellPromotor.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellTecnico = new PdfPCell(new Phrase(item.pocentajeTecnico, _standardFont));
                    cellTecnico.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellFormador = new PdfPCell(new Phrase(item.pocentajeFormador, _standardFont));
                    cellFormador.HorizontalAlignment = Element.ALIGN_CENTER;

                    //Añadimos las celdas a la tabla
                    TblReport.AddCell(cellRegion);
                    TblReport.AddCell(cellActCap);
                    TblReport.AddCell(cellActRech);
                    TblReport.AddCell(cellPorcentajActRech);
                    TblReport.AddCell(cellPromotor);
                    TblReport.AddCell(cellTecnico);
                    TblReport.AddCell(cellFormador);
                }

                //Añadimos la tabla al documento PDF
                DocumentoPDF.Add(TblReport);

                //Espacio en blanco salto
                PdfPCell cellSalto = new PdfPCell(new Phrase(" ", _standardFontBold));
                cellSalto.BorderWidth = 0;
                cellSalto.BackgroundColor = BaseColor.WHITE;
                cellSalto.Colspan = 8;
                TblReportZona.AddCell(cellSalto);

                //Titulo
                PdfPCell cellTituloZona = new PdfPCell(new Phrase("ZONA", _standardFontBold));
                cellTituloZona.BackgroundColor = BaseColor.WHITE;
                cellTituloZona.Colspan = 8;
                cellTituloZona.HorizontalAlignment = Element.ALIGN_CENTER;
                TblReportZona.AddCell(cellTituloZona);

                //Crear Celdas para el PDF ZONA
                PdfPCell cellRegionZona = new PdfPCell(new Phrase("Región", _standardFontBold));
                cellRegionZona.BackgroundColor = new BaseColor(156, 195, 230);
                cellRegionZona.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellZona = new PdfPCell(new Phrase("Zona", _standardFontBold));
                cellZona.BackgroundColor = new BaseColor(156, 195, 230);
                cellZona.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellEduPreZona = new PdfPCell(new Phrase("Actividades aprobadas", _standardFontBold));
                cellEduPreZona.BackgroundColor = new BaseColor(156, 195, 230);
                cellEduPreZona.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellEduIncZona = new PdfPCell(new Phrase("Actividades con observaciones", _standardFontBold));
                cellEduIncZona.BackgroundColor = new BaseColor(156, 195, 230);
                cellEduIncZona.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellPorcentajeEduIncZona = new PdfPCell(new Phrase("%Observaciones", _standardFontBold));
                cellPorcentajeEduIncZona.BackgroundColor = new BaseColor(156, 195, 230);
                cellPorcentajeEduIncZona.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellPromotorZona = new PdfPCell(new Phrase("%Promotor", _standardFontBold));
                cellPromotorZona.BackgroundColor = new BaseColor(156, 195, 230);
                cellPromotorZona.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellTecnicoZona = new PdfPCell(new Phrase("%Técnicos docentes", _standardFontBold));
                cellTecnicoZona.BackgroundColor = new BaseColor(156, 195, 230);
                cellTecnicoZona.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellFormadorZona = new PdfPCell(new Phrase("%Formadores", _standardFontBold));
                cellFormadorZona.BackgroundColor = new BaseColor(156, 195, 230);
                cellFormadorZona.HorizontalAlignment = Element.ALIGN_CENTER;

                //añadir las celdas a la tabla
                TblReportZona.AddCell(cellRegionZona);
                TblReportZona.AddCell(cellZona);
                TblReportZona.AddCell(cellEduPreZona);
                TblReportZona.AddCell(cellEduIncZona);
                TblReportZona.AddCell(cellPorcentajeEduIncZona);
                TblReportZona.AddCell(cellPromotorZona);
                TblReportZona.AddCell(cellTecnicoZona);
                TblReportZona.AddCell(cellFormadorZona);


                //Llenado de la tabla con la informacion
                foreach (var item in lstPlanSemanalZona)
                {
                    string actCap = (item.planeadastotal == null ? 0 : item.planeadastotal.Value).ToString();
                    string actRech = (item.incumplidastotal == null ? 0 : item.incumplidastotal.Value).ToString();
                    cellRegionZona = new PdfPCell(new Phrase(item.Region, _standardFont));
                    cellRegionZona.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellZona = new PdfPCell(new Phrase(item.Zona, _standardFont));
                    cellZona.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellEduPreZona = new PdfPCell(new Phrase(actCap, _standardFont));
                    cellEduPreZona.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellEduIncZona = new PdfPCell(new Phrase(actRech, _standardFont));
                    cellEduIncZona.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellPorcentajeEduIncZona = new PdfPCell(new Phrase(item.porcentajeRechazadasT, _standardFont));
                    cellPorcentajeEduIncZona.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellPromotorZona = new PdfPCell(new Phrase(item.pocentajePromotor, _standardFont));
                    cellPromotorZona.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellTecnicoZona = new PdfPCell(new Phrase(item.pocentajeTecnico, _standardFont));
                    cellTecnicoZona.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellFormadorZona = new PdfPCell(new Phrase(item.pocentajeFormador, _standardFont));
                    cellFormadorZona.HorizontalAlignment = Element.ALIGN_CENTER;

                    //Añadimos las celdas a la tabla
                    TblReportZona.AddCell(cellRegionZona);
                    TblReportZona.AddCell(cellZona);
                    TblReportZona.AddCell(cellEduPreZona);
                    TblReportZona.AddCell(cellEduIncZona);
                    TblReportZona.AddCell(cellPorcentajeEduIncZona);
                    TblReportZona.AddCell(cellPromotorZona);
                    TblReportZona.AddCell(cellTecnicoZona);
                    TblReportZona.AddCell(cellFormadorZona);
                }

                //Añadimos la tabla al documento PDF
                DocumentoPDF.Add(TblReportZona);

                //Espacio en blanco salto
                PdfPCell cellSaltoZFigura = new PdfPCell(new Phrase(" ", _standardFontBold));
                cellSaltoZFigura.BorderWidth = 0;
                cellSaltoZFigura.BackgroundColor = BaseColor.WHITE;
                cellSaltoZFigura.Colspan = 8;
                TblReportZonaFigura.AddCell(cellSaltoZFigura);

                //Titulo
                PdfPCell cellTituloZonaFig = new PdfPCell(new Phrase("Por Figura durante la Aprobación", _standardFontBold));
                cellTituloZonaFig.BackgroundColor = BaseColor.WHITE;
                cellTituloZonaFig.Colspan = 8;
                cellTituloZonaFig.HorizontalAlignment = Element.ALIGN_CENTER;
                TblReportZonaFigura.AddCell(cellTituloZonaFig);

                //Crear Celdas para el PDF ZONA
                PdfPCell cellPeriodoZonaFig = new PdfPCell(new Phrase("Periodo", _standardFontBold));
                cellPeriodoZonaFig.BackgroundColor = new BaseColor(156, 195, 230);
                cellPeriodoZonaFig.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellRegionZonaFig = new PdfPCell(new Phrase("Región", _standardFontBold));
                cellRegionZonaFig.BackgroundColor = new BaseColor(156, 195, 230);
                cellRegionZonaFig.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellZonaFigura = new PdfPCell(new Phrase("Zona", _standardFontBold));
                cellZonaFigura.BackgroundColor = new BaseColor(156, 195, 230);
                cellZonaFigura.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellTipoFiZonaFig = new PdfPCell(new Phrase("Tipo Figura", _standardFontBold));
                cellTipoFiZonaFig.BackgroundColor = new BaseColor(156, 195, 230);
                cellTipoFiZonaFig.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellNomFiZonaFig = new PdfPCell(new Phrase("Nombre Figura", _standardFontBold));
                cellNomFiZonaFig.BackgroundColor = new BaseColor(156, 195, 230);
                cellNomFiZonaFig.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellEduPreZonaFig = new PdfPCell(new Phrase("Actividades aprobadas", _standardFontBold));
                cellEduPreZonaFig.BackgroundColor = new BaseColor(156, 195, 230);
                cellEduPreZonaFig.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellEduIncZonaFig = new PdfPCell(new Phrase("Actividades observadas", _standardFontBold));
                cellEduIncZonaFig.BackgroundColor = new BaseColor(156, 195, 230);
                cellEduIncZonaFig.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellPorcentajeEduIncZonaFig = new PdfPCell(new Phrase("%Observaciones", _standardFontBold));
                cellPorcentajeEduIncZonaFig.BackgroundColor = new BaseColor(156, 195, 230);
                cellPorcentajeEduIncZonaFig.HorizontalAlignment = Element.ALIGN_CENTER;

                //añadir las celdas a la tabla
                TblReportZonaFigura.AddCell(cellPeriodoZonaFig);
                TblReportZonaFigura.AddCell(cellRegionZonaFig);
                TblReportZonaFigura.AddCell(cellZonaFigura);
                TblReportZonaFigura.AddCell(cellTipoFiZonaFig);
                TblReportZonaFigura.AddCell(cellNomFiZonaFig);
                TblReportZonaFigura.AddCell(cellEduPreZonaFig);
                TblReportZonaFigura.AddCell(cellEduIncZonaFig);
                TblReportZonaFigura.AddCell(cellPorcentajeEduIncZonaFig);


                //Llenado de la tabla con la informacion
                foreach (var item in lstPlanSemanalFigura)
                {
                    string actCap = (item.planeadastotal == null ? 0 : item.planeadastotal.Value).ToString();
                    string actRech = (item.incumplidastotal == null ? 0 : item.incumplidastotal.Value).ToString();
                    cellPeriodoZonaFig = new PdfPCell(new Phrase(item.Periodo, _standardFont));
                    cellPeriodoZonaFig.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellRegionZonaFig = new PdfPCell(new Phrase(item.Region, _standardFont));
                    cellRegionZonaFig.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellZonaFigura = new PdfPCell(new Phrase(item.Zona, _standardFont));
                    cellZonaFigura.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellTipoFiZonaFig = new PdfPCell(new Phrase(item.TipoFigura, _standardFont));
                    cellTipoFiZonaFig.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellNomFiZonaFig = new PdfPCell(new Phrase(item.NombreFigura, _standardFont));
                    cellNomFiZonaFig.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellEduPreZonaFig = new PdfPCell(new Phrase(actCap, _standardFont));
                    cellEduPreZonaFig.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellEduIncZonaFig = new PdfPCell(new Phrase(actRech, _standardFont));
                    cellEduIncZonaFig.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellPorcentajeEduIncZonaFig = new PdfPCell(new Phrase(item.porcentajeRechazadasT, _standardFont));
                    cellPorcentajeEduIncZonaFig.HorizontalAlignment = Element.ALIGN_CENTER;

                    //Añadimos las celdas a la tabla
                    TblReportZonaFigura.AddCell(cellPeriodoZonaFig);
                    TblReportZonaFigura.AddCell(cellRegionZonaFig);
                    TblReportZonaFigura.AddCell(cellZonaFigura);
                    TblReportZonaFigura.AddCell(cellTipoFiZonaFig);
                    TblReportZonaFigura.AddCell(cellNomFiZonaFig);
                    TblReportZonaFigura.AddCell(cellEduPreZonaFig);
                    TblReportZonaFigura.AddCell(cellEduIncZonaFig);
                    TblReportZonaFigura.AddCell(cellPorcentajeEduIncZonaFig);
                }

                //Añadimos la tabla al documento PDF
                DocumentoPDF.Add(TblReportZonaFigura);

                //Espacio en blanco salto
                PdfPCell cellSaltoEducando = new PdfPCell(new Phrase(" ", _standardFontBold));
                cellSaltoEducando.BorderWidth = 0;
                cellSaltoEducando.BackgroundColor = BaseColor.WHITE;
                cellSaltoEducando.Colspan = 7;
                TblReportEducando.AddCell(cellSaltoEducando);

                //Titulo
                PdfPCell cellTituloEducando = new PdfPCell(new Phrase("Detalle por Figura durante Aprobación", _standardFontBold));
                cellTituloEducando.BackgroundColor = BaseColor.WHITE;
                cellTituloEducando.Colspan = 7;
                cellTituloEducando.HorizontalAlignment = Element.ALIGN_CENTER;
                TblReportEducando.AddCell(cellTituloEducando);

                //Crear Celdas para el PDF ZONA
                PdfPCell cellPeriodoEdu = new PdfPCell(new Phrase("Periodo", _standardFontBold));
                cellPeriodoEdu.BackgroundColor = new BaseColor(156, 195, 230);
                cellPeriodoEdu.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellRegionEdu = new PdfPCell(new Phrase("Región", _standardFontBold));
                cellRegionEdu.BackgroundColor = new BaseColor(156, 195, 230);
                cellRegionEdu.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellZonaEdu = new PdfPCell(new Phrase("Zona", _standardFontBold));
                cellZonaEdu.BackgroundColor = new BaseColor(156, 195, 230);
                cellZonaEdu.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellTipoFigEdu = new PdfPCell(new Phrase("Tipo Figura", _standardFontBold));
                cellTipoFigEdu.BackgroundColor = new BaseColor(156, 195, 230);
                cellTipoFigEdu.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellNomFigEdu = new PdfPCell(new Phrase("Nombre Figura", _standardFontBold));
                cellNomFigEdu.BackgroundColor = new BaseColor(156, 195, 230);
                cellNomFigEdu.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellNomEdu = new PdfPCell(new Phrase("Tipo de Actividad", _standardFontBold));
                cellNomEdu.BackgroundColor = new BaseColor(156, 195, 230);
                cellNomEdu.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellFechaEdu = new PdfPCell(new Phrase("Comentarios CZ", _standardFontBold));
                cellFechaEdu.BackgroundColor = new BaseColor(156, 195, 230);
                cellFechaEdu.HorizontalAlignment = Element.ALIGN_CENTER;

                //añadir las celdas a la tabla
                TblReportEducando.AddCell(cellPeriodoEdu);
                TblReportEducando.AddCell(cellRegionEdu);
                TblReportEducando.AddCell(cellZonaEdu);
                TblReportEducando.AddCell(cellTipoFigEdu);
                TblReportEducando.AddCell(cellNomFigEdu);
                TblReportEducando.AddCell(cellNomEdu);
                TblReportEducando.AddCell(cellFechaEdu);

                //Llenado de la tabla con la informacion
                foreach (var item in lstPlanSemanalDetFigura)
                {
                    cellPeriodoEdu = new PdfPCell(new Phrase(item.Periodo, _standardFont));
                    cellPeriodoEdu.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellRegionEdu = new PdfPCell(new Phrase(item.Region, _standardFont));
                    cellRegionEdu.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellZonaEdu = new PdfPCell(new Phrase(item.Zona, _standardFont));
                    cellZonaEdu.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellTipoFigEdu = new PdfPCell(new Phrase(item.TipoFigura, _standardFont));
                    cellTipoFigEdu.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellNomFigEdu = new PdfPCell(new Phrase(item.NombreFigura, _standardFont));
                    cellNomFigEdu.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellNomEdu = new PdfPCell(new Phrase(item.DescripcionActividad, _standardFont));
                    cellNomEdu.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellFechaEdu = new PdfPCell(new Phrase(item.ComentariosRechazo, _standardFont));
                    cellFechaEdu.HorizontalAlignment = Element.ALIGN_CENTER;

                    //Añadimos las celdas a la tabla
                    TblReportEducando.AddCell(cellPeriodoEdu);
                    TblReportEducando.AddCell(cellRegionEdu);
                    TblReportEducando.AddCell(cellZonaEdu);
                    TblReportEducando.AddCell(cellTipoFigEdu);
                    TblReportEducando.AddCell(cellNomFigEdu);
                    TblReportEducando.AddCell(cellNomEdu);
                    TblReportEducando.AddCell(cellFechaEdu);
                }

                //Añadimos la tabla al documento PDF
                DocumentoPDF.Add(TblReportEducando);

                DocumentoPDF.Close();
                writer.Close();


                HttpContext.Response.ContentType = "pdf/application";
                HttpContext.Response.AddHeader("content-disposition", "attachment;" +
               "filename=IndicadorFigurasObservaciones" + "_" + DateTime.Now.Date + ".pdf");
                HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                HttpContext.Response.Write(DocumentoPDF);
                HttpContext.Response.End();


            } // if lstProcesoOrd.Count()

        }

        public void ExportIndicadorIncumplidasPDF(int idUsuario, string idPeriodo, string Region, string Zonas, string TipoActividad)
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
                periodo = C.DecripcionPeriodo,
                region = C.Region,
                planeadastotal = C.PlaneadasTotal,
                incumplidastotal = C.IncumplidasTotal,
                porcentajeTotal = (C.PlaneadasTotal == 0 ? 0 : (Convert.ToDecimal(C.IncumplidasTotal) / Convert.ToDecimal(C.PlaneadasTotal))).ToString("0.00%"),
                pocentajepromotor = (C.PromotorPlanTotal == 0 ? 0 : (Convert.ToDecimal(C.PromotorIncumplio) / Convert.ToDecimal(C.PromotorPlanTotal))).ToString("0.00%"),
                pocentajetecnico = (C.TecnicoPlan == 0 ? 0 : (Convert.ToDecimal(C.TecnicoIncumplio) / Convert.ToDecimal(C.TecnicoPlan))).ToString("0.00%"),
                pocentajeformador = (C.FormadorPlan == 0 ? 0 : (Convert.ToDecimal(C.FormadorIncumplio) / Convert.ToDecimal(C.FormadorPlan))).ToString("0.00%"),
            });

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

            //INicia Especial

            var dataConsultActividad = (from u in db.udf_IncumplimientoActividadesPeriodoZonaActividadList(idPeriodo, Region, Zonas, TipoActividad)
                               group u by new { u.Actividad, u.Zona } into groupZona
                               select new
                               {
                                   actividad = groupZona.Key.Actividad,
                                   zona = groupZona.Key.Zona,
                                   planeadastotal = (groupZona.Sum(s => s.PlaneadasTotal) == null ? 0 : groupZona.Sum(s => s.PlaneadasTotal)),
                                   incumplidastotal = (groupZona.Sum(s => s.IncumplidasTotal) == null ? 0 : groupZona.Sum(s => s.IncumplidasTotal)),
                               }).ToList();

            var headerZona = (from pr in dataConsultActividad
                              select pr.zona).Distinct().ToArray();

            var rowActividad = (from s in dataConsultActividad
                                select s.actividad).Distinct().ToList();
            System.Data.DataTable resultData = new System.Data.DataTable();
            if (dataConsultActividad.Count > 0)
            {

                resultData.Columns.Add("Tipo Actividad");

                foreach (var itemP in headerZona)
                {
                    resultData.Columns.Add(itemP);
                }

                foreach (var row in rowActividad)
                {
                    var resultTipoAct = (from u in dataConsultActividad where u.actividad == row select u).ToList();
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

            //Termina Especial

            var dataConsult = (from u in db.udf_IncumplimientoActividadesPeriodoZonaActividadList(idPeriodo, Region, Zonas, TipoActividad)
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

            var lstPlanSemanalFiguraAct = dataConsult.Select(C => new
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

                // creacion del objeto documento
                Document DocumentoPDF = new Document(PageSize.A4.Rotate());
                PdfWriter writer = PdfWriter.GetInstance(DocumentoPDF, HttpContext.Response.OutputStream);

                ITextEvents evPdf = new ITextEvents();
                evPdf.Title = "INSTITUTO DE ALFABETIZACIÓN Y EDUCACIÓN BÁSICA PARA ADULTOS";
                evPdf.TipoReporte = "INDICADOR INCUMPLIMIENTO DE ACTIVIDADES POR PERIODO";
                evPdf.Coordinacion = "";
                evPdf.Count = lstPlanSemanal.Count();
                writer.PageEvent = evPdf;

                // Le colocamos el título y el autor
                // **Nota: Esto no será visible en el documento
                DocumentoPDF.AddTitle("INDICADOR INCUMPLIMIENTO DE ACTIVIDADES POR PERIODO");
                DocumentoPDF.AddCreator("Inaeba");


                // Abrimos el archivo
                DocumentoPDF.Open();


                // Creamos el tipo de Font que vamos utilizar
                iTextSharp.text.Font _standardFont = new Font(Font.FontFamily.HELVETICA, 8, Font.NORMAL, BaseColor.BLACK);
                iTextSharp.text.Font _standardFontBold = new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.BLACK);


                //Crear tabla pdf
                PdfPTable TblReport = new PdfPTable(8);
                TblReport.WidthPercentage = 100;

                PdfPTable TblReportZona = new PdfPTable(9);
                TblReportZona.WidthPercentage = 100;

                PdfPTable TblReportZonaFigura = new PdfPTable(resultData.Columns.Count);
                TblReportZonaFigura.WidthPercentage = 100;

                PdfPTable TblReportEducando = new PdfPTable(7);
                TblReportEducando.WidthPercentage = 100;

                //Titulo
                PdfPCell cellTitulo = new PdfPCell(new Phrase("REGIÓN", _standardFontBold));
                cellTitulo.BackgroundColor = BaseColor.WHITE;
                cellTitulo.Colspan = 8;
                cellTitulo.HorizontalAlignment = Element.ALIGN_CENTER;
                TblReport.AddCell(cellTitulo);

                //Crear Celdas para el PDF
                PdfPCell cellPeriodo = new PdfPCell(new Phrase("Periodo", _standardFontBold));
                cellPeriodo.BackgroundColor = new BaseColor(156, 195, 230);
                cellPeriodo.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellRegion = new PdfPCell(new Phrase("Región", _standardFontBold));
                cellRegion.BackgroundColor = new BaseColor(156, 195, 230);
                cellRegion.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellActCap = new PdfPCell(new Phrase("Total Actividades planeadas", _standardFontBold));
                cellActCap.BackgroundColor = new BaseColor(156, 195, 230);
                cellActCap.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellActRech = new PdfPCell(new Phrase("Incumplidas", _standardFontBold));
                cellActRech.BackgroundColor = new BaseColor(156, 195, 230);
                cellActRech.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellPorIncumplimiento = new PdfPCell(new Phrase("%Incumplimiento", _standardFontBold));
                cellPorIncumplimiento.BackgroundColor = new BaseColor(156, 195, 230);
                cellPorIncumplimiento.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellPromotor = new PdfPCell(new Phrase("%Incumplimiento Promotor", _standardFontBold));
                cellPromotor.BackgroundColor = new BaseColor(156, 195, 230);
                cellPromotor.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellTecnico = new PdfPCell(new Phrase("%Incumplimiento Tecnico docente", _standardFontBold));
                cellTecnico.BackgroundColor = new BaseColor(156, 195, 230);
                cellTecnico.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellFormador = new PdfPCell(new Phrase("%Incumplimiento Formador", _standardFontBold));
                cellFormador.BackgroundColor = new BaseColor(156, 195, 230);
                cellFormador.HorizontalAlignment = Element.ALIGN_CENTER;

                //añadir las celdas a la tabla
                TblReport.AddCell(cellPeriodo);
                TblReport.AddCell(cellRegion);
                TblReport.AddCell(cellActCap);
                TblReport.AddCell(cellActRech);
                TblReport.AddCell(cellPorIncumplimiento);
                TblReport.AddCell(cellPromotor);
                TblReport.AddCell(cellTecnico);
                TblReport.AddCell(cellFormador);


                //Llenado de la tabla con la informacion
                foreach (var item in lstPlanSemanal)
                {
                    string actCap = (item.planeadastotal == null ? 0 : item.planeadastotal.Value).ToString();
                    string actRech = (item.incumplidastotal == null ? 0 : item.incumplidastotal.Value).ToString();
                    cellPeriodo = new PdfPCell(new Phrase(item.periodo, _standardFont));
                    cellPeriodo.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellRegion = new PdfPCell(new Phrase(item.region, _standardFont));
                    cellRegion.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellActCap = new PdfPCell(new Phrase(actCap, _standardFont));
                    cellActCap.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellActRech = new PdfPCell(new Phrase(actRech, _standardFont));
                    cellActRech.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellPorIncumplimiento = new PdfPCell(new Phrase(item.porcentajeTotal, _standardFont));
                    cellPorIncumplimiento.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellPromotor = new PdfPCell(new Phrase(item.pocentajepromotor, _standardFont));
                    cellPromotor.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellTecnico = new PdfPCell(new Phrase(item.pocentajetecnico, _standardFont));
                    cellTecnico.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellFormador = new PdfPCell(new Phrase(item.pocentajeformador, _standardFont));
                    cellFormador.HorizontalAlignment = Element.ALIGN_CENTER;

                    //Añadimos las celdas a la tabla
                    TblReport.AddCell(cellPeriodo);
                    TblReport.AddCell(cellRegion);
                    TblReport.AddCell(cellActCap);
                    TblReport.AddCell(cellActRech);
                    TblReport.AddCell(cellPorIncumplimiento);
                    TblReport.AddCell(cellPromotor);
                    TblReport.AddCell(cellTecnico);
                    TblReport.AddCell(cellFormador);
                }

                //Añadimos la tabla al documento PDF
                DocumentoPDF.Add(TblReport);

                //Espacio en blanco salto
                PdfPCell cellSalto = new PdfPCell(new Phrase(" ", _standardFontBold));
                cellSalto.BorderWidth = 0;
                cellSalto.BackgroundColor = BaseColor.WHITE;
                cellSalto.Colspan = 9;
                TblReportZona.AddCell(cellSalto);

                //Titulo
                PdfPCell cellTituloZona = new PdfPCell(new Phrase("ZONA", _standardFontBold));
                cellTituloZona.BackgroundColor = BaseColor.WHITE;
                cellTituloZona.Colspan = 9;
                cellTituloZona.HorizontalAlignment = Element.ALIGN_CENTER;
                TblReportZona.AddCell(cellTituloZona);

                //Crear Celdas para el PDF ZONA
                PdfPCell cellPeriodoZona = new PdfPCell(new Phrase("Periodo", _standardFontBold));
                cellPeriodoZona.BackgroundColor = new BaseColor(156, 195, 230);
                cellPeriodoZona.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellRegionZona = new PdfPCell(new Phrase("Región", _standardFontBold));
                cellRegionZona.BackgroundColor = new BaseColor(156, 195, 230);
                cellRegionZona.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellZona = new PdfPCell(new Phrase("Zona", _standardFontBold));
                cellZona.BackgroundColor = new BaseColor(156, 195, 230);
                cellZona.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellEduPreZona = new PdfPCell(new Phrase("Total Actividades planeadas", _standardFontBold));
                cellEduPreZona.BackgroundColor = new BaseColor(156, 195, 230);
                cellEduPreZona.HorizontalAlignment = Element.ALIGN_CENTER;


                PdfPCell cellActIncumZona= new PdfPCell(new Phrase("Incumplidas", _standardFontBold));
                cellActIncumZona.BackgroundColor = new BaseColor(156, 195, 230);
                cellActIncumZona.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellPorIncumplimientoZona = new PdfPCell(new Phrase("%Incumplimiento", _standardFontBold));
                cellPorIncumplimientoZona.BackgroundColor = new BaseColor(156, 195, 230);
                cellPorIncumplimientoZona.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellPromotorZona = new PdfPCell(new Phrase("%Incumplimiento Promotor", _standardFontBold));
                cellPromotorZona.BackgroundColor = new BaseColor(156, 195, 230);
                cellPromotorZona.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellTecnicoZona = new PdfPCell(new Phrase("%Incumplimiento Promotor", _standardFontBold));
                cellTecnicoZona.BackgroundColor = new BaseColor(156, 195, 230);
                cellTecnicoZona.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellFormadorZona = new PdfPCell(new Phrase("%Incumplimiento Promotor", _standardFontBold));
                cellFormadorZona.BackgroundColor = new BaseColor(156, 195, 230);
                cellFormadorZona.HorizontalAlignment = Element.ALIGN_CENTER;

                //añadir las celdas a la tabla
                TblReportZona.AddCell(cellPeriodoZona);
                TblReportZona.AddCell(cellRegionZona);
                TblReportZona.AddCell(cellZona);
                TblReportZona.AddCell(cellEduPreZona);
                TblReportZona.AddCell(cellActIncumZona);
                TblReportZona.AddCell(cellPorIncumplimientoZona);
                TblReportZona.AddCell(cellPromotorZona);
                TblReportZona.AddCell(cellTecnicoZona);
                TblReportZona.AddCell(cellFormadorZona);


                //Llenado de la tabla con la informacion
                foreach (var item in lstPlanSemanalZona)
                {
                    string actCap = (item.planeadastotal == null ? 0 : item.planeadastotal.Value).ToString();
                    string actincum = (item.incumplidastotal == null ? 0 : item.incumplidastotal.Value).ToString();
                    cellPeriodoZona = new PdfPCell(new Phrase(item.periodo, _standardFont));
                    cellPeriodoZona.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellRegionZona = new PdfPCell(new Phrase(item.region, _standardFont));
                    cellRegionZona.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellZona = new PdfPCell(new Phrase(item.zona, _standardFont));
                    cellZona.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellEduPreZona = new PdfPCell(new Phrase(actCap, _standardFont));
                    cellEduPreZona.HorizontalAlignment = Element.ALIGN_CENTER;

                    cellActIncumZona = new PdfPCell(new Phrase(actincum, _standardFont));
                    cellActIncumZona.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellPorIncumplimientoZona = new PdfPCell(new Phrase(item.porcentajeTotal, _standardFont));
                    cellPorIncumplimientoZona.HorizontalAlignment = Element.ALIGN_CENTER;

                    cellPromotorZona = new PdfPCell(new Phrase(item.pocentajepromotor, _standardFont));
                    cellPromotorZona.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellTecnicoZona = new PdfPCell(new Phrase(item.pocentajetecnico, _standardFont));
                    cellTecnicoZona.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellFormadorZona = new PdfPCell(new Phrase(item.pocentajeformador, _standardFont));
                    cellFormadorZona.HorizontalAlignment = Element.ALIGN_CENTER;

                    //Añadimos las celdas a la tabla
                    TblReportZona.AddCell(cellPeriodoZona);
                    TblReportZona.AddCell(cellRegionZona);
                    TblReportZona.AddCell(cellZona);
                    TblReportZona.AddCell(cellEduPreZona);
                    TblReportZona.AddCell(cellActIncumZona);
                    TblReportZona.AddCell(cellPorIncumplimientoZona);
                    TblReportZona.AddCell(cellPromotorZona);
                    TblReportZona.AddCell(cellTecnicoZona);
                    TblReportZona.AddCell(cellFormadorZona);
                }

                //Añadimos la tabla al documento PDF
                DocumentoPDF.Add(TblReportZona);

                //Espacio en blanco salto
                PdfPCell cellSaltoZFigura = new PdfPCell(new Phrase(" ", _standardFontBold));
                cellSaltoZFigura.BorderWidth = 0;
                cellSaltoZFigura.BackgroundColor = BaseColor.WHITE;
                cellSaltoZFigura.Colspan = 8;
                TblReportZonaFigura.AddCell(cellSaltoZFigura);

                //Titulo
                PdfPCell cellTituloZonaFig = new PdfPCell(new Phrase("Tipo Actividad", _standardFontBold));
                cellTituloZonaFig.BackgroundColor = BaseColor.WHITE;
                cellTituloZonaFig.Colspan = resultData.Columns.Count;
                cellTituloZonaFig.HorizontalAlignment = Element.ALIGN_CENTER;
                TblReportZonaFigura.AddCell(cellTituloZonaFig);

                //Crear Celdas para el PDF ZONA
                PdfPCell cellPeriodoZonaFig = new PdfPCell(new Phrase("Tipo Actividad", _standardFontBold));
                cellPeriodoZonaFig.BackgroundColor = new BaseColor(156, 195, 230);
                cellPeriodoZonaFig.HorizontalAlignment = Element.ALIGN_CENTER;
                TblReportZonaFigura.AddCell(cellPeriodoZonaFig);

                foreach (var item in headerZona)
                {
                    PdfPCell cellCOl = new PdfPCell(new Phrase(item, _standardFontBold));
                    cellCOl.BackgroundColor = new BaseColor(156, 195, 230);
                    cellCOl.HorizontalAlignment = Element.ALIGN_CENTER;

                    //añadir las celdas a la tabla
                    TblReportZonaFigura.AddCell(cellCOl);
                }

                foreach (DataRow dr in resultData.Rows)
                {
                    foreach (DataColumn dc in resultData.Columns)
                    {
                        PdfPCell cellDet = new PdfPCell(new Phrase(dr[dc].ToString(), _standardFont));
                        cellDet.HorizontalAlignment = Element.ALIGN_CENTER;

                        TblReportZonaFigura.AddCell(cellDet);
                    }

                    
                }




                //Añadimos la tabla al documento PDF
                DocumentoPDF.Add(TblReportZonaFigura);

                //Espacio en blanco salto
                PdfPCell cellSaltoEducando = new PdfPCell(new Phrase(" ", _standardFontBold));
                cellSaltoEducando.BorderWidth = 0;
                cellSaltoEducando.BackgroundColor = BaseColor.WHITE;
                cellSaltoEducando.Colspan = 7;
                TblReportEducando.AddCell(cellSaltoEducando);

                //Titulo
                PdfPCell cellTituloEducando = new PdfPCell(new Phrase("Tipo Figura", _standardFontBold));
                cellTituloEducando.BackgroundColor = BaseColor.WHITE;
                cellTituloEducando.Colspan = 7;
                cellTituloEducando.HorizontalAlignment = Element.ALIGN_CENTER;
                TblReportEducando.AddCell(cellTituloEducando);

                //Crear Celdas para el PDF ZONA
                PdfPCell cellPeriodoEdu = new PdfPCell(new Phrase("Periodo", _standardFontBold));
                cellPeriodoEdu.BackgroundColor = new BaseColor(156, 195, 230);
                cellPeriodoEdu.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellRegionEdu = new PdfPCell(new Phrase("Región", _standardFontBold));
                cellRegionEdu.BackgroundColor = new BaseColor(156, 195, 230);
                cellRegionEdu.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellZonaEdu = new PdfPCell(new Phrase("Zona", _standardFontBold));
                cellZonaEdu.BackgroundColor = new BaseColor(156, 195, 230);
                cellZonaEdu.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellTipoFigEdu = new PdfPCell(new Phrase("Actividad", _standardFontBold));
                cellTipoFigEdu.BackgroundColor = new BaseColor(156, 195, 230);
                cellTipoFigEdu.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellNomFigEdu = new PdfPCell(new Phrase("%Promotor", _standardFontBold));
                cellNomFigEdu.BackgroundColor = new BaseColor(156, 195, 230);
                cellNomFigEdu.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellNomEdu = new PdfPCell(new Phrase("%Tecnico docente", _standardFontBold));
                cellNomEdu.BackgroundColor = new BaseColor(156, 195, 230);
                cellNomEdu.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellFechaEdu = new PdfPCell(new Phrase("%Formador", _standardFontBold));
                cellFechaEdu.BackgroundColor = new BaseColor(156, 195, 230);
                cellFechaEdu.HorizontalAlignment = Element.ALIGN_CENTER;

                //añadir las celdas a la tabla
                TblReportEducando.AddCell(cellPeriodoEdu);
                TblReportEducando.AddCell(cellRegionEdu);
                TblReportEducando.AddCell(cellZonaEdu);
                TblReportEducando.AddCell(cellTipoFigEdu);
                TblReportEducando.AddCell(cellNomFigEdu);
                TblReportEducando.AddCell(cellNomEdu);
                TblReportEducando.AddCell(cellFechaEdu);

                //Llenado de la tabla con la informacion
                foreach (var item in lstPlanSemanalFiguraAct)
                {
                    cellPeriodoEdu = new PdfPCell(new Phrase(item.periodo, _standardFont));
                    cellPeriodoEdu.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellRegionEdu = new PdfPCell(new Phrase(item.region, _standardFont));
                    cellRegionEdu.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellZonaEdu = new PdfPCell(new Phrase(item.zona, _standardFont));
                    cellZonaEdu.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellTipoFigEdu = new PdfPCell(new Phrase(item.actividad, _standardFont));
                    cellTipoFigEdu.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellNomFigEdu = new PdfPCell(new Phrase(item.promotorIncumplio, _standardFont));
                    cellNomFigEdu.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellNomEdu = new PdfPCell(new Phrase(item.tecnicoIncumplio, _standardFont));
                    cellNomEdu.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellFechaEdu = new PdfPCell(new Phrase(item.formadorIncumplio, _standardFont));
                    cellFechaEdu.HorizontalAlignment = Element.ALIGN_CENTER;

                    //Añadimos las celdas a la tabla
                    TblReportEducando.AddCell(cellPeriodoEdu);
                    TblReportEducando.AddCell(cellRegionEdu);
                    TblReportEducando.AddCell(cellZonaEdu);
                    TblReportEducando.AddCell(cellTipoFigEdu);
                    TblReportEducando.AddCell(cellNomFigEdu);
                    TblReportEducando.AddCell(cellNomEdu);
                    TblReportEducando.AddCell(cellFechaEdu);
                }

                //Añadimos la tabla al documento PDF
                DocumentoPDF.Add(TblReportEducando);

                DocumentoPDF.Close();
                writer.Close();


                HttpContext.Response.ContentType = "pdf/application";
                HttpContext.Response.AddHeader("content-disposition", "attachment;" +
               "filename=IndicadorIncumplimiento" + "_" + DateTime.Now.Date + ".pdf");
                HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                HttpContext.Response.Write(DocumentoPDF);
                HttpContext.Response.End();


            } // if lstProcesoOrd.Count()

        }

        public byte[] GetPDF(string pHTML)
        {
            Byte[] bytes;


            using (var ms = new MemoryStream())
            {
                // Create an iTextSharp Document which is an abstraction of a PDF but **NOT** a PDF
                using (var doc = new Document())
                {
                    // Create a writer that's bound to our PDF abstraction and our stream
                    using (var writer = PdfWriter.GetInstance(doc, ms))
                    {
                        // Open the document for writing
                        doc.Open();
                        string finalHtml = pHTML;

                        // Read your html by database or file here and store it into finalHtml e.g. a string
                        // XMLWorker also reads from a TextReader and not directly from a string
                        using (var srHtml = new StringReader(finalHtml))
                        {
                            // Parse the HTML
                            iTextSharp.tool.xml.XMLWorkerHelper.GetInstance().ParseXHtml(writer, doc, srHtml);
                        }

                        doc.Close();
                    }
                }

                // After all of the PDF "stuff" above is done and closed but **before** we
                // close the MemoryStream, grab all of the active bytes from the stream
                bytes = ms.ToArray();
            }

            return bytes;
        }

    }

    public class ITextEvents : PdfPageEventHelper
    {
        PdfContentByte cb;
        PdfTemplate template;
        BaseFont bf = null;
        DateTime Date = DateTime.Now;

        #region Fields
        private string _title;
        private string _tiporeporte;
        private string _coordinacion;
        private int _count;
        #endregion

        #region Properties
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        public string TipoReporte
        {
            get { return _tiporeporte; }
            set { _tiporeporte = value; }
        }

        public string Coordinacion
        {
            get { return _coordinacion; }
            set { _coordinacion = value; }
        }
        public int Count
        {
            get { return _count; }
            set { _count = value; }
        }


        #endregion

        public override void OnStartPage(PdfWriter writer, Document document)
        {

            base.OnStartPage(writer, document);

            string titulo = Title;
            string tiporeporte = TipoReporte;
            string coordinacion = Coordinacion;
            int count = Count;
            
            Image logo = Image.GetInstance(HttpContext.Current.Server.MapPath("../Content/images/gto_logo.png"));
            logo.ScaleToFit(90.0F, 36.0F);
            logo.Alignment = Image.UNDERLYING + Image.ALIGN_LEFT;
            logo.IndentationRight = 9.0F;
            document.Add(logo);
           
            logo = Image.GetInstance(HttpContext.Current.Server.MapPath("../Content/images/logo-soy-inaeba.png"));
            logo.ScaleToFit(90.0F, 36.0F);
            logo.Alignment = Image.UNDERLYING + Image.ALIGN_RIGHT;
            logo.IndentationRight = 9.0F;
            document.Add(logo);

            Font headerFont = FontFactory.GetFont("Arial", 8);
            bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);

            document.Add(new Phrase(Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine, new Font(bf, 8, Font.NORMAL)));

            addTextPosition(writer, bf, 10, titulo, PdfContentByte.ALIGN_CENTER, document.PageSize.Width / 2, document.PageSize.Height - 48);
            addTextPosition(writer, bf, 10, tiporeporte, PdfContentByte.ALIGN_CENTER, document.PageSize.Width / 2, document.PageSize.Height - 58);
            if (TipoReporte != "REPORTE DE INCIDENCIAS")
            {
                addTextPosition(writer, bf, 10, coordinacion, PdfContentByte.ALIGN_CENTER, document.PageSize.Width / 2, document.PageSize.Height - 68);
            }
            addTextPosition(writer, bf, 8, "FECHA: " + Date.Year.ToString() + "/" + Date.Month.ToString("00") + "/" + Date.Day.ToString("00"), PdfContentByte.ALIGN_CENTER, document.PageSize.Width -98, document.PageSize.Height - 75);
            //addTextPosition(writer, bf, 8, count + " records", PdfContentByte.ALIGN_CENTER, document.PageSize.Width -785, document.PageSize.Height - 75);

            if (document.PageNumber > 1) {
                gridHeader(writer, document, TipoReporte);
            }

        }

        public override void OnOpenDocument(PdfWriter writer, Document document)
        {
               cb = writer.DirectContent;
                template = cb.CreateTemplate(50, 50);
        }


        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            //----------------------------------------------------------------------------------------------           
            base.OnCloseDocument(writer, document);

            BaseFont bf  = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.EMBEDDED);

            template.BeginText();
            template.SetFontAndSize(bf, 7);
            template.SetTextMatrix(0, 0);
            template.ShowText("" + (writer.PageNumber));
            template.EndText();
            //----------------------------------------------------------------------------------------------

        }

        public void addTextPosition(PdfWriter writer, BaseFont font, int fontSize, string texto, int align, float xPos, float yPos)
        {
            PdfContentByte cb =  writer.DirectContent;
            cb.BeginText();
            cb.SetFontAndSize(font, fontSize);
            cb.ShowTextAligned(align, texto, xPos, yPos, 0);
            cb.EndText();
        }

        public void gridHeader(PdfWriter writer, Document document, string reporte)
        {
            if (reporte == "REPORTE DE INCIDENCIAS")
            {
                //Crear tabla pdf
                PdfPTable TblReport = new PdfPTable(8);
                TblReport.WidthPercentage = 100;

                iTextSharp.text.Font _standardFont2 = new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.BLACK);

                //Crear Celdas para el PDF
                PdfPCell cellCZ = new PdfPCell(new Phrase("CZ", _standardFont2));
                cellCZ.BorderWidth = 0;
                cellCZ.BorderWidthBottom = 0.75f;
                cellCZ.BackgroundColor = BaseColor.LIGHT_GRAY;
                cellCZ.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellPeriodo = new PdfPCell(new Phrase("PERIODO", _standardFont2));
                cellPeriodo.BorderWidth = 0;
                cellPeriodo.BorderWidthBottom = 0.75f;
                cellPeriodo.BackgroundColor = BaseColor.LIGHT_GRAY;
                cellPeriodo.HorizontalAlignment = Element.ALIGN_CENTER;


                PdfPCell cellUsuario = new PdfPCell(new Phrase("USUARIO", _standardFont2));
                cellUsuario.BorderWidth = 0;
                cellUsuario.BorderWidthBottom = 0.75f;
                cellUsuario.BackgroundColor = BaseColor.LIGHT_GRAY;
                cellUsuario.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellActividad = new PdfPCell(new Phrase("ACTIVIDAD", _standardFont2));
                cellActividad.BorderWidth = 0;
                cellActividad.BorderWidthBottom = 0.75f;
                cellActividad.BackgroundColor = BaseColor.LIGHT_GRAY;
                cellActividad.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellDescripcion = new PdfPCell(new Phrase("DESCRIPCIÓN", _standardFont2));
                cellDescripcion.BorderWidth = 0;
                cellDescripcion.BorderWidthBottom = 0.75f;
                cellDescripcion.BackgroundColor = BaseColor.LIGHT_GRAY;
                cellDescripcion.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellFecha = new PdfPCell(new Phrase("FECHA", _standardFont2));
                cellFecha.BorderWidth = 0;
                cellFecha.BorderWidthBottom = 0.75f;
                cellFecha.BackgroundColor = BaseColor.LIGHT_GRAY;
                cellFecha.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellComentarios = new PdfPCell(new Phrase("COMENTARIOS CZ", _standardFont2));
                cellComentarios.BorderWidth = 0;
                cellComentarios.BorderWidthBottom = 0.75f;
                cellComentarios.BackgroundColor = BaseColor.LIGHT_GRAY;
                cellComentarios.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellNumeroEmp = new PdfPCell(new Phrase("NÚMERO EMPLEADO", _standardFont2));
                cellNumeroEmp.BorderWidth = 0;
                cellNumeroEmp.BorderWidthBottom = 0.75f;
                cellNumeroEmp.BackgroundColor = BaseColor.LIGHT_GRAY;
                cellNumeroEmp.HorizontalAlignment = Element.ALIGN_CENTER;

                //añadir las celdas a la tabla
                TblReport.AddCell(cellCZ);
                TblReport.AddCell(cellPeriodo);
                TblReport.AddCell(cellUsuario);
                TblReport.AddCell(cellActividad);
                TblReport.AddCell(cellDescripcion);
                TblReport.AddCell(cellFecha);
                TblReport.AddCell(cellComentarios);
                TblReport.AddCell(cellNumeroEmp);

                document.Add(TblReport);


            } else if (reporte == "PLAN DE ACTIVIDADES SEMANAL")
            {
                //Crear tabla pdf
                PdfPTable TblReport = new PdfPTable(16);
                TblReport.WidthPercentage = 100;

                iTextSharp.text.Font _standardFont2 = new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.BLACK);

                //Crear Celdas para el PDF
                PdfPCell cellFecha = new PdfPCell(new Phrase("FECHA", _standardFont2));
                cellFecha.BorderWidth = 0;
                cellFecha.BorderWidthBottom = 0.75f;
                cellFecha.BackgroundColor = BaseColor.LIGHT_GRAY;
                cellFecha.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellLugar = new PdfPCell(new Phrase("LUGAR A VISITAR", _standardFont2));
                cellLugar.BorderWidth = 0;
                cellLugar.BorderWidthBottom = 0.75f;
                cellLugar.BackgroundColor = BaseColor.LIGHT_GRAY;
                cellLugar.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellHora = new PdfPCell(new Phrase("HORA INICIO", _standardFont2));
                cellHora.BorderWidth = 0;
                cellHora.BorderWidthBottom = 0.75f;
                cellHora.BackgroundColor = BaseColor.LIGHT_GRAY;
                cellHora.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellHoraFin = new PdfPCell(new Phrase("HORA FIN", _standardFont2));
                cellHoraFin.BorderWidth = 0;
                cellHoraFin.BorderWidthBottom = 0.75f;
                cellHoraFin.BackgroundColor = BaseColor.LIGHT_GRAY;
                cellHoraFin.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellActividad = new PdfPCell(new Phrase("ACTIVIDADES A REALIZAR (ESPECIFICAR)", _standardFont2));
                cellActividad.BorderWidth = 0;
                cellActividad.BorderWidthBottom = 0.75f;
                cellActividad.BackgroundColor = BaseColor.LIGHT_GRAY;
                cellActividad.HorizontalAlignment = Element.ALIGN_CENTER;
                cellActividad.Colspan = 4;

                PdfPCell cellObjetivo = new PdfPCell(new Phrase("OBJETIVO (RESULTADO)", _standardFont2));
                cellObjetivo.BorderWidth = 0;
                cellObjetivo.BorderWidthBottom = 0.75f;
                cellObjetivo.BackgroundColor = BaseColor.LIGHT_GRAY;
                cellObjetivo.HorizontalAlignment = Element.ALIGN_CENTER;
                cellObjetivo.Colspan = 5;

                PdfPCell cellComentarios = new PdfPCell(new Phrase("COMENTARIOS CZ", _standardFont2));
                cellComentarios.BorderWidth = 0;
                cellComentarios.BorderWidthBottom = 0.75f;
                cellComentarios.BackgroundColor = BaseColor.LIGHT_GRAY;
                cellComentarios.HorizontalAlignment = Element.ALIGN_CENTER;
                cellComentarios.Colspan = 3;


                //añadir las celdas a la tabla
                TblReport.AddCell(cellFecha);
                TblReport.AddCell(cellLugar);
                TblReport.AddCell(cellHora);
                TblReport.AddCell(cellHoraFin);
                TblReport.AddCell(cellActividad);
                TblReport.AddCell(cellObjetivo);
                TblReport.AddCell(cellComentarios);

                document.Add(TblReport);
            } else if (reporte == "ACTIVIDADES REALIZADAS")
            {
                //Crear tabla pdf
                PdfPTable TblReport = new PdfPTable(9);
                TblReport.WidthPercentage = 100;

                iTextSharp.text.Font _standardFont2 = new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.BLACK);

                //Crear Celdas para el PDF
                PdfPCell cellDia = new PdfPCell(new Phrase("DIA", _standardFont2));
                //cellFecha.BorderWidth = 0;
                cellDia.BackgroundColor = new BaseColor(156, 195, 230);
                cellDia.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellFecha = new PdfPCell(new Phrase("FECHA", _standardFont2));
                //cellHora.BorderWidth = 0;
                cellFecha.BorderWidthBottom = 0.75f;
                cellFecha.BackgroundColor = new BaseColor(156, 195, 230);
                cellFecha.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellHora = new PdfPCell(new Phrase("HORA INICIO", _standardFont2));
                //cellActividad.BorderWidth = 0;
                cellHora.BackgroundColor = new BaseColor(156, 195, 230);
                cellHora.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellHoraFin = new PdfPCell(new Phrase("HORA FIN", _standardFont2));
                //cellActividad.BorderWidth = 0;
                cellHoraFin.BackgroundColor = new BaseColor(156, 195, 230);
                cellHoraFin.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellActividad = new PdfPCell(new Phrase("ACTIVIDAD", _standardFont2));
                //cellObjetivo.BorderWidth = 0;
                cellActividad.BorderWidthBottom = 0.75f;
                cellActividad.BackgroundColor = new BaseColor(156, 195, 230);
                cellActividad.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellDescActividad = new PdfPCell(new Phrase("DESCRIPCIÓN ACTIVIDAD", _standardFont2));
                //cellObjetivo.BorderWidth = 0;
                cellDescActividad.BackgroundColor = new BaseColor(156, 195, 230);
                cellDescActividad.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellIncidencia = new PdfPCell(new Phrase("INCIDENCIA", _standardFont2));
                //cellObjetivo.BorderWidth = 0;
                cellIncidencia.BackgroundColor = new BaseColor(156, 195, 230);
                cellIncidencia.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellEstatus = new PdfPCell(new Phrase("ESTATUS ACTIVIDAD", _standardFont2));
                //cellObjetivo.BorderWidth = 0;
                cellEstatus.BackgroundColor = new BaseColor(156, 195, 230);
                cellEstatus.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellComentarios = new PdfPCell(new Phrase("COMENTARIOS CZ", _standardFont2));
                //cellObjetivo.BorderWidth = 0;
                cellComentarios.BackgroundColor = new BaseColor(156, 195, 230);
                cellComentarios.HorizontalAlignment = Element.ALIGN_CENTER;

                //añadir las celdas a la tabla
                TblReport.AddCell(cellDia);
                TblReport.AddCell(cellFecha);
                TblReport.AddCell(cellHora);
                TblReport.AddCell(cellHoraFin);
                TblReport.AddCell(cellActividad);
                TblReport.AddCell(cellDescActividad);
                TblReport.AddCell(cellIncidencia);
                TblReport.AddCell(cellEstatus);
                TblReport.AddCell(cellComentarios);

                document.Add(TblReport);
            }
        }

    }



}
