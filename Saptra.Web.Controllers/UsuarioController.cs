///Propósito:Controlador para la administracion de Usuarios
///Fecha creación: 26/Septiembre/18
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

namespace Sispro.Web.Controllers
{
    public class UsuarioController : Controller
    {
        private Inaeba_SaptraEntities db = new Inaeba_SaptraEntities();

        public ActionResult Index()
        {
            return View();
        }


        public PartialViewResult CargaMenuUsuario(int? id)
        {
            var objUsuario = db.mUsuarios.Find(id);
            objUsuario.ImagenUsuario = objUsuario.ImagenUsuario == null ? "/Content/images/avatar.jpg" : objUsuario.ImagenUsuario;
            return PartialView("_MenuUsuario", objUsuario);
        }

        [HttpGet]
        public JsonResult CargarUsuarios(int? idUsu, int? idEstatus)
        {
            try
            {
                var result = (from cat in db.mUsuarios
                              where cat.EstatusId == (idEstatus == null ? cat.EstatusId : idEstatus)
                                  && cat.UsuarioId == (idUsu == null ? cat.UsuarioId : idUsu)
                              select new
                              {
                                  id = cat.UsuarioId,
                                  cat.EstatusId,
                                  cat.cEstatus1.NombreEstatus,
                                  nombres = cat.NombresUsuario.ToUpper(),
                                  apellidos = cat.ApellidosUsuario.ToUpper(),
                                  nombreCompleto = cat.NombresUsuario.ToUpper() + " " + cat.ApellidosUsuario.ToUpper(),
                                  cat.RolId,
                                  nombreRol = cat.mRoles.NombreRol.ToUpper(),
                                  imagen = cat.ImagenUsuario == null ? "/Content/images/avatar.jpg" : cat.ImagenUsuario
                              }).ToList();



                return Json(new { Success = true, datos = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = exp.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        public ActionResult Nuevo()
        {
            var objUsuario = new mUsuarios();
            ViewBag.Titulo = "Nuevo";
            return PartialView("_Nuevo", objUsuario);
        }

        [HttpPost]
        public JsonResult Nuevo(mUsuarios pobjModelo)
        {
            if (ModelState.IsValid)
            {
                try
                {                    
                    if (correoExistente(pobjModelo.EmailUsuario)==true)
                        return Json(new { Success = false, Message = "The user's email is already in use" });
                    else if (loginExistente(pobjModelo.LoginUsuario))
                        return Json(new { Success = false, Message = "The user's login is already in use" });
                    else
                    {
                        pobjModelo.FechaCreacion = DateTime.Now;
                        pobjModelo.ImagenUsuario = pobjModelo.ImagenUsuario == null ? "/Content/images/avatar.jpg" : pobjModelo.ImagenUsuario;
                        db.mUsuarios.Add(pobjModelo);
                        db.SaveChanges();

                        return Json(new { Success = true, id = pobjModelo.UsuarioId, Message = "Se guardó correctamente el usuario " });
                    }
                }
                catch (Exception exp)
                {
                    return Json(new { Success = false, Message = exp.Message });
                }
            }

            return Json(new { Success = false, Message = "Información incompleta" });
        }

        [HttpGet]
        public ActionResult Actualiza(int id)
        {
            var objUsuario = db.mUsuarios.Find(id);
            objUsuario.ImagenUsuario = objUsuario.ImagenUsuario == null ? "/Content/images/avatar.jpg" : objUsuario.ImagenUsuario;
            return PartialView("_Actualiza", objUsuario);
        }

        [HttpGet]
        public ActionResult ActualizaPerfil(int id)
        {
            var objUsuario = db.mUsuarios.Find(id);
            objUsuario.ImagenUsuario = objUsuario.ImagenUsuario == null ? "/Content/images/avatar.jpg" : objUsuario.ImagenUsuario;
            return PartialView("_ActualizaPerfil", objUsuario);
        }

        [HttpGet]
        public ActionResult ActualizaPassword(int id)
        {
            var objUsuario = db.mUsuarios.Find(id);
            ViewBag.imagenUsuario = objUsuario.ImagenUsuario == null ? "/Content/images/avatar.jpg" : objUsuario.ImagenUsuario;
            ViewBag.idUsuario = objUsuario.UsuarioId;
            return PartialView("_ActualizaPassword");
        }



        [HttpPost]
        public JsonResult ActualizaPerfil(mUsuarios pobjModelo)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var result = from u in db.mUsuarios where (u.UsuarioId == pobjModelo.UsuarioId) select u;
                    if (result.Count() != 0)
                    {
                        var dbUsu = result.First();
                        dbUsu.EmailUsuario = pobjModelo.EmailUsuario;
                        if (pobjModelo.ImagenUsuario != null)
                        {
                            dbUsu.ImagenUsuario = pobjModelo.ImagenUsuario;
                        }
                        db.SaveChanges();
                    }


                    var UsuarioPerfil = (from u in db.mUsuarios
                                         where (u.UsuarioId == pobjModelo.UsuarioId)
                                         select new
                                         {
                                             id = u.UsuarioId
                                         }).ToList();


                    return Json(new { Success = true, UsuarioPerfil, Message = "Se actualizo correctamente el usuario " });
                }
                catch (Exception exp)
                {
                    return Json(new { Success = false, Message = exp.Message });
                }
            }

            return Json(new { Success = false, Message = "Información del usuario esta incompleta" });
        }

        [HttpPost]
        public JsonResult ActualizaPassword(int idUser, string passwordActual, string passwordNuevo, string passwordConfirmar)
        {
            try
            {
                var resultado = db.Database.SqlQuery<string>("execute [dbo].[sp_CambiarPassword] @p0,@p1,@p2,@p3", idUser, passwordActual, passwordNuevo, passwordConfirmar).FirstOrDefault();
                string[] resultados = resultado.Split('|');

                if (resultados[0] == "0")
                {
                    return Json(new { Success = true, id = idUser, Message = resultados[1] });

                }
                else
                {
                    return Json(new { Success = false, id = idUser, Message = "Error : " + resultados[1] });
                }
            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = exp.Message });
            }
        }


