using System;

namespace DNU.Usuarios.DataContract.Entities
{
    public class DAplicaciones
    {
        public Guid ID { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Icono { get; set; }
        public string OrdenDespliegue { get; set; }
    }
}
