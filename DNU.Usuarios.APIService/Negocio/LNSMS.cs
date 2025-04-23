using DNU.Usuarios.APIService.Models;
using DNU.Usuarios.APIService.WebReferenceSMS;
using DNU.Usuarios.Common;
using DNU.Usuarios.Common.Extensiones;
using DNU.Usuarios.Common.Utilerias;
using DNU.Usuarios.DataContract.BD;
using DNU.Usuarios.DataContract.Request;
using DNU.Usuarios.DataContract.Response;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;

namespace DNU.Usuarios.APIService.Negocio
{
    public class LNSMS
    {
        public static ResponseTokenSMS updateSMS(RequerimientoSMSPut requerimiento, Logueo log)
        {
            Hashtable ht = new Hashtable();
            Hashtable htLimpiaToken = new Hashtable();
            ResponseTokenSMS smsToken = new ResponseTokenSMS();
            ResponseTokenSMSEMAIL smsTokenEmail = new ResponseTokenSMSEMAIL();
            ResponseEmail userEmail = new ResponseEmail();

            //valida parametro tarjeta
            if (!string.IsNullOrEmpty(requerimiento.Tarjeta))
            {
                ht.Add("@tarjeta", requerimiento.Tarjeta);
                userEmail = SPs.executeSPAU("[ws_Parabilium_ConsultaDNUUsuario]", ht, log);
                ht = new Hashtable();
            }
            if (!string.IsNullOrEmpty(userEmail.idUsuario))
            {
                ht.Add("@userID", userEmail.idUsuario);

            }
            else
            {
                ht.Add("@userID", requerimiento.UserID);
            }
            ////
            //
            if (string.IsNullOrEmpty(userEmail.idUsuario) &&
                string.IsNullOrEmpty(requerimiento.UserID) &&
               (!string.IsNullOrEmpty(requerimiento.Tarjeta))
             )
            {
                smsToken.CodigoRespuesta = 98;
                smsToken.Mensaje = "No es posible generar un código de autenticación. El tarjetahabiente no cuenta con Onboarding.";
                return smsToken;
            }


            ht.Add("@nombreUsuario", requerimiento.NombreUsuario);

            if (!string.IsNullOrEmpty(requerimiento.Telefono))
            {
                ht.Add("@telefonoUsuario", requerimiento.Telefono);
            }

            if (!string.IsNullOrEmpty(requerimiento.Email))
            {
                ht.Add("@correoUsuario", requerimiento.Email);
            }

            // smsToken = SPs.executeSPToken("wsU_usuarios_updateTokenSMS", ht, log);
            smsTokenEmail = SPs.executeSPTokenWithEmail("wsU_usuarios_updateTokenSMS", ht, log);
            smsToken = new ResponseTokenSMS
            {
                CodigoRespuesta = smsTokenEmail.CodigoRespuesta,
                Mensaje = smsTokenEmail.Mensaje,
                Token = smsTokenEmail.Token
            };

            if (!string.IsNullOrEmpty(requerimiento.Tarjeta))
            {
                smsTokenEmail.Correo = userEmail.Correo;
            }

            if (string.IsNullOrEmpty(requerimiento.Telefono))
            {
                requerimiento.Telefono = smsTokenEmail.Telefono.ToString();
            }

            if ((!string.IsNullOrEmpty(requerimiento.Tarjeta)) &&
                (!string.IsNullOrEmpty(userEmail.Telefono))
                && (string.IsNullOrEmpty(requerimiento.Telefono)))
            {
                requerimiento.Telefono = userEmail.Telefono;
            }



            if (smsToken.CodigoRespuesta == 0)
            {
                try
                {
                    bool enviarCorreo = Convert.ToBoolean(ConfigurationManager.AppSettings["envioCorreo"].ToString());
                    //correo
                    if (enviarCorreo)
                    {
                        envioCorreo(new Correo
                        {
                            correoEmisor = ConfigurationManager.AppSettings["correo"].ToString(),
                            usuario = AzureExtensions.ObtenerValorSecretoAzure(ConfigurationManager.AppSettings["usuarioCorreo"].ToString(), "", "", log),
                            correoReceptor = smsTokenEmail.Correo,
                            host = ConfigurationManager.AppSettings["host"].ToString(),
                            puerto = ConfigurationManager.AppSettings["puerto"].ToString(),
                            password = AzureExtensions.ObtenerValorSecretoAzure(ConfigurationManager.AppSettings["passCorreo"].ToString(), "", "", log),
                            titulo = "Código SMS"
                        }, smsToken.Token, log);
                    }
                }
                catch (Exception ex)
                {
                    log.Error("[updateSMS] " + ex.Message);
                }
                wsMensaje paramMensaje = new wsMensaje();
                paramMensaje.Destinatario = requerimiento.Telefono;
                paramMensaje.MensajeSMS = ConfigurationManager.AppSettings["textoSMS"].ToString() + smsToken.Token;
                paramMensaje.wsPassword = AzureExtensions.ObtenerValorSecretoAzure(ConfigurationManager.AppSettings["pwdSMS"].ToString(), "", "", log);// ConfigurationManager.AppSettings["pwdSMS"].ToString();
                paramMensaje.wsUsuario = ConfigurationManager.AppSettings["userSMS"].ToString();


                var operSMS = new WebReferenceSMS.ServicioMensajes();
                log.EntradaSalida("[wsSMS] " + JsonSerializer.Serialize(operSMS), "DNU.Usuarios", false);
                var wsrespuestaSMS = operSMS.MENSAJERO_EnviarMensaje(paramMensaje);
                wsrespuestaSMS.Mensaje = ConfigurationManager.AppSettings["textoSMS"].ToString() + "***"+smsToken.Token.Substring(2);
                log.EntradaSalida("[wsSMS] " + JsonSerializer.Serialize(wsrespuestaSMS), "DNU.Usuarios", true);

                if (wsrespuestaSMS.CodigoRespuesta == 0)
                {
                    smsToken.CodigoRespuesta = 0;
                    smsToken.Mensaje = "SMS Enviado correctamente";
                }
                else
                {
                    smsToken.CodigoRespuesta = wsrespuestaSMS.CodigoRespuesta;
                    smsToken.Mensaje = wsrespuestaSMS.Descripcion;

                    htLimpiaToken.Add("@userID", requerimiento.UserID);
                    htLimpiaToken.Add("@nombreUsuario", requerimiento.NombreUsuario);

                    smsToken = SPs.executeSPToken("wsU_usuarios_updateLimpiaTokenSMS", htLimpiaToken, log);
                    if (smsToken.CodigoRespuesta == 0)
                    {
                        smsToken.CodigoRespuesta = 9;
                        smsToken.Mensaje = "Se elimino el Token generado, debido a que no se envió el SMS";
                        //    log.Evento("[updateSMS] " + JsonSerializer.Serialize(smsToken));
                    }
                }
            }
            else
            {
                smsToken.CodigoRespuesta = smsToken.CodigoRespuesta;
                smsToken.Mensaje = smsToken.Mensaje;
                //    log.Evento("[updateSMS] " + JsonSerializer.Serialize(smsToken));
            }

            return smsToken;
        }

