using DNU.Usuarios.APIService.Negocio;
using DNU.Usuarios.Common;
using DNU.Usuarios.Common.Utilerias;
using DNU.Usuarios.DataContract.Request;
using DNU.Usuarios.DataContract.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace DNU.Usuarios.APIService.Controllers
{

    public class LogInController : ApiController
    {
        private Logueo log;

        public LogInController()
        {
            log = new Logueo("");
        }

        [AllowAnonymous]
        public IHttpActionResult Post([FromBody] RequerimientoLogInValidaCredenciales requerimiento)
        {
            log.User = requerimiento.NombreUsuario;
            log.EntradaSalida("[POST: api/LogIn/] " + JsonSerializer.Serialize(new { requerimiento.Aplicacion, requerimiento.NombreUsuario, requerimiento.Cifrado}), "DBU.Usuarios", true);

            var logIn = new ResponseLogInPost()
            {
                CodigoRespuesta = 99,
                Mensaje = "Error inesperado. Consulte al administrador"
            };

            if (requerimiento is null)
            {
                logIn.CodigoRespuesta = 56;
                logIn.Mensaje = "Solicitud incorrecta, revise el valor de los parámetros enviados";

                var respJsonE = JsonSerializer.Serialize(logIn);
                log.Error("[POST: api/LogIn/] " + respJsonE);
                IHttpActionResult responseE;
                HttpResponseMessage msgResponseE = new HttpResponseMessage()
                {
                    Content = new StringContent(respJsonE, Encoding.UTF8, "application/json")
                };
                responseE = ResponseMessage(msgResponseE);
                log.EntradaSalida("[POST: api/LogIn/] " + respJsonE, "DBU.Usuarios", true);
                return responseE;
            }

            try
            {
                logIn = LNLogIn.logInUsuario(requerimiento, log);
                log.EntradaSalida("[POST: api/LogIn/] " + "{CodigoRespuesta:" + logIn.CodigoRespuesta + ",Mensaje:" + logIn.Mensaje + "}", "DBU.Usuarios", false);
            }
            catch (Exception ex)
            {
                log.Error("[POST: api/LogIn/] " + "[" + ex.Message + "]" + ex.StackTrace);

            }
            var respJson = JsonSerializer.Serialize(logIn);
            //
            IHttpActionResult response;
            HttpResponseMessage msgResponse = new HttpResponseMessage()
            {
                Content = new StringContent(respJson, Encoding.UTF8, "application/json")
            };
            response = ResponseMessage(msgResponse);
            return response;
        }

        [Authorize]
        public HttpResponseMessage Get()
        {
            HttpStatusCode statusCode;

            statusCode = HttpStatusCode.OK;
            // log.EntradaSalida("[GET: api/LogIn/] [validaToken]", "DNU.Usuarios", true);

            return new HttpResponseMessage(statusCode);
        }

        [Authorize]
        public IHttpActionResult Delete([FromBody] RequerimientoLogInDelete requerimiento)
        {

            string nombre = "";
            try
            {
                nombre = RequestContext.Principal.Identity.Name;
            }
            catch (Exception ex)
            {

            }

            log.User = nombre;//requerimiento.NombreUsuario;
            log.EntradaSalida("[DELETE: api/LogIn/] " + JsonSerializer.Serialize(requerimiento), "DNU.Usuarios", true);

            var logIn = new ResponseGral()
            {
                CodigoRespuesta = 99,
                Mensaje = "Error inesperado. Consulte al administrador"
            };
            if (requerimiento is null)
            {
                logIn.CodigoRespuesta = 56;
                logIn.Mensaje = "Solicitud incorrecta, revise el valor de los parámetros enviados";

                var respJsonE = JsonSerializer.Serialize(logIn);
                log.Error("[DELETE: api/LogIn/] " + respJsonE);
                IHttpActionResult responseE;
                HttpResponseMessage msgResponseE = new HttpResponseMessage()
                {
                    Content = new StringContent(respJsonE, Encoding.UTF8, "application/json")
                };
                responseE = ResponseMessage(msgResponseE);
                log.EntradaSalida("[DELETE: api/LogIn/] " + respJsonE, "DNU.Usuarios", false);
                return responseE;
            }

            try
            {
                string token = null;
                IEnumerable<string> authzHeaders;
                Request.Headers.TryGetValues("Authorization", out authzHeaders);
                var bearerToken = authzHeaders.ElementAt(0);
                token = bearerToken.StartsWith("Bearer ") ? bearerToken.Substring(7) : bearerToken;
                logIn = LNLogIn.deleteSession(requerimiento, token, log);
            }
            catch (Exception ex)
            {
                log.Error("[DELETE: api/LogIn/] " + "[" + ex.Message + "]" + ex.StackTrace);
            }
            var respJson = JsonSerializer.Serialize(logIn);
            log.EntradaSalida("[DELETE: api/LogIn/] " + respJson, "DNU.Usuarios", false);
            IHttpActionResult response;
            HttpResponseMessage msgResponse = new HttpResponseMessage()
            {
                Content = new StringContent(respJson, Encoding.UTF8, "application/json")
            };
            response = ResponseMessage(msgResponse);
            return response;
        }


        [HttpPost]
        [Route("api/LogIn/ValidarCredenciales/")]
        [AllowAnonymous]
        public IHttpActionResult validarCredenciales([FromBody] RequerimientoLogInValidaCredenciales requerimiento)
        {
            //[FromBody]RequerimientoLogInValidaCredenciales requerimiento)
            //RequerimientoLogInValidaCredenciales requerimiento = new RequerimientoLogInValidaCredenciales();
            //try
            //{
            //    var bytes = Request.Content.ReadAsByteArrayAsync().Result;
            //    string peticionUTF8 = Encoding.UTF8.GetString(bytes);
            //    requerimiento = JsonSerializer.Deserialize<RequerimientoLogInValidaCredenciales>(peticionUTF8);
            //    log.EntradaSalida("[POST: api/ValidaCredenciales/] " + JsonSerializer.Serialize(new { requerimiento.NombreUsuario, requerimiento.Cifrado }), "DNU.Usuarios", true);

            //    bytes = Request.Content.ReadAsByteArrayAsync().Result;
            //    string peticionDefault = Encoding.Default.GetString(bytes);
            //    requerimiento = JsonSerializer.Deserialize<RequerimientoLogInValidaCredenciales>(peticionDefault);
            //    log.EntradaSalida("[POST: api/ValidaCredenciales Default/] " + JsonSerializer.Serialize(new { requerimiento.NombreUsuario, requerimiento.Cifrado }), "DNU.Usuarios", true);

            //    bytes = Request.Content.ReadAsByteArrayAsync().Result;
            //    string peticionASCII = Encoding.ASCII.GetString(bytes);
            //    requerimiento = JsonSerializer.Deserialize<RequerimientoLogInValidaCredenciales>(peticionASCII);
            //    log.EntradaSalida("[POST: api/ValidaCredenciales ASCII/] " + JsonSerializer.Serialize(new { requerimiento.NombreUsuario, requerimiento.Cifrado }), "DNU.Usuarios", true);
            //    try
            //    {
            //        bytes = Request.Content.ReadAsByteArrayAsync().Result;
            //        string peticionUnicode = Encoding.Unicode.GetString(bytes);
            //        requerimiento = JsonSerializer.Deserialize<RequerimientoLogInValidaCredenciales>(peticionASCII);
            //        log.EntradaSalida("[POST: api/ValidaCredenciales Unicode/] " + JsonSerializer.Serialize(new { requerimiento.NombreUsuario, requerimiento.Cifrado }), "DNU.Usuarios", true);
            //    }
            //    catch (Exception ex) { 
                
            //    }

            //    try
            //    {
            //        bytes = Request.Content.ReadAsByteArrayAsync().Result;
            //        string peticionUnicode = Encoding.UTF32.GetString(bytes);
            //        requerimiento = JsonSerializer.Deserialize<RequerimientoLogInValidaCredenciales>(peticionASCII);
            //        log.EntradaSalida("[POST: api/ValidaCredenciales UTF32/] " + JsonSerializer.Serialize(new { requerimiento.NombreUsuario, requerimiento.Cifrado }), "DNU.Usuarios", true);
            //    }
            //    catch (Exception ex)
            //    {

            //    }

            //    if (peticionUTF8.Contains('?'))
            //    {
            //        bytes = Request.Content.ReadAsByteArrayAsync().Result;
            //        peticionUTF8 = Encoding.Default.GetString(bytes);
            //        log.EntradaSalida("[POST: api/ValidaCredenciales Default/] " + JsonSerializer.Serialize(new { requerimiento.NombreUsuario, requerimiento.Cifrado }), "DNU.Usuarios", true);

            //        requerimiento = JsonSerializer.Deserialize<RequerimientoLogInValidaCredenciales>(peticionUTF8);
            //        if (peticionUTF8.Contains('?'))
            //        {
            //            bytes = Request.Content.ReadAsByteArrayAsync().Result;
            //            peticionUTF8 = Encoding.ASCII.GetString(bytes);
            //            log.EntradaSalida("[POST: api/ValidaCredenciales ASCII/] " + JsonSerializer.Serialize(new { requerimiento.NombreUsuario, requerimiento.Cifrado }), "DNU.Usuarios", true);

            //            requerimiento = JsonSerializer.Deserialize<RequerimientoLogInValidaCredenciales>(peticionUTF8);
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    log.Error("[POST: api/ValidarCredenciales/] " + "[" + ex.Message + "]" + ex.StackTrace);


            //}


            log.User = requerimiento.NombreUsuario;
            log.EntradaSalida("[POST: api/ValidaCredenciales/] " + JsonSerializer.Serialize(new { requerimiento.NombreUsuario, requerimiento.Cifrado }), "DNU.Usuarios", true);





            var validarCredenciales = new ResponseGral()
            {
                CodigoRespuesta = 99,
                Mensaje = "Error inesperado. Consulte al administrador"
            };

            if (requerimiento is null)
            {
                validarCredenciales.CodigoRespuesta = 56;
                validarCredenciales.Mensaje = "Solicitud incorrecta, revise el valor de los parámetros enviados";

                var respJsonE = JsonSerializer.Serialize(validarCredenciales);
                log.Error("[POST: api/ValidaCredenciales/] " + respJsonE);
                IHttpActionResult responseE;
                HttpResponseMessage msgResponseE = new HttpResponseMessage()
                {
                    Content = new StringContent(respJsonE, Encoding.UTF8, "application/json")
                };
                responseE = ResponseMessage(msgResponseE);
                log.EntradaSalida("[POST: api/ValidaCredenciales/] " + respJsonE, "DBU.Usuarios", false);
                return responseE;
            }

            try
            {
                validarCredenciales = LNLogIn.validarCredenciales(requerimiento, log);
            }
            catch (Exception ex)
            {
                log.Error("[POST: api/ValidaCredenciales/] " + "[" + ex.Message + "]" + ex.StackTrace);
            }
            var respJson = JsonSerializer.Serialize(validarCredenciales);
            log.EntradaSalida("[POST: api/ValidaCredenciales/] " + respJson, "DBU.Usuarios", false);
            IHttpActionResult response;
            HttpResponseMessage msgResponse = new HttpResponseMessage()
            {
                Content = new StringContent(respJson, Encoding.UTF8, "application/json")
            };
            response = ResponseMessage(msgResponse);
            return response;
        }


        [HttpPost]
        [Route("api/LogIn/generarToken/")]
        [AllowAnonymous]
        public IHttpActionResult generarToken([FromBody] RequerimientoGenToken requerimiento)
        {
            log.User = requerimiento.Usuario;
            log.EntradaSalida("[POST: api/LogIn/generarToken] " + JsonSerializer.Serialize(requerimiento), "DBU.Usuarios", true);

            var genToken = new ResponseGenToken()
            {
                CodigoRespuesta = 99,
                Mensaje = "Error inesperado. Consulte al administrador"
            };

            if (requerimiento is null)
            {
                genToken.CodigoRespuesta = 56;
                genToken.Mensaje = "Solicitud incorrecta, revise el valor de los parámetros enviados";

                var respJsonE = JsonSerializer.Serialize(genToken);
                log.Error("[POST: api/LogIn/generarToken] " + respJsonE);
                IHttpActionResult responseE;
                HttpResponseMessage msgResponseE = new HttpResponseMessage()
                {
                    Content = new StringContent(respJsonE, Encoding.UTF8, "application/json")
                };
                responseE = ResponseMessage(msgResponseE);
                log.EntradaSalida("[POST: api/LogIn/generarToken] " + respJsonE, "DBU.Usuarios", false);
                return responseE;
            }

            try
            {
                genToken = LNLogIn.generarToken(requerimiento, log);
            }
            catch (Exception ex)
            {
                log.Error("[POST: api/LogIn/generarToken] " + "[" + ex.Message + "]" + ex.StackTrace);
            }
            var respJson = JsonSerializer.Serialize(genToken);
            log.EntradaSalida("[POST: api/LogIn/generarToken] " + respJson, "DBU.Usuarios", false);
            IHttpActionResult response;
            HttpResponseMessage msgResponse = new HttpResponseMessage()
            {
                Content = new StringContent(respJson, Encoding.UTF8, "application/json")
            };
            response = ResponseMessage(msgResponse);
            return response;
        }
    }
}
