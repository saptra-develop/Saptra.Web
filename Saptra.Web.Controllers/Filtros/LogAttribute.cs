///Propósito: Datos de la configuración
///Fecha creación: 05/Septiembre/2016
///Creador: David Galvan
///Fecha modifiacción: 
///Modificó:
///Dependencias de conexiones e interfaces: 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;

using Saptra.Web.Data;
using Saptra.Web.Models;


namespace Saptra.Web.Controllers
{
    public class LogAttribute: ActionFilterAttribute
    {

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Log(filterContext.RouteData, filterContext.HttpContext);

            base.OnActionExecuting(filterContext);
        }

        private async Task Log(System.Web.Routing.RouteData routeData, HttpContextBase httpContext)
        {
            if (httpContext.Request.RequestType == "POST")
            {
                string userIP = httpContext.Request.UserHostAddress;
                string userName = httpContext.User.Identity.Name;
                string reqType = httpContext.Request.RequestType;
                string reqData = GetRequestData(httpContext);
                string controller = routeData.Values["controller"].ToString();
                string action = routeData.Values["action"].ToString();

                //SAVE LOG
                try
                {
                   Inaeba_SaptraEntities db = new Inaeba_SaptraEntities();
                    db.mBitacora.Add(new mBitacora()
                    {
                        ExecuteDate = DateTime.Now,
                        Ip = userIP,
                        Username = userName,
                        Reqtype = reqType,
                        Reqdata = reqData,
                        Controller = controller,
                        Action = action
                    });
                    await db.SaveChangesAsync();
                    db.Dispose();
                }
                catch { }
            }
        }

        private string GetRequestData(HttpContextBase context)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < context.Request.QueryString.Count; i++)
            {
                sb.AppendFormat("Key={0}, Value={1}<br/>", context.Request.QueryString.Keys[i], context.Request.QueryString[i]);
            }

            for (int i = 0; i < context.Request.Form.Count; i++)
            {
                sb.AppendFormat("Key={0}, Value={1}<br/>", context.Request.Form.Keys[i], context.Request.Form[i]);
            }

            return sb.ToString();
        }
        
    }
}