        public static ResponseTokenSMS updateSMSV2(RequerimientoSMSPutV2 requerimiento, Logueo log)
        {
            Hashtable ht = new Hashtable();
            ResponseTokenSMS smsToken = new ResponseTokenSMS();

            // smsToken = SPs.executeSPToken("wsU_usuarios_updateTokenSMS", ht, log);
            if (string.IsNullOrEmpty(requerimiento.Token))
            {
                smsToken = SPs.executeSPTokenV2("wsU_usuarios_GeneraTokenSMS", ht, log);
            }
            else
            {
                smsToken = new ResponseTokenSMS { CodigoRespuesta = 0, Mensaje = "Token Recibido", Token = requerimiento.Token };
            }

            if (smsToken.CodigoRespuesta == 0)
            {

                try
                {
                    bool enviarCorreo = Convert.ToBoolean(ConfigurationManager.AppSettings["envioCorreo"].ToString());
                    //correo
                    if (enviarCorreo)
                    {
                        envioCorreo(new Correo
                        {
                            correoEmisor = ConfigurationManager.AppSettings["correo"].ToString(),
                            usuario = AzureExtensions.ObtenerValorSecretoAzure(ConfigurationManager.AppSettings["usuarioCorreo"].ToString(), "", "", log),
                            correoReceptor = requerimiento.Correo,
                            host = ConfigurationManager.AppSettings["host"].ToString(),
                            puerto = ConfigurationManager.AppSettings["puerto"].ToString(),
                            password = AzureExtensions.ObtenerValorSecretoAzure(ConfigurationManager.AppSettings["passCorreo"].ToString(), "", "", log),
                            titulo = "Código SMS"
                        }, smsToken.Token, log);
                    }
                }
                catch (Exception ex)
                {
                    log.Error("[updateSMSV2] " + ex.Message);
                }

                wsMensaje paramMensaje = new wsMensaje();
                paramMensaje.Destinatario = requerimiento.Telefono;
                paramMensaje.MensajeSMS = ConfigurationManager.AppSettings["textoSMS"].ToString() + smsToken.Token;
                paramMensaje.wsPassword = AzureExtensions.ObtenerValorSecretoAzure(ConfigurationManager.AppSettings["pwdSMS"].ToString(), "", "", log);// ConfigurationManager.AppSettings["pwdSMS"].ToString();
                paramMensaje.wsUsuario = ConfigurationManager.AppSettings["userSMS"].ToString();

                var operSMS = new WebReferenceSMS.ServicioMensajes();
                log.EntradaSalida("[wsSMSV2] " + JsonSerializer.Serialize(operSMS), "DNU.Usuarios", false);
                var wsrespuestaSMS = operSMS.MENSAJERO_EnviarMensaje(paramMensaje);
                wsrespuestaSMS.Mensaje = ConfigurationManager.AppSettings["textoSMS"].ToString() + "***" + smsToken.Token.Substring(2);
                log.EntradaSalida("[wsSMSV2] " + JsonSerializer.Serialize(wsrespuestaSMS), "DNU.Usuarios", true);

                if (wsrespuestaSMS.CodigoRespuesta == 0)
                {
                    smsToken.CodigoRespuesta = 0;
                    smsToken.Mensaje = "SMS Enviado correctamente";
                }
                else
                {
                    smsToken.CodigoRespuesta = wsrespuestaSMS.CodigoRespuesta;
                    smsToken.Mensaje = wsrespuestaSMS.Descripcion;
                }
            }
            else
            {
                smsToken.CodigoRespuesta = smsToken.CodigoRespuesta;
                smsToken.Mensaje = smsToken.Mensaje;
                //    log.Evento("[updateSMS] " + JsonSerializer.Serialize(smsToken));
            }

            return smsToken;
        }

