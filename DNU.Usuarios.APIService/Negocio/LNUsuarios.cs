using Autenticacion;
using DNU.Usuarios.Common.Utilerias;
using DNU.Usuarios.DataContract.BD;
using DNU.Usuarios.DataContract.Request;
using DNU.Usuarios.DataContract.Response;
using Log_PCI.Entidades;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;
using System.Web;

namespace DNU.Usuarios.APIService.Negocio
{
    public class LNUsuarios
    {
        public static ResponseUsuarioPost insertUsuarios(RequerimientoUsuariosPost requerimiento, Logueo log)
        {
            Hashtable ht = new Hashtable();
            ResponseUsuarioPost usuario = new ResponseUsuarioPost();
            LogHeader LH_Login = new LogHeader();
            string pwdSalt = null, pwdKey = null;
            int mixed = 0;
            LH_Login.IP_Address = Hashing.GetClientIp();
            LH_Login.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["applicationId"].ToString());
            LH_Login.Trace_ID = Guid.NewGuid();

            int @StatusHashIPSecurity = 0;

            var ip = string.Empty;

            Hashing.CreaPasswordUsuario(requerimiento.Password, ref pwdKey, ref pwdSalt,
            ref mixed, LH_Login, ip, requerimiento.NombreUsuario);

            ht.Add("@nombre", requerimiento.Nombre);
            ht.Add("@primerApellido", requerimiento.PrimerApellido);
            ht.Add("@segundoApellido", requerimiento.SegundoApellido);
            ht.Add("@nombreUsuario", requerimiento.NombreUsuario);
            ht.Add("@aplicacion", requerimiento.Aplicacion);
            ht.Add("@movil", requerimiento.Movil);
            ht.Add("@administradorID", requerimiento.AdministradorID);
            ht.Add("@roles", Operaciones.convierteStringComasRoles(requerimiento.Roles));
            ht.Add("@tableId", ConfigurationManager.AppSettings["tableID"].ToString());
            //Para guardar password encriptado
            ht.Add("@StatusHashIPSecurity", @StatusHashIPSecurity);
            ht.Add("@password", pwdKey);
            ht.Add("@pwdSalt", pwdSalt);
            ht.Add("@mixed", mixed);
            usuario = SPs.executeSPRegisterUser("wsI_usuarios_insertarUsuario", ht, log);

            return usuario;
        }

        public static ResponseUsuarioPost insertUsuariosV2(RequerimientoUsuariosPostV2 requerimiento, Logueo log)
        {
            Hashtable ht = new Hashtable();
            Hashtable htD = new Hashtable();
            bool rollBack = true;

            ResponseUsuarioPost usuario = new ResponseUsuarioPost();
            LogHeader LH_Login = new LogHeader();
            string pwdSalt = null, pwdKey = null;
            int mixed = 0;
            LH_Login.IP_Address = Hashing.GetClientIp();
            LH_Login.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["applicationId"].ToString());
            LH_Login.Trace_ID = Guid.NewGuid();

            int @StatusHashIPSecurity = 0;

            var ip = string.Empty;


