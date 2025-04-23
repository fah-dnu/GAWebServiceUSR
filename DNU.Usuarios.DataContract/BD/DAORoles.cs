using DNU.Usuarios.Common.Utilerias;
using DNU.Usuarios.DataContract.Entities;
using DNU.Usuarios.DataContract.Response;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DNU.Usuarios.DataContract.Request;
using System.Collections;
using System.Data.SqlClient;

namespace DNU.Usuarios.DataContract.BD
{
    public class DAORoles
    {
        public static ResponseRolesGet ObtieneRolesDisponibles(string IDSolicitud, Logueo log)
        {
            ResponseRolesGet response = new ResponseRolesGet();

            try
            {
                SqlDatabase database = new SqlDatabase(DBCentralApp.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("wsU_ObtieneRoles");
                database.AddInParameter(command, "@IDSolicitud", DbType.String, IDSolicitud);
                DataSet ds = database.ExecuteDataSet(command);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    response.IDSolicitud = IDSolicitud;
                    List<DRolesDisponibles> listaRoles = new List<DRolesDisponibles>();

                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        DRolesDisponibles rol = new DRolesDisponibles();

                        rol.ID = (Guid)row["IDRol"];
                        rol.Nombre = row["Nombre"].ToString();
                        rol.Descripcion = row["Descripcion"].ToString();
                        listaRoles.Add(rol);
                    }

                    response.Roles = listaRoles;
                }
            }
            catch (Exception ex)
            {
                log.Error($"[ObtieneRolesDisponibles] [{ex.Message}]");
                response.CodigoRespuesta = 99;
                response.Mensaje = ex.Message;
            }
            
            return response;
        }
    }
}
