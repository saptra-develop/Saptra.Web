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
using System.Text;
using System.Security.Cryptography;
using System.Threading.Tasks;


using Saptra.Web.Data;
using Saptra.Web.Models;


namespace Saptra.Web.Controllers
{
    public class EncriptarController : Controller
    {
         public static string GetMD5(string str)
        {
            //Encoding iso = Encoding.GetEncoding("ISO-8859-1");
            //Encoding utf8 = Encoding.UTF8;
            //byte[] utfBytes = utf8.GetBytes(str);
            //byte[] isoBytes = Encoding.Convert(utf8, iso, utfBytes);
            //string pas = iso.GetString(isoBytes);

            MD5 md5 = MD5CryptoServiceProvider.Create();
            //ASCIIEncoding encoding = new ASCIIEncoding();
            Encoding encoding = Encoding.GetEncoding("ISO-8859-1");
            byte[] stream = null;
            StringBuilder sb = new StringBuilder();
            stream = md5.ComputeHash(encoding.GetBytes(str));
            for (int i = 0; i < stream.Length; i++) sb.AppendFormat("{0:x2}", stream[i]);
            return sb.ToString();
        }
    }
}