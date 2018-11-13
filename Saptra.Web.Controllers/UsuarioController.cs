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

namespace Saptra.Web.Controllers
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
            objUsuario.ImagenUsuario = objUsuario.ImagenUsuario == null ? "../Content/images/avatar.jpg" : objUsuario.ImagenUsuario;
            return PartialView("_MenuUsuario", objUsuario);
        }

        [HttpGet]
        public JsonResult CargarUsuarios(int? idUsu, int? idEstatus)
        {
            try
            {
                var result = (from cat in db.mUsuarios
                              where  cat.UsuarioId == (idUsu == null ? cat.UsuarioId : idUsu)
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
                                  imagen = cat.ImagenUsuario == null ? "../Content/images/avatar.jpg" : cat.ImagenUsuario,
                                  email = cat.EmailUsuario
                              }).ToList();



                return Json(new { Success = true, datos = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = "Error al cargar la información."}, JsonRequestBehavior.AllowGet);
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
        public JsonResult Nuevo(mUsuarios pobjModelo, int RegionId, int ZonaId, int idUsuario)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var jRegion = (from j in db.mCoordinacionRegionZonaUsuario
                                      where j.CoordinacionRegionId == RegionId
                                      select j).ToList();

                    var jZona = (from z in db.mCoordinacionZonaUsuario
                                 where z.CoordinacionZonaId == ZonaId
                                 && z.JefeCoordinacionZona == true
                                 select z).ToList();
               
                    if (correoExistente(pobjModelo.EmailUsuario) == true)
                        return Json(new { Success = false, Message = "The user's email is already in use" });
                    else if (loginExistente(pobjModelo.LoginUsuario))
                        return Json(new { Success = false, Message = "The user's login is already in use" });
                    else
                    {
                        string contrasenaRec;
                        contrasenaRec = EncriptarController.GetMD5(pobjModelo.PasswordUsuario);
                        var fig = (pobjModelo.RolId == 2 ? 1 : (pobjModelo.RolId == 4 ? 2 : (pobjModelo.RolId == 5 ? 3 : 0)));
                        pobjModelo.FechaCreacion = DateTime.Now;
                        pobjModelo.ImagenUsuario = pobjModelo.ImagenUsuario == null ? null : pobjModelo.ImagenUsuario;
                        pobjModelo.PasswordUsuario = contrasenaRec;
                        if (fig != 0)
                        {
                            pobjModelo.TipoFiguraId = fig;
                        }
                        db.mUsuarios.Add(pobjModelo);
                        db.SaveChanges();

                        if (jZona.Count() == 0 && pobjModelo.RolId == 3)
                        {
                            mCoordinacionZonaUsuario objCoordinacionZona = new mCoordinacionZonaUsuario();

                            objCoordinacionZona.UsuarioId = pobjModelo.UsuarioId;
                            objCoordinacionZona.CoordinacionZonaId = ZonaId;
                            objCoordinacionZona.JefeCoordinacionZona = true;
                            objCoordinacionZona.UsuarioCreacionId = idUsuario;
                            objCoordinacionZona.FechaCreacion = DateTime.Now;
                            db.mCoordinacionZonaUsuario.Add(objCoordinacionZona);
                            db.SaveChanges();
                        }
                        else if(pobjModelo.RolId != 6)
                        {

                            if (pobjModelo.RolId == 2 || pobjModelo.RolId == 4 || pobjModelo.RolId == 5)
                            {

                                mCoordinacionZonaUsuario objCoordinacionZona = new mCoordinacionZonaUsuario();

                                objCoordinacionZona.UsuarioId = pobjModelo.UsuarioId;
                                objCoordinacionZona.CoordinacionZonaId = ZonaId;
                                objCoordinacionZona.JefeCoordinacionZona = false;
                                objCoordinacionZona.UsuarioCreacionId = idUsuario;
                                objCoordinacionZona.FechaCreacion = DateTime.Now;
                                db.mCoordinacionZonaUsuario.Add(objCoordinacionZona);
                                db.SaveChanges();
                            }
                            else if (pobjModelo.RolId == 3)
                            {
                                var temp = jZona.First();
                                temp.UsuarioId = pobjModelo.UsuarioId;
                                db.SaveChanges();
                            }
                        }

                        if (jRegion.Count() == 0 && pobjModelo.RolId == 6)
                        {
                            mCoordinacionRegionZonaUsuario objCoordinacionZonaRegion = new mCoordinacionRegionZonaUsuario();

                            objCoordinacionZonaRegion.UsuarioJefeRegionId = pobjModelo.UsuarioId;
                            //objCoordinacionZonaRegion.CoordinacionZonaId = ZonaId;
                            objCoordinacionZonaRegion.CoordinacionRegionId = RegionId;
                            db.mCoordinacionRegionZonaUsuario.Add(objCoordinacionZonaRegion);
                            db.SaveChanges();
                        }
                        else 
                        {
                            if (pobjModelo.RolId == 6)
                            {
                                var dbjefe = jRegion.First();
                                dbjefe.UsuarioJefeRegionId = pobjModelo.UsuarioId;
                                db.SaveChanges();
                            }
                        }


                        return Json(new { Success = true, id = pobjModelo.UsuarioId, Message = "Se guardó correctamente el usuario " });
                    }
                }
                catch (Exception exp)
                {
                    return Json(new { Success = false, Message = "No fue posible guardar la información" });
                }
            }

            return Json(new { Success = false, Message = "Información incompleta" });
        }

        [HttpGet]
        public ActionResult Actualiza(int id)
        {
            var objUsuario = db.mUsuarios.Find(id);
            objUsuario.ImagenUsuario = objUsuario.ImagenUsuario == null ? "../Content/images/avatar.jpg" : objUsuario.ImagenUsuario;
            ViewBag.RegionId = objUsuario.mCoordinacionRegionZonaUsuario.FirstOrDefault() == null ? 0 : objUsuario.mCoordinacionRegionZonaUsuario.FirstOrDefault().CoordinacionRegionId;
            ViewBag.ZonaId = objUsuario.mCoordinacionZonaUsuario1.FirstOrDefault() == null ? 0 : objUsuario.mCoordinacionZonaUsuario1.FirstOrDefault().CoordinacionZonaId;
            return PartialView("_Actualiza", objUsuario);
        }

        [HttpGet]
        public ActionResult ActualizaPerfil(int id)
        {
            var objUsuario = db.mUsuarios.Find(id);
            objUsuario.ImagenUsuario = objUsuario.ImagenUsuario == null ? "../Content/images/avatar.jpg" : objUsuario.ImagenUsuario;
            return PartialView("_ActualizaPerfil", objUsuario);
        }

        [HttpGet]
        public ActionResult ActualizaPassword(int id)
        {
            var objUsuario = db.mUsuarios.Find(id);
            ViewBag.imagenUsuario = objUsuario.ImagenUsuario == null ? "../Content/images/avatar.jpg" : objUsuario.ImagenUsuario;
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
                                             id = u.UsuarioId,
                                             emailUsuario = u.EmailUsuario,
                                             imagenUsuario = u.ImagenUsuario
                                         }).ToList();


                    return Json(new { Success = true, UsuarioPerfil, Message = "Se actualizo correctamente el usuario " });
                }
                catch (Exception exp)
                {
                    return Json(new { Success = false, Message = "No fue posible guardar la información." });
                }
            }

            return Json(new { Success = false, Message = "Información del usuario esta incompleta" });
        }

        [HttpPost]
        public JsonResult ActualizaPassword(int idUser, string passwordActual, string passwordNuevo, string passwordConfirmar)
        {
            try
            {
                var result = from u in db.mUsuarios where (u.UsuarioId == idUser) select u;

                if (result.Count() != 0)
                {
                    var dbUsu = result.First();
                    string contrasenaAct;
                    contrasenaAct = EncriptarController.GetMD5(passwordActual);

                    if (contrasenaAct == dbUsu.PasswordUsuario)
                    {
                        if (passwordNuevo.Trim() == passwordConfirmar.Trim())
                        {
                            string contrasenaNueva;
                            contrasenaNueva = EncriptarController.GetMD5(passwordConfirmar);
                            dbUsu.PasswordUsuario = contrasenaNueva;
                            db.SaveChanges();

                            return Json(new { Success = true, id = idUser, Message = "Contraseña actualizada correctamente." });
                        }
                        else
                        {
                            return Json(new { Success = false, id = idUser, tipo = 1, Message = "Las contraseñas no coinciden." });
                        }
                    }
                    else
                    {
                        return Json(new { Success = false, id = idUser, tipo = 2, Message = "La contraseña actual es incorrecta." });
                    }
                }
                else
                {
                    return Json(new { Success = false, id = idUser, Message = "Usuario no existe." });
                }


                //var resultado = db.Database.SqlQuery<string>("execute [dbo].[sp_CambiarPassword] @p0,@p1,@p2,@p3", idUser, passwordActual, passwordNuevo, passwordConfirmar).FirstOrDefault();
                //string[] resultados = resultado.Split('|');

                //if (resultados[0] == "0")
                //{
                //    return Json(new { Success = true, id = idUser, Message = resultados[1] });

                //}
                //else
                //{
                //    return Json(new { Success = false, id = idUser, Message = "Error : " + resultados[1] });
                //}
            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = "No fue posible guardar la información." });
            }
        }


        [HttpPost]
        public JsonResult Actualiza(mUsuarios pobjModelo, int RegionId, int ZonaId, int idUsuario)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var jRegion = (from j in db.mCoordinacionRegionZonaUsuario
                                   where j.CoordinacionRegionId == RegionId
                                   select j).ToList();

                    var usarioexisteReg = (from j in db.mCoordinacionRegionZonaUsuario
                                                         where j.UsuarioJefeRegionId == pobjModelo.UsuarioId
                                                         select j).ToList();

                    var jZona = (from z in db.mCoordinacionZonaUsuario
                                 where z.CoordinacionZonaId == ZonaId
                                 && z.JefeCoordinacionZona == true
                                 select z).ToList();


                    var usarioexisteZona= (from j in db.mCoordinacionZonaUsuario
                                           where j.UsuarioId == pobjModelo.UsuarioId
                                           && j.JefeCoordinacionZona == true
                                           select j).ToList();

                    var result = from u in db.mUsuarios where (u.UsuarioId == pobjModelo.UsuarioId) select u;
                    string contrasenaRec;
                    if (result.First().PasswordUsuario != pobjModelo.PasswordUsuario)
                    {
                        contrasenaRec = EncriptarController.GetMD5(pobjModelo.PasswordUsuario);
                    }
                    else
                    {
                        contrasenaRec = result.First().PasswordUsuario;
                    }

                    if (result.Count() != 0)
                    {

                        var dbUsu = result.First();
                        if (dbUsu.EmailUsuario != pobjModelo.EmailUsuario)
                        {
                            if (correoExistente(pobjModelo.EmailUsuario))
                                return Json(new { Success = false, Message = "El correo electrónico del usuario ya está en uso" });                          
                        }

                        if (dbUsu.LoginUsuario != pobjModelo.LoginUsuario)
                        {
                            if (loginExistente(pobjModelo.LoginUsuario))
                                return Json(new { Success = false, Message = "El login del usuario ya está en uso." });
                        }

                        dbUsu.NombresUsuario = pobjModelo.NombresUsuario;
                        dbUsu.ApellidosUsuario = pobjModelo.ApellidosUsuario;
                        dbUsu.LoginUsuario = pobjModelo.LoginUsuario;
                        dbUsu.PasswordUsuario = contrasenaRec;
                        dbUsu.EmailUsuario = pobjModelo.EmailUsuario;
                        dbUsu.RFCUsuario = pobjModelo.RFCUsuario;
                        dbUsu.RolId = pobjModelo.RolId;
                        if (pobjModelo.ImagenUsuario != null)
                        {
                            dbUsu.ImagenUsuario = pobjModelo.ImagenUsuario;
                        }
                        
                        dbUsu.EstatusId = pobjModelo.EstatusId;


                        db.SaveChanges();

                        if (jZona.Count() == 0 && pobjModelo.RolId == 3)
                        {
                            if (usarioexisteZona.Count == 0)
                            {
                                mCoordinacionZonaUsuario objCoordinacionZona = new mCoordinacionZonaUsuario();

                                objCoordinacionZona.UsuarioId = pobjModelo.UsuarioId;
                                objCoordinacionZona.CoordinacionZonaId = ZonaId;
                                objCoordinacionZona.JefeCoordinacionZona = true;
                                objCoordinacionZona.UsuarioCreacionId = idUsuario;
                                objCoordinacionZona.FechaCreacion = DateTime.Now;
                                db.mCoordinacionZonaUsuario.Add(objCoordinacionZona);
                                db.SaveChanges();
                            }
                            else
                            {
                                var col = db.mCoordinacionZonaUsuario.Where(t => t.UsuarioId == pobjModelo.UsuarioId);
                                db.mCoordinacionZonaUsuario.RemoveRange(col);
                                db.SaveChanges();

                                mCoordinacionZonaUsuario objCoordinacionZona = new mCoordinacionZonaUsuario();

                                objCoordinacionZona.UsuarioId = pobjModelo.UsuarioId;
                                objCoordinacionZona.CoordinacionZonaId = ZonaId;
                                objCoordinacionZona.JefeCoordinacionZona = true;
                                objCoordinacionZona.UsuarioCreacionId = idUsuario;
                                objCoordinacionZona.FechaCreacion = DateTime.Now;
                                db.mCoordinacionZonaUsuario.Add(objCoordinacionZona);
                                db.SaveChanges();
                            }
                        }
                        else if (pobjModelo.RolId != 6)
                        {

                            if (pobjModelo.RolId == 2 || pobjModelo.RolId == 4 || pobjModelo.RolId == 5)
                            {
                                var usuarioZona = (from j in db.mCoordinacionZonaUsuario
                                                        where j.UsuarioId == pobjModelo.UsuarioId
                                                        select j).ToList();
                                if(usuarioZona.Count() == 0)
                                {
                                    mCoordinacionZonaUsuario objCoordinacionZona = new mCoordinacionZonaUsuario();

                                    objCoordinacionZona.UsuarioId = pobjModelo.UsuarioId;
                                    objCoordinacionZona.CoordinacionZonaId = ZonaId;
                                    objCoordinacionZona.JefeCoordinacionZona = false;
                                    objCoordinacionZona.UsuarioCreacionId = idUsuario;
                                    objCoordinacionZona.FechaCreacion = DateTime.Now;
                                    db.mCoordinacionZonaUsuario.Add(objCoordinacionZona);
                                    db.SaveChanges();
                                }
                                else
                                {
                                    var col = db.mCoordinacionZonaUsuario.Where(t => t.UsuarioId == pobjModelo.UsuarioId);
                                    db.mCoordinacionZonaUsuario.RemoveRange(col);
                                    db.SaveChanges();

                                    mCoordinacionZonaUsuario objCoordinacionZona = new mCoordinacionZonaUsuario();

                                    objCoordinacionZona.UsuarioId = pobjModelo.UsuarioId;
                                    objCoordinacionZona.CoordinacionZonaId = ZonaId;
                                    objCoordinacionZona.JefeCoordinacionZona = false;
                                    objCoordinacionZona.UsuarioCreacionId = idUsuario;
                                    objCoordinacionZona.FechaCreacion = DateTime.Now;
                                    db.mCoordinacionZonaUsuario.Add(objCoordinacionZona);
                                    db.SaveChanges();
                                }

                                
                            }
                            else if (pobjModelo.RolId == 3)
                            {
                                var temp = jZona.First();
                                temp.UsuarioId = pobjModelo.UsuarioId;
                                db.SaveChanges();
                            }
                        }

                        if (jRegion.Count() == 0 && pobjModelo.RolId == 6)
                        {
                            if (usarioexisteReg.Count() == 0)
                            {
                                mCoordinacionRegionZonaUsuario objCoordinacionZonaRegion = new mCoordinacionRegionZonaUsuario();

                                objCoordinacionZonaRegion.UsuarioJefeRegionId = pobjModelo.UsuarioId;
                                //objCoordinacionZonaRegion.CoordinacionZonaId = ZonaId;
                                objCoordinacionZonaRegion.CoordinacionRegionId = RegionId;
                                db.mCoordinacionRegionZonaUsuario.Add(objCoordinacionZonaRegion);
                                db.SaveChanges();
                            }
                            else
                            {
                                var col = db.mCoordinacionRegionZonaUsuario.Where(t => t.UsuarioJefeRegionId == pobjModelo.UsuarioId);
                                db.mCoordinacionRegionZonaUsuario.RemoveRange(col);
                                db.SaveChanges();

                                mCoordinacionRegionZonaUsuario objCoordinacionZonaRegion = new mCoordinacionRegionZonaUsuario();

                                objCoordinacionZonaRegion.UsuarioJefeRegionId = pobjModelo.UsuarioId;
                                //objCoordinacionZonaRegion.CoordinacionZonaId = ZonaId;
                                objCoordinacionZonaRegion.CoordinacionRegionId = RegionId;
                                db.mCoordinacionRegionZonaUsuario.Add(objCoordinacionZonaRegion);
                                db.SaveChanges();
                            }
                        }
                        else
                        {
                            if (pobjModelo.RolId == 6)
                            {
                                var dbjefe = jRegion.First();
                                dbjefe.UsuarioJefeRegionId = pobjModelo.UsuarioId;
                                db.SaveChanges();
                            }
                        }


                    }
                    return Json(new { Success = true, id = pobjModelo.UsuarioId, Message = "Se actualizó correctamente el usuario " });

                }
                catch (Exception exp)
                {
                    return Json(new { Success = false, Message = "No fue posible guardar la información." });
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
                var result = (from usu in db.mUsuarios
                              where usu.UsuarioId == (id)
                              select usu).FirstOrDefault();

                result.EstatusId = 6;
                db.SaveChanges();

                return Json(new { Success = true, Message = "Se borro correctamente el usuario " });
            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = "No fue posible eliminar la información." });
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
        public JsonResult CargarNombresFiguraPorCZ(int? id, int? idEstatus, int idUsuarioCZ)
        {
            try
            {
                var cz = (from c in db.mCoordinacionZonaUsuario
                          where c.UsuarioId == idUsuarioCZ
                          && c.JefeCoordinacionZona == true
                          select c).FirstOrDefault();

                var result = (from cat in db.mUsuarios
                              join c in db.mCoordinacionZonaUsuario on cat.UsuarioId equals c.UsuarioId
                              where cat.EstatusId == (idEstatus == null ? cat.EstatusId : idEstatus)
                              && c.CoordinacionZonaId == cz.CoordinacionZonaId
                              && c.JefeCoordinacionZona == false
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
                return Json(new { Success = false, Message = "No fue posible cargar la información." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult ValidaJefeRegion(int RegionId)
        {
            try
            {
                var jefe = (from sec in db.mCoordinacionRegionZonaUsuario
                              where sec.CoordinacionRegionId == RegionId
                            select sec).ToList();

                return (Json(new
                {
                    Success = (jefe.Count() == 0 ? false : true)
                }, JsonRequestBehavior.AllowGet));

            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = "No fue posible validar la información." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult ValidaJefeZona(int ZonaId)
        {
            try
            {
                var jefe = (from sec in db.mCoordinacionZonaUsuario
                            where sec.CoordinacionZonaId == ZonaId
                            && sec.JefeCoordinacionZona == true
                            select sec).ToList();

                return (Json(new
                {
                    Success = (jefe.Count() == 0 ? false : true)
                }, JsonRequestBehavior.AllowGet));

            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = "No fue posible validar la información." }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Metodo para subir los archivos del proyecto
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SubirArchivo(mUsuarios pobjModelo)
        {
            string filePath = "";
            string nombreArchivo = "";
            string nombre = "";
            try
            {
                string urlImagen = "prueba";
                if (Request.Files.Count > 0)
                {

                    var file = Request.Files[0];


                    string extension = System.IO.Path.GetExtension(file.FileName);
                    nombre = string.Format("{0}{1}", Guid.NewGuid().ToString("N"), extension.ToLower());

                    //"C:\inetpub\wwwroot\saptra\saptraImages\12884462db394dfb8a4293328c148ce1.jpg"
                    filePath = Path.Combine(Server.MapPath("~/saptraImages/"), nombre);
                    file.SaveAs(filePath);
                    filePath = filePath.Replace(@"C:\inetpub\wwwroot\", "http://200.33.114.167/").Replace(@"\", "/");



                }

                   

               

                return Json(new { Success = true, Archivo = filePath });
            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = "No fue posible subir la imagen." });
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