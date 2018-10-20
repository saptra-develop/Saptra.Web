///Propósito:Controlador para la administracion de tipos de actividades
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


using Saptra.Web.Data;
using Saptra.Web.Models;
using Saptra.Web.Utils;

namespace Sispro.Web.Controllers
{
    public class TipoActividadesController : Controller
    {
        private Inaeba_SaptraEntities db = new Inaeba_SaptraEntities();
        
        public ActionResult Actividades()
        {
            return View();
        }

        [HttpGet]
        public JsonResult CargarTipoActividades(int? id, int? idEstatus)
        {
            try
            {
                var result = (from cat in db.cTipoActividades
                              where cat.EstatusId == (idEstatus == null ? cat.EstatusId : idEstatus)
                              select new
                              {
                                  id = cat.TipoActividadId,
                                  nombre = cat.NombreActividad,
                                  requiereck = (cat.RequiereCheckIn == true ? "Si" : "No" ),
                                  estatus = cat.cEstatus.NombreEstatus
                              })
                              .OrderBy(cat => cat.nombre)
                              .ToList();

                return (Json(result, JsonRequestBehavior.AllowGet));

            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = exp.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult Nuevo()
        {
            var objTiposActividades= new cTipoActividades();
            ViewBag.Titulo = "Nuevo actividad";
            return PartialView("_Nuevo", objTiposActividades);
        }

        [HttpPost]
        public JsonResult Nuevo(cTipoActividades pobjModelo)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    pobjModelo.FechaCreacion = DateTime.Now;
                    pobjModelo.EstatusId = pobjModelo.EstatusId;
                    pobjModelo.DescripcionActividad = pobjModelo.NombreActividad;
                    db.cTipoActividades.Add(pobjModelo);
                    db.SaveChanges();

                    return Json(new { Success = true, id = pobjModelo.TipoActividadId, Message = "guardado correctamente " });
                }
                catch (Exception exp)
                {
                    return Json(new { Success = false, Message = exp.Message });
                }
            }

            return Json(new { Success = false, Message = "Informacion incompleta" });
        }

        [HttpGet]
        public ActionResult Actualizar(int id)
        {
            var objTipoActividad = db.cTipoActividades.Find(id);
            ViewBag.Titulo = "Actualizar actividad";
            return PartialView("_Actualizar", objTipoActividad);
        }

        [HttpPost]
        public JsonResult Actualizar(cTipoActividades pobjModelo)
        {
            try
            {
                var result = (from ps in db.cTipoActividades
                              where ps.TipoActividadId == pobjModelo.TipoActividadId
                              select ps).ToList();

                //Actualiza
                var dbTemp = result.First();
                dbTemp.NombreActividad = pobjModelo.NombreActividad;
                dbTemp.DescripcionActividad = pobjModelo.NombreActividad;
                dbTemp.RequiereCheckIn = pobjModelo.RequiereCheckIn;
                dbTemp.EstatusId = pobjModelo.EstatusId;
                db.SaveChanges();


                return Json(new { Success = true, id = pobjModelo.TipoActividadId, Message = "actualizada correctamente " });

            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = exp.Message });
            }
        }

        [HttpPost]
        public JsonResult Borrar(int id)
        {
            try
            {
                var result = (from usu in db.cTipoActividades
                              where usu.TipoActividadId == (id)
                              select usu).FirstOrDefault();

                result.EstatusId = 6;
                db.SaveChanges();

                return Json(new { Success = true, Message = "Se borro correctamente la actividad " });
            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = exp.Message });
            }
        }

        [HttpGet]
        public JsonResult CargarTipoActividadesSinCheckIn(int? id, int? idEstatus)
        {
            try
            {
                var result = (from cat in db.cTipoActividades
                              where cat.EstatusId == (idEstatus == null ? cat.EstatusId : idEstatus)
                              && cat.RequiereCheckIn == false
                              select new
                              {
                                  id = cat.TipoActividadId,
                                  nombre = cat.NombreActividad.ToUpper()
                              })
                              .OrderBy(cat => cat.nombre)
                              .ToList();

                return (Json(result, JsonRequestBehavior.AllowGet));

            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = exp.Message }, JsonRequestBehavior.AllowGet);
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