        public static ResponseGral insertSMS(RequerimientoSMSPost requerimiento, Logueo log)
        {
            ResponseGral sms = new ResponseGral();
            Hashtable ht = new Hashtable();

            if (requerimiento.TokenSMS == null)
            {
                sms.CodigoRespuesta = 17;
                sms.Mensaje = "El código no puede ser vacío";
            }
            else if (requerimiento.TokenSMS.Trim().Length < 6)
            {
                sms.CodigoRespuesta = 18;
                sms.Mensaje = "El código debe ser de 6 posiciones";
            }
            else
            {

                ResponseEmail userEmail = new ResponseEmail();
                //valida parametro tarjeta
                if (!string.IsNullOrEmpty(requerimiento.Tarjeta))
                {
                    ht.Add("@tarjeta", requerimiento.Tarjeta);
                    userEmail = SPs.executeSPAU("[ws_Parabilium_ConsultaDNUUsuario]", ht, log);
                    ht = new Hashtable();
                }
                if (!string.IsNullOrEmpty(userEmail.idUsuario))
                {
                    ht.Add("@userID", userEmail.idUsuario);

                }
                else
                {
                    ht.Add("@userID", requerimiento.UserID);
                }

               // ht.Add("@userID", requerimiento.UserID);
                ht.Add("@nombreUsuario", requerimiento.NombreUsuario);
                ht.Add("@tokenSMS", requerimiento.TokenSMS);

                sms = SPs.executeSP("wsR_usuarios_reportTokenSMS", ht, log);
            }

            return sms;
        }


