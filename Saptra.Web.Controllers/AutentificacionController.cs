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
using System.IO;

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
                //EncriptarController DesencriptaController = new EncriptarController();
                string contrasenaRec;
                contrasenaRec = EncriptarController.GetMD5(model.Contrasena);

                if (ModelState.IsValid)
                {
                    string agentName = model.NombreUsuario;

                    var result = (from user in db.mUsuarios
                                  where user.LoginUsuario == agentName && user.EstatusId == 5
                                  select user).FirstOrDefault();

                    if (result != null)
                    {
                        if (result.PasswordUsuario == contrasenaRec)
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
                ViewBag.error = "Login:" + "Error al obtener la información";
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

        [AllowAnonymous]
        [HttpPost]
        public JsonResult EnviarEmail(Models.Login objLogin)
        
{

            try
            {
                var result = from u in db.mUsuarios where (u.EmailUsuario == objLogin.Email.Trim()) select u;
                if (result.Count() != 0)
                {
                    var dbUsu = result.First();
                    var objResetPassword = new mResetPassword();

                    objResetPassword.Id = Guid.NewGuid();
                    objResetPassword.UsuarioId = dbUsu.UsuarioId;
                    objResetPassword.Fecha = DateTime.Now;
                    objResetPassword.Liga = Request.Url.Scheme + "://" + Request.Url.Authority + "/Autentificacion/ResetPassword";
                    objResetPassword.EstatusId = 5;


                    db.mResetPassword.Add(objResetPassword);
                    db.SaveChanges();

                    string from = "sieron.celula@gmail.com";
                    List<string> ListaTo = new List<string>();
                    List<string> ListaCc = new List<string>();
                    List<string> ListaCco = new List<string>();
                    ListaTo.Add(objLogin.Email.Trim());

                    string subject = "Saptra Soporte";
                    string body = "<!DOCTYPE html>" +
                                    "<html lang=\"en\">" +
                                    "<head>" +
                                        "<meta charset=\"utf-8\">" +
                                        "<meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\">" +
                                        "<meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">" +
                                        "<style type=\"text/css\">" +
                                            " body {" +
                                            " font-family: \"Helvetica Neue\",Helvetica,Arial,sans-serif;" +
                                            " font-size: 14px;" +
                                            " line-height: 1.428571429;" +
                                            " color: #333;" +
                                            " background-color: #fff;" +
                                            " }" +
                                            " body {" +
                                            " margin: 0;" +
                                            " }" +
                                            " html {" +
                                            " font-size: 62.5%;" +
                                            " -webkit-tap-highlight-color: rgba(0,0,0,0);" +
                                            " }" +
                                            " *, *:before, *:after {" +
                                            " -webkit-box-sizing: border-box;" +
                                            " -moz-box-sizing: border-box;" +
                                            " box-sizing: border-box;" +
                                            " }" +
                                            " .container {" +
                                            " width: 1170px;" +
                                            " }" +
                                            " .container {" +
                                            " padding-right: 15px;" +
                                            " padding-left: 15px;" +
                                            " margin-right: auto;" +
                                            " margin-left: auto;" +
                                            " }" +
                                            " .container:before, .container:after {" +
                                            " display: table;" +
                                            " content: \" \";" +
                                            " } " +
                                            " .row {" +
                                            " margin-right: -15px;" +
                                            " margin-left: -15px;" +
                                            " }" +
                                            " .col-md-offset-4 {" +
                                            " margin-left: 25%;" +
                                            " }" +
                                            " .col-md-4 {" +
                                            " width: 41.66666666666667%;" +
                                            " position: relative;" +
                                            " min-height: 1px;" +
                                            " padding-right: 15px;" +
                                            " padding-left: 15px;" +
                                            " float: left;" +
                                            " }" +
                                            " .panel-default {" +
                                            " border-color: #ddd;" +
                                            " }" +
                                            " .panel {" +
                                            " margin-bottom: 20px;" +
                                            " background-color: #fff;" +
                                            " border: 1px solid transparent;" +
                                            " border-radius: 4px;" +
                                            " -webkit-box-shadow: 0 1px 1px rgba(0,0,0,0.05);" +
                                            " box-shadow: 0 1px 1px rgba(0,0,0,0.05);" +
                                            " }" +
                                            " .panel-default>.panel-heading {" +
                                            " color: #333;" +
                                            " background-color: #f5f5f5;" +
                                            " border-color: #ddd;" +
                                            " }" +
                                            " .panel-heading {" +
                                            " padding: 10px 15px;" +
                                            " border-bottom: 1px solid transparent;" +
                                            " border-top-right-radius: 3px;" +
                                            " border-top-left-radius: 3px;" +
                                            " }" +
                                            " h3 {" +
                                            " display: block;" +
                                            " font-size: 1.17em;" +
                                            " -webkit-margin-before: 1em;" +
                                            " -webkit-margin-after: 1em;" +
                                            " -webkit-margin-start: 0px;" +
                                            " -webkit-margin-end: 0px;" +
                                            " font-weight: bold;" +
                                            " font-family: \"Helvetica Neue\",Helvetica,Arial,sans-serif;" +
                                            " font-weight: 500;" +
                                            " line-height: 1.1;" +
                                            " color: inherit;" +
                                            " }" +
                                            " .panel-title {" +
                                            " margin-top: 0;" +
                                            " margin-bottom: 0;" +
                                            " font-size: 16px;" +
                                            " color: inherit;" +
                                            " }" +
                                            " .panel-body {" +
                                            " padding: 15px;" +
                                            " }" +
                                            " .btn-block {" +
                                            " display: block;" +
                                            " width: 100%;" +
                                            " padding-right: 0;" +
                                            " padding-left: 0;" +
                                            " }" +
                                            " .btn-primary {" +
                                            " color: #fff;" +
                                            " background-color: #428bca;" +
                                            " border-color: #357ebd;" +
                                            " }" +
                                            " .btn {" +
                                            " display: inline-block;" +
                                            " padding: 6px 12px;" +
                                            " margin-bottom: 0;" +
                                            " font-size: 14px;" +
                                            " font-weight: normal;" +
                                            " line-height: 1.428571429;" +
                                            " text-align: center;" +
                                            " white-space: nowrap;" +
                                            " vertical-align: middle;" +
                                            " cursor: pointer;" +
                                            " background-image: none;" +
                                            " border: 1px solid transparent;" +
                                            " border-radius: 4px;" +
                                            " -webkit-user-select: none;" +
                                            " -moz-user-select: none;" +
                                            " -ms-user-select: none;" +
                                            " -o-user-select: none;" +
                                            " user-select: none;" +
                                            " }" +
                                            " a {" +
                                            " color: #428bca;" +
                                            " text-decoration: none;" +
                                            " }" +
                                            " a {" +
                                            " background: transparent;" +
                                            " }" +
                                            ".text-info {" +
                                               "color: #31708f;" +
                                            "}" +
                                        "</style>" +
                                    "</head>" +
                                    "<body>" +
                                        "<div class=\"container\" id=\"page\">" +
                                            "<div class=\"row\">" +
                                                "<div class=\"col-md-4 col-md-offset-4\">" +
                                                    "<img src=\"http://saptra.gear.host/Content/images/Logo_SAPTRA_min.jpg\" style=\"margin-top:20%; width:100%\" />" +
                                                "</div>" +
                                            "</div>" +
                                            "<div class=\"row\">" +
                                                "<div class=\"col-md-4 col-md-offset-4\">" +
                                                    "<div class=\"login-panel panel panel-default\">" +
                                                        "<div class=\"panel-heading\">" +
                                                            "<h3 class=\"panel-title\">Cambiar mi contraseña</h3>" +
                                                        "</div>" +
                                                        "<div class=\"panel-body\">" +
                                                                "<p class=\"text-info\">Alguien ha solicitado un enlace para cambiar su contraseña.Puede hacerlo a través del botón de abajo.</p>" +
                                                                "<a href=\"" + objResetPassword.Liga + "/" + objResetPassword.Id + "\" class=\"btn  btn-xl btn-primary btn-block\">Cambiar mi contraseña</a>" +
                                                                 "<p class=\"text-info\">Si no ha solicitado esto, ignore este correo electrónico. Su contraseña no cambiará hasta que acceda al enlace anterior y cree uno nuevo.</p>" +
                                                        "</div>" +
                                                    "</div>" +
                                                "</div>" +
                                            "</div>" +
                                        "</div>" +
                                    "</body>" +
                                    "</html>";

                    MailController Mail = new MailController();
                    string resultadoEmail = Mail.EnviarEmail(from, ListaTo, subject, true, body, ListaCc);

                    if (resultadoEmail == "")
                    {
                        return Json(new { Success = true, Message = "Mensaje Enviado! Verifica tú correo." });
                    }
                    else
                    {
                        return Json(new { Success = false, Message = resultadoEmail });
                    }
                }
                else
                {
                    return Json(new { Success = false, Message = "Correo no encontradod" });
                }
            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = "Error al obtener la información" });
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult ResetPassword(string NuevoPassword, string confirmaPassword, mResetPassword model)
        {
            ViewBag.errorResetPassword = string.Empty;

            var objResetPassword = (from liga in db.mResetPassword
                                    where liga.Id == model.Id
                                    select liga).FirstOrDefault();

            try
            {
                if (NuevoPassword.Trim() == confirmaPassword.Trim())
                {
                    if (objResetPassword != null)
                    {
                        if (objResetPassword.EstatusId == 5)
                        {
                            DateTime f1 = Convert.ToDateTime(objResetPassword.Fecha).AddHours(3);
                            DateTime f2 = DateTime.Now;

                            var result = DateTime.Compare(f2, f1);
                            if (result < 0)
                            {

                                var Usuario = (from user in db.mUsuarios
                                               where user.UsuarioId == objResetPassword.UsuarioId && user.EstatusId == 5
                                               select user).FirstOrDefault();
                                string contrasenaRec;
                                contrasenaRec = EncriptarController.GetMD5(confirmaPassword);
                                Usuario.PasswordUsuario = contrasenaRec;
                                objResetPassword.EstatusId = 6;
                                db.SaveChanges();
                                ViewBag.successChangePassword = "Contraseña modificada!";
                                ViewBag.error = string.Empty;
                                var objLogin = new Models.Login();
                                return View("Login", objLogin);
                            }
                            else
                            {
                                ViewBag.errorResetPassword = Resources.Autentificacion.msgLigaNoActiva;
                                return View();
                            }
                        }
                        else
                        {
                            ViewBag.errorResetPassword = Resources.Autentificacion.msgLigaNoActiva;
                            return View();
                        }

                    }
                    else
                    {
                        ViewBag.errorResetPassword = Resources.Autentificacion.msgLigaNoExiste;
                        return View();
                    }
                }
                else
                {
                    ViewBag.errorResetPassword = Resources.Autentificacion.msgPasswordNoCoincide;
                    return View();
                }
            }
            catch (Exception exp)
            {
                ViewBag.error = "Login:" + "Error al guardar la información";
            }
            return View();
        }


        [AllowAnonymous]
        public ActionResult ResetPassword(Guid? id)
        {
            var objResetPassword = (from liga in db.mResetPassword
                                    where liga.Id == id
                                    select liga).FirstOrDefault();
            ViewBag.errorResetPassword = string.Empty;

            if (objResetPassword != null)
            {
                if (objResetPassword.EstatusId == 5)
                {
                    return View(objResetPassword);
                }
                else
                {
                    ViewBag.error = Resources.Autentificacion.msgLigaNoActiva;
                    ViewBag.successChangePassword = string.Empty;
                    var objLogin = new Models.Login();

                    return View("Login", objLogin);
                }
            }
            else
            {
                ViewBag.error = Resources.Autentificacion.msgLigaNoExiste;
                ViewBag.successChangePassword = string.Empty;
                var objLogin = new Models.Login();

                return View("Login", objLogin);
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
