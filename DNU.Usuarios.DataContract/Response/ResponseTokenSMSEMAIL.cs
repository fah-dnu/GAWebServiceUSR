using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNU.Usuarios.DataContract.Response
{
    public class ResponseTokenSMSEMAIL : ResponseGral
    {
        public string Token { get; set; }
        public string Correo { get; set; }
        public string Telefono { get; set; }
    }
    
}
