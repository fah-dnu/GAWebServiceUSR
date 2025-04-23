using DALCentralAplicaciones.Entidades;
using DNU.Usuarios.APIService.Negocio;
using DNU.Usuarios.Common;
using DNU.Usuarios.Common.Utilerias;
using DNU.Usuarios.DataContract.Request;
using DNU.Usuarios.DataContract.Response;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace DNU.Usuarios.APIService.Controllers
{
    [Authorize]
    [RoutePrefix("api/Roles")]
    public class RolesController : ApiController
    {
        private Logueo log;

        public RolesController() 
        {
            log = new Logueo("");
        }
        // GET: api/Roles
        //public IEnumerable<string> Get()
        public IHttpActionResult Get(string IDSolicitud)
        {
            log.EntradaSalida("[get: api/Roles/] " + JsonSerializer.Serialize(IDSolicitud), "DNU.Usuarios", true);

            var aplicacion = new ResponseRolesGet()
            {
                CodigoRespuesta = 99,
                Mensaje = "Error inesperado. Consulte al administrador"
            };

            try
            {
                aplicacion = LNRoles.ObtieneRolesDisponibles(IDSolicitud, log);
            }
            catch (Exception ex)
            {
                log.Error("[POST: api/Roles/] " + "[" + ex.Message + "]" + ex.StackTrace);
            }

            var respJson = JsonSerializer.Serialize(aplicacion);
            log.EntradaSalida("[POST: api/Roles/] " + respJson, "DNU.Usuarios", false);
            IHttpActionResult response;
            HttpResponseMessage msgResponse = new HttpResponseMessage()
            {
                Content = new StringContent(respJson, Encoding.UTF8, "application/json")
            };
            response = ResponseMessage(msgResponse);

            return response;
        }

        // GET: api/Roles/5
        public string Get(int id)
        {
            return "value";
        }

        // PUT: api/Roles/5
        public IHttpActionResult Put([FromBody]RequerimientoRolesPut requerimiento)
        {
            log.EntradaSalida("[POST: api/Roles/] " + JsonSerializer.Serialize(new
            {
                requerimiento.IDSolicitud,
                requerimiento.UserID,
                requerimiento.NombreUsuario,
                requerimiento.RolID
            }), "DNU.Usuarios", true);

            var asignacionResponse = new ResponseGral()
            {
                CodigoRespuesta = 99,
                Mensaje = "Error inesperado. Consulte al administrador"
            };

            if (requerimiento is null)
            {
                asignacionResponse.CodigoRespuesta = 56;
                asignacionResponse.Mensaje = "Solicitud incorrecta, revise el valor de los parámetros enviados";

                var respJsonE = JsonSerializer.Serialize(asignacionResponse);
                log.Error("[POST: api/Roles/] " + respJsonE);
                IHttpActionResult responseE;
                HttpResponseMessage msgResponseE = new HttpResponseMessage()
                {
                    Content = new StringContent(respJsonE, Encoding.UTF8, "application/json")
                };
                responseE = ResponseMessage(msgResponseE);
                log.EntradaSalida("[POST: api/Roles/] " + respJsonE, "DNU.Usuarios", false);
                return responseE;
            }

            try
            {
                requerimiento.IDSolicitud = requerimiento.IDSolicitud.Trim();
                asignacionResponse = LNRoles.AsignaRolesUsuario(requerimiento, log);
            }
            catch (Exception ex)
            {
                log.Error("[POST: api/Roles/] " + "[" + ex.Message + "]" + ex.StackTrace);
            }

            var respJson = JsonSerializer.Serialize(asignacionResponse);
            log.EntradaSalida("[POST: api/Roles/] " + respJson, "DNU.Usuarios", false);
            IHttpActionResult response;
            HttpResponseMessage msgResponse = new HttpResponseMessage()
            {
                Content = new StringContent(respJson, Encoding.UTF8, "application/json")
            };
            response = ResponseMessage(msgResponse);

            return response;
        }

        // DELETE: api/Roles/5
        public void Delete(int id)
        {
        }
    }
}
