using DNU.Usuarios.DataContract.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNU.Usuarios.DataContract.Request
{
    public class RequerimientoUsuariosPost
    {
        public string Nombre { get; set; }
        public string PrimerApellido { get; set; }
        public string SegundoApellido { get; set; }
        public string NombreUsuario { get; set; }
        public string Password { get; set; }
        public string Aplicacion { get; set; }
        public string Movil { get; set; }
        public string AdministradorID { get; set; }
        public List<DRolesDisponibless> Roles { get; set; }
    }

    public class RequerimientoUsuariosPostV2
    {
        public string Nombre { get; set; }
        public string PrimerApellido { get; set; }
        public string SegundoApellido { get; set; }
        public string NombreUsuario { get; set; }
        public string Password { get; set; }
        public string Aplicacion { get; set; }
        public string Movil { get; set; }
        public string AdministradorID { get; set; }
        public List<DRolesDisponibless> Roles { get; set; }
        public string CodigoDescuento { get; set; }
    }
}
