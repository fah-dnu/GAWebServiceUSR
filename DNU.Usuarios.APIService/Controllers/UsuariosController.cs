using DNU.Usuarios.APIService.Negocio;
using DNU.Usuarios.Common;
using DNU.Usuarios.Common.Utilerias;
using DNU.Usuarios.DataContract.BD;
using DNU.Usuarios.DataContract.Request;
using DNU.Usuarios.DataContract.Response;
using System;
using System.Collections;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Microsoft.Web.Http;

namespace DNU.Usuarios.APIService.Controllers
{
    [ApiVersion("1.0")]
    [ApiVersion("1.1")]
    [RoutePrefix("api/Usuarios")]
    public class UsuariosController : ApiController
    {
        private Logueo log;

        public UsuariosController()
        {
            log = new Logueo("");
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult Post([FromBody]RequerimientoUsuariosPost requerimiento)
        {
           // log.User = requerimiento.NombreUsuario;
            log.EntradaSalida("[POST: api/Usuarios/] " + JsonSerializer.Serialize(new { requerimiento.Aplicacion, requerimiento.NombreUsuario, requerimiento.AdministradorID,
                                                               requerimiento.Nombre}), "DNU.Usuarios",true);


            var usuario = new ResponseUsuarioPost()
            {
                CodigoRespuesta = 99,
                Mensaje = "Error inesperado. Consulte al administrador"
            };

            if (requerimiento is null)
            {
                usuario.CodigoRespuesta = 56;
                usuario.Mensaje = "Solicitud incorrecta, revise el valor de los parámetros enviados";

                var respJsonE = JsonSerializer.Serialize(usuario);
                log.Error("[POST: api/Usuarios/] " + respJsonE);
                IHttpActionResult responseE;
                HttpResponseMessage msgResponseE = new HttpResponseMessage()
                {
                    Content = new StringContent(respJsonE, Encoding.UTF8, "application/json")
                };
                responseE = ResponseMessage(msgResponseE);
                log.EntradaSalida("[POST: api/Usuarios/] " + respJsonE, "DNU.Usuarios", false);
                return responseE;
            }

            try
            {
                requerimiento.Nombre = requerimiento.Nombre.Trim();
                requerimiento.NombreUsuario = requerimiento.NombreUsuario.Trim();
                requerimiento.Password = requerimiento.Password.Trim();
                usuario = LNUsuarios.insertUsuarios(requerimiento, log);
            }
            catch (Exception ex)
            {
                log.Error("[POST: api/Usuarios/] " + "[" + ex.Message + "]" + ex.StackTrace);
            }
            var respJson = JsonSerializer.Serialize(usuario);
            log.EntradaSalida("[POST: api/Usuarios/] " + respJson, "DNU.Usuarios", false);
            IHttpActionResult response;
            HttpResponseMessage msgResponse = new HttpResponseMessage()
            {
                Content = new StringContent(respJson, Encoding.UTF8, "application/json")
            };
            response = ResponseMessage(msgResponse);

            return response;
        }

        [HttpPost]
        [Route("")]
        [MapToApiVersion("1.1")]
        public IHttpActionResult PostV1_1([FromBody] RequerimientoUsuariosPostV2 requerimiento)
        {
            // log.User = requerimiento.NombreUsuario;
            log.EntradaSalida("[POST: api/UsuariosV2/] " + JsonSerializer.Serialize(new
            {
                requerimiento.Aplicacion,
                requerimiento.NombreUsuario,
                requerimiento.AdministradorID,
                requerimiento.Nombre,
                requerimiento.CodigoDescuento
               
            }), "DNU.Usuarios", true);


            var usuario = new ResponseUsuarioPost()
            {
                CodigoRespuesta = 99,
                Mensaje = "Error inesperado. Consulte al administrador"
            };

            if (requerimiento is null)
            {
                usuario.CodigoRespuesta = 56;
                usuario.Mensaje = "Solicitud incorrecta, revise el valor de los parámetros enviados";

                var respJsonE = JsonSerializer.Serialize(usuario);
                log.Error("[POST: api/UsuariosV2/] " + respJsonE);
                IHttpActionResult responseE;
                HttpResponseMessage msgResponseE = new HttpResponseMessage()
                {
                    Content = new StringContent(respJsonE, Encoding.UTF8, "application/json")
                };
                responseE = ResponseMessage(msgResponseE);
                log.EntradaSalida("[POST: api/UsuariosV2/] " + respJsonE, "DNU.Usuarios", false);
                return responseE;
            }

            try
            {
                requerimiento.Nombre = requerimiento.Nombre.Trim();
                requerimiento.NombreUsuario = requerimiento.NombreUsuario.Trim();
                requerimiento.Password = requerimiento.Password.Trim();
                usuario = LNUsuarios.insertUsuariosV2(requerimiento, log);
            }
            catch (Exception ex)
            {
                log.Error("[POST: api/UsuariosV2/] " + "[" + ex.Message + "]" + ex.StackTrace);
            }
            var respJson = JsonSerializer.Serialize(usuario);
            log.EntradaSalida("[POST: api/UsuariosV2/] " + respJson, "DNU.Usuarios", false);
            IHttpActionResult response;
            HttpResponseMessage msgResponse = new HttpResponseMessage()
            {
                Content = new StringContent(respJson, Encoding.UTF8, "application/json")
            };
            response = ResponseMessage(msgResponse);

            return response;
        }

        [Authorize]
        public IHttpActionResult Put([FromBody]RequerimientoUsuariosPut requerimiento)
        {

            string nombre = "";
            try
            {
                nombre = RequestContext.Principal.Identity.Name;
            }
            catch (Exception ex)
            {

            }
            log.User = nombre;//requerimiento.nombre;

            log.EntradaSalida("[PUT: api/Usuarios/] " + JsonSerializer.Serialize(requerimiento),"DNU.USuarios",true);

            var usuario = new ResponseGral()
            {
                CodigoRespuesta = 99,
                Mensaje = "Error inesperado. Consulte al administrador"
            };

            if (requerimiento is null)
            {
                usuario.CodigoRespuesta = 56;
                usuario.Mensaje = "Solicitud incorrecta, revise el valor de los parámetros enviados";

                var respJsonE = JsonSerializer.Serialize(usuario);
                log.Error("[PUT: api/Usuarios/] " + respJsonE);
                IHttpActionResult responseE;
                HttpResponseMessage msgResponseE = new HttpResponseMessage()
                {
                    Content = new StringContent(respJsonE, Encoding.UTF8, "application/json")
                };
                responseE = ResponseMessage(msgResponseE);
                log.EntradaSalida("[PUT: api/Usuarios/] " + respJsonE, "DNU.USuarios", false);
                return responseE;
            }

            if (requerimiento.Desactivar is true)
            {
				try
				{
					usuario = LNUsuarios.desactivarUsuario(requerimiento, log);
				}
				catch (Exception ex)
				{
					log.Error("[PUT: api/DesactivarUsuario/] " + "[" + ex.Message + "]" + ex.StackTrace);
				}
				var respJsonD = JsonSerializer.Serialize(usuario);
				log.EntradaSalida("[PUT: api/DesactivarUsuario/] " + respJsonD, "DNU.Usuarios", false);
				IHttpActionResult responseD;
				HttpResponseMessage msgResponseD = new HttpResponseMessage()
				{
					Content = new StringContent(respJsonD, Encoding.UTF8, "application/json")
				};
				responseD = ResponseMessage(msgResponseD);
				return responseD;
			}

            try
            {
                usuario = LNUsuarios.updateUsuario(requerimiento, log);
            }
            catch (Exception ex)
            {
                log.Error("[PUT: api/Usuarios/] " + "[" + ex.Message + "]" + ex.StackTrace);
            }
            var respJson = JsonSerializer.Serialize(usuario);
            log.EntradaSalida("[PUT: api/Usuarios/] " + respJson, "DNU.USuarios", false);
            IHttpActionResult response;
            HttpResponseMessage msgResponse = new HttpResponseMessage()
            {
                Content = new StringContent(respJson, Encoding.UTF8, "application/json")
            };
            response = ResponseMessage(msgResponse);
            return response;
        }

        [Authorize]
        public IHttpActionResult Get()
        {
            string nombre = "";
            try
            {
                nombre = RequestContext.Principal.Identity.Name;
            }
            catch (Exception ex)
            {

            }

            var re = Request;
            var headers = re.Headers;
            string idUser = null;

            idUser = headers.GetValues("UserID").First();
            log.User = nombre;//idUser;
            log.EntradaSalida("[Get: api/Usuarios/] " + JsonSerializer.Serialize(idUser),"DNU.Usuarios",true);

            var usuario = new ResponseUsuariosGet()
            {
                CodigoRespuesta = 99,
                Mensaje = "Error inesperado. Consulte al administrador"
            };

            try
            {
                usuario = LNUsuarios.getUsuarios(idUser, log);
            }
            catch (Exception ex)
            {
                log.Error("[Get: api/Usuarios/] " + "[" + ex.Message + "]" + ex.StackTrace);
            }

            var respJson = JsonSerializer.Serialize(usuario);
            log.EntradaSalida("[Get: api/Usuarios/] " + respJson, "DNU.Usuarios", false);
            IHttpActionResult response;
            HttpResponseMessage msgResponse = new HttpResponseMessage()
            {
                Content = new StringContent(respJson, Encoding.UTF8, "application/json")
            };
            response = ResponseMessage(msgResponse);
            return response;
        }

        public IHttpActionResult Delete([FromBody]RequerimientoUsuariosDelete requerimiento)
        {
            log.User = requerimiento.UserId;
            log.EntradaSalida("[DELETE: api/Usuarios/] " + JsonSerializer.Serialize(new
            {
                requerimiento
            }), "DNU.Usuarios", true);

            var usuario = new ResponseGral()
            {
                CodigoRespuesta = 99,
                Mensaje = "Error inesperado. Consulte al administrador"
            };

            if (requerimiento is null)
            {
                usuario.CodigoRespuesta = 56;
                usuario.Mensaje = "Solicitud incorrecta, revise el valor de los parámetros enviados";

                var respJsonE = JsonSerializer.Serialize(usuario);
                log.Error("[DELETE: api/Usuarios/] " + respJsonE);
                IHttpActionResult responseE;
                HttpResponseMessage msgResponseE = new HttpResponseMessage()
                {
                    Content = new StringContent(respJsonE, Encoding.UTF8, "application/json")
                };
                responseE = ResponseMessage(msgResponseE);
                log.EntradaSalida("[DELETE: api/Usuarios/] " + respJsonE, "DNU.Usuarios", false);
                return responseE;
            }

            try
            {
                usuario = LNUsuarios.deleteUsuario(requerimiento, log);
            }
            catch (Exception ex)
            {
                log.Error("[DELETE: api/Usuarios/] " + "[" + ex.Message + "]" + ex.StackTrace);
            }
            var respJson = JsonSerializer.Serialize(usuario);
            log.EntradaSalida("[DELETE: api/Usuarios/] " + respJson, "DNU.Usuarios", false);
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
