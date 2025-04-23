using Autenticacion;
using DALCentralAplicaciones.Entidades;
using DALCentralAplicaciones.LogicaNegocio;
using DNU.Usuarios.Common.Extensiones;
using DNU.Usuarios.Common.Utilerias;
using DNU.Usuarios.DataContract.BD;
using DNU.Usuarios.DataContract.Entities;
using DNU.Usuarios.DataContract.Request;
using DNU.Usuarios.DataContract.Response;
using Log_PCI.Entidades;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Management;

namespace DNU.Usuarios.APIService.Negocio
{
    public class LNPassword
    {
        public static ResponseGral insertPwd(RequerimientoPasswordPost requerimiento, Logueo log)
        {
            ResponseGral pwd = new ResponseGral();
            PwdUpdate respPwd = new PwdUpdate();
            Hashtable ht = new Hashtable();
            string pwdSalt = null, pwdKey = null;
            int mixed = 0;
            
            ht.Add("@nombreUsuario", requerimiento.NombreUsuario);

            ht.Add("@appID", requerimiento.Aplicacion);

            respPwd = SPs.executeSPUpdatePwd("wsR_password_reportPassword", ht, log);
            if (respPwd.CodigoRespuesta == 0)
            {

                LogHeader LH_Login = new LogHeader();
                LH_Login.IP_Address = Autenticacion.Hashing.GetClientIp();
                LH_Login.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["applicationId"].ToString());
                LH_Login.Trace_ID = Guid.NewGuid();

                int @StatusHashIPSecurity = respPwd.StatusHashIPSecurity;

                var ip = string.Empty;
                if (@StatusHashIPSecurity == (int)HashIpValidationsLifeCycle.ACTIVE ||
                   @StatusHashIPSecurity == (int)HashIpValidationsLifeCycle.ACTIVE_TO_NONE)
                    ip = requerimiento.UserIP;


                if (Hashing.PasswordOK(requerimiento.PasswordActual, respPwd.Mensaje,
                        respPwd.PwdSalt, Convert.ToInt32(respPwd.Mixed),
                        LH_Login, ip, requerimiento.NombreUsuario))
                {
                    

                    @StatusHashIPSecurity = GetNextHastSecurityStatus(@StatusHashIPSecurity);
                    

                    ip = string.Empty;
                    if (@StatusHashIPSecurity == (int)HashIpValidationsLifeCycle.ACTIVE ||
                       @StatusHashIPSecurity == (int)HashIpValidationsLifeCycle.NONE_TO_ACTIVE)
                        ip = requerimiento.UserIP;

                    Hashing.CreaPasswordUsuario(requerimiento.PasswordNuevo, ref pwdKey, ref pwdSalt,
                    ref mixed, LH_Login, ip, requerimiento.NombreUsuario);


                    //PassWord.CreaPasswordUsuario(requerimiento.PasswordNuevo, ref pwdKey
                    //                            , ref pwdSalt, ref mixed, log, requerimiento.NombreUsuario);
                    ht.Add("@StatusHashIPSecurity", @StatusHashIPSecurity);
                    ht.Add("@pwdNuevo", pwdKey);
                    ht.Add("@pwdSalt", pwdSalt);
                    ht.Add("@mixed", mixed);

                    pwd = SPs.executeSP("wsU_password_updatePassword", ht, log);
                    
                }
                else
                {
                    pwd.CodigoRespuesta = 7;
                    pwd.Mensaje = "La contraseña Actual es Incorrecta.";
                }

                //LNEncripcion lnEncripcion = new LNEncripcion(log);
                //if (lnEncripcion.descifrar(pwd.Mensaje).Equals(lnEncripcion.descifrar(requerimiento.PasswordActual))
                //         && lnEncripcion.descifrar(requerimiento.PasswordActual) != "")
                //{
                //    ht.Add("@pwdNuevo", requerimiento.PasswordNuevo);
                //    pwd = SPs.executeSP("wsU_password_updatePassword", ht, log);
                //}
            }

            return pwd;
        }


        private static int GetNextHastSecurityStatus(int statusHashIPSecurity)
        {
            if (statusHashIPSecurity == (int)HashIpValidationsLifeCycle.NONE_TO_ACTIVE)
                return (int)HashIpValidationsLifeCycle.ACTIVE;

            if (statusHashIPSecurity == (int)HashIpValidationsLifeCycle.ACTIVE_TO_NONE)
                return (int)HashIpValidationsLifeCycle.NONE;

            return statusHashIPSecurity;
        }

        public static ResponseGral updatePwd(RequerimientoPasswordPut requerimiento, Logueo log)
        {
            ResponseGral pwd = new ResponseGral();
            Hashtable ht = new Hashtable();
            bool correoEnviado = true;

            LogHeader LH_Login = new LogHeader();
            LH_Login.IP_Address = Autenticacion.Hashing.GetClientIp();
            LH_Login.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["applicationId"].ToString());
            LH_Login.Trace_ID = log.IdLog;
            LH_Login.User = log.User;

            string pwdSalt = null, pwdKey = null;
            int mixed = 0;

            var user = LNUsuarios.getUsuarios(requerimiento.UserID, log);

            var @StatusHashIPSecurity = GetNextHastSecurityStatus(user.Usuario.StatusHashIPSecurity);

            string ip = string.Empty;
            if (@StatusHashIPSecurity == (int)HashIpValidationsLifeCycle.ACTIVE)
                ip = requerimiento.UserIP;

            Hashing.CreaPasswordUsuario(requerimiento.PasswordNuevo, ref pwdKey, ref pwdSalt,
                ref mixed, LH_Login, ip, requerimiento.NombreUsuario);


            

            ht.Add("@StatusHashIPSecurity", @StatusHashIPSecurity);
            ht.Add("@nombreUsuario", requerimiento.NombreUsuario);
            ht.Add("@pwdNuevo", pwdKey);
            ht.Add("@pwdSalt", pwdSalt);
            ht.Add("@mixed", mixed);
            ht.Add("@appID", requerimiento.Aplicacion);

            pwd = SPs.executeSP("wsU_password_updatePassword", ht, log);

            if (!string.IsNullOrEmpty(requerimiento.Email))
            {
                correoEnviado = LNSMS.envioCorreo(new Models.Correo
                {
                    correoEmisor = ConfigurationManager.AppSettings["correo"].ToString(),
                    usuario = AzureExtensions.ObtenerValorSecretoAzure(ConfigurationManager.AppSettings["usuarioCorreo"].ToString(), "", "", log),
                    correoReceptor = requerimiento.Email,// smsTokenEmail.Correo,
                    host = ConfigurationManager.AppSettings["host"].ToString(),
                    puerto = ConfigurationManager.AppSettings["puerto"].ToString(),
                    password = AzureExtensions.ObtenerValorSecretoAzure(ConfigurationManager.AppSettings["passCorreo"].ToString(), "", "", log),
                    titulo = "Nuevo password temporal",
                    cuerpoMensaje = $"Su password temporal es el siguiente: {requerimiento.PasswordNuevo}"
                }, log);
            }

            return pwd;
        }
    }
}