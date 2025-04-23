//#define firma 
using DALCentralAplicaciones.LogicaNegocio;
using DNU.Usuarios.APIService.Negocio.Authorization;
using DNU.Usuarios.Common;
using DNU.Usuarios.Common.Utilerias;
using DNU.Usuarios.DataContract.BD;
using DNU.Usuarios.DataContract.Entities;
using DNU.Usuarios.DataContract.Request;
using DNU.Usuarios.DataContract.Response;
using Log_PCI.Entidades;
using Interfases;
using Log_PCI;
using Log_PCI.Entidades;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using DALCentralAplicaciones.Entidades;
using Autenticacion;

namespace DNU.Usuarios.APIService.Negocio
{
    public class LNLogIn
    {
        public static ResponseLogInPost logInUsuario(RequerimientoLogInValidaCredenciales requerimiento, Logueo log)
        {
            ResponseLogInPost _respuesta = new ResponseLogInPost();
            Hashtable ht = new Hashtable();


#if firma
            bool validarUsuario = LNBloqueoUsuario.ValidaUsuario(requerimiento.NombreUsuario);
            if (!validarUsuario)
            {
                _respuesta.CodigoRespuesta = 12;
                _respuesta.Mensaje = "El usuario esta bloquedado";
                _respuesta.NombreUsuario = requerimiento.NombreUsuario;
                return _respuesta;
            }
#endif

            ht.Add("@nombreUsuario", requerimiento.NombreUsuario);
            ht.Add("@appID", requerimiento.Aplicacion);
            if (requerimiento.Cifrado == 1)
            {
                _respuesta = validarPwdLogIn(SPs.executeSPDT("wsR_LogIn_reportLogIn", ht, log), requerimiento, log);
            }
            else
            {
                _respuesta = validarPwdLogIn(SPs.executeSPDT("wsR_LogIn_reportLogInDesEnc", ht, log), requerimiento, log);
            }

            //SI ES RESPUESTA DE AUTORIZADO ENTONCES ENVIA LOS PERMISOS A LAS BD.

            try
            {
                if (_respuesta.CodigoRespuesta == 0)
                {
                    LogHeader logHF = new LogHeader();
                    logHF.IP_Address = log.IpAddr;
                    logHF.Application_ID = Guid.Parse("36EF1469-05AF-425A-84B7-131BECABB6C5");
                    logHF.User = log.User;
                    logHF.Trace_ID = log.IdLog;


                    Usuario elUsuario = DAOUsuario.ObtieneCaracteristicasUsuario(requerimiento.NombreUsuario, log);

                    LNFiltro.ExportaFiltros(elUsuario, logHF);
                    //LNFiltro.ExportaFiltrosWebService(elUsuario, log.IdLog, elUsuario.ClaveUsuario);
#if firma
                    LNBloqueoUsuario.BorrarUsuarioListaNegra(requerimiento.NombreUsuario);
#endif
                }
                else
                {
#if firma
                    bool usuarioBloqueado = LNBloqueoUsuario.BloquearUsuario(requerimiento.NombreUsuario);
                    if (usuarioBloqueado)
                    {
                        _respuesta.CodigoRespuesta = 12;
                        _respuesta.Mensaje = "El usuario esta bloquedado";
                        _respuesta.NombreUsuario = requerimiento.NombreUsuario;
                    }
#endif
                }
            }
            catch (Exception err)
            {
                log.Error("logInUsuario mensaje: " + err.Message + " " + err.StackTrace);

            }


            return _respuesta;
        }

        public static ResponseGral deleteSession(RequerimientoLogInDelete requerimiento, string pToken, Logueo log)
        {
            ResponseGral logIn = new ResponseGral();
            Hashtable ht = new Hashtable();

            ht.Add("@token", pToken);

            logIn = SPs.executeSP("wsU_DeleteToken_updateLogIn", ht, log);
            if (logIn.CodigoRespuesta == 0)
            {
                logIn.Mensaje = "Sesión cerrada correctamente";
            }

            return logIn;
        }


