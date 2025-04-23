using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNU.Usuarios.DataContract.Request
{
    public class RequerimientoSMSPut
    {
        public string UserID { get; set; }
        public string NombreUsuario { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public string Tarjeta { get; set; }
    }

    public class RequerimientoSMSPutV2
    {
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public string Token { get; set; }
    }
}
