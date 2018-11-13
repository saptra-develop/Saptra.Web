///Propósito:Controlador para la administracion de roles
///Fecha creación: 08/Septiembre/16
///Creador: Juan Lopepe
///Fecha modifiacción: 
///Modificó:
///Dependencias de conexiones e interfaces: 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Newtonsoft.Json;

using Saptra.Web.Data;
using Saptra.Web.Models;

namespace Saptra.Web.Controllers
{
    public class RolController : Controller
    {
        private Inaeba_SaptraEntities db = new Inaeba_SaptraEntities();

        public ActionResult Index()
        {
            return View();
        }
        
        [HttpGet]
        public JsonResult CargarRoles(int? id, int? idEstatus)
        {
            try
            {
                var result = (from cat in db.mRoles
                              where cat.EstatusId == 5
                                  && cat.RolId == (id == null ? cat.RolId : id)
                              select new {
                                  id = cat.RolId,
                                  nombre = cat.NombreRol.ToUpper(),
                                  cat.cEstatus.EstatusId,
                                  cat.cEstatus.NombreEstatus
                              }).ToList();

                return (Json(result, JsonRequestBehavior.AllowGet));
            }
            catch(Exception exp)
            {
                return Json(new { Success = false, Message = "Error al obtener la información" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult NuevoRol()
        {
            var objRol = new mRoles();
            return PartialView("_NuevoRol", objRol);
        }

        [HttpPost]
        public JsonResult NuevoRol(mRoles pobjModelo)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    pobjModelo.FechaCreacion = DateTime.Now;
                    db.mRoles.Add(pobjModelo);
                    db.SaveChanges();

                    return Json(new { Success = true, id = pobjModelo.RolId, Message = "Se guardo correctamente el Rol con id " + pobjModelo.RolId });
                }
                catch (Exception exp)
                {
                    return Json(new { Success = false, Message = "Error al guardar la información" });
                }
            }

            return Json(new { Success = false, Message = "La informacion esta incompleta" });
        }

        [HttpGet]
        public ActionResult ActualizaRol(int id)
        {
            var objRol = db.mRoles.Find(id);
            return PartialView("_ActualizaRol", objRol);
        }

        [HttpPost]
        public JsonResult ActualizaRol(mRoles pobjModelo)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var result = from u in db.mRoles where (u.RolId == pobjModelo.RolId) select u;
                    if (result.Count() != 0)
                    {
                        var dbTemp = result.First();
                        dbTemp.NombreRol = pobjModelo.NombreRol;
                        dbTemp.EstatusId = pobjModelo.EstatusId;
                        db.SaveChanges();
                    }

                    return Json(new { Success = true, id = pobjModelo.RolId, Message = "Se actualizo correctamente el Rol con id " + pobjModelo.RolId });
                }
                catch (Exception exp)
                {
                    return Json(new { Success = false, Message = "Error al guardar la información" });
                }
            }

            return Json(new { Success = false, Message = "La informacion esta incompleta" });
        }

        [HttpPost]
        public JsonResult BorrarRol(int id)
        {
            try
            {
                var result = (from rol in db.mRoles
                              where rol.RolId == (id)
                              select rol).FirstOrDefault();

                result.EstatusId = 6;
                db.SaveChanges();

                return Json(new { Success = true, Message = "Se borro correctamente el Rol" });
            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = "Error al eliminar la información" });
            }
        }

        public ActionResult Modulos(string id)
        {
            ViewBag.idPerfil = id;
            return PartialView("_Modulos");
        }

        [HttpGet]
        public JsonResult CargarModulos(int id)
        {
            try
            {
                var result = (from cat in db.udf_ModulosSeguridadxRol(id)
                              select new {
                                  id = cat.ModuloId,
                                  cat.NombreMenuGrupo,
                                  cat.nombreModulo, //
                                  cat.EstatusId,
                                  lecturaPermiso = cat.lecturaPermiso ? 1 : 0,
                                  escrituraPermiso = cat.escrituraPermiso ? 1 : 0,
                                  edicionPermiso = cat.edicionPermiso ? 1 : 0,
                                  clonadoPermiso = cat.clonadoPermiso ? 1 : 0,
                                  borradoPermiso = cat.borradoPermiso ? 1 : 0,
                                  cat.OrdenGrupo,
                                  cat.OrdenModulo
                              }).OrderBy(o => o.OrdenGrupo).ThenBy(o => o.OrdenModulo).ToList();

                var lstModulosRet = result.Select(C => new
                {
                    id = C.id,
                    nombreModulo = Resources.Global.ResourceManager.GetString(C.NombreMenuGrupo).ToUpper() + " - " + Resources.Global.ResourceManager.GetString(C.nombreModulo).ToUpper(),
                    C.EstatusId,
                    C.lecturaPermiso,
                    C.escrituraPermiso,
                    C.edicionPermiso,
                    C.clonadoPermiso,
                    C.borradoPermiso
                });

                return (Json(lstModulosRet, JsonRequestBehavior.AllowGet));
            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = "Error al obtener la información" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult GuardaSeguridad(List<Models.ListaPermisos> lstModulos, int idRol)
        {
            try
            {
                foreach (ListaPermisos modulo in lstModulos) {
                    Data.mPermisos dbPermiso = (from p in db.mPermisos
                                                where p.RolId == idRol && p.ModuloId == modulo.id
                                                select p).SingleOrDefault();
                    if (dbPermiso == null)
                    {
                        dbPermiso = new Data.mPermisos();
                        dbPermiso.ModuloId = modulo.id;
                        dbPermiso.RolId = idRol;
                        dbPermiso.FechaCreacion = DateTime.Now;
                        dbPermiso.EstatusId = 5;
                        dbPermiso.LecturaPermiso = modulo.lecturaPermiso == 1 ? true : false;
                        dbPermiso.EscrituraPermiso = modulo.escrituraPermiso == 1 ? true : false;
                        dbPermiso.EdicionPermiso = modulo.escrituraPermiso == 1 ? true : false;
                        dbPermiso.ClonadoPermiso = modulo.clonadoPermiso == 1 ? true : false;
                        dbPermiso.BorradoPermiso = modulo.borradoPermiso == 1 ? true : false;

                        db.mPermisos.Add(dbPermiso);
                        db.SaveChanges();
                    }
                    else 
                    {
                        dbPermiso.LecturaPermiso = modulo.lecturaPermiso == 1 ? true : false;
                        dbPermiso.EscrituraPermiso = modulo.escrituraPermiso == 1 ? true : false;
                        dbPermiso.EdicionPermiso = modulo.escrituraPermiso == 1 ? true : false;
                        dbPermiso.ClonadoPermiso = modulo.clonadoPermiso == 1 ? true : false;
                        dbPermiso.BorradoPermiso = modulo.borradoPermiso == 1 ? true : false;
                        db.SaveChanges();
                    }
                }

                return Json(new { Success = true, Message = "Los permisos fueron actualizados" });
            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = "Error al guardar la información" });
            }
        }

        //[HttpPost]
        //public JsonResult ValidaRol(int id)
        //{
        //    try
        //    {
        //        var result = (from cat in db.mUsuarios
        //                      where cat.UsuarioId == id
        //                      select cat).FirstOrDefault();


        //        return Json(new { Success = true, liberaMatriz = (result.liberaMatriz == null ? 0 : result.liberaMatriz) });
        //    }
        //    catch (Exception exp)
        //    {
        //        return Json(new { Success = false, Message = exp.Message }, JsonRequestBehavior.AllowGet);
        //    }
        //}

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