        public static ResponseLogInPost validarPwdLogIn(DataTable dtInfo, RequerimientoLogInValidaCredenciales pRequerimiento, Logueo log)
        {
            //log.Evento($"validarPwdLogIn dtInfo.Rows {Newtonsoft.Json.JsonConvert.SerializeObject(dtInfo.Rows)} ===============>");

            // log.Evento($"validarPwdLogIn {dtInfo.Columns.Count} ===============>");
            ResponseLogInPost logInResp = new ResponseLogInPost();
            if (dtInfo.Columns.Count == 2)
            {
                logInResp.CodigoRespuesta = Convert.ToInt32(dtInfo.Rows[0]["codigoRespuesta"]);
                logInResp.Mensaje = dtInfo.Rows[0]["mensaje"].ToString();
                logInResp.NombreUsuario = pRequerimiento.NombreUsuario;
            }
            else
            {
                if (pRequerimiento.Cifrado == 1)
                {
                    LNEncripcion lnEncripcion = new LNEncripcion(log);
                    if (lnEncripcion.descifrar(dtInfo.Rows[0]["Password"].ToString()).Equals(lnEncripcion.descifrar(pRequerimiento.Password))
                                    && lnEncripcion.descifrar(pRequerimiento.Password) != "")
                    {
                        // logInResp = obtenerEstructuraLogIn(dtInfo, pRequerimiento, log);
                        logInResp = obtenerEstructuraLogIn(dtInfo, pRequerimiento, log);
                    }
                    else
                    {
                        logInResp.CodigoRespuesta = 11;
                        logInResp.Mensaje = "El usuario o contraseña son incorrectos";
                    }
                }
                else
                {
                   // log.Evento($"validarPwdLogIn dtInfo.Rows {Newtonsoft.Json.JsonConvert.SerializeObject(dtInfo.Rows)} ===============>");
                    LogHeader LH_Login = new LogHeader();
                    LH_Login.IP_Address = Autenticacion.Hashing.GetClientIp();
                    LH_Login.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["applicationId"].ToString());
                 //   LH_Login.Trace_ID = Guid.NewGuid();
                    LH_Login.User = log.User;
                    LH_Login.Trace_ID = log.IdLog;

                    Int32.TryParse(dtInfo.Rows[0]["StatusHashIPSecurity"].ToString(), out int @StatusHashIPSecurity);

                    var ip = string.Empty;
                    if (@StatusHashIPSecurity == (int)HashIpValidationsLifeCycle.ACTIVE ||
                       @StatusHashIPSecurity == (int)HashIpValidationsLifeCycle.ACTIVE_TO_NONE)
                        ip = pRequerimiento.UserIP;

                    //log.Error($"@StatusHashIPSecurity  {@StatusHashIPSecurity }");
                    //log.Error($"[Hashing.PasswordOK] ip {ip} pRequerimiento.NombreUsuario {pRequerimiento.NombreUsuario}");

                    if (Hashing.PasswordOK(pRequerimiento.Password, dtInfo.Rows[0]["Password"].ToString(),
                        dtInfo.Rows[0]["PasswordSalt"].ToString(), Convert.ToInt32(dtInfo.Rows[0]["MobilePIN"]),
                        LH_Login, ip, pRequerimiento.NombreUsuario))
                    {
                        logInResp = obtenerEstructuraLogIn(dtInfo, pRequerimiento, log);
                    }
                    else
                    {
                        logInResp.CodigoRespuesta = 11;
                        logInResp.Mensaje = "El usuario o contraseña son incorrectos";
                        logInResp.NombreUsuario = pRequerimiento.NombreUsuario;
                    }

                }
            }
            //log.Evento("[obtenerEstructuraLogIn] " + JsonSerializer.Serialize(logInResp));

            return logInResp;
        }

