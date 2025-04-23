using DALCentralAplicaciones.BaseDatos;
using DALCentralAplicaciones.Entidades;
using DNU.Usuarios.Common.Utilerias;
using DNU.Usuarios.DataContract.BD;
using Log_PCI.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNU.Usuarios.DataContract.Entities
{
    class ValoresInicial
    {

        /// <summary>
        /// Required designer variable.
        /// </summary>

        /// <summary>
        /// Id de la aplicación
        /// </summary>
        public static Guid idAplicacion;

        /// <summary>
        /// Cadena de conexión a la base de datos
        /// </summary>
        public static string cadenaConexionBaseDatos;

       // public static System.Collections.Generic.Dictionary<String, List<String>> PaginasRol;

        public static Dictionary<Guid, Dictionary<String, Propiedad>> ConfigApps;


        public static void InicializarContexto(Logueo log)
        {
          

            try
            {
                LogHeader logHF = new LogHeader();
                logHF.IP_Address = log.IpAddr;
                logHF.Application_ID = Guid.Parse("36EF1469-05AF-425A-84B7-131BECABB6C5");
                logHF.User = log.User;
                logHF.Trace_ID = log.IdLog;

                ValoresInicial.ConfigApps = DAOAplicacion.GetConfiguraciones(logHF);
            }
            catch (Exception err)
            {
                log.Error(err.Message);
            }
        }



    }

}
