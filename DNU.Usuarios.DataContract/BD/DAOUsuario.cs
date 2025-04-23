using DALCentralAplicaciones.Entidades;
using DNU.Usuarios.Common.Utilerias;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace DNU.Usuarios.DataContract.BD
{
    public class DAOUsuario
    {
        public static Usuario ObtieneCaracteristicasUsuario(string UserID, Logueo log)
        {
            try
            {
                Usuario elUsuarioDB = new Usuario();
                SqlDatabase database = new SqlDatabase(DBCentralApp.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_ObtienePerfilesUsuario");
                database.AddInParameter(command, "@Usuario", DbType.String, UserID);
                DataSet losDatos = database.ExecuteDataSet(command);

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@Usuario=" + UserID);

                if (losDatos.Tables[0].Rows.Count > 0)
                {
                    elUsuarioDB.ClaveUsuario = UserID;
                    elUsuarioDB.Email = losDatos.Tables[0].Rows[0]["email"].ToString();
                    elUsuarioDB.UsuarioTemp = (Guid)losDatos.Tables[0].Rows[0]["UserTemp"];
                    elUsuarioDB.ID_Colectiva = (Int64)losDatos.Tables[0].Rows[0]["ID_Colectiva"];
                    elUsuarioDB.ClaveColectiva = (String)losDatos.Tables[0].Rows[0]["ClaveTipoColectiva"];
                    //elUsuarioDB.StatusHashIPSecurity = (int)losDatos.Tables[0].Rows[0]["StatusHashIPSecurity"];

                    for (int k = 0; k < losDatos.Tables[0].Rows.Count; k++)
                    {
                        elUsuarioDB.Roles.Add(losDatos.Tables[0].Rows[k]["RoleName"] == null ? "" : (string)losDatos.Tables[0].Rows[k]["RoleName"]);
                        elUsuarioDB.UsuarioId = (Guid)losDatos.Tables[0].Rows[k]["UserId"];
                    }
                }

                return elUsuarioDB;
            }
            catch (Exception err)
            {
                log.Error("[ObtieneCaracteristicasUsuario] [" + err.Message + "]");
                throw err;
            }
        }
    }
}

