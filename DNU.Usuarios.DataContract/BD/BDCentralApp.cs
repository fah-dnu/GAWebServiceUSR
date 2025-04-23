using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
using DNU.Usuarios.Common.Utilerias;

using Newtonsoft.Json;
using Dnu.AutorizadorParabiliaAzure.Models;
using Dnu.AutorizadorParabiliaAzure.Services;

namespace DNU.Usuarios.DataContract.BD
{
    public static class DBCentralApp
    {
       // static SqlConnection _BDLectura = new SqlConnection(ConfigurationManager.ConnectionStrings["CajeroConsulta"].ToString());
       // static SqlConnection _BDEscritura = new SqlConnection(ConfigurationManager.ConnectionStrings["CajeroEscritura"].ToString());

        public static SqlConnection BDLectura
        {
            get
            {
                return new SqlConnection(strBDLectura);
            }
        }

        public static SqlConnection BDEscritura
        {
            get
            {
                return new SqlConnection(strBDEscritura);
            }
        }

        public static String strBDLectura
        {
            get
            {
                return obtenerCadenaAzure(ConfigurationManager.ConnectionStrings["ADMIN_READ"].ToString());
            }
        }

        public static String strBDAULectura
        {
            get
            {
                return obtenerCadenaAzure(ConfigurationManager.ConnectionStrings["BDAutorizadorRead"].ToString());
            }
        }

        public static String strBDEscritura
        {
            get
            {
                return obtenerCadenaAzure(ConfigurationManager.ConnectionStrings["ADMIN_WRITE"].ToString());
            }
        }


        public static string obtenerCadenaAzure(string cadena)
        {
            string app = ConfigurationManager.AppSettings["applicationId"].ToString();
            string clave = ConfigurationManager.AppSettings["clientKey"].ToString();
            responseAzure respuestaObtenerCadena = KeyVaultProvider.ObtenerCadenasDeConexionAzure(app, clave, cadena);
            if (respuestaObtenerCadena.codRespuesta == "0000")
            {
                cadena = respuestaObtenerCadena.valorAzure;
            }
            else
            {
                Logueo log = new Logueo("");
                log.Error("[Azure " + JsonConvert.SerializeObject(respuestaObtenerCadena) + "]");
            }

            return cadena;

        }

    }
}
