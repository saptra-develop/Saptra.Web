using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using System.Net;

using Saptra.Web.Data;
namespace Saptra.Web.Controllers
{
    class MailController : Controller
    {
        private Inaeba_SaptraEntities db = new Inaeba_SaptraEntities();
        public void EnviarEmail(MailMessage message)
        {
            SmtpClient client = new SmtpClient();
            client.Port = 587;
            client.Host = "smtp.gmail.com";
            client.EnableSsl = true;
            //client.Timeout = 10000;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential("sieron.proyectos@gmail.com", "C123456c");

            client.Send(message);
        }


        public string EnviarEmail(string from, List<string> addListTo, string Subject, bool isBodyHTML, string Body, List<string> addCc = null, List<string> addCco = null)
        {
            var confCorreo = (from c in db.mCorreo
                              where c.TipoCorreoId == 1 && c.EstatusId == 5
                              select c).FirstOrDefault();

            MailMessage message = new MailMessage();
            MailAddress From = new MailAddress(confCorreo.Correo);
            message.From = From;
            foreach (string address in addListTo)
            {
                MailAddress to = new MailAddress(address);
                message.To.Add(to);
            }
            if (addCc != null)
            {
                foreach (string address in addCc)
                {
                    MailAddress Cc = new MailAddress(address);
                    message.CC.Add(Cc);
                }
            }

            if (addCco != null)
            {
                foreach (string address in addCco)
                {
                    MailAddress Cco = new MailAddress(address);
                    message.To.Add(Cco);
                }
            }
            message.Subject = Subject;
            message.IsBodyHtml = isBodyHTML;
            message.Body = Body;

            SmtpClient client = new SmtpClient();
            client.Port = Convert.ToInt32(confCorreo.Puerto);
            client.Host = confCorreo.Host;
            client.EnableSsl = true;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential(confCorreo.Correo, confCorreo.Contrasena);
            //client.TargetName = "STARTTLS/smtp.gmail.com";

            // Mailtrap
            //client.Port = 2525;
            //client.Host = "smtp.mailtrap.io";
            //client.EnableSsl = true;
            //client.DeliveryMethod = SmtpDeliveryMethod.Network;
            //client.UseDefaultCredentials = false;
            //client.Credentials = new System.Net.NetworkCredential("9f07828f5b638f", "c5aea6f1dd398e");

            try 
            {
                client.Send(message);
                return "";
            }
#pragma warning disable CS0168 // La variable 'ex' se ha declarado pero nunca se usa
            catch (Exception ex) 
#pragma warning restore CS0168 // La variable 'ex' se ha declarado pero nunca se usa
            {
                return "Error al enviar el correo";
            } 
            
        }

    }
}
