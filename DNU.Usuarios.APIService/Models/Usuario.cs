using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DNU.Usuarios.APIService.Models
{
    public class Usuario
    {
        public string NombreUsuario { get; set; }
        public int NumeroReintento { get; set; }
        public DateTime FechaBloqueo { get; set; }
    }
}