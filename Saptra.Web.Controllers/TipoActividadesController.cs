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
        private SipaeEntities dbSipae = new SipaeEntities();

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
                                    where cat.EstatusId == ( idEstatus == null ? cat.EstatusId : idEstatus)
                              select new
                              {
                                  id = cat.TipoActividadId,
                                  nombre = cat.NombreActividad,
                                  requiereck = (cat.RequiereCheckIn == true ? "Si" : "No" ),
                                  estatus = cat.cEstatus.NombreEstatus
                              })
                              .OrderBy(cat => cat.id)
                              .ToList();

                return (Json(result, JsonRequestBehavior.AllowGet));

            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = "Error al obtener la información" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult Nuevo()
        {
            var objTiposActividades= new cTipoActividades();
            ViewBag.Titulo = "Nueva actividad";
            return PartialView("_Nuevo", objTiposActividades);
        }

        [HttpPost]
        public JsonResult Nuevo(cTipoActividades pobjModelo)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var validacion = 0;

                    validacion = (from c in db.cTipoActividades
                                  where c.NombreActividad == (pobjModelo.NombreActividad.TrimStart()).TrimEnd()
                                  select c).Count();
                    if (validacion == 0)
                    {
                        pobjModelo.FechaCreacion = DateTime.Now;
                        pobjModelo.EstatusId = 5;
                        pobjModelo.DescripcionActividad = (pobjModelo.NombreActividad.TrimStart()).TrimEnd();
                        db.cTipoActividades.Add(pobjModelo);
                        db.SaveChanges();
                        return Json(new { Success = true, id = pobjModelo.TipoActividadId, Message = "guardado correctamente " });
                    }
                    else
                    {
                        return Json(new { Success = false, Message = "Actividad existente." });
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
                var validacion = 0;

                validacion = (from c in db.cTipoActividades
                              where c.NombreActividad == (pobjModelo.NombreActividad.TrimStart()).TrimEnd()
                              select c).Count();
                if (validacion == 0)
                {
                    //Actualiza
                    var dbTemp = result.First();
                    dbTemp.NombreActividad = (pobjModelo.NombreActividad.TrimStart()).TrimEnd();
                    dbTemp.DescripcionActividad = (pobjModelo.NombreActividad.TrimStart()).TrimEnd();
                    dbTemp.RequiereCheckIn = pobjModelo.RequiereCheckIn;
                    dbTemp.EstatusId = pobjModelo.EstatusId;
                    db.SaveChanges();
                    return Json(new { Success = true, id = pobjModelo.TipoActividadId, Message = "actualizada correctamente " });
                }
                else
                {
                    return Json(new { Success = false, Message = "Actividad existente." });
                }

            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = "Error al guardar la información" });
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
                return Json(new { Success = false, Message = "Error al eliminar la información" });
            }
        }

        public JsonResult CargarVehiculosSipae()
        {
            try
            {
                var result = (from v in dbSipae.Vehiculos
                              where v.Estatus == true
                              select new
                              {
                                  id = v.VehiculoId,
                                  nombre = v.Marca + " " + v.Modelo + " " + v.Anio + " " + v.Color + " (" + v.Matricula + ")"
                              })
                          .OrderBy(cat => cat.id)
                          .ToList();

                return (Json(result, JsonRequestBehavior.AllowGet));

            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = "Error al obtener la información" }, JsonRequestBehavior.AllowGet);
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
                return Json(new { Success = false, Message = "Error al obtener la información" }, JsonRequestBehavior.AllowGet);
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