using DNU.Usuarios.Common.Utilerias;
using DNU.Usuarios.DataContract.Entities;
using DNU.Usuarios.DataContract.Response;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace DNU.Usuarios.DataContract.BD
{
    public class DAOAplicaciones
    {
        public static ResponseAplicacionesGet ObtieneAplicaciones(string IDSolicitud, Logueo log)
        {
            try
            {
                ResponseAplicacionesGet response = new ResponseAplicacionesGet();
                SqlDatabase database = new SqlDatabase(DBCentralApp.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("wsU_ObtieneAplicaciones");
                database.AddInParameter(command, "@IDSolicitud", DbType.String, IDSolicitud);
                DataSet ds = database.ExecuteDataSet(command);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    response.IDSolicitud = IDSolicitud;
                    List<DAplicaciones> listaAplicaciones = new List<DAplicaciones>();

                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        DAplicaciones app = new DAplicaciones();

                        app.ID = (Guid)row["IDAplicacion"];
                        app.Nombre = row["Nombre"].ToString();
                        app.Descripcion = row["Descripcion"].ToString();
                        app.Icono = row["Icono"].ToString();
                        app.OrdenDespliegue = row["OrdenDespliegue"].ToString();
                        listaAplicaciones.Add(app);
                    }

                    response.Aplicaciones = listaAplicaciones;
                }
                return response;
            }
            catch (Exception ex)
            {
                log.Error($"[ObtieneAplicaciones] [{ex.Message}]");
                throw ex;
            }
        }
    }
}
