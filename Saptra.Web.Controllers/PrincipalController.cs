///Propósito:Controlador para la pantalla inicial del sistema
///Fecha creación: 05/Septiembre/16
///Creador: David Galvan
///Fecha modifiacción: 
///Modificó:
///Dependencias de conexiones e interfaces: 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.IO;
using Saptra.Web.Data;
using Saptra.Web.Models;
using Saptra.Web.Utils;


namespace Saptra.Web.Controllers
{
    public class PrincipalController : Controller
    {
        private Inaeba_SaptraEntities db = new Inaeba_SaptraEntities();

        public ActionResult Inicio()
        {
            int intIdUsuario = 0;

            var cookie = HttpContext.Request.Cookies["data"] ?? new HttpCookie("data");
            if (cookie.Value != null && cookie.Value != "")
            {
                string idUsuario = cookie.Value;
                intIdUsuario = int.Parse(idUsuario);
                var result = (from user in db.mUsuarios
                              where user.UsuarioId == intIdUsuario
                              select user).FirstOrDefault();

                ViewBag.IdUsuario = result.UsuarioId;
                ViewBag.Rol = result.mRoles.NombreRol;
                // Se comenta esta linea, ya que no se va a obtener el id del jefe desde aqui.
                // Se va a hacer desde la seleccion del proyecto
                //if (result.idRol == Utils.Constantes.CONST_ROL_DESIGNER_ID) 
                //{
                //    ViewBag.IdJefe = result.idJefeUsuario;
                //}
                //if (result.idRol == Constantes.CONST_ROL_SUPPLIER)
                //{
                //    ViewBag.IdProveedor = result.idProveedor;
                //}
                ViewBag.UserName = result.NombresUsuario + " " + result.ApellidosUsuario;
                ViewBag.UserAvatar = result.ImagenUsuario == null ? "../Content/images/avatar.jpg" : result.ImagenUsuario;

                Response.Cookies.Add(cookie);
            }

            if (intIdUsuario != 0)
            {
                var usr = (from user in db.mUsuarios
                           where user.UsuarioId == intIdUsuario
                           select user).FirstOrDefault();

                // Pantalla de Line Builder
                //int[] arrLBs = new int[] 
                //{
                //    Constantes.CONST_ROL_LINEBUILDER_ID,
                //    Constantes.CONST_ROL_DESIGNER_ID,
                //    Constantes.CONST_ROL_LINEBUILDERCORD_ID,
                //    Constantes.CONST_ROL_PROJECTCOORD_ID,
                //    Constantes.CONST_ROL_ASIASUB_ID,
                //    Constantes.CONST_ROL_PRODUCTSUB_ID
                //};

                //// Pantalla de Proveedor
                //int[] arrSuppliers = new int[]
                //{
                //    Constantes.CONST_ROL_SUPPLIER
                //};

                //int[] arrCustomers = new int[]
                //{
                //    Constantes.CONST_ROL_CUSTOMERS_ID
                //};


                //if (arrLBs.Contains(usr.idRol.Value))
                //{
                //    ViewBag.Module = "Inicio";
                //    return View("LineBuilder");
                //}
                //else if (arrSuppliers.Contains(usr.idRol.Value))
                //{
                //    ViewBag.Module = "Inicio";
                //    return View("Proveedor");
                //}
                //else if (arrCustomers.Contains(usr.idRol.Value))
                //{
                //    ViewBag.Module = "Inicio";
                //    return View("Customer");
                //}
                //else
                //{
                ViewBag.Module = "Inicio";
                return View();
                //}
            }
            else
            {
                System.Web.Security.FormsAuthentication.SignOut();
                return RedirectToAction("Inicio", "Principal");
            }
        }

        [OutputCache(CacheProfile = "Long", Location = OutputCacheLocation.Client)]
        public PartialViewResult _sideBar(int id, string lang, string type)
        {
            ViewBag.idUser = id;
            ViewBag.lang = lang.Length > 0 ? lang + "/" : lang;

            ViewBag.type = type;

            var lstPermisos = SeguridadData.CargaUsuarioPermisos(id);

            return PartialView(lstPermisos);
        }

        //[HttpGet]
        //public ActionResult SeleccionarManual()
        //{
        //    var objManual = new catManuales();
        //    ViewBag.Titulo = "Choose an owner´s manual";
        //    return PartialView("_SeleccionarManual", objManual);
        //}


        //[HttpGet]
        //public JsonResult CargaManuales()
        //{
        //    try
        //    {
        //        var lstManuales = new List<catManuales>();

        //        var result = (from man in db.catManuales
        //                      select man).OrderBy(b => b.orden).ToList();

        //        lstManuales = result;
        //        return Json(new { Success = true, data = result }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception exp)
        //    {
        //        return (Json(new { Success = false, Message = exp.Message }, JsonRequestBehavior.AllowGet));
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
