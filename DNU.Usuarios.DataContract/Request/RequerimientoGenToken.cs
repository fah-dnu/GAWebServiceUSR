using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNU.Usuarios.DataContract.Request
{
    public class RequerimientoGenToken
    {
        public string Usuario { get; set; }
        public string UserIP { get; set; } = Autenticacion.Hashing.GetClientIp();
    }
}
