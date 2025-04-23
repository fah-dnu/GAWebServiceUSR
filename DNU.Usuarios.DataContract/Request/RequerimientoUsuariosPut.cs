using DNU.Usuarios.DataContract.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNU.Usuarios.DataContract.Request
{
    public class RequerimientoUsuariosPut
    {
        public string UserID { get; set; }
        public string Nombre { get; set; }
        public string PrimerApellido { get; set; }
        public string SegundoApellido { get; set; }
        public string Aplicacion { get; set; }
        public string Movil { get; set; }
        public string AdministradorID { get; set; }
        public List<DRolesDisponibless> Roles { get; set; }
        public bool Desactivar { get; set; }
        public string Usuario { get; set; }
    }
}
