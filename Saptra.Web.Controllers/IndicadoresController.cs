///Propósito:Controlador para la consulta de incidencias
///Fecha creación: 25/Septiembre/18
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
using OfficeOpenXml;
using System.Threading.Tasks;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Style;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Text.RegularExpressions;
//using Spire.Xls;
//using Spire.Pdf;



using Saptra.Web.Data;
using Saptra.Web.Models;
using Saptra.Web.Utils;
//using Spire.Xls.Converter;

namespace Sispro.Web.Controllers
{
    public class IndicadoresController : Controller
    {
        private Inaeba_SaptraEntities db = new Inaeba_SaptraEntities();

        #region NIDICADORES
        public ActionResult Index()
        {
            return View();
        }
        #endregion


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