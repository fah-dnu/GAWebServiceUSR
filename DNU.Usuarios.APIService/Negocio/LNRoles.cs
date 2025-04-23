using DNU.Usuarios.Common.Utilerias;
using DNU.Usuarios.DataContract.BD;
using DNU.Usuarios.DataContract.Request;
using DNU.Usuarios.DataContract.Response;
using System;
using System.Collections;

namespace DNU.Usuarios.APIService.Negocio
{
    public class LNRoles
    {
        public static ResponseRolesGet ObtieneRolesDisponibles(string IdSolicitud, Logueo log)
        {
            Hashtable ht = new Hashtable();
            ResponseRolesGet respRoles = new ResponseRolesGet();

            try
            {
                respRoles = DAORoles.ObtieneRolesDisponibles(IdSolicitud, log);

                respRoles.CodigoRespuesta = 0;
                respRoles.Mensaje = "Aprobada";
            }
            catch (Exception ex)
            {
                respRoles.CodigoRespuesta = 99;
                respRoles.Mensaje = "No es posible obtener el listado de roles";
                log.Error("[GET: api/Roles/] " + "[" + ex.Message + "]" + ex.StackTrace);
            }
            
            return respRoles;
        }

        public static ResponseGral AsignaRolesUsuario(RequerimientoRolesPut requerimiento, Logueo log)
        {
            Hashtable ht = new Hashtable();
            ResponseGral respAsignacion = new ResponseGral();

            try
            {
                if (!string.IsNullOrEmpty(requerimiento.IDSolicitud))
                    ht.Add("@IDSolicitud", requerimiento.IDSolicitud);
                if (!string.IsNullOrEmpty(requerimiento.UserID))
                    ht.Add("@IDUsuario", requerimiento.UserID);
                if (!string.IsNullOrEmpty(requerimiento.NombreUsuario))
                    ht.Add("@NombreUsuario", requerimiento.NombreUsuario);
                if (!string.IsNullOrEmpty(requerimiento.RolID))
                    ht.Add("@IDRolAsignar", requerimiento.RolID);

                respAsignacion = SPs.executeSP("wsU_AsignarRolesUsuario", ht, log);
            }
            catch (Exception ex)
            {
                respAsignacion.CodigoRespuesta = 99;
                respAsignacion.Mensaje = $"No es posible asignar el rol al usuario";
                log.Error("[PUT: api/Roles/] " + "[" + ex.Message + "]" + ex.StackTrace);
            }

            return respAsignacion;
        }
    }
}