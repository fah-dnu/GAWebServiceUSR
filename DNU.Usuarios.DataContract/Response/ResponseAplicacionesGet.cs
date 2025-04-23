using DNU.Usuarios.DataContract.Entities;
using System;
using System.Collections.Generic;

namespace DNU.Usuarios.DataContract.Response
{
    public class ResponseAplicacionesGet : ResponseGral
    {
        public string IDSolicitud { get; set; }
        public List<DAplicaciones> Aplicaciones { get; set; }
    }
}