        public static bool envioCorreo(Correo _correo, string otp, Logueo log)
        {
            string nombreMes = "";

            StringBuilder linea = new StringBuilder();
            bool envioCorrecto = true;
            try
            {
                /*-------------------------MENSAJE DE CORREO----------------------*/

                //Creamos un nuevo Objeto de mensaje
                MailMessage mmsg = new MailMessage();

                //Direccion de correo electronico a la que queremos enviar el mensaje
                mmsg.To.Add(_correo.correoReceptor);

                //Nota: La propiedad To es una colección que permite enviar el mensaje a más de un destinatario

                //Asunto
                mmsg.Subject = _correo.titulo;
                mmsg.SubjectEncoding = System.Text.Encoding.UTF8;

                //Direccion de correo electronico que queremos que reciba una copia del mensaje
                // mmsg.Bcc.Add("pruebaPagoPinPad@outlook.com"); //Opcional

                //Cuerpo del Mensaje
                DateTime Hoy = DateTime.Now;
                string fechaCreacion = Hoy.ToString("dd/MM/yyyy");
                linea.AppendLine("");
                linea.AppendLine("Su codigo OTP es el siguiente: " + otp);
                //linea.AppendLine("Fecha: " + fechaCreacion/*DateTime.Now.ToShortDateString()*/ + " Hora: " + DateTime.Now.ToShortTimeString());


                mmsg.Body = linea.ToString();
                //body
                mmsg.BodyEncoding = System.Text.Encoding.UTF8;
                mmsg.IsBodyHtml = false; //Si no queremos que se envíe como HTML

                //Correo electronico desde la que enviamos el mensaje
                mmsg.From = new MailAddress(_correo.correoEmisor);
                //  mmsg.ReplyToList.Add(new MailAddress("foo@bar.net"));
                //archivos adjuntos

                //la lista se llena con direcciones fisicas, por ejemplo: c:/pato.txt
                if (_correo.archivos != null)
                {
                    //agregado de archivo
                    foreach (string archivo in _correo.archivos)
                    {
                        //comprobamos si existe el archivo y lo agregamos a los adjuntos
                        if (System.IO.File.Exists(@archivo))
                            mmsg.Attachments.Add(new Attachment(@archivo));

                    }
                }

                /*-------------------------CLIENTE DE CORREO----------------------*/

                //Creamos un objeto de cliente de correo
                SmtpClient cliente = new SmtpClient();

                //Hay que crear las credenciales del correo emisor
                cliente.UseDefaultCredentials = false;
                cliente.Credentials =
                    new System.Net.NetworkCredential(_correo.usuario,
                     _correo.password);

                //Lo siguiente es obligatorio si enviamos el mensaje desde Gmail
                /*
                cliente.Port = 587;
                cliente.EnableSsl = true;
                */

                cliente.Host = _correo.host;//"smtp-mail.outlook.com"; //"smtp.live.com"; //Para Gmail "smtp.gmail.com";
                cliente.Port = Convert.ToInt32(_correo.puerto);//587;
                cliente.EnableSsl = false;// true;// true;
                cliente.DeliveryMethod = SmtpDeliveryMethod.Network;
                // cliente.UseDefaultCredentials = false;

                /*-------------------------ENVIO DE CORREO----------------------*/

                try
                {
                    //Enviamos el mensaje      
                    cliente.Send(mmsg);
                }
                catch (System.Net.Mail.SmtpException e)
                {
                    //El servidor SMTP requiere una conexión segura o el cliente no se autenticó. La respuesta del servidor fue: 5.7.57 SMTP; Client was not authenticated to send anonymous mail during MAIL FROM [BN4PR12CA0013.namprd12.prod.outlook.com]
                    //requiere ssl

                    //Aquí gestionamos los errores al intentar enviar el correo
                    //  LogsPinPad.generaLog(rutaLogError, "Mensaje:" + e.Message + " " + e.StackTrace);

                    log.Error("[wsSMS] " + e.Message + "DNU.Usuarios");

                    envioCorrecto = false;

                }
            }
            catch (Exception ex)
            {
                log.Error("[wsSMS] " + ex.Message + "DNU.Usuarios");
                envioCorrecto = false;

            }
            return envioCorrecto;
        }

