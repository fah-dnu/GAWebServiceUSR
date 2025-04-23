using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNU.Usuarios.DataContract.Response
{
    public class ResponseValidarCredenciales : ResponseGral
    {
        public string UserID { get; set; }
    }
}
