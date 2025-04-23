//#define firma 
using DNU.Monitoreo.Entidades;
using DNU.Usuarios.APIService.App_Start;
using DNU.Usuarios.APIService.Negocio;
using DNU.Usuarios.Common.Utilerias;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace DNU.Usuarios.APIService
{
    public class WebApiApplication : System.Web.HttpApplication
    {

#if !firma
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);


            string app = ConfigurationManager.AppSettings["applicationId"].ToString();
            string clave = ConfigurationManager.AppSettings["clientKey"].ToString();
            Environment.SetEnvironmentVariable("akvID", app);
            Environment.SetEnvironmentVariable("akvKey", clave);

        }
#else
        protected void Application_Start()
        {



            bool EstaHabilitadaMonitoreoTabla = false;
            bool EstaHabilitadaMonitoreoArchivo = false;
            //Libreria Monitoreo
            LNLibreriaMonitoreo _libreriaMonitoreo = new LNLibreriaMonitoreo();
            LNLibreriaMonitoreo.CargarValoresFijos();
            Logueo log = new Logueo("");
            //Validar Firma de Parametros
            RespuestaGenerica RespuestaEvaluarFirmaParametros = _libreriaMonitoreo.EvaluarFirmaParametros();
            if (RespuestaEvaluarFirmaParametros.Codigo != 0)
            {
                log.Error("No se pudo evaluar la firma del archivo de configuraciones, favor de volver a firmar el archivo con el configurador");
                log.Error(RespuestaEvaluarFirmaParametros.Respuesta.ToString());
                //log.Evento("No se pudo Evaluar la firma del archivo de configuraciones, favor de volver a firmar el archivo con el configurador");
                //log.Evento(RespuestaEvaluarFirmaParametros.Respuesta.ToString());
                log.EntradaSalida("No se pudo Evaluar la firma del archivo de configuraciones, favor de volver a firmar el archivo con el configurador", "", true);
                log.EntradaSalida(RespuestaEvaluarFirmaParametros.Respuesta.ToString(), "", true);
                GlobalConfiguration.Configure(MonitoreoConfig.Register);
                return;
            }
            else
            {
                log.Evento(RespuestaEvaluarFirmaParametros.Respuesta.ToString());
            }

            ////////////Monitoreo Tablas            


            RespuestaGenerica RespuestaVerificarFirmasAplicativos = _libreriaMonitoreo.VerificarFirmasAplicativos();
            if (RespuestaVerificarFirmasAplicativos.Codigo != 0)
            {
                log.Error("No se pudo Iniciar la libreria Monitoreo firmas");
                log.Error(RespuestaVerificarFirmasAplicativos.Respuesta.ToString());
                //log.Evento("No se pudo Iniciar la libreria Monitoreo firmas");
                //log.Evento(RespuestaVerificarFirmasAplicativos.Respuesta.ToString());
                GlobalConfiguration.Configure(MonitoreoConfig.Register);
                return;
            }
            else
            {
                log.Evento(RespuestaVerificarFirmasAplicativos.Respuesta.ToString());
            }

            RespuestaGenerica RespuestaEncenderMonitoreoTabla = _libreriaMonitoreo.EncenderMonitoreoTabla();
            if (RespuestaEncenderMonitoreoTabla.Codigo != 0)
            {
                log.Error("No se pudo Iniciar la libreria Monitoreo Tabla");
                log.Error(RespuestaEncenderMonitoreoTabla.Respuesta.ToString());
                //log.Evento("No se pudo Iniciar la libreria Monitoreo Tabla");
                //log.Evento(RespuestaEncenderMonitoreoTabla.Respuesta.ToString());
                //LibreriaMonitoreo.EstaHabilitadaMonitoreoTabla = false;
                EstaHabilitadaMonitoreoTabla = false;
            }
            else
            {
                log.Evento(RespuestaEncenderMonitoreoTabla.Respuesta.ToString());
                //LibreriaMonitoreo.EstaHabilitadaMonitoreoTabla = true;
                EstaHabilitadaMonitoreoTabla = true;
            }
            //////////////Monitoreo Archivos            
            RespuestaGenerica RespuestaEncenderMonitoreoArchivos = _libreriaMonitoreo.EncenderMonitoreoArchivo();
            if (RespuestaEncenderMonitoreoArchivos.Codigo != 0)
            {
                log.Error("No se pudo Iniciar la libreria Monitoreo Archivo");
                log.Error(RespuestaEncenderMonitoreoArchivos.Respuesta.ToString());
                //log.Evento("No se pudo Iniciar la libreria Monitoreo Archivo");
                //log.Evento(RespuestaEncenderMonitoreoArchivos.Respuesta.ToString());
                //LibreriaMonitoreo.EstaHabilitadaMonitoreoArchivo = false;
                EstaHabilitadaMonitoreoArchivo = false;
            }
            else
            {
                log.Evento(RespuestaEncenderMonitoreoArchivos.Respuesta.ToString());
                //LibreriaMonitoreo.EstaHabilitadaMonitoreoArchivo = true;
                EstaHabilitadaMonitoreoArchivo = true;
            }
            ///////////////////////
            ///

            if (EstaHabilitadaMonitoreoArchivo)
            {
                AreaRegistration.RegisterAllAreas();
                GlobalConfiguration.Configure(WebApiConfig.Register);
                FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
                //

               // AreaRegistration.RegisterAllAreas();
               // GlobalConfiguration.Configure(WebApiConfig.Register);
               // FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
                RouteConfig.RegisterRoutes(RouteTable.Routes);
                BundleConfig.RegisterBundles(BundleTable.Bundles);
            }
            else
            {
                GlobalConfiguration.Configure(MonitoreoConfig.Register);
                //
                //AreaRegistration.RegisterAllAreas();
                //GlobalConfiguration.Configure(WebApiConfig.Register);
                //FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
                //RouteConfig.RegisterRoutes(RouteTable.Routes);
                //BundleConfig.RegisterBundles(BundleTable.Bundles);
            }
            string app = ConfigurationManager.AppSettings["applicationId"].ToString();
            string clave = ConfigurationManager.AppSettings["clientKey"].ToString();
            Environment.SetEnvironmentVariable("akvID", app);
            Environment.SetEnvironmentVariable("akvKey", clave);
      
        }

        //comentar para debuguear todo el Application_End
        //protected void Application_End()
        //{
        //    Logueo log = new Logueo("");
        //    LNLibreriaMonitoreo _libreriaMonitoreo = new LNLibreriaMonitoreo();
        //    RespuestaGenerica RespuestaInsertarAlertamientoReinicio = _libreriaMonitoreo.InsertarAlertamientoReinicio();
        //    if (RespuestaInsertarAlertamientoReinicio.Codigo != 0)
        //    {
        //        log.Error("No se pudo InsertarAlertamientoReinicio");
        //        log.Error(RespuestaInsertarAlertamientoReinicio.Respuesta.ToString());
        //        log.Evento("No se pudo InsertarAlertamientoReinicio");
        //        log.Evento(RespuestaInsertarAlertamientoReinicio.Respuesta.ToString());
        //    }
        //    else
        //    {
        //        log.Evento("Se inserto alerta por que se apago el servicio");
        //        log.Evento(RespuestaInsertarAlertamientoReinicio.Respuesta.ToString());
        //    }


        //}
#endif
    }
}
