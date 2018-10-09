///Propósito:Controlador para la administracion de notificaciones
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
    public class NotificacionesController : Controller
    {
        private Inaeba_SaptraEntities db = new Inaeba_SaptraEntities();

        public ActionResult Index()
        {
            return View();
        }


        public PartialViewResult CargaNotificaciones(int? idUsuario)
        {
            var lstNotificaciones = (from n in db.mNotificaciones
                                where n.EstatusId == 3 
                                && n.UsuarioId == idUsuario
                                select n)
                            .OrderByDescending(not => not.NotificacionId)
                            .ToList();

            return PartialView("_MenuNotificaciones", lstNotificaciones);
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