using DNU.Usuarios.APIService.Negocio;
using DNU.Usuarios.Common;
using DNU.Usuarios.Common.Utilerias;
using DNU.Usuarios.DataContract.Request;
using DNU.Usuarios.DataContract.Response;
using Microsoft.Web.Http;
using System;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace DNU.Usuarios.APIService.Controllers
{
    //[Authorize]
    [ApiVersion("1.0")]
    [ApiVersion("1.1")]
    [RoutePrefix("api/SMS")]
    public class SMSController : ApiController
    {
        private Logueo log;

        public SMSController()
        {
            string nombre = "";
            try
            {
                nombre = RequestContext.Principal.Identity.Name;
            }
            catch (Exception ex) {

            }
            log = new Logueo(nombre);
        }

        public IHttpActionResult Put([FromBody]RequerimientoSMSPut requerimiento)
        {
           // log.User = requerimiento.NombreUsuario;

            String logSinTarjeta = JsonSerializer.Serialize(requerimiento);
            RequerimientoSMSPut requerimientoLog = JsonSerializer.Deserialize<RequerimientoSMSPut>(logSinTarjeta);
            requerimientoLog.Tarjeta = "";

            log.EntradaSalida("[PUT: api/SMS/] " + JsonSerializer.Serialize(requerimientoLog),"DNU.Usuarios",true);
          
            var smsToken = new ResponseTokenSMS()
            {
                CodigoRespuesta = 99,
                Mensaje = "Error inesperado. Consulte al administrador",
                Token = null
            };

            if (requerimiento is null)
            {
                smsToken.CodigoRespuesta = 56;
                smsToken.Mensaje = "Solicitud incorrecta, revise el valor de los parámetros enviados";

                var respJsonE = JsonSerializer.Serialize(smsToken);
                log.Error("[PUT: api/SMS/] " + respJsonE);
                IHttpActionResult responseE;
                HttpResponseMessage msgResponseE = new HttpResponseMessage()
                {
                    Content = new StringContent(respJsonE, Encoding.UTF8, "application/json")
                };
                responseE = ResponseMessage(msgResponseE);
                log.EntradaSalida("[PUT: api/SMS/] " + respJsonE, "DNU.Usuarios", false);
                return responseE;
            }

            try
            {
                smsToken = LNSMS.updateSMS(requerimiento, log);
            }
            catch (Exception ex)
            {
                log.Error("[PUT: api/SMS/] " + "[" + ex.Message + "]" + ex.StackTrace);
            }



            var respJson = JsonSerializer.Serialize(smsToken);
            log.EntradaSalida("[PUT: api/SMS/] " + respJson, "DNU.Usuarios", false);
            IHttpActionResult response;
            HttpResponseMessage msgResponse = new HttpResponseMessage()
            {
                Content = new StringContent(respJson, Encoding.UTF8, "application/json")
            };
            response = ResponseMessage(msgResponse);
            return response;
        }

        
        public IHttpActionResult Post([FromBody]RequerimientoSMSPost requerimiento)
        {
            //log.User = requerimiento.NombreUsuario;
            String logSinTarjeta = JsonSerializer.Serialize(requerimiento);
            RequerimientoSMSPut requerimientoLog = JsonSerializer.Deserialize<RequerimientoSMSPut>(logSinTarjeta);
            requerimientoLog.Tarjeta = "";
            log.EntradaSalida("[POST: api/SMS/] " + JsonSerializer.Serialize(requerimientoLog),"DNU.Usuarios",true);

            var sms = new ResponseGral()
            {
                CodigoRespuesta = 99,
                Mensaje = "Error inesperado. Consulte al administrador"
            };

            if (requerimiento is null)
            {
                sms.CodigoRespuesta = 56;
                sms.Mensaje = "Solicitud incorrecta, revise el valor de los parámetros enviados";

                var respJsonE = JsonSerializer.Serialize(sms);
                log.Error("[POST: api/SMS/] " + respJsonE);
                IHttpActionResult responseE;
                HttpResponseMessage msgResponseE = new HttpResponseMessage()
                {
                    Content = new StringContent(respJsonE, Encoding.UTF8, "application/json")
                };
                responseE = ResponseMessage(msgResponseE);
                log.EntradaSalida("[POST: api/SMS/] " + respJsonE, "DNU.Usuarios", false);
                return responseE;
            }

            try
            {
                sms = LNSMS.insertSMS(requerimiento, log);
            }
            catch (Exception ex)
            {
                log.Error("[POST: api/SMS/] " + "[" + ex.Message + "]" + ex.StackTrace);
            }
            var respJson = JsonSerializer.Serialize(sms);
            log.EntradaSalida("[POST: api/SMS/] " + respJson, "DNU.Usuarios", false);
            IHttpActionResult response;
            HttpResponseMessage msgResponse = new HttpResponseMessage()
            {
                Content = new StringContent(respJson, Encoding.UTF8, "application/json")
            };
            response = ResponseMessage(msgResponse);
            return response;
        }


        /// <summary>
        /// genera codigo que envian externamente y solo es para enviar el sms y que sera validado por otro lado
        /// </summary>
        /// <param name="requerimiento"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("")]
        [MapToApiVersion("1.1")]
        public IHttpActionResult PutV1_1([FromBody] RequerimientoSMSPutV2 requerimiento)
        {

            log.EntradaSalida("[PUT: api/SMSV2/] " + JsonSerializer.Serialize(requerimiento), "DNU.Usuarios", true);

            var smsToken = new ResponseTokenSMS()
            {
                CodigoRespuesta = 99,
                Mensaje = "Error inesperado. Consulte al administrador",
                Token = null
            };

            if (requerimiento is null || String.IsNullOrEmpty(requerimiento.Telefono) || String.IsNullOrEmpty(requerimiento.Token))
            {
                smsToken.CodigoRespuesta = 56;
                smsToken.Mensaje = "Solicitud incorrecta, revise el valor de los parámetros enviados";

                var respJsonE = JsonSerializer.Serialize(smsToken);
                log.Error("[PUT: api/SMSV2/] " + respJsonE);
                IHttpActionResult responseE;

                HttpResponseMessage msgResponseE = new HttpResponseMessage()
                {
                    Content = new StringContent(respJsonE, Encoding.UTF8, "application/json")
                };

                responseE = ResponseMessage(msgResponseE);
                log.EntradaSalida("[PUT: api/SMSV2/] " + respJsonE, "DNU.Usuarios", false);

                return responseE;
            }

            try
            {
                smsToken = LNSMS.updateSMSV2(requerimiento, log);
            }
            catch (Exception ex)
            {
                log.Error("[PUT: api/SMSV2/] " + "[" + ex.Message + "]" + ex.StackTrace);
            }

            var respJson = JsonSerializer.Serialize(smsToken);
            log.EntradaSalida("[PUT: api/SMSV2/] " + respJson, "DNU.Usuarios", false);
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