        public static ResponseLogInPost obtenerEstructuraLogIn(DataTable dtInfo, RequerimientoLogInValidaCredenciales pRequerimiento, Logueo log)
        {
            ResponseLogInPost logInResp = new ResponseLogInPost();
            List<DRol> lstRoles;
            List<DAplicacion> lstAplicaciones;
            List<DMenu> lstMenus;
            DRol rol;
            DAplicacion app;
            DMenu menu;


            logInResp.CodigoRespuesta = Convert.ToInt32(dtInfo.Rows[0]["codigoRespuesta"]);
            logInResp.Mensaje = dtInfo.Rows[0]["mensaje"].ToString();
            logInResp.UserID = dtInfo.Rows[0]["UserId"].ToString();
            logInResp.UserTemp = dtInfo.Rows[0]["UserTemp"].ToString();
            logInResp.NombreUsuario = dtInfo.Rows[0]["Nombre"].ToString();
            logInResp.PrimerApellido = dtInfo.Rows[0]["Apaterno"].ToString();
            logInResp.SegundoApellido = dtInfo.Rows[0]["Amaterno"].ToString();
            // logInResp.Token = TokenGenerator.GenerateTokenJwt(pRequerimiento.NombreUsuario, log, dtInfo.Rows[0]["UserTemp"].ToString());
            logInResp.Token = TokenGenerator.GenerateTokenJwt(pRequerimiento.NombreUsuario, log, dtInfo.Rows[0]["UserTemp"].ToString(), pRequerimiento.Aplicacion);
            lstRoles = new List<DRol>();
            var distinctRoles = dtInfo.AsEnumerable().Select(s => new { rolId = s.Field<Guid>("roleId") })
                        .Distinct().ToList();
            foreach (var dato in distinctRoles)
            {
                lstAplicaciones = new List<DAplicacion>();
                DataRow drRoles = dtInfo.Select("roleId = " + "'" + dato.rolId + "'").First();
                var distinctApps = dtInfo.AsEnumerable().Select(s => new { AppId = s.Field<Guid>("AppId") })
                        .Distinct().ToList();
                foreach (var datApps in distinctApps)
                {
                    DataRow drApp = dtInfo.Select("roleId = " + "'" + dato.rolId +
                            "' AND AppId = '" + datApps.AppId + "'").FirstOrDefault();
                    if (drApp != null)
                    {
                        lstMenus = new List<DMenu>();
                        if (datApps.ToString() != "00000000-0000-0000-0000-000000000000")
                        {
                            DataRow[] drMenus = dtInfo.Select("AppId =" + "'" + datApps.AppId + "' AND roleId = " + "'" + dato.rolId + "'");
                            foreach (DataRow datMenus in drMenus)
                            {
                                if (datMenus["meAppId"].ToString() != "00000000-0000-0000-0000-000000000000")
                                {
                                    menu = new DMenu()
                                    {
                                        ID_Aplicacion = datMenus["meAppId"].ToString(),
                                        ID_Menu = datMenus["menuId"].ToString(),
                                        ID_MenuPadre = datMenus["menuIdPadre"].ToString(),
                                        Nombre = datMenus["NomMenu"].ToString(),
                                        Path = datMenus["Path"].ToString(),
                                        NumeroIcono = datMenus["IconoMenu"].ToString(),
                                        OrdenDespliegue = datMenus["OrdenDespliegueMenu"].ToString()
                                    };
                                    lstMenus.Add(menu);
                                }
                            }
                        }
                        app = new DAplicacion()
                        {
                            ID_Aplicacion = drApp["AppId"].ToString(),
                            Nombre = drApp["DescriptionApp"].ToString(),
                            URL = drApp["ApplicationName"].ToString(),
                            Icono = drApp["IconoApp"].ToString(),
                            OrdenDespliegue = drApp["OrdenDespliegueApp"].ToString(),
                            Menu = lstMenus
                        };
                        lstAplicaciones.Add(app);
                    }
                }
                rol = new DRol()
                {
                    RollID = drRoles["roleId"].ToString(),
                    Clave = drRoles["RoleName"].ToString(),
                    Nombre = drRoles["DescriptionRol"].ToString(),
                    Aplicaciones = lstAplicaciones
                };
                lstRoles.Add(rol);
            }

            logInResp.Roles = lstRoles;

            return logInResp;
        }

        public static ResponseGral validarCredenciales(RequerimientoLogInValidaCredenciales requerimiento, Logueo log)
        {
            ResponseGral response = new ResponseGral();
            Hashtable ht = new Hashtable();

            ht.Add("@nombreUsuario", requerimiento.NombreUsuario);
            ht.Add("@appId", requerimiento.Aplicacion);

            /****
             * 
             *  TEST 
             * 
            string pwdSalt = null, pwdKey = null;
            int mixed = 0;
            PassWord.CreaPasswordUsuario(requerimiento.Password, ref pwdKey
                             , ref pwdSalt, ref mixed, log, requerimiento.NombreUsuario);
            Hashtable ht1 = new Hashtable();
            ht1.Add("@pwdNuevo", pwdKey);
            ht1.Add("@pwdSalt", pwdSalt);
            ht1.Add("@mixed", mixed);
            ht1.Add("@nombreUsuario", requerimiento.NombreUsuario);
            ht1.Add("@appID", requerimiento.Aplicacion);
            var pwd = SPs.executeSP("wsU_password_updatePassword", ht1, log);

            /****
             * TEST
             * */
            if (requerimiento.Cifrado == 1)
            {
                response = validarAccesos(SPs.executeSPDT("wsR_usuarios_validarCredenciales", ht, log), requerimiento, log);
            }
            else
            {
                byte[] myByteCredential = Encoding.UTF8.GetBytes(requerimiento.NombreUsuario + ":" + requerimiento.Password);
                string credential = Convert.ToBase64String(myByteCredential);
                if (requerimiento.UpdatePwd == 1)
                {
                    TableCredential.deleteCredential(credential);

                    response = validarAccesos(SPs.executeSPDT("wsR_usuarios_validarCredencialesDesEnc", ht, log), requerimiento, log);

                }
                else
                {
                    if (TableCredential.getCredential(credential))
                    {
                        response.CodigoRespuesta = 0;
                        response.Mensaje = "Acceso Autorizado";
                    }
                    else
                    {
                        response = validarAccesos(SPs.executeSPDT("wsR_usuarios_validarCredencialesDesEnc", ht, log), requerimiento, log);

                        if (response.CodigoRespuesta == 0)
                            TableCredential.setCredential(credential);
                    }
                }
            }
            return response;
        }


