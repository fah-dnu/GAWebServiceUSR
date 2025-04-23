using DNU.Usuarios.DataContract.Entities;
using System.Collections.Generic;

namespace DNU.Usuarios.DataContract.Response
{
    public class ResponseRolesGet : ResponseGral
    {
        public string IDSolicitud { get; set; }
        public List<DRolesDisponibles> Roles { get; set; }
    }
}
