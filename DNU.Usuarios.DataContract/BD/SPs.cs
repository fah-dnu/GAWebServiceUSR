using System;
using System.Collections;
using DNU.Usuarios.DataContract.Response;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using DNU.Usuarios.DataContract.Entities;
using DNU.Usuarios.Common.Utilerias;
using DNU.Usuarios.Common;

namespace DNU.Usuarios.DataContract.BD
{
    public class SPs
    {
        public static ResponseGral executeSP(string nomSP, Hashtable parametros, Logueo log)
        {


            SqlConnection conexion = new SqlConnection(DBCentralApp.strBDEscritura);
            DataSet retorno = new DataSet();
            Dictionary<string, string> valuesHt = new Dictionary<string, string>();
            try
            {
                log.Evento("[executeSP] [" + nomSP + "] [IP][" + conexion.DataSource + "]");
            }
            catch (Exception ex) {
                log.Error("Ejecucion SP" + "[" + ex.Message + "]" + ex.StackTrace);
            }
            try
            {
                conexion.Open();
                SqlCommand query = new SqlCommand(nomSP, conexion)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 0
                };

                if (parametros != null && parametros.Count > 0)
                {
                    foreach (DictionaryEntry parametro in parametros)
                    {
                        query.Parameters.AddWithValue(parametro.Key.ToString(), parametro.Value);
                        if (!parametro.Key.ToString().Equals("@pwdNuevo"))
                            valuesHt.Add(parametro.Key.ToString(), parametro.Value == null ? "" : parametro.Value.ToString());
                    }
                }
                SqlParameter outparam = query.Parameters.Add("@codigoRespuesta", SqlDbType.VarChar,2);
                outparam.Direction = ParameterDirection.Output;

                SqlParameter outparamMensaje = query.Parameters.Add("@mensaje", SqlDbType.VarChar,-1);
                outparamMensaje.Direction = ParameterDirection.Output;
                //log.Evento("[executeSP] [" + nomSP + "] " + JsonSerializer.Serialize(valuesHt));
                query.ExecuteNonQuery();
                var response = new ResponseGral()
                {
                    CodigoRespuesta = Convert.ToInt32(query.Parameters["@codigoRespuesta"].Value),
                    Mensaje = query.Parameters["@mensaje"].Value.ToString()
                };
              //  log.Evento("[executeSP] [" + nomSP + "] " + JsonSerializer.Serialize(response));
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conexion.Close();
            }

        }

