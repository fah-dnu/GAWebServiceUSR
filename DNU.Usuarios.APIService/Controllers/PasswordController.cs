using DNU.Usuarios.APIService.Negocio;
using DNU.Usuarios.Common;
using DNU.Usuarios.Common.Utilerias;
using DNU.Usuarios.DataContract.Request;
using DNU.Usuarios.DataContract.Response;
using System;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace DNU.Usuarios.APIService.Controllers
{
    [Authorize]
    [RoutePrefix("api/Password")]
    public class PasswordController : ApiController
    {
        private Logueo log;

        public PasswordController()
        {
            log = new Logueo("");
        }

        public IHttpActionResult Post([FromBody]RequerimientoPasswordPost requerimiento)
        {
            log.User = requerimiento.NombreUsuario;
            log.EntradaSalida("[POST: api/Password/] " ,"DNU.Usuarios",true);

            var pwd = new ResponseGral()
            {
                CodigoRespuesta = 99,
                Mensaje = "Error inesperado. Consulte al administrador"
            };
            if (requerimiento is null)
            {
                pwd.CodigoRespuesta = 56;
                pwd.Mensaje = "Solicitud incorrecta, revise el valor de los parámetros enviados";

                var respJsonE = JsonSerializer.Serialize(pwd);
                log.Error("[POST: api/Password/] " + respJsonE);
                IHttpActionResult responseE;
                HttpResponseMessage msgResponseE = new HttpResponseMessage()
                {
                    Content = new StringContent(respJsonE, Encoding.UTF8, "application/json")
                };
                responseE = ResponseMessage(msgResponseE);
                log.EntradaSalida("[POST: api/Password/] " + respJsonE, "DNU.Usuarios", false);
                return responseE;
            }

            try
            {
               
                requerimiento.NombreUsuario = requerimiento.NombreUsuario.Trim();
                requerimiento.PasswordActual = requerimiento.PasswordActual.Trim();
                requerimiento.PasswordNuevo = requerimiento.PasswordNuevo.Trim();
                pwd = LNPassword.insertPwd(requerimiento, log);
            }
            catch (Exception ex)
            {
                log.Error("[POST: api/Password/] " + "[" + ex.Message + "]" + ex.StackTrace);
            }
            var respJson = JsonSerializer.Serialize(pwd);
            log.EntradaSalida("[POST: api/Password/] " + respJson, "DNU.Usuarios", false);
            IHttpActionResult response;
            HttpResponseMessage msgResponse = new HttpResponseMessage()
            {
                Content = new StringContent(respJson, Encoding.UTF8, "application/json")
            };
            response = ResponseMessage(msgResponse);
            return response;
        }

        [AllowAnonymous]
        public IHttpActionResult Put([FromBody]RequerimientoPasswordPut requerimiento)
        {
            log.User = requerimiento.NombreUsuario;
            log.EntradaSalida("[ PUT: api/Password/ ]" + JsonSerializer.Serialize(new { requerimiento.Aplicacion, requerimiento.NombreUsuario}), "DNU.Usuarios",true);

            var pwd = new ResponseGral()
            {
                CodigoRespuesta = 99,
                Mensaje = "Error inesperado. Consulte al administrador"
            };

            if (requerimiento is null)
            {
                pwd.CodigoRespuesta = 56;
                pwd.Mensaje = "Solicitud incorrecta, revise el valor de los parámetros enviados";

                var respJsonE = JsonSerializer.Serialize(pwd);
                log.Error("[PUT: api/Password/] " + respJsonE);
                IHttpActionResult responseE;
                HttpResponseMessage msgResponseE = new HttpResponseMessage()
                {
                    Content = new StringContent(respJsonE, Encoding.UTF8, "application/json")
                };
                responseE = ResponseMessage(msgResponseE);
                log.EntradaSalida("[ PUT: api/Password/ ]" + respJsonE, "DNU.Usuarios", false);
                return responseE;
            }

            try
            {
                requerimiento.NombreUsuario = requerimiento.NombreUsuario.Trim();
                requerimiento.PasswordNuevo = requerimiento.PasswordNuevo.Trim();
                pwd = LNPassword.updatePwd(requerimiento, log);
            }
            catch (Exception ex)
            {
                log.Error("[PUT: api/Password/] " + "[" + ex.Message + "]" + ex.StackTrace);
            }

            var respJson = JsonSerializer.Serialize(pwd);
            log.EntradaSalida("[ PUT: api/Password/ ]" + respJson, "DNU.Usuarios", false);
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
