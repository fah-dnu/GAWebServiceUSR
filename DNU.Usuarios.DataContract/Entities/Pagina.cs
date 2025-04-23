using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DNU.Usuarios.DataContract.Entities
{
    public class Pagina
    {
        public String Nombre { get; set; }
        public String URL { get; set; }
        public Guid Padre { get; set; }
        public Guid MenuId { get; set; }
        public String RolID { get; set; }
        public Dictionary<Guid,Pagina> Hijos = new Dictionary<Guid,Pagina>();
        public Int32 Icono { get; set;}
        public int? OrdenDespliegue { get; set; }

        public Pagina(String _RolID, String _Nombre,
         String _URL, Guid _padre, Guid _MenuId,
         Int32 _Icono)
        {
            Nombre = _Nombre;

          String unfile=  Path.GetFileName(_URL);

          if (unfile.Trim().Length>0)
            {
                URL = _URL;
            }
            else
            {
                URL = "";
            }

            Icono = _Icono;
            Padre = _padre;
         MenuId=_MenuId;
         RolID = _RolID;
        }

        public Pagina( String _Nombre,
       String _URL, Guid _padre, Guid _MenuId,
       Int32 _Icono)
        {


            Nombre = _Nombre;

            String unfile = Path.GetFileName(_URL);

            if (unfile.Trim().Length > 0)
            {
                URL = _URL;
            }
            else
            {
                URL = "";
            }

            Icono = _Icono;
            Padre = _padre;
            MenuId = _MenuId;
        }

        public Pagina(String _Nombre,
    String _URL, Guid _padre, Guid _MenuId,
    Int32 _Icono, int? _OrdenDespliegue)
        {


            Nombre = _Nombre;

            String unfile = Path.GetFileName(_URL);

            if (unfile.Trim().Length > 0)
            {
                URL = _URL;
            }
            else
            {
                URL = "";
            }

            Icono = _Icono;
            Padre = _padre;
            MenuId = _MenuId;
            OrdenDespliegue = _OrdenDespliegue;
        }
    }

   
}
