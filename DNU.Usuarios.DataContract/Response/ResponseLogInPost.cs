using DNU.Usuarios.DataContract.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNU.Usuarios.DataContract.Response
{
    public class ResponseLogInPost : ResponseGral
    {
        public string UserID { get; set; }
        public string UserTemp { get; set; }
        public string NombreUsuario { get; set; }
        public string PrimerApellido { get; set; }
        public string SegundoApellido { get; set; }
        public string Token { get; set; }
        public List<DRol> Roles { get; set; }
    }
}
