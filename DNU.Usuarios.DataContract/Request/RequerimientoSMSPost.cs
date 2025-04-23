using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNU.Usuarios.DataContract.Request
{
    public class RequerimientoSMSPost
    {
        public string UserID { get; set; }
        public string NombreUsuario { get; set; }
        public string TokenSMS { get; set; }
        public string Tarjeta { get; set; }
    }
}
