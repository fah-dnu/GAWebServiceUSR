using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DNU.Usuarios.DataContract.Entities
{
   public class Filtro
    {
        public String Tabla { get; set; }
        public String Campo { get; set; }
        public String Valor { get; set; }
        public bool Permitir { get; set; }
        public String UsuarioCAPP{ get; set; }
        public String Descripcion { get; set; }
        public String ConexionName { get; set; }
        public String StoredProcedure { get; set; }
        public Guid UsuarioTemp { get; set; }
        public Guid AppID { get; set; }
        public int minExpiracion { get; set; }
        public Guid ID_Filtro { get; set; }
        public String ConexionParaMigrar { get; set; }
    }
}