        [HttpPost]
        public JsonResult Actualiza(mUsuarios pobjModelo)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var result = from u in db.mUsuarios where (u.UsuarioId == pobjModelo.UsuarioId) select u;


                    if (result.Count() != 0)
                    {

                        var dbUsu = result.First();
                        if (dbUsu.EmailUsuario != pobjModelo.EmailUsuario)
                        {
                            if (correoExistente(pobjModelo.EmailUsuario))
                                return Json(new { Success = false, Message = "The user's email is already in use" });                          
                        }

                        if (dbUsu.LoginUsuario != pobjModelo.LoginUsuario)
                        {
                            if (loginExistente(pobjModelo.LoginUsuario))
                                return Json(new { Success = false, Message = "The user's login is already in use" });
                        }

                        dbUsu.NombresUsuario = pobjModelo.NombresUsuario;
                        dbUsu.ApellidosUsuario = pobjModelo.ApellidosUsuario;
                        dbUsu.LoginUsuario = pobjModelo.LoginUsuario;
                        dbUsu.PasswordUsuario = pobjModelo.PasswordUsuario;
                        dbUsu.EmailUsuario = pobjModelo.EmailUsuario;
                        dbUsu.RolId = pobjModelo.RolId;
                        if (pobjModelo.ImagenUsuario != null)
                        {
                            dbUsu.ImagenUsuario = pobjModelo.ImagenUsuario;
                        }
                        
                        dbUsu.cEstatus1 = pobjModelo.cEstatus1;


                        db.SaveChanges();


                    }
                    return Json(new { Success = true, id = pobjModelo.UsuarioId, Message = "Se actualizó correctamente el usuario " });

                }
                catch (Exception exp)
                {
                    return Json(new { Success = false, Message = exp.Message });
                }
            }

            return Json(new { Success = false, Message = "Información del usuario esta incompleta" });
        }

        [HttpGet]
        public ActionResult Clonar(int id)
        {
            mUsuarios objUsuario = null;
            var result = (from usu in db.mUsuarios
                          where usu.UsuarioId == (id)
                          select usu).FirstOrDefault();
            objUsuario = result;
            objUsuario.UsuarioId = 0;
            return PartialView("_Nuevo", objUsuario);
        }

        [HttpPost]
        public JsonResult Borrar(int id)
        {
            try
            {
                var usu = db.mUsuarios.Where(t => t.UsuarioId == id);
                db.mUsuarios.RemoveRange(usu);
                db.SaveChanges();

                return Json(new { Success = true, Message = "Se borro correctamente el usuario " });
            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = exp.Message });
            }
        }

        private bool correoExistente(string correo)
        {
            bool existe = true;
            var usuario = (from i in db.mUsuarios
                           where i.EmailUsuario == correo
                           select i);

            if (usuario.ToList().Count == 0)
                existe = false;

            return existe;
        }

        private bool loginExistente(string login)
        {
            bool existe = true;
            var usuario = (from i in db.mUsuarios
                           where i.LoginUsuario == login
                           select i);

            if (usuario.ToList().Count == 0)
                existe = false;

            return existe;
        }

        [HttpGet]
        public JsonResult CargarNombresFigura(int? id, int? idEstatus)
        {
            try
            {
                var result = (from cat in db.mUsuarios
                              where cat.EstatusId == (idEstatus == null ? cat.EstatusId : idEstatus)
                              && cat.RolId == 2
                              select new
                              {
                                  id = cat.UsuarioId,
                                  nombre = cat.NombresUsuario + " " + cat.ApellidosUsuario,
                              })
                              .OrderBy(cat => cat.id)
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