        public static ResponseValidarCredenciales validarAccesos(DataTable dtInfo, RequerimientoLogInValidaCredenciales pRequerimiento, Logueo log)
        {

            //log.Evento("Validando validarAccesos");
            //log.Evento($"pRequerimiento {Newtonsoft.Json.JsonConvert.SerializeObject(pRequerimiento)} ");
            //log.Evento($"dtInfo  {Newtonsoft.Json.JsonConvert.SerializeObject(dtInfo)} ");
            ResponseValidarCredenciales _respuesta = new ResponseValidarCredenciales();
            if (dtInfo.Columns.Count == 6 && pRequerimiento.Cifrado == 0)
            {


                LogHeader LH_Login = new LogHeader();
                LH_Login.IP_Address = Autenticacion.Hashing.GetClientIp();
                LH_Login.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["applicationId"].ToString());
                LH_Login.Trace_ID = Guid.NewGuid();

                Int32.TryParse(dtInfo.Rows[0]["StatusHashIPSecurity"].ToString(), out int @StatusHashIPSecurity);

                var ip = string.Empty;
                if (@StatusHashIPSecurity == (int)HashIpValidationsLifeCycle.ACTIVE ||
                   @StatusHashIPSecurity == (int)HashIpValidationsLifeCycle.ACTIVE_TO_NONE)
                    ip = pRequerimiento.UserIP;


                if (Hashing.PasswordOK(pRequerimiento.Password, dtInfo.Rows[0]["Password"].ToString(),
                        dtInfo.Rows[0]["PasswordSalt"].ToString(), Convert.ToInt32(dtInfo.Rows[0]["MobilePIN"]),
                        LH_Login, ip, pRequerimiento.NombreUsuario))
                {
                    _respuesta.CodigoRespuesta = Convert.ToInt32(dtInfo.Rows[0]["codigoRespuesta"]);
                    _respuesta.Mensaje = dtInfo.Rows[0]["mensaje"].ToString();
                }
                else
                {
                    _respuesta.CodigoRespuesta = 11;
                    _respuesta.Mensaje = "El usuario o contraseña son incorrectos";
                }
            }
            else
            {
                if (dtInfo.Columns.Count == 2)
                {
                    _respuesta.CodigoRespuesta = Convert.ToInt32(dtInfo.Rows[0]["codigoRespuesta"]);
                    _respuesta.Mensaje = dtInfo.Rows[0]["mensaje"].ToString();
                }
                else
                {
                    LNEncripcion lnEncripcion = new LNEncripcion(log);
                    if (lnEncripcion.descifrar(dtInfo.Rows[0]["Password"].ToString()).Equals(lnEncripcion.descifrar(pRequerimiento.Password))
                                    && lnEncripcion.descifrar(pRequerimiento.Password) != "")
                    {
                        _respuesta.CodigoRespuesta = 0;
                        _respuesta.Mensaje = "Acceso Autorizado";
                        _respuesta.UserID = dtInfo.Rows[0]["UserId"].ToString();
                    }
                    else
                    {
                        _respuesta.CodigoRespuesta = 11;
                        _respuesta.Mensaje = "El usuario o contraseña son incorrectos";
                    }
                }

            }
            //log.EntradaSalida("[validarAccesos] " + JsonSerializer.Serialize(_respuesta), "DNU.Usuarios", true);
            return _respuesta;
        }

        public static ResponseGenToken generarToken(RequerimientoGenToken requerimiento, Logueo log)
        {
            ResponseGenToken _respuesta = new ResponseGenToken();

            _respuesta.CodigoRespuesta = 0;
            _respuesta.Mensaje = "Token Generado";
            _respuesta.Token = TokenGenerator.GenerateTokenJwt(requerimiento.Usuario, log, null);

            return _respuesta;
        }

    }
}