using DNU.Usuarios.APIService.Handlers;
using DNU.Usuarios.APIService.Negocio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Dispatcher;

namespace DNU.Usuarios.APIService.App_Start
{
    public static class MonitoreoConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            //config.SuppressDefaultHostAuthentication();
            //config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));
            //config.Filters.Add(new Dnu.Autorizador.PlugInManager.Filters.ExceptionFilter());
            //// Web API routes
            //config.MapHttpAttributeRoutes();

            //var container = new UnityContainer();
            //container.RegisterType<IPluginLoadingManager, PluginLoadingManager>(new HierarchicalLifetimeManager());
            //container.RegisterType<IPluginLoaderConfiguration, PluginLoaderConfiguration>(new HierarchicalLifetimeManager());
            //container.RegisterType<IValidacionesCampos, LNValidacionesCampos>(new HierarchicalLifetimeManager());



            //config.DependencyResolver = new UnityResolver(container);

            //config.Routes.MapHttpRoute(
            //    name: "SwaggerURL",
            //    routeTemplate: "swagger/ui/index",
            //    defaults: new { id = RouteParameter.Optional },
            //    constraints: null,
            //    handler: null

            //);

            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional },
            //    constraints: null,
            //    handler: HttpClientFactory.CreatePipeline(
            //            new HttpControllerDispatcher(config),
            //            new DelegatingHandler[] { new TokenValidationHandler() }
            //        )
            //);


            config.MessageHandlers.Add(new LibreriaMonitoreo());


        }
    }
}