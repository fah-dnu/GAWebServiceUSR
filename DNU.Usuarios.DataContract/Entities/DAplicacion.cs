using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNU.Usuarios.DataContract.Entities
{
    public class DAplicacion
    {
        public string ID_Aplicacion { get; set; }
        public string Nombre { get; set; }
        public string URL { get; set; }
        public string Icono { get; set; }
        public string OrdenDespliegue { get; set; }
        public List<DMenu> Menu { get; set; }

        public String CadenaConexionFiltro;
    }
}