            using (SqlConnection conexion = new SqlConnection(DBCentralApp.strBDEscritura))
            {
                conexion.Open();
                using (SqlTransaction transaction = conexion.BeginTransaction())
                {
                    try
                    {
                        Hashing.CreaPasswordUsuario(requerimiento.Password, ref pwdKey, ref pwdSalt,
                        ref mixed, LH_Login, ip, requerimiento.NombreUsuario);
                        ht.Add("@nombre", requerimiento.Nombre);
                        ht.Add("@primerApellido", requerimiento.PrimerApellido);
                        ht.Add("@segundoApellido", requerimiento.SegundoApellido);
                        ht.Add("@nombreUsuario", requerimiento.NombreUsuario);
                        ht.Add("@aplicacion", requerimiento.Aplicacion);
                        ht.Add("@movil", requerimiento.Movil);
                        ht.Add("@administradorID", requerimiento.AdministradorID);
                        ht.Add("@roles", Operaciones.convierteStringComasRoles(requerimiento.Roles));
                        ht.Add("@tableId", ConfigurationManager.AppSettings["tableID"].ToString());
                        //Para guardar password encriptado
                        ht.Add("@StatusHashIPSecurity", @StatusHashIPSecurity);
                        ht.Add("@password", pwdKey);
                        ht.Add("@pwdSalt", pwdSalt);
                        ht.Add("@mixed", mixed);
                        usuario = SPs.executeSPRegisterWithConnection("wsI_usuarios_insertarUsuario", ht, log, conexion, transaction);
                       // usuario codigo respuesta 0
                        if (!String.IsNullOrEmpty(usuario.UserId))
                        {
                            if (!String.IsNullOrEmpty(requerimiento.CodigoDescuento))
                            {
                                using (SqlConnection conexionAutorizador = new SqlConnection(DBCentralApp.strBDAULectura))
                                {
                                    //peticion al autorizador

                                    conexionAutorizador.Open();
                                    //htD.Add("@userID", usuario.UserId);
                                    htD.Add("@nombreCompleto", requerimiento.Nombre + ' ' + requerimiento.PrimerApellido + ' ' + requerimiento.SegundoApellido);
                                    htD.Add("@claveDescuento", requerimiento.CodigoDescuento);
                                    htD.Add("@telefono", requerimiento.Movil);
                                    var response = SPs.executeSPWithConnection("ws_Parabilium_AsignarDescuento", htD, log, conexionAutorizador);
                                    rollBack = false;
                                    conexionAutorizador.Close();
                                }
                            }
                            else
                            {
                                rollBack = false;

                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        log.Error("[POST: api/UsuariosV2/] " + "[" + ex.Message + "]" + ex.StackTrace);
                        rollBack = true;
                    }

                    if (rollBack)
                    {
                        //usuario codigo respuesta 0
                        //if (usuario.CodigoRespuesta > 2)
                        //{
                            usuario.CodigoRespuesta = 98;
                            usuario.Mensaje = "Ocurrio un error al almacenar el usuario";
                        //}
                        transaction.Rollback();
                        conexion.Close();
                    }
                    else
                    {
                        transaction.Commit();
                        conexion.Close();
                    }
                }

            }
            return usuario;
        }

        public static ResponseGral updateUsuario(RequerimientoUsuariosPut requerimiento, Logueo log)
        {
            Hashtable ht = new Hashtable();
            ResponseGral usuario = new ResponseGral();

            ht.Add("@userID", requerimiento.UserID);
            ht.Add("@nombre", requerimiento.Nombre);
            ht.Add("@primerApellido", requerimiento.PrimerApellido);
            ht.Add("@segundoApellido", requerimiento.SegundoApellido);
            ht.Add("@aplicacion", requerimiento.Aplicacion);
            ht.Add("@movil", requerimiento.Movil);
            ht.Add("@administradorID", requerimiento.AdministradorID);
            ht.Add("@roles", Operaciones.convierteStringComasRoles(requerimiento.Roles));

            usuario = SPs.executeSP("wsU_usuarios_updateUsuario", ht, log);

            return usuario;
        }

        public static ResponseGral desactivarUsuario(RequerimientoUsuariosPut requerimiento, Logueo log)
        {
            Hashtable ht = new Hashtable();
            ResponseGral usuario = new ResponseGral();

            ht.Add("@Usuario", requerimiento.Usuario);
            ht.Add("@Aplicacion", requerimiento.Aplicacion);

            usuario = SPs.executeSP("wsU_usuarios_desactivarUsuario", ht, log);

            return usuario;
        }

        public static ResponseUsuariosGet getUsuarios(string IdUser, Logueo log)
        {
            Hashtable ht = new Hashtable();
            ResponseUsuariosGet usuario = new ResponseUsuariosGet();

            ht.Add("@userID", IdUser);

            usuario = Operaciones.obtieneDatosUsuarios(SPs.executeSPDT("wsR_usuarios_reportUsuario", ht, log), log);

            return usuario;
        }

        public static ResponseGral deleteUsuario(RequerimientoUsuariosDelete requerimiento, Logueo log)
        {
            Hashtable ht = new Hashtable();
            ResponseGral usuarioD = new ResponseGral();

            ht.Add("@userId", requerimiento.UserId);
            ht.Add("@adminId", requerimiento.AdminId);
            ht.Add("@appId", requerimiento.AppId);
            ht.Add("@tableId", ConfigurationManager.AppSettings["tableID"].ToString());

            usuarioD = SPs.executeSP("wsD_usuarios_eliminarUsuario", ht, log);

            return usuarioD;
        }
    }
}