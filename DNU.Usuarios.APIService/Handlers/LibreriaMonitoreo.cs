using DNU.Usuarios.Common.Utilerias;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace DNU.Usuarios.APIService.Handlers
{
    public class LibreriaMonitoreo : DelegatingHandler
    {
        public static bool EstaHabilitadaMonitoreoTabla;
        public static bool EstaHabilitadaMonitoreoArchivo;
        public static bool ConsultarAlertamientosAbiertosXClave;
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {

            //if (EstaHabilitadaMonitoreoTabla == false)
            //{
            //    HttpStatusCode statusCode;
            //    statusCode = HttpStatusCode.BadRequest;
            //    var response = new HttpResponseMessage(statusCode)
            //    {
            //        Content = new StringContent("La libreria Monitoreo Tabla no se esta ejecutando")
            //    };

            //    var respTask = new TaskCompletionSource<HttpResponseMessage>();
            //    respTask.SetResult(response);
            //    return respTask.Task;
            //}
            //if (EstaHabilitadaMonitoreoArchivo == false)
            //{
            //    HttpStatusCode statusCode;
            //    statusCode = HttpStatusCode.BadRequest;
            //    var response = new HttpResponseMessage(statusCode)
            //    {
            //        Content = new StringContent("La libreria Monitoreo Archivo no se esta ejecutando")
            //    };

            //    var respTask = new TaskCompletionSource<HttpResponseMessage>();
            //    respTask.SetResult(response);
            //    return respTask.Task;
            //}

            //return base.SendAsync(request, cancellationToken);
            HttpStatusCode statusCode;
            statusCode = HttpStatusCode.Conflict;
            var response = new HttpResponseMessage(statusCode)
            {
                Content = new StringContent("Debe Iniciar la libreria Monitoreo")
            };
            new Logueo("").Error("[LibreriaMonitoreo] [SendAsync] No se pudo Iniciar la libreria Monitoreo");
            var respTask = new TaskCompletionSource<HttpResponseMessage>();
            respTask.SetResult(response);

            return respTask.Task;
        }
    }
}