using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNU.Usuarios.DataContract.Request
{
    public class RequerimientoUsuariosDelete
    {
        public string AdminId { get; set; }
        public string UserId { get; set; }
        public string AppId { get; set; }
    }
}