        public static bool envioCorreo(Correo _correo, Logueo log)
        {
            string nombreMes = "";

            StringBuilder linea = new StringBuilder();
            bool envioCorrecto = true;
            try
            {
                /*-------------------------MENSAJE DE CORREO----------------------*/

                //Creamos un nuevo Objeto de mensaje
                MailMessage mmsg = new MailMessage();

                //Direccion de correo electronico a la que queremos enviar el mensaje
                mmsg.To.Add(_correo.correoReceptor);

                //Nota: La propiedad To es una colección que permite enviar el mensaje a más de un destinatario

                //Asunto
                mmsg.Subject = _correo.titulo;
                mmsg.SubjectEncoding = System.Text.Encoding.UTF8;

                //Direccion de correo electronico que queremos que reciba una copia del mensaje
                // mmsg.Bcc.Add("pruebaPagoPinPad@outlook.com"); //Opcional

                //Cuerpo del Mensaje
                DateTime Hoy = DateTime.Now;
                string fechaCreacion = Hoy.ToString("dd/MM/yyyy");
                linea.AppendLine("");
                linea.AppendLine(_correo.cuerpoMensaje);
                //linea.AppendLine("Fecha: " + fechaCreacion/*DateTime.Now.ToShortDateString()*/ + " Hora: " + DateTime.Now.ToShortTimeString());


                mmsg.Body = linea.ToString();
                //body
                mmsg.BodyEncoding = System.Text.Encoding.UTF8;
                mmsg.IsBodyHtml = false; //Si no queremos que se envíe como HTML

                //Correo electronico desde la que enviamos el mensaje
                mmsg.From = new MailAddress(_correo.correoEmisor);
                //  mmsg.ReplyToList.Add(new MailAddress("foo@bar.net"));
                //archivos adjuntos

                //la lista se llena con direcciones fisicas, por ejemplo: c:/pato.txt
                if (_correo.archivos != null)
                {
                    //agregado de archivo
                    foreach (string archivo in _correo.archivos)
                    {
                        //comprobamos si existe el archivo y lo agregamos a los adjuntos
                        if (System.IO.File.Exists(@archivo))
                            mmsg.Attachments.Add(new Attachment(@archivo));

                    }
                }

                /*-------------------------CLIENTE DE CORREO----------------------*/

                //Creamos un objeto de cliente de correo
                SmtpClient cliente = new SmtpClient();

                //Hay que crear las credenciales del correo emisor
                cliente.UseDefaultCredentials = false;
                cliente.Credentials =
                    new System.Net.NetworkCredential(_correo.usuario,
                     _correo.password);

                //Lo siguiente es obligatorio si enviamos el mensaje desde Gmail
                /*
                cliente.Port = 587;
                cliente.EnableSsl = true;
                */

                cliente.Host = _correo.host;//"smtp-mail.outlook.com"; //"smtp.live.com"; //Para Gmail "smtp.gmail.com";
                cliente.Port = Convert.ToInt32(_correo.puerto);//587;
                cliente.EnableSsl = false;// true;// true;
                cliente.DeliveryMethod = SmtpDeliveryMethod.Network;
                // cliente.UseDefaultCredentials = false;

                /*-------------------------ENVIO DE CORREO----------------------*/

                try
                {
                    //Enviamos el mensaje      
                    cliente.Send(mmsg);
                }
                catch (System.Net.Mail.SmtpException e)
                {
                    //El servidor SMTP requiere una conexión segura o el cliente no se autenticó. La respuesta del servidor fue: 5.7.57 SMTP; Client was not authenticated to send anonymous mail during MAIL FROM [BN4PR12CA0013.namprd12.prod.outlook.com]
                    //requiere ssl

                    //Aquí gestionamos los errores al intentar enviar el correo
                    //  LogsPinPad.generaLog(rutaLogError, "Mensaje:" + e.Message + " " + e.StackTrace);

                    log.Error("[wsSMS] " + e.Message + "DNU.Usuarios");

                    envioCorrecto = false;

                }
            }
            catch (Exception ex)
            {
                log.Error("[wsSMS] " + ex.Message + "DNU.Usuarios");
                envioCorrecto = false;

            }
            return envioCorrecto;
        }
    }
}