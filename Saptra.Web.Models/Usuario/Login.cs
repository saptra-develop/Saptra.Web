using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Saptra.Web.Models
{
    public class Login
    {
        [Display(Name = "userName", ResourceType = typeof(Resources.Autentificacion))]
        //[Required(AllowEmptyStrings = false,
        //    ErrorMessageResourceName = "StreetErrorMandatory",
        //    ErrorMessageResourceType = typeof(ViewResources))] 
        public string NombreUsuario { get; set; }

        [Display(Name = "password", ResourceType = typeof(Resources.Autentificacion))]
        public string Contrasena { get; set; }

        public string Email { get; set; }

    }
}