using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DNU.Usuarios.DataContract.Entities
{
    public class ValorFiltro
    {
        public String Value { get; set; }
        public String ValueDescription { get; set; }
        public String ID_Field { get; set; }
        public String RegisterId { get; set; }
        public bool Permitir { get; set; }
    }
}
