///Propósito:Controlador para la administracion de periodos
///Fecha creación: 03/Octubre/18
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
using System.Globalization;
using System.Text.RegularExpressions;


using Saptra.Web.Data;
using Saptra.Web.Models;
using Saptra.Web.Utils;

namespace Sispro.Web.Controllers
{
    public class PeriodosController : Controller
    {
        private Inaeba_SaptraEntities db = new Inaeba_SaptraEntities();

        public ActionResult Periodos()
        {
            return View();
        }

        [HttpGet]
        public JsonResult CargarPeriodos(int? id, int? idEstatus)
        {
            try
            {
                DateTime fecha = Convert.ToDateTime(DateTime.Now.ToShortDateString());
                var result = (from cat in db.cPeriodos
                              where cat.EstatusId == (idEstatus == null ? cat.EstatusId : idEstatus)
                              select cat)
                              .OrderBy(cat => cat.FechaInicio)
                              .ToList();

                var lstPeriodos = result.Select(cat => new
                {
                    id = cat.PeriodoId,
                    nombre = cat.DecripcionPeriodo,
                    fechaInicio = cat.FechaInicio.ToString("dd/MM/yyyy"),
                    fechaFin = cat.FechaFin.ToString("dd/MM/yyyy"),
                    estatus = cat.cEstatus.NombreEstatus
                });

                return (Json(lstPeriodos, JsonRequestBehavior.AllowGet));

            }
#pragma warning disable CS0168 // La variable 'exp' se ha declarado pero nunca se usa
            catch (Exception exp)
#pragma warning restore CS0168 // La variable 'exp' se ha declarado pero nunca se usa
            {
                return Json(new { Success = false, Message = "Error al obtener la información" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult CargarPeriodoActual(int? id, int? idEstatus)
        {
            try
            {
                DateTime fecha = Convert.ToDateTime(DateTime.Now.ToShortDateString());
                var result = (from cat in db.cPeriodos
                              where cat.EstatusId == (idEstatus == null ? cat.EstatusId : idEstatus)
                              && (fecha >= cat.FechaInicio && fecha <= cat.FechaFin)
                              select new
                              {
                                  id = cat.PeriodoId,
                                  nombre = cat.DecripcionPeriodo,
                              })
                              .OrderBy(cat => cat.id)
                              .ToList();

                return (Json(result, JsonRequestBehavior.AllowGet));

            }
#pragma warning disable CS0168 // La variable 'exp' se ha declarado pero nunca se usa
            catch (Exception exp)
#pragma warning restore CS0168 // La variable 'exp' se ha declarado pero nunca se usa
            {
                return Json(new { Success = false, Message = "Error al obtener la información" }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        public ActionResult Nuevo()
        {
            var objPeriodos = new cPeriodos();
            ViewBag.Titulo = "Nuevo Periodo";
            objPeriodos.FechaInicio.ToString("dd/MM/yyyy");
            objPeriodos.FechaFin.ToString("dd/MM/yyyy");
            return PartialView("_Nuevo", objPeriodos);
        }

        [HttpPost]
        public JsonResult Nuevo(cPeriodos pobjModelo, string fIni, string fFin)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    DateTime feIni = DateTime.ParseExact(fIni, "dd/MM/yyyy", null);
                    DateTime feFin = DateTime.ParseExact(fFin, "dd/MM/yyyy", null);
                    string diaInicio = feIni.ToString("dd");
                    string diaFin = feFin.ToString("dd");
                    string mesInicio = feIni.ToString("MMM", new CultureInfo("es-ES"));
                    string mesFin = feFin.ToString("MMM", new CultureInfo("es-ES"));
                    string anio = feFin.ToString("yyyy");

                    pobjModelo.FechaCreacion = DateTime.Now;
                    pobjModelo.EstatusId = 5;
                    pobjModelo.DecripcionPeriodo = diaInicio + " " + mesInicio + " - " + diaFin + " " + mesFin + " " + anio;
                    pobjModelo.FechaInicio = feIni;
                    pobjModelo.FechaFin = feFin;
                    db.cPeriodos.Add(pobjModelo);
                    db.SaveChanges();

                    return Json(new { Success = true, id = pobjModelo.PeriodoId, Message = "guardado correctamente " });
                }
#pragma warning disable CS0168 // La variable 'exp' se ha declarado pero nunca se usa
                catch (Exception exp)
#pragma warning restore CS0168 // La variable 'exp' se ha declarado pero nunca se usa
                {
                    return Json(new { Success = false, Message = "Error al guardar la información" });
                }
            }

            return Json(new { Success = false, Message = "Informacion incompleta" });
        }

        [HttpGet]
        public ActionResult Actualizar(int id)
        {
            var objPeriodo = db.cPeriodos.Find(id);
            ViewBag.Titulo = "Actualizar Periodo";
            return PartialView("_Actualizar", objPeriodo);
        }

        [HttpPost]
        public JsonResult Actualizar(cPeriodos pobjModelo)
        {
            try
            {
                string diaInicio = pobjModelo.FechaInicio.ToString("dd");
                string diaFin = pobjModelo.FechaFin.ToString("dd");
                string mesInicio = pobjModelo.FechaInicio.ToString("MMM", new CultureInfo("es-ES"));
                string mesFin = pobjModelo.FechaFin.ToString("MMM", new CultureInfo("es-ES"));
                string anio = pobjModelo.FechaFin.ToString("yyyy");

                var result = (from ps in db.cPeriodos
                              where ps.PeriodoId == pobjModelo.PeriodoId
                              select ps).ToList();

                //Actualiza
                var dbTemp = result.First();
                dbTemp.FechaInicio = pobjModelo.FechaInicio;
                dbTemp.FechaFin = pobjModelo.FechaFin;
                dbTemp.DecripcionPeriodo = diaInicio + " " + mesInicio + " - " + diaFin + " " + mesFin + " " + anio;
                dbTemp.EstatusId = pobjModelo.EstatusId;
                db.SaveChanges();


                return Json(new { Success = true, id = pobjModelo.PeriodoId, Message = "actualizada correctamente " });

            }
#pragma warning disable CS0168 // La variable 'exp' se ha declarado pero nunca se usa
            catch (Exception exp)
#pragma warning restore CS0168 // La variable 'exp' se ha declarado pero nunca se usa
            {
                return Json(new { Success = false, Message = "Error al guardar la información" });
            }
        }

        [HttpPost]
        public JsonResult Borrar(int id)
        {
            try
            {
                var result = (from usu in db.cPeriodos
                              where usu.PeriodoId == (id)
                              select usu).FirstOrDefault();

                result.EstatusId = 6;
                db.SaveChanges();

                return Json(new { Success = true, Message = "Se borro correctamente el periodo " });
            }
#pragma warning disable CS0168 // La variable 'exp' se ha declarado pero nunca se usa
            catch (Exception exp)
#pragma warning restore CS0168 // La variable 'exp' se ha declarado pero nunca se usa
            {
                return Json(new { Success = false, Message = "Error al eliminar la información" });
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