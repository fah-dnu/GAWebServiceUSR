using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using DNU.Usuarios.Common.Utilerias;
using DNU.Usuarios.DataContract.Entities;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace DNU.Usuarios.DataContract.BD
{
    public class Filtros
    {

       

        public static List<Filtro> ObtieneFiltros(Guid AppID, Guid elUser, Guid elUserTemporal, Logueo log)
        {
            DataSet losDatos = null;
            List<Filtro> Respuesta = new List<Filtro>();

            try
            {

                Dictionary<Guid, DAplicacion> lasApps = new Dictionary<Guid, DAplicacion>();
                SqlDatabase database = new SqlDatabase(DBCentralApp.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_ObtieneFiltroDeUsuario");
                database.AddInParameter(command, "@AppID", DbType.Guid, AppID);
                database.AddInParameter(command, "@UserId", DbType.Guid, elUser);

                losDatos = database.ExecuteDataSet(command);

                if (null != losDatos)
                {
                    for (int k = 0; k < losDatos.Tables[0].Rows.Count; k++)
                    {
                        
                        Filtro unFiltro = new Filtro();
                        unFiltro.Campo = losDatos.Tables[0].Rows[k]["FieldName"] == null ? "" : (String)losDatos.Tables[0].Rows[k]["FieldName"];
                        unFiltro.AppID = losDatos.Tables[0].Rows[k]["ApplicationId"] == null ? new Guid(): (Guid)losDatos.Tables[0].Rows[k]["ApplicationId"];
                        unFiltro.Tabla = losDatos.Tables[0].Rows[k]["TableName"] == null ? "" : (String)losDatos.Tables[0].Rows[k]["TableName"];
                        unFiltro.Valor = losDatos.Tables[0].Rows[k]["Value"] == null ? "" : (String)losDatos.Tables[0].Rows[k]["Value"];
                        unFiltro.ConexionParaMigrar = losDatos.Tables[0].Rows[k]["DataBaseMigrateFilter"] == null ? "" : (String)losDatos.Tables[0].Rows[k]["DataBaseMigrateFilter"];
                        unFiltro.Permitir = losDatos.Tables[0].Rows[k]["Permitir"] == null ? false : (bool)losDatos.Tables[0].Rows[k]["Permitir"];
                        unFiltro.minExpiracion = 1000;// Int32.Parse(Configuracion.Get(AppID, "MinExpiracionUserTemp").Valor);
                        unFiltro.UsuarioCAPP = elUser.ToString();
                        unFiltro.UsuarioTemp = elUserTemporal;
                        
                        Respuesta.Add(unFiltro);
                         
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw new Exception("Ha sucedido un error al obtener las propiedades de la Aplicacion: " + ex);
            }

            return Respuesta;
        }



    }
}
