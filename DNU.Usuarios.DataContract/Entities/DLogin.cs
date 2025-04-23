using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNU.Usuarios.DataContract.Entities
{
    public class DLogin
    {
        public int CodigoRespuesta { get; set; }
        public string Mensaje { get; set; }
        public string UserID { get; set; }
        public string NombreUsuario { get; set; }
        public string PrimerApellido { get; set; }
        public string SegundoApellido { get; set; }
        public string Token { get; set; }
        public string pwd { get; set; }
    }
}
