using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNU.Usuarios.DataContract.Entities
{
    public class DRol
    {
        public string RollID { get; set; }
        public string Clave { get; set; }
        public string Nombre { get; set; }
        public List<DAplicacion> Aplicaciones { get; set; }
    }
}
