using DNU.Usuarios.DataContract.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNU.Usuarios.DataContract.Request
{
    public class RequerimientoPasswordPut
    {
        public string UserID { get; set; }
        public string NombreUsuario { get; set; }
        public string PasswordNuevo { get; set; }
        public string Aplicacion { get; set; }
        public string UserIP { get; set; } = Autenticacion.Hashing.GetClientIp();
        public string Email { get; set; }
    }
}
