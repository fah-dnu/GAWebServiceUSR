using DNU.Usuarios.Common;
using DNU.Usuarios.Common.Utilerias;
using DNU.Usuarios.DataContract.Entities;
using DNU.Usuarios.DataContract.Response;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace DNU.Usuarios.APIService.Negocio
{
    public class Operaciones
    {
        public static string convierteStringComasRoles(List<DRolesDisponibless> parametro)
        {
            string retorno = null;
            if (parametro != null)
            {
                foreach (var valor in parametro)
                {
                    retorno = retorno + valor.RolID + ",";
                }

                retorno = retorno.Remove(retorno.Length - 1);
            }

            return retorno;
        }

        public static ResponseUsuariosGet obtieneDatosUsuarios(DataTable dtInfo, Logueo log)
        {
            ResponseUsuariosGet response = new ResponseUsuariosGet();
            if (dtInfo.Columns.Count == 2)
            {
                response.CodigoRespuesta = Convert.ToInt32(dtInfo.Rows[0]["codigoRespuesta"]);
                response.Mensaje = dtInfo.Rows[0]["mensaje"].ToString();
            }
            else
            {
                DUsuario usu = new DUsuario();
                List<DRolesDisponibless> lstRoles = new List<DRolesDisponibless>();
                
                foreach (DataRow datos in dtInfo.Rows)
                {
                    DRolesDisponibless rol = new DRolesDisponibless();
                    response.CodigoRespuesta = Convert.ToInt32(datos["codigoRespuesta"]);
                    response.Mensaje = datos["mensaje"].ToString();
                    usu.UserID = datos["UserId"].ToString();
                    usu.Nombre = datos["Nombre"].ToString();
                    usu.PrimerApellido = datos["Apaterno"].ToString();
                    usu.SegundoApellido = datos["Amaterno"].ToString();
                    usu.NombreUsuario = datos["UserName"].ToString();
                    usu.Movil = datos["MobileAlias"].ToString();
                    usu.AppId = datos["ApplicationId"].ToString();
                    usu.AppIdDescripcion = datos["descAppId"].ToString();
                    usu.StatusHashIPSecurity = (int)datos["StatusHashIPSecurity"];
                    rol.RolID = datos["RoleId"].ToString();
                    rol.Descripcion = datos["Description"].ToString();

                    lstRoles.Add(rol);
                }
                usu.Roles = lstRoles;
                response.Usuario = usu;
            }
          //  log.EntradaSalida("[obtieneDatosUsuarios] " + JsonSerializer.Serialize(response), "DNU.Usuarios", true);
            return response;
        }
    }
}