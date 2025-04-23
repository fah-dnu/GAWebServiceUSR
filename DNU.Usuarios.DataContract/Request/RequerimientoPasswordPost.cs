using DNU.Usuarios.DataContract.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNU.Usuarios.DataContract.Request
{
    public class RequerimientoPasswordPost : RequerimientoPasswordPut
    {
        public string PasswordActual { get; set; }
    }
}
