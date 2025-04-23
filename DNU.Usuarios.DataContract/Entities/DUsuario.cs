using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNU.Usuarios.DataContract.Entities
{
    public class DUsuario
    {
        public string UserID { get; set; }
        public string Nombre { get; set; }
        public string PrimerApellido { get; set; }
        public string SegundoApellido { get; set; }
        public string NombreUsuario { get; set; }
        public string Movil { get; set; }
        public string AppId { get; set; }
        public string AppIdDescripcion { get; set; }
        public int StatusHashIPSecurity { get; set; }
        public List<DRolesDisponibless> Roles { get; set; }
    }
}
