using DNU.Usuarios.DataContract.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNU.Usuarios.DataContract.Entities
{
    public class PwdUpdate : ResponseGral
    {
        public string PwdSalt { get; set; }
        public string Mixed { get; set; }
        public int StatusHashIPSecurity { get; set; }
    }
}
