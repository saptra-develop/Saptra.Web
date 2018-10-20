///Propósito:Controlador para la administracion de periodos
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
    public class TiposFiguraController : Controller
    {
        private Inaeba_SaptraEntities db = new Inaeba_SaptraEntities();


        public ActionResult Figuras()
        {
            return View();
        }

        [HttpGet]
        public JsonResult CargarTiposFigura(int? id, int? idEstatus)
        {
            try
            {
                var result = (from cat in db.cTipoFiguras
                              where cat.EstatusId == (idEstatus == null ? cat.EstatusId : idEstatus)
                              select new
                              {
                                  id = cat.TipoFiguraId,
                                  nombre = cat.DescripcionTipoFigura,
                                  estatus = cat.cEstatus.NombreEstatus
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

        [HttpGet]
        public ActionResult Nuevo()
        {
            var objTiposFigura = new cTipoFiguras();
            ViewBag.Titulo = "Nuevo tipo figura";
            return PartialView("_Nuevo", objTiposFigura);
        }

        [HttpPost]
        public JsonResult Nuevo(cTipoFiguras pobjModelo)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    pobjModelo.FechaCreacion = DateTime.Now;
                    pobjModelo.EstatusId = pobjModelo.EstatusId;
                    db.cTipoFiguras.Add(pobjModelo);
                    db.SaveChanges();

                    return Json(new { Success = true, id = pobjModelo.TipoFiguraId, Message = "guardado correctamente " });
                }
                catch (Exception exp)
                {
                    return Json(new { Success = false, Message = exp.Message });
                }
            }

            return Json(new { Success = false, Message = "Informacion incompleta" });
        }

        [HttpGet]
        public ActionResult Actualizar(int id)
        {
            var objTipoFigura = db.cTipoFiguras.Find(id);
            ViewBag.Titulo = "Actualizar tipo figura";
            return PartialView("_Actualizar", objTipoFigura);
        }

        [HttpPost]
        public JsonResult Actualizar(cTipoFiguras pobjModelo)
        {
            try
            {
                var result = (from ps in db.cTipoFiguras
                              where ps.TipoFiguraId == pobjModelo.TipoFiguraId
                              select ps).ToList();

                //Actualiza
                var dbTemp = result.First();
                dbTemp.DescripcionTipoFigura = pobjModelo.DescripcionTipoFigura;
                dbTemp.EstatusId = pobjModelo.EstatusId;
                db.SaveChanges();


                return Json(new { Success = true, id = pobjModelo.TipoFiguraId, Message = "actualizada correctamente " });

            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = exp.Message });
            }
        }

        [HttpPost]
        public JsonResult Borrar(int id)
        {
            try
            {
                var result = (from usu in db.cTipoFiguras
                              where usu.TipoFiguraId == (id)
                              select usu).FirstOrDefault();

                result.EstatusId = 6;
                db.SaveChanges();

                return Json(new { Success = true, Message = "Se borro correctamente el tipo figura " });
            }
            catch (Exception exp)
            {
                return Json(new { Success = false, Message = exp.Message });
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