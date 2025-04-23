using DNU.Usuarios.Common.Utilerias;
using DNU.Usuarios.DataContract.BD;
using DNU.Usuarios.DataContract.Response;
using System;
using System.Collections;
using System.Data;

namespace DNU.Usuarios.APIService.Negocio
{
    public class LNAplicaciones
    {
        public static ResponseAplicacionesGet getAplicaciones(string IdSolicitud, Logueo log)
        {
            Hashtable ht = new Hashtable();
            ResponseAplicacionesGet respAplicaciones = new ResponseAplicacionesGet();

            try
            {
                respAplicaciones = DAOAplicaciones.ObtieneAplicaciones(IdSolicitud, log);

                respAplicaciones.CodigoRespuesta = 0;
                respAplicaciones.Mensaje = "Aprobada";
            }
            catch (Exception ex)
            {
                respAplicaciones.CodigoRespuesta = 99;
                respAplicaciones.Mensaje = "No es posible obtener el listado de aplicaciones";
                log.Error("[GET: api/Aplicaciones/] " + "[" + ex.Message + "]" + ex.StackTrace);
            }
            
            return respAplicaciones;
        }
    }
}