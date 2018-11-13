using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using System.Net;

namespace Saptra.Web.Controllers
{
    class MailController : Controller
    {
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
            MailMessage message = new MailMessage();
            MailAddress From = new MailAddress(from);
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
            client.Port = 587;
            client.Host = "smtp.gmail.com";
            client.EnableSsl = true;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential("sieron.celula@gmail.com", "C123456c");
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
            catch (Exception ex) 
            {
                return "Error al enviar el correo";
            } 
            
        }

    }
}