        public static ResponseEmail executeSPAU(string nomSP, Hashtable parametros, Logueo log)
        {
            SqlConnection conexion = new SqlConnection(DBCentralApp.strBDAULectura);
            DataSet retorno = new DataSet();
            Dictionary<string, string> valuesHt = new Dictionary<string, string>();

            try
            {
                log.Evento("[executeSP] [" + nomSP + "] [IP][" + conexion.DataSource + "]");
            }
            catch (Exception ex)
            {
                log.Error("Ejecucion SP" + "[" + ex.Message + "]" + ex.StackTrace);
            }
            try
            {
                conexion.Open();
                SqlCommand query = new SqlCommand(nomSP, conexion)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 0
                };

                if (parametros != null && parametros.Count > 0)
                {
                    foreach (DictionaryEntry parametro in parametros)
                    {
                        SqlParameter paramSSN = query.CreateParameter();
                        paramSSN.ParameterName = parametro.Key.ToString();
                        paramSSN.DbType = DbType.AnsiStringFixedLength;
                        paramSSN.Direction = ParameterDirection.Input;
                        paramSSN.Value = parametro.Value;
                        paramSSN.Size = parametro.Value.ToString().Length;//parametro.longitud;
                        query.Parameters.Add(paramSSN);                  
                    }
                }
                //SqlParameter outparam = query.Parameters.Add("@codigoRespuesta", SqlDbType.VarChar, 2);
                //outparam.Direction = ParameterDirection.Output;

                //SqlParameter outparamMensaje = query.Parameters.Add("@mensaje", SqlDbType.VarChar, -1);
                //outparamMensaje.Direction = ParameterDirection.Output;

                SqlParameter outparamCorreo = query.Parameters.Add("@correo", SqlDbType.VarChar, 50);
                outparamCorreo.Direction = ParameterDirection.Output;

                SqlParameter outparamIdUsuario = query.Parameters.Add("@idUsuario", SqlDbType.VarChar, 50);
                outparamIdUsuario.Direction = ParameterDirection.Output;

                SqlParameter outparamTelefono = query.Parameters.Add("@telefono", SqlDbType.VarChar, 50);
                outparamTelefono.Direction = ParameterDirection.Output;

                ////SqlParameter outparamCorreo = query.Parameters.Add("@correo", SqlDbType.VarChar, 100);
                ////outparamCorreo.Direction = ParameterDirection.Output;


                //   log.Evento("[executeSPToken] [" + nomSP + "] " + JsonSerializer.Serialize(valuesHt));
                query.ExecuteNonQuery();
                var response = new ResponseEmail()
                {
                    //CodigoRespuesta = Convert.ToInt32(query.Parameters["@codigoRespuesta"].Value),
                    //Mensaje = query.Parameters["@mensaje"].Value.ToString(),
                    Correo = query.Parameters["@correo"].Value.ToString(),
                    idUsuario = query.Parameters["@idUsuario"].Value.ToString(),
                    Telefono = query.Parameters["@telefono"].Value.ToString()
                    //      Correo = query.Parameters["@correo"].Value.ToString()
                };

                //   log.Evento("[executeSPToken] [" + nomSP + "] " + JsonSerializer.Serialize(response));
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conexion.Close();
            }

        }

        public static DataTable executeSPDT(string nomSP, Hashtable parametros, Logueo log)
        {
            SqlConnection conexion = new SqlConnection(DBCentralApp.strBDEscritura);
            DataSet retorno = new DataSet();
            Dictionary<string, string> valuesHt = new Dictionary<string, string>();

            string database = conexion.Database;
            string datasource = conexion.DataSource;

            try
            {
                log.Evento("[executeSP] [" + nomSP + "] [IP][" + conexion.DataSource + "]");
            }
            catch (Exception ex)
            {
                log.Error("Ejecucion SP" + "[" + ex.Message + "]" + ex.StackTrace);
            }


            try
            {
                conexion.Open();
                SqlCommand query = new SqlCommand(nomSP, conexion)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 0
                };

                if (parametros != null && parametros.Count > 0)
                {
                    foreach (DictionaryEntry parametro in parametros)
                    {
                        query.Parameters.Add(parametro.Key.ToString(), parametro.Value);
                        if(!parametro.Key.ToString().Equals("@pwd"))
                            valuesHt.Add(parametro.Key.ToString(), parametro.Value == null ? "" : parametro.Value.ToString());
                    }
                }
                SqlDataAdapter da = new SqlDataAdapter(query);
                DataTable dt = new DataTable();
               // log.Evento("[executeSPDT] [" + nomSP + "] " + JsonSerializer.Serialize(valuesHt));
                da.Fill(dt);

                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conexion.Close();
            }

        }

        public static ResponseTokenSMS executeSPToken(string nomSP, Hashtable parametros, Logueo log)
        {
            SqlConnection conexion = new SqlConnection(DBCentralApp.strBDEscritura);
            DataSet retorno = new DataSet();
            Dictionary<string, string> valuesHt = new Dictionary<string, string>();
            try
            {
                log.Evento("[executeSP] [" + nomSP + "] [IP][" + conexion.DataSource + "]");
            }
            catch (Exception ex)
            {
                log.Error("Ejecucion SP" + "[" + ex.Message + "]" + ex.StackTrace);
            }
            try
            {
                conexion.Open();
                SqlCommand query = new SqlCommand(nomSP, conexion)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 0
                };

                if (parametros != null && parametros.Count > 0)
                {
                    foreach (DictionaryEntry parametro in parametros)
                    {
                        query.Parameters.AddWithValue(parametro.Key.ToString(), parametro.Value);
                        valuesHt.Add(parametro.Key.ToString(), parametro.Value == null ? "" : parametro.Value.ToString());
                    }
                }
                SqlParameter outparam = query.Parameters.Add("@codigoRespuesta", SqlDbType.VarChar, 2);
                outparam.Direction = ParameterDirection.Output;

                SqlParameter outparamMensaje = query.Parameters.Add("@mensaje", SqlDbType.VarChar, -1);
                outparamMensaje.Direction = ParameterDirection.Output;

                //SqlParameter outparamToken = query.Parameters.Add("@token", SqlDbType.VarChar, 6);
                //outparamToken.Direction = ParameterDirection.Output;

                //SqlParameter outparamCorreo = query.Parameters.Add("@correo", SqlDbType.VarChar, 100);
                //outparamCorreo.Direction = ParameterDirection.Output;


                //   log.Evento("[executeSPToken] [" + nomSP + "] " + JsonSerializer.Serialize(valuesHt));
                query.ExecuteNonQuery();
                var response = new ResponseTokenSMS()
                {
                    CodigoRespuesta = Convert.ToInt32(query.Parameters["@codigoRespuesta"].Value),
                    Mensaje = query.Parameters["@mensaje"].Value.ToString(),
                    //Token = query.Parameters["@token"].Value.ToString(),
              //      Correo = query.Parameters["@correo"].Value.ToString()
                };

             //   log.Evento("[executeSPToken] [" + nomSP + "] " + JsonSerializer.Serialize(response));
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conexion.Close();
            }

        }

        public static ResponseTokenSMSEMAIL executeSPTokenWithEmail(string nomSP, Hashtable parametros, Logueo log)
        {
            SqlConnection conexion = new SqlConnection(DBCentralApp.strBDEscritura);
            DataSet retorno = new DataSet();
            Dictionary<string, string> valuesHt = new Dictionary<string, string>();
            try
            {
                log.Evento("[executeSP] [" + nomSP + "] [IP][" + conexion.DataSource + "]");
            }
            catch (Exception ex)
            {
                log.Error("Ejecucion SP" + "[" + ex.Message + "]" + ex.StackTrace);
            }
            try
            {
                conexion.Open();
                SqlCommand query = new SqlCommand(nomSP, conexion)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 0
                };

                if (parametros != null && parametros.Count > 0)
                {
                    foreach (DictionaryEntry parametro in parametros)
                    {
                        query.Parameters.AddWithValue(parametro.Key.ToString(), parametro.Value);
                        valuesHt.Add(parametro.Key.ToString(), parametro.Value == null ? "" : parametro.Value.ToString());
                    }
                }
                SqlParameter outparam = query.Parameters.Add("@codigoRespuesta", SqlDbType.VarChar, 2);
                outparam.Direction = ParameterDirection.Output;

                SqlParameter outparamMensaje = query.Parameters.Add("@mensaje", SqlDbType.VarChar, -1);
                outparamMensaje.Direction = ParameterDirection.Output;

                SqlParameter outparamToken = query.Parameters.Add("@token", SqlDbType.VarChar, 6);
                outparamToken.Direction = ParameterDirection.Output;

                SqlParameter outparamCorreo = query.Parameters.Add("@correo", SqlDbType.VarChar, 100);
                outparamCorreo.Direction = ParameterDirection.Output;

                SqlParameter outparamTelefono = query.Parameters.Add("@telefono", SqlDbType.VarChar, 100);
                outparamTelefono.Direction = ParameterDirection.Output;


                //   log.Evento("[executeSPToken] [" + nomSP + "] " + JsonSerializer.Serialize(valuesHt));
                query.ExecuteNonQuery();
                var response = new ResponseTokenSMSEMAIL()
                {
                    CodigoRespuesta = Convert.ToInt32(query.Parameters["@codigoRespuesta"].Value),
                    Mensaje = query.Parameters["@mensaje"].Value.ToString(),
                    Token = query.Parameters["@token"].Value.ToString(),
                    Correo = query.Parameters["@correo"].Value.ToString(),
                    Telefono = query.Parameters["@telefono"].Value.ToString(),
                };

                //   log.Evento("[executeSPToken] [" + nomSP + "] " + JsonSerializer.Serialize(response));
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conexion.Close();
            }

        }


        public static ResponseTokenSMS executeSPTokenV2(string nomSP, Hashtable parametros, Logueo log)
        {
            SqlConnection conexion = new SqlConnection(DBCentralApp.strBDEscritura);
            DataSet retorno = new DataSet();
            Dictionary<string, string> valuesHt = new Dictionary<string, string>();
            try
            {
                log.Evento("[executeSP] [" + nomSP + "] [IP][" + conexion.DataSource + "]");
            }
            catch (Exception ex)
            {
                log.Error("Ejecucion SP" + "[" + ex.Message + "]" + ex.StackTrace);
            }
            try
            {
                conexion.Open();
                SqlCommand query = new SqlCommand(nomSP, conexion)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 0
                };

                if (parametros != null && parametros.Count > 0)
                {
                    foreach (DictionaryEntry parametro in parametros)
                    {
                        query.Parameters.AddWithValue(parametro.Key.ToString(), parametro.Value);
                        valuesHt.Add(parametro.Key.ToString(), parametro.Value == null ? "" : parametro.Value.ToString());
                    }
                }
                SqlParameter outparam = query.Parameters.Add("@codigoRespuesta", SqlDbType.VarChar, 2);
                outparam.Direction = ParameterDirection.Output;

                SqlParameter outparamMensaje = query.Parameters.Add("@mensaje", SqlDbType.VarChar, -1);
                outparamMensaje.Direction = ParameterDirection.Output;

                SqlParameter outparamToken = query.Parameters.Add("@token", SqlDbType.VarChar, 6);
                outparamToken.Direction = ParameterDirection.Output;

                //   log.Evento("[executeSPToken] [" + nomSP + "] " + JsonSerializer.Serialize(valuesHt));
                query.ExecuteNonQuery();
                var response = new ResponseTokenSMS()
                {
                    CodigoRespuesta = Convert.ToInt32(query.Parameters["@codigoRespuesta"].Value),
                    Mensaje = query.Parameters["@mensaje"].Value.ToString(),
                    Token = query.Parameters["@token"].Value.ToString()
                };

                //   log.Evento("[executeSPToken] [" + nomSP + "] " + JsonSerializer.Serialize(response));
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conexion.Close();
            }

        }


        public static ResponseUsuarioPost executeSPRegisterUser(string nomSP, Hashtable parametros, Logueo log)
        {
            SqlConnection conexion = new SqlConnection(DBCentralApp.strBDEscritura);
            DataSet retorno = new DataSet();
            Dictionary<string, string> valuesHt = new Dictionary<string, string>();
            try
            {
                log.Evento("[executeSP] [" + nomSP + "] [IP][" + conexion.DataSource + "]");
            }
            catch (Exception ex)
            {
                log.Error("Ejecucion SP" + "[" + ex.Message + "]" + ex.StackTrace);
            }
            try
            {
                conexion.Open();
                SqlCommand query = new SqlCommand(nomSP, conexion)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 0
                };

                if (parametros != null && parametros.Count > 0)
                {
                    foreach (DictionaryEntry parametro in parametros)
                    {
                        query.Parameters.AddWithValue(parametro.Key.ToString(), parametro.Value);
                        if(!parametro.Key.ToString().Equals("@password"))
                            valuesHt.Add(parametro.Key.ToString(), parametro.Value == null ? "" : parametro.Value.ToString());
                    }
                }
                SqlParameter outparam = query.Parameters.Add("@codigoRespuesta", SqlDbType.VarChar, 2);
                outparam.Direction = ParameterDirection.Output;

                SqlParameter outparamMensaje = query.Parameters.Add("@mensaje", SqlDbType.VarChar, -1);
                outparamMensaje.Direction = ParameterDirection.Output;

                SqlParameter outparamUserId = query.Parameters.Add("@user_Id", SqlDbType.VarChar, 50);
                outparamUserId.Direction = ParameterDirection.Output;
                //
               // log.Evento("[executeSPRegisterUser] [" + nomSP + "] " + JsonSerializer.Serialize(valuesHt));
                query.ExecuteNonQuery();
                var response = new ResponseUsuarioPost()
                {
                    CodigoRespuesta = Convert.ToInt32(query.Parameters["@codigoRespuesta"].Value),
                    Mensaje = query.Parameters["@mensaje"].Value.ToString(),
                    UserId = query.Parameters["@user_Id"].Value.ToString()
                };

                //log.Evento("[executeSPRegisterUser] [" + nomSP + "] " + JsonSerializer.Serialize(response));


                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conexion.Close();
            }

        }

        public static ResponseGral executeSPDescuento(string nomSP, Hashtable parametros, Logueo log)
        {
            SqlConnection conexion = new SqlConnection(DBCentralApp.strBDAULectura);
            DataSet retorno = new DataSet();
            Dictionary<string, string> valuesHt = new Dictionary<string, string>();
            try
            {
                log.Evento("[executeSP] [" + nomSP + "] [IP][" + conexion.DataSource + "]");
            }
            catch (Exception ex)
            {
                log.Error("Ejecucion SP" + "[" + ex.Message + "]" + ex.StackTrace);
            }
            try
            {
                conexion.Open();
                SqlCommand query = new SqlCommand(nomSP, conexion)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 0
                };

                if (parametros != null && parametros.Count > 0)
                {
                    foreach (DictionaryEntry parametro in parametros)
                    {
                        query.Parameters.AddWithValue(parametro.Key.ToString(), parametro.Value);
                        if (!parametro.Key.ToString().Equals("@password"))
                            valuesHt.Add(parametro.Key.ToString(), parametro.Value == null ? "" : parametro.Value.ToString());
                    }
                }
                SqlParameter outparam = query.Parameters.Add("@codigoRespuesta", SqlDbType.VarChar, 2);
                outparam.Direction = ParameterDirection.Output;

                SqlParameter outparamMensaje = query.Parameters.Add("@mensaje", SqlDbType.VarChar, -1);
                outparamMensaje.Direction = ParameterDirection.Output;

                //SqlParameter outparamUserId = query.Parameters.Add("@user_Id", SqlDbType.VarChar, 50);
                //outparamUserId.Direction = ParameterDirection.Output;
                //
                // log.Evento("[executeSPRegisterUser] [" + nomSP + "] " + JsonSerializer.Serialize(valuesHt));
                query.ExecuteNonQuery();
                var response = new ResponseUsuarioPost()
                {
                    CodigoRespuesta = Convert.ToInt32(query.Parameters["@codigoRespuesta"].Value),
                    Mensaje = query.Parameters["@mensaje"].Value.ToString()
                    //UserId = query.Parameters["@user_Id"].Value.ToString()
                };

                //log.Evento("[executeSPRegisterUser] [" + nomSP + "] " + JsonSerializer.Serialize(response));
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conexion.Close();
            }

        }


        public static PwdUpdate executeSPUpdatePwd(string nomSP, Hashtable parametros, Logueo log)
        {
            SqlConnection conexion = new SqlConnection(DBCentralApp.strBDEscritura);
            DataSet retorno = new DataSet();
            Dictionary<string, string> valuesHt = new Dictionary<string, string>();
            try
            {
                log.Evento("[executeSP] [" + nomSP + "] [IP][" + conexion.DataSource + "]");
            }
            catch (Exception ex)
            {
                log.Error("Ejecucion SP" + "[" + ex.Message + "]" + ex.StackTrace);
            }
            try
            {
                conexion.Open();
                SqlCommand query = new SqlCommand(nomSP, conexion)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 0
                };

                if (parametros != null && parametros.Count > 0)
                {
                    foreach (DictionaryEntry parametro in parametros)
                    {
                        query.Parameters.AddWithValue(parametro.Key.ToString(), parametro.Value);
                        if (!parametro.Key.ToString().Equals("@pwdNuevo"))
                            valuesHt.Add(parametro.Key.ToString(), parametro.Value == null ? "" : parametro.Value.ToString());
                    }
                }
                SqlParameter outparam = query.Parameters.Add("@codigoRespuesta", SqlDbType.VarChar, 2);
                outparam.Direction = ParameterDirection.Output;

                SqlParameter outparamMensaje = query.Parameters.Add("@mensaje", SqlDbType.VarChar, -1);
                outparamMensaje.Direction = ParameterDirection.Output;

                SqlParameter outparamPwdSalt = query.Parameters.Add("@pwdSalt", SqlDbType.VarChar, -1);
                outparamPwdSalt.Direction = ParameterDirection.Output;

                SqlParameter outparamMixed = query.Parameters.Add("@mix", SqlDbType.VarChar, -1);
                outparamMixed.Direction = ParameterDirection.Output;

                SqlParameter outparamStatHashIpSecurity = query.Parameters.Add("@StatusHashIPSecurity", SqlDbType.Int);
                outparamStatHashIpSecurity.Direction = ParameterDirection.Output;

             //   log.Evento("[executeSP] [" + nomSP + "] " + JsonSerializer.Serialize(valuesHt));
                query.ExecuteNonQuery();
                var response = new PwdUpdate()
                {
                    CodigoRespuesta = Convert.ToInt32(query.Parameters["@codigoRespuesta"].Value),
                    Mensaje = query.Parameters["@mensaje"].Value.ToString(),
                    PwdSalt = query.Parameters["@pwdSalt"].Value.ToString(),
                    Mixed = query.Parameters["@mix"].Value.ToString(),
                    StatusHashIPSecurity = (int)query.Parameters["@StatusHashIPSecurity"].Value,
                };
            //    log.Evento("[executeSP] [" + nomSP + "] " + JsonSerializer.Serialize(response));
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conexion.Close();
            }

        }

        public static ResponseGral executeSPWithConnection(string nomSP, Hashtable parametros, Logueo log, SqlConnection conexion)
        {


            //SqlConnection conexion = new SqlConnection(DBCentralApp.strBDEscritura);
            DataSet retorno = new DataSet();
            Dictionary<string, string> valuesHt = new Dictionary<string, string>();
            try
            {
                log.Evento("[executeSP] [" + nomSP + "] [IP][" + conexion.DataSource + "]");
            }
            catch (Exception ex)
            {
                log.Error("Ejecucion SP" + "[" + ex.Message + "]" + ex.StackTrace);
            }
            try
            {
             //   conexion.Open();
                SqlCommand query = new SqlCommand(nomSP, conexion)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 0
                };

                if (parametros != null && parametros.Count > 0)
                {
                    foreach (DictionaryEntry parametro in parametros)
                    {
                        query.Parameters.AddWithValue(parametro.Key.ToString(), parametro.Value);
                        if (!parametro.Key.ToString().Equals("@pwdNuevo"))
                            valuesHt.Add(parametro.Key.ToString(), parametro.Value == null ? "" : parametro.Value.ToString());
                    }
                }
                SqlParameter outparam = query.Parameters.Add("@codigoRespuesta", SqlDbType.VarChar, 2);
                outparam.Direction = ParameterDirection.Output;

                SqlParameter outparamMensaje = query.Parameters.Add("@mensaje", SqlDbType.VarChar, -1);
                outparamMensaje.Direction = ParameterDirection.Output;
                //log.Evento("[executeSP] [" + nomSP + "] " + JsonSerializer.Serialize(valuesHt));
                query.ExecuteNonQuery();
                var response = new ResponseGral()
                {
                    CodigoRespuesta = Convert.ToInt32(query.Parameters["@codigoRespuesta"].Value),
                    Mensaje = query.Parameters["@mensaje"].Value.ToString()
                };
                //  log.Evento("[executeSP] [" + nomSP + "] " + JsonSerializer.Serialize(response));
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //finally
            //{
            //    conexion.Close();
            //}

        }

        public static ResponseUsuarioPost executeSPRegisterWithConnection(string nomSP, Hashtable parametros, Logueo log, SqlConnection conexion, SqlTransaction tran)
        {


            //SqlConnection conexion = new SqlConnection(DBCentralApp.strBDEscritura);
            DataSet retorno = new DataSet();
            Dictionary<string, string> valuesHt = new Dictionary<string, string>();
            try
            {
                log.Evento("[executeSP] [" + nomSP + "] [IP][" + conexion.DataSource + "]");
            }
            catch (Exception ex)
            {
                log.Error("Ejecucion SP" + "[" + ex.Message + "]" + ex.StackTrace);
            }
            try
            {
                //   conexion.Open();
                SqlCommand query = new SqlCommand(nomSP, conexion, tran)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 0
                };

                if (parametros != null && parametros.Count > 0)
                {
                    foreach (DictionaryEntry parametro in parametros)
                    {
                        query.Parameters.AddWithValue(parametro.Key.ToString(), parametro.Value);
                        if (!parametro.Key.ToString().Equals("@pwdNuevo"))
                            valuesHt.Add(parametro.Key.ToString(), parametro.Value == null ? "" : parametro.Value.ToString());
                    }
                }
                SqlParameter outparam = query.Parameters.Add("@codigoRespuesta", SqlDbType.VarChar, 2);
                outparam.Direction = ParameterDirection.Output;

                SqlParameter outparamMensaje = query.Parameters.Add("@mensaje", SqlDbType.VarChar, -1);
                outparamMensaje.Direction = ParameterDirection.Output;

                SqlParameter outparamUserId = query.Parameters.Add("@user_Id", SqlDbType.VarChar, 50);
                outparamUserId.Direction = ParameterDirection.Output;
                //log.Evento("[executeSP] [" + nomSP + "] " + JsonSerializer.Serialize(valuesHt));
                query.ExecuteNonQuery();
                var response = new ResponseUsuarioPost()
                {
                    CodigoRespuesta = Convert.ToInt32(query.Parameters["@codigoRespuesta"].Value),
                    Mensaje = query.Parameters["@mensaje"].Value.ToString(),
                    UserId = query.Parameters["@user_Id"].Value.ToString()

                };
                //  log.Evento("[executeSP] [" + nomSP + "] " + JsonSerializer.Serialize(response));
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //finally
            //{
            //    conexion.Close();
            //}

        }

    }
}
