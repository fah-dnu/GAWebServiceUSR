using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Configuration;
using DNU.Usuarios.Common.Utilerias;
using DNU.Usuarios.DataContract.Entities;

namespace DNU.Usuarios.DataContract.BD
{
    public class DAOCatalogos
    {
        //static string _DBAdministradorConsulta = ConfigurationManager.ConnectionStrings["AdministradorConsulta"].ToString();
        //static string _DBAdministradorEscritura = ConfigurationManager.ConnectionStrings["AdministradorEscritura"].ToString();


        public static DataSet ListaAplicaciones(Logueo log)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(DBCentralApp.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_ObtieneAplicaciones");
                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
               log.Error(ex.Message);
                throw ex;
            }
        }

        public static DataSet ListaAplicacionesUsuario(Guid ID_Usuario, Guid AppID, Logueo log)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(DBCentralApp.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_ObtieneAplicacionesUsuario");
                database.AddInParameter(command, "@IDUser", DbType.Guid, ID_Usuario);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);
                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static DataSet ListaPerfiles(Guid UserId, Logueo log)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(DBCentralApp.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_ObtienePerfiles");
                database.AddInParameter(command, "UserId", DbType.Guid, UserId);


                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static DataSet ListaPerfiles(Logueo log)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(DBCentralApp.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_ObtieneTodosPerfiles");


                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static DataSet ObtieneTablasFiltro(Guid AppID, Logueo log)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(DBCentralApp.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_ObtieneTablasFiltro");
                database.AddInParameter(command, "@ApplicationId", DbType.Guid, AppID);

                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static DataSet ListaCamposDeTabla(String Tabla, Guid AppID, Logueo log)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(DBCentralApp.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_ObtieneCampos");
                database.AddInParameter(command, "@ApplicationId", DbType.Guid, AppID);
                database.AddInParameter(command, "@Tabla", DbType.String, Tabla);

                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static DataSet ListaPerfilesUsuario(Guid Usuario, Logueo log)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(DBCentralApp.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_ObtienePerfilesDelUsuario");
                database.AddInParameter(command, "@UserId", DbType.Guid, Usuario);

                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static DataSet ListaTablaValues(Guid UserId, Logueo log)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(DBCentralApp.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_ObtieneTablesValues");
                database.AddInParameter(command, "@UserId", DbType.Guid, UserId);

                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static List<ValorFiltro> ListaCombinacionTablaValues(Guid UserId, Guid TableID, DataTable ValoresPosibles, String FiltroValue, Logueo log)
        {
            try
            {
                DataSet unaDataSet = new DataSet();

                Dictionary<String, ValorFiltro> unListado = new Dictionary<String, ValorFiltro>();

                SqlDatabase database = new SqlDatabase(DBCentralApp.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_ObtieneTablesValues");
                database.AddInParameter(command, "@UserId", DbType.Guid, UserId);
                database.AddInParameter(command, "@TableID", DbType.Guid, TableID);
                //database.AddInParameter(command, "@FiltroValue", DbType.String, FiltroValue);

                unaDataSet = database.ExecuteDataSet(command);

                //llena todos los valores posibles
                if (ValoresPosibles.Rows.Count > 0)
                {
                    for (int k = 0; k < ValoresPosibles.Rows.Count; k++)
                    {
                        ValorFiltro unValorFiltro = new ValorFiltro();

                        unValorFiltro.Permitir = false;
                        unValorFiltro.Value = ValoresPosibles.Rows[k]["Value"].ToString();
                        unValorFiltro.ValueDescription = ValoresPosibles.Rows[k]["ValueDescription"].ToString();
                        unListado.Add(unValorFiltro.Value, unValorFiltro);

                    }
                }

                //de los que ya existen la BD de datos como permitidos entonces los marca como permitidos y se le agregan los demas datos como el ID.
                if (unaDataSet.Tables[0].Rows.Count > 0)
                {
                    for (int k = 0; k < unaDataSet.Tables[0].Rows.Count; k++)
                    {
                        if (unListado.ContainsKey(unaDataSet.Tables[0].Rows[k]["Value"].ToString()))
                        {
                            ValorFiltro unValorFiltro = new ValorFiltro();
                            unValorFiltro = unListado[unaDataSet.Tables[0].Rows[k]["Value"].ToString()];
                            unValorFiltro.Permitir = true;
                            unValorFiltro.ID_Field = unaDataSet.Tables[0].Rows[k]["RegisterId"].ToString();
                            unValorFiltro.RegisterId = unaDataSet.Tables[0].Rows[k]["RegisterId"].ToString();
                            unListado.Remove(unValorFiltro.Value);
                            unListado.Add(unValorFiltro.Value, unValorFiltro);
                        }
                    }
                }

                return unListado.Values.ToList();

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }


        public static DataSet ObtieneUsuarios(Guid UserTempId, Guid AppId, Logueo log)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(DBCentralApp.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_ObtieneUsuarios");
                database.AddInParameter(command, "@AppId", DbType.Guid, AppId);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, UserTempId);

                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

    }
}
