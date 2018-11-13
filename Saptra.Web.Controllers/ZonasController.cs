///Propósito:Controlador para la administracion de zonas
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
    public class ZonasController : Controller
    {
        private Inaeba_SaptraEntities db = new Inaeba_SaptraEntities();
        
        public ActionResult Zonas()
        {
            return View();
        }

        [HttpGet]
        public JsonResult CargarZonas(int? id, int? idEstatus)
        {
            try
            {
                var result = (from cat in db.cCoordinacionesZona
                              where cat.EstatusId == 5
                              select new
                              {
                                  id = cat.CoordinacionZonaId,
                                  nombre = cat.DescripcionCoordinacionZona,
                                  region = cat.CoordinacionRegionId == null ? "" : cat.cCoordinacionesRegion.DescripcionCoordinacionRegion,
                                  estatus = cat.cEstatus.NombreEstatus
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

        [HttpGet]
        public JsonResult CargarZonasPorRol(int? id, int? idEstatus, int idUsuario)
        {
            try
            {
                var usuario = (from u in db.mUsuarios
                               where u.UsuarioId == idUsuario
                               select u).FirstOrDefault();

                if (usuario.RolId == Constantes.CONST_ROL_COORDINADOR_REGIONAL_ID)
                {
                    int idregion = usuario.mCoordinacionRegionZonaUsuario.First().CoordinacionRegionId;
                    var result = (from cat in db.cCoordinacionesZona
                                  where cat.EstatusId == 5 && cat.CoordinacionRegionId == idregion
                                  select new
                                  {
                                      id = cat.CoordinacionZonaId,
                                      nombre = cat.DescripcionCoordinacionZona,
                                      estatus = cat.cEstatus.NombreEstatus
                                  })
                              .OrderBy(cat => cat.nombre)
                              .ToList();

                    return (Json(result, JsonRequestBehavior.AllowGet));
                }
                else if (usuario.RolId == Constantes.CONST_ROL_COORDINADOR_ZONA_ID)
                {
                    int idZona= usuario.mCoordinacionZonaUsuario.First().CoordinacionZonaId;

                    var result = (from cat in db.cCoordinacionesZona
                                  where cat.EstatusId == 5 && cat.CoordinacionZonaId == idZona
                                  select new
                                  {
                                      id = cat.CoordinacionZonaId,
                                      nombre = cat.DescripcionCoordinacionZona,
                                      estatus = cat.cEstatus.NombreEstatus
                                  })
                            .OrderBy(cat => cat.nombre)
                            .ToList();

                    return (Json(result, JsonRequestBehavior.AllowGet));
                }
                else
                {
                    var result = (from cat in db.cCoordinacionesZona
                                  where cat.EstatusId == 5
                                  select new
                                  {
                                      id = cat.CoordinacionZonaId,
                                      nombre = cat.DescripcionCoordinacionZona,
                                      estatus = cat.cEstatus.NombreEstatus
                                  })
                 .OrderBy(cat => cat.nombre)
                 .ToList();

                    return (Json(result, JsonRequestBehavior.AllowGet));
                }

            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = "Error al obtener la información" }, JsonRequestBehavior.AllowGet);
            }
        }

        //Obtiene zonas separadas por coma
        public string ZonasPorRol(int idUsuario)
        {

            var usuario = (from u in db.mUsuarios
                           where u.UsuarioId == idUsuario
                           select u).FirstOrDefault();

            if (usuario.RolId == Constantes.CONST_ROL_COORDINADOR_REGIONAL_ID)
            {
                int idregion = usuario.mCoordinacionRegionZonaUsuario.First().CoordinacionRegionId;
                var result = string.Join(",", (from cat in db.cCoordinacionesZona
                                               where cat.EstatusId == 5 && cat.CoordinacionRegionId == idregion
                                               select cat.CoordinacionZonaId).ToList());

                return result;
            }
            else if (usuario.RolId == Constantes.CONST_ROL_COORDINADOR_ZONA_ID)
            {
                int idZona = usuario.mCoordinacionZonaUsuario1.First().CoordinacionZonaId;

                var result = string.Join(",", (from cat in db.cCoordinacionesZona
                                               where cat.EstatusId == 5 && cat.CoordinacionZonaId == idZona
                                               select cat.CoordinacionZonaId).ToList());

                return result;
            }
            else
            {
                var result = string.Join(",", (from cat in db.cCoordinacionesZona
                                               where cat.EstatusId == 5
                                               select cat.CoordinacionZonaId).ToList());

                return result;
            }
        }

        [HttpGet]
        public JsonResult CargarZonasRegion(int? id)
        {
            try
            {
                var result = (from cat in db.cCoordinacionesZona
                              where cat.EstatusId == 5 && cat.CoordinacionRegionId == (id == null ? cat.CoordinacionRegionId  : id)
                              select new
                              {
                                  id = cat.CoordinacionZonaId,
                                  nombre = cat.DescripcionCoordinacionZona,
                                  region = cat.CoordinacionRegionId == null ? "" : cat.cCoordinacionesRegion.DescripcionCoordinacionRegion,
                                  estatus = cat.cEstatus.NombreEstatus
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

        [HttpGet]
        public ActionResult Nuevo()
        {
            var objZona = new cCoordinacionesZona();
            ViewBag.Titulo = "Nueva Zona";
            return PartialView("_Nuevo", objZona);
        }

        [HttpPost]
        public JsonResult Nuevo(cCoordinacionesZona pobjModelo)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    pobjModelo.FechaCreacion = DateTime.Now;
                    pobjModelo.EstatusId = pobjModelo.EstatusId;
                    pobjModelo.DescripcionCoordinacionZona = pobjModelo.DescripcionCoordinacionZona;
                    pobjModelo.CoordinacionRegionId = pobjModelo.CoordinacionRegionId;
                    db.cCoordinacionesZona.Add(pobjModelo);
                    db.SaveChanges();

                    return Json(new { Success = true, id = pobjModelo.CoordinacionZonaId, Message = "guardado correctamente " });
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
            var objZona= db.cCoordinacionesZona.Find(id);
            ViewBag.Titulo = "Actualizar Zona";
            return PartialView("_Actualizar", objZona);
        }

        [HttpPost]
        public JsonResult Actualizar(cCoordinacionesZona pobjModelo)
        {
            try
            {
                var result = (from ps in db.cCoordinacionesZona
                              where ps.CoordinacionZonaId == pobjModelo.CoordinacionZonaId
                              select ps).ToList();

                //Actualiza
                var dbTemp = result.First();
                dbTemp.DescripcionCoordinacionZona = pobjModelo.DescripcionCoordinacionZona;
                dbTemp.CoordinacionRegionId = pobjModelo.CoordinacionRegionId;
                dbTemp.EstatusId = pobjModelo.EstatusId;
                db.SaveChanges();


                return Json(new { Success = true, id = pobjModelo.CoordinacionZonaId, Message = "actualizada correctamente " });

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
                var result = (from usu in db.cCoordinacionesZona
                              where usu.CoordinacionZonaId == (id)
                              select usu).FirstOrDefault();

                result.EstatusId = 6;
                db.SaveChanges();

                return Json(new { Success = true, Message = "Se borro correctamente la Zona " });
            }
            catch (Exception exp)
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