using System;
using System.Configuration;
using System.Web.Http;

using Microsoft.Owin;
using Owin;
using Dnu.AutorizadorParabiliaAzure.Models;
using Dnu.AutorizadorParabiliaAzure.Services;

[assembly: OwinStartup(typeof(DNU.Usuarios.APIService.Startup))]

namespace DNU.Usuarios.APIService
{
    public partial class Startup
    {//
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            string appAKV = ConfigurationManager.AppSettings["applicationId"].ToString();
            string clave = ConfigurationManager.AppSettings["clientKey"].ToString();

            responseAzure respuesta = KeyVaultProvider.RegistrarProvedorCEK(appAKV, clave);

            //responseAzure respuestaObtenerCadena = KeyVaultProvider.ObtenerCadenasDeConexionAzure(appAKV, clave, "DNU-SB-WSU-ADMIN-R");
            //string cadena = "";
            //if (respuestaObtenerCadena.codRespuesta == "0000")
            //{
            //    cadena = respuestaObtenerCadena.valorAzure;
            //}
        }
    }
}
