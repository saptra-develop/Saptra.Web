using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.Mvc;
using Saptra.Web.Data;
using Saptra.Web.Models;
using System.Net.Mail;
using System.Net;

namespace Saptra.Web.Controllers
{
    public class AutentificacionController : Controller
    {
        private Inaeba_SaptraEntities db = new Inaeba_SaptraEntities();

        [AllowAnonymous]
        public ActionResult Login()
        {
            ViewBag.error = string.Empty;
            ViewBag.successChangePassword = string.Empty;
            var objLogin = new Models.Login();

            return View(objLogin);
        }


        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(Models.Login model, string returnUrl)
        {
            ViewBag.error = string.Empty;
            try
            {
                if (ModelState.IsValid)
                {
                    string agentName = model.NombreUsuario;

                    var result = (from user in db.mUsuarios
                                  where user.LoginUsuario == agentName && user.EstatusId == 5
                                  select user).FirstOrDefault();

                    if (result != null)
                    {
                        if (result.PasswordUsuario == model.Contrasena)
                        {
                            FormsAuthentication.SetAuthCookie(result.UsuarioId.ToString(), false);
                            var objCookie = new HttpCookie("data", result.UsuarioId.ToString());
                            Response.Cookies.Add(objCookie);

                            if (returnUrl != null)
                                return Redirect(returnUrl);
                            else
                                return Redirect(Url.Content("~/Principal/Inicio"));
                        }
                        else
                        {
                            ViewBag.error = Resources.Autentificacion.msg2;
                            ViewBag.successChangePassword = string.Empty;
                            return View();
                        }
                    }
                    else
                    {
                        ViewBag.error = Resources.Autentificacion.msg2;
                        ViewBag.successChangePassword = string.Empty;
                        return View();
                    }
                }
                else
                    ViewBag.error = Resources.Autentificacion.msg1;
                ViewBag.successChangePassword = string.Empty;
            }
            catch (Exception exp)
            {
                ViewBag.error = "Login:" + exp.Message;
                ViewBag.successChangePassword = string.Empty;
            }

            return View();
        }

        public ActionResult LogOut()
        {
            var cookie = HttpContext.Request.Cookies["data"];
            cookie.Expires = DateTime.Now.AddDays(-1);

            FormsAuthentication.SignOut();
            return RedirectToAction("Inicio", "Principal");
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
