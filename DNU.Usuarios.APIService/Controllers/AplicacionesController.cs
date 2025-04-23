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
    [RoutePrefix("api/Aplicaciones")]
    public class AplicacionesController : ApiController
    {
        private Logueo log;

        public AplicacionesController()
        {
            log = new Logueo("");
        }

        // GET: api/Aplicaciones/5
        public IHttpActionResult Get(string IDSolicitud)
        {
            log.EntradaSalida("[GET: api/Aplicaciones/] " + JsonSerializer.Serialize(IDSolicitud), "DNU.Usuarios", true);

            var aplicacion = new ResponseAplicacionesGet()
            {
                CodigoRespuesta = 99,
                Mensaje = "Error inesperado. Consulte al administrador"
            };

            try
            {
                aplicacion = LNAplicaciones.getAplicaciones(IDSolicitud, log);
            }
            catch (Exception ex)
            {
                log.Error("[GET: api/Aplicaciones/] " + "[" + ex.Message + "]" + ex.StackTrace);
            }
            var respJson = JsonSerializer.Serialize(aplicacion);
            log.EntradaSalida("[GET: api/Aplicaciones/] " + respJson, "DNU.Usuarios", false);
            IHttpActionResult response;
            HttpResponseMessage msgResponse = new HttpResponseMessage()
            {
                Content = new StringContent(respJson, Encoding.UTF8, "application/json")
            };
            response = ResponseMessage(msgResponse);

            return response;
        }

        // PUT: api/Aplicaciones/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Aplicaciones/5
        public void Delete(int id)
        {
        }
    }
}
