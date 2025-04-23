using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNU.Usuarios.DataContract.Entities
{
    public class DMenu
    {
        public string ID_Aplicacion { get; set; }
        public string ID_Menu { get; set; }
        public string ID_MenuPadre { get; set; }
        public string Nombre { get; set; }
        public string Path { get; set; }
        public string NumeroIcono { get; set; }
        public string OrdenDespliegue { get; set; }
    }
}
