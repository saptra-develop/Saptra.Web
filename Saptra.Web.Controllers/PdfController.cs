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
                fecha = "03/11/2018",/*C.FechaActividad.Value.ToString("MM/dd/yyyy")*/
                comentarios = C.ComentariosRechazo,
                C.CoordinacionZona
            });

            if (lstPlanSemanal.Count() > 0)
            {

                // creacion del objeto documento
                Document DocumentoPDF = new Document(PageSize.A4.Rotate());
                PdfWriter writer = PdfWriter.GetInstance(DocumentoPDF, HttpContext.Response.OutputStream);

                ITextEvents evPdf = new ITextEvents();
                evPdf.Title = "INSTITUTO DE ALFABETIZACIÓN Y EDUCACIÓN BÁSICA PARA ADULTOS";
                evPdf.TipoReporte = "REPORTE DE INCIDENCIAS";
                evPdf.Coordinacion = "COORDINACIÓN DE ZONA: " + lstPlanSemanal.FirstOrDefault().CoordinacionZona;
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
                PdfPTable TblReport = new PdfPTable(6);
                TblReport.WidthPercentage = 100;     

                //Crear Celdas para el PDF
                PdfPCell cellPeriodo = new PdfPCell(new Phrase("PERIODO", _standardFontBold));
                //cellPeriodo.BorderWidth = 1;
                //cellPeriodo.BorderWidthBottom = 0.75f;
                cellPeriodo.BackgroundColor = BaseColor.LIGHT_GRAY;
                cellPeriodo.HorizontalAlignment = Element.ALIGN_CENTER;


                PdfPCell cellUsuario = new PdfPCell(new Phrase("USUARIO", _standardFontBold));
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

                //añadir las celdas a la tabla
                TblReport.AddCell(cellPeriodo);
                TblReport.AddCell(cellUsuario);
                TblReport.AddCell(cellActividad);
                TblReport.AddCell(cellDescripcion);
                TblReport.AddCell(cellFecha);
                TblReport.AddCell(cellComentarios);

                //Llenado de la tabla con la informacion
                foreach (var item in lstPlanSemanal)
                {
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
                   

                    //Añadimos las celdas a la tabla
                    TblReport.AddCell(cellPeriodo);
                    TblReport.AddCell(cellUsuario);
                    TblReport.AddCell(cellActividad);
                    TblReport.AddCell(cellDescripcion);
                    TblReport.AddCell(cellFecha);
                    TblReport.AddCell(cellComentarios);
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
                                 select cat).ToList();

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
                comentarios = (cat.ComentariosNoValidacion == null ? "" : cat.ComentariosNoValidacion) + (cat.ComentariosRechazo == null ? "" : ", " + cat.ComentariosRechazo)
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
                                 select cat).ToList();

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

                PdfPCell cellIncidencia = new PdfPCell(new Phrase("INCIDENCIA", _standardFontBold));
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
            addTextPosition(writer, bf, 10, coordinacion, PdfContentByte.ALIGN_CENTER, document.PageSize.Width / 2, document.PageSize.Height - 68);
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
                PdfPTable TblReport = new PdfPTable(6);
                TblReport.WidthPercentage = 100;

                iTextSharp.text.Font _standardFont2 = new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.BLACK);

                 //Crear Celdas para el PDF
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

                //añadir las celdas a la tabla
                TblReport.AddCell(cellPeriodo);
                TblReport.AddCell(cellUsuario);
                TblReport.AddCell(cellActividad);
                TblReport.AddCell(cellDescripcion);
                TblReport.AddCell(cellFecha);
                TblReport.AddCell(cellComentarios);

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
