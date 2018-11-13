///Propósito:Controlador para la administracion de regiones
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
    public class RegionesController : Controller
    {
        private Inaeba_SaptraEntities db = new Inaeba_SaptraEntities();
        
        public ActionResult Regiones()
        {
            return View();
        }

        [HttpGet]
        public JsonResult CargarRegiones(int? id, int? idEstatus)
        {
            try
            {
                var result = (from cat in db.cCoordinacionesRegion
                              where cat.EstatusId == 5
                              select new
                              {
                                  id = cat.CoordinacionRegionId,
                                  nombre = cat.DescripcionCoordinacionRegion,
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
        public JsonResult CargarRegionesPorRol(int? id, int? idEstatus, int idUsuario)
        {
            try
            {
                var usuario = (from u in db.mUsuarios
                               where u.UsuarioId == idUsuario
                               select u).FirstOrDefault();
                if (usuario.RolId == Constantes.CONST_ROL_COORDINADOR_REGIONAL_ID )
                {
                    var result = (from cat in db.cCoordinacionesRegion
                                  join c in db.mCoordinacionRegionZonaUsuario on cat.CoordinacionRegionId equals c.CoordinacionRegionId
                                  where cat.EstatusId == 5 && c.UsuarioJefeRegionId == idUsuario
                                  select new
                                  {
                                      id = cat.CoordinacionRegionId,
                                      nombre = cat.DescripcionCoordinacionRegion,
                                      estatus = cat.cEstatus.NombreEstatus
                                  })
                              .OrderBy(cat => cat.nombre)
                              .ToList();

                    return (Json(result, JsonRequestBehavior.AllowGet));
                } else if (usuario.RolId == Constantes.CONST_ROL_COORDINADOR_ZONA_ID)
                {
                    var czona = (from c in db.mCoordinacionZonaUsuario
                                 join cc in db.cCoordinacionesZona on c.CoordinacionZonaId equals cc.CoordinacionZonaId
                                 where c.UsuarioId == idUsuario
                                 select cc).FirstOrDefault();

                    var result = (from cat in db.cCoordinacionesRegion
                                  where cat.EstatusId == 5 && cat.CoordinacionRegionId == czona.CoordinacionRegionId
                                  select new
                                  {
                                      id = cat.CoordinacionRegionId,
                                      nombre = cat.DescripcionCoordinacionRegion,
                                      estatus = cat.cEstatus.NombreEstatus
                                  })
                           .OrderBy(cat => cat.nombre)
                           .ToList();

                    return (Json(result, JsonRequestBehavior.AllowGet));
                }else
                {
                    var result = (from cat in db.cCoordinacionesRegion
                                  where cat.EstatusId == 5
                                  select new
                                  {
                                      id = cat.CoordinacionRegionId,
                                      nombre = cat.DescripcionCoordinacionRegion,
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

        //Obtiene regiones separadas por coma
        public string RegionesPorRol(int idUsuario)
        {
            var usuario = (from u in db.mUsuarios
                           where u.UsuarioId == idUsuario
                           select u).FirstOrDefault();
            if (usuario.RolId == Constantes.CONST_ROL_COORDINADOR_REGIONAL_ID)
            {
                var result = string.Join(",", (from cat in db.cCoordinacionesRegion
                              join c in db.mCoordinacionRegionZonaUsuario on cat.CoordinacionRegionId equals c.CoordinacionRegionId
                              where cat.EstatusId == 5 && c.UsuarioJefeRegionId == idUsuario
                              select cat.CoordinacionRegionId).ToList());

                return result;
            }
            else if (usuario.RolId == Constantes.CONST_ROL_COORDINADOR_ZONA_ID)
            {
                var czona = (from c in db.mCoordinacionZonaUsuario
                             join cc in db.cCoordinacionesZona on c.CoordinacionZonaId equals cc.CoordinacionZonaId
                             where c.UsuarioId == idUsuario
                             select cc).FirstOrDefault();

                var result = string.Join(",", (from cat in db.cCoordinacionesRegion
                              where cat.EstatusId == 5 && cat.CoordinacionRegionId == czona.CoordinacionRegionId
                              select cat.CoordinacionRegionId).ToList());

                return result;
            }
            else
            {
                var result = string.Join(",", (from cat in db.cCoordinacionesRegion
                              where cat.EstatusId == 5
                              select cat.CoordinacionRegionId).ToList());

                return result;
            }
           
        }

        [HttpGet]
        public ActionResult Nuevo()
        {
            var objRegiones = new cCoordinacionesRegion();
            ViewBag.Titulo = "Nueva Región";
            return PartialView("_Nuevo", objRegiones);
        }

        [HttpPost]
        public JsonResult Nuevo(cCoordinacionesRegion pobjModelo)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    pobjModelo.FechaCreacion = DateTime.Now;
                    pobjModelo.EstatusId = pobjModelo.EstatusId;
                    pobjModelo.DescripcionCoordinacionRegion = pobjModelo.DescripcionCoordinacionRegion;
                    db.cCoordinacionesRegion.Add(pobjModelo);
                    db.SaveChanges();

                    return Json(new { Success = true, id = pobjModelo.CoordinacionRegionId, Message = "guardado correctamente " });
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
            var objRegion= db.cCoordinacionesRegion.Find(id);
            ViewBag.Titulo = "Actualizar Región";
            return PartialView("_Actualizar", objRegion);
        }

        [HttpPost]
        public JsonResult Actualizar(cCoordinacionesRegion pobjModelo)
        {
            try
            {
                var result = (from ps in db.cCoordinacionesRegion
                              where ps.CoordinacionRegionId == pobjModelo.CoordinacionRegionId
                              select ps).ToList();

                //Actualiza
                var dbTemp = result.First();
                dbTemp.DescripcionCoordinacionRegion = pobjModelo.DescripcionCoordinacionRegion;
                dbTemp.EstatusId = pobjModelo.EstatusId;
                db.SaveChanges();


                return Json(new { Success = true, id = pobjModelo.CoordinacionRegionId, Message = "actualizada correctamente " });

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
                var result = (from usu in db.cCoordinacionesRegion
                              where usu.CoordinacionRegionId == (id)
                              select usu).FirstOrDefault();

                result.EstatusId = 6;
                db.SaveChanges();

                return Json(new { Success = true, Message = "Se borro correctamente la Región " });
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