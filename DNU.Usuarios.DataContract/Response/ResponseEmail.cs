using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNU.Usuarios.DataContract.Response
{
    public class ResponseEmail : ResponseGral
    {
        public string Correo { get; set; }
        public string idUsuario { get; set; }
        public string Telefono { get; set; }
    }
}
