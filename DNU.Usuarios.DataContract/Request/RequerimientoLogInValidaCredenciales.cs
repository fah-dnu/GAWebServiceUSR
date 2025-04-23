using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNU.Usuarios.DataContract.Request
{
    public class RequerimientoLogInValidaCredenciales
    {
        public string NombreUsuario { get; set; }
        public string Password { get; set; }
        public string Aplicacion { get; set; }
        public int Cifrado { get; set; }
        private int m_UpdatePwd;

        public int UpdatePwd { get => m_UpdatePwd; set => m_UpdatePwd = value; }
        public string UserIP { get; set; } = Autenticacion.Hashing.GetClientIp();
    }
}
