using DNU.ConexionLogSockets.LNConexion;
using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace DNU.Usuarios.Common.Utilerias
{
    public class Logueo
    {
        private readonly log4net.ILog _loggerError = log4net.LogManager.GetLogger(ConfigurationManager.AppSettings["LogError"]);
        private readonly log4net.ILog _loggerInfo = log4net.LogManager.GetLogger(ConfigurationManager.AppSettings["LogInfo"]);
      //  private readonly log4net.ILog _loggerDebug = log4net.LogManager.GetLogger(ConfigurationManager.AppSettings["LogDebug"]);
        private string m_IpAddr;
        private Guid m_IdLog;
        private string m_User;
        Thread hilo;

        public Logueo(string User)
        {
            XmlConfigurator.Configure();
            IdLog = Guid.NewGuid();
            this.User = User;
            IpAddr = obtenerIP();
            ThreadContext.Properties["ts"] = DateTime.Now.ToString("yyyyMMddHHmmss");
        //    hilo = new Thread();
        }

        public string IpAddr { get => m_IpAddr; set => m_IpAddr = value; }
        public Guid IdLog { get => m_IdLog; set => m_IdLog = value; }
        public string User { get => m_User; set => m_User = value; }

        public void Error(String Error, bool esError = true)
        {
            ThreadContext.Properties["IpAddress"] = IpAddr;
            ThreadContext.Properties["AppId"] = "36EF1469-05AF-425A-84B7-131BECABB6C5";
            ThreadContext.Properties["User"] = User;
            ThreadContext.Properties["TraceId"] = IdLog;

            if (esError)
                _loggerError.Error(Error);
            else
                _loggerError.Warn(Error);

        }

        public void EntradaSalida(string message, string userNot, Boolean esEntrada)
        {
            ThreadContext.Properties["IpAddress"] = IpAddr;
            ThreadContext.Properties["AppId"] = "36EF1469-05AF-425A-84B7-131BECABB6C5";
            ThreadContext.Properties["User"] = User;
            ThreadContext.Properties["TraceId"] = IdLog;

            String leyenda = esEntrada ? "ENTRADA <<" : "SALIDA >>";
            _loggerInfo.Info("[" + leyenda + message + "]");
        }


        public void Evento(String Evento)
        {
            ThreadContext.Properties["IpAddress"] = IpAddr;
            ThreadContext.Properties["AppId"] = "36EF1469-05AF-425A-84B7-131BECABB6C5";
            ThreadContext.Properties["User"] = User;
            ThreadContext.Properties["TraceId"] = IdLog;

            try
            {
                hilo = new Thread(new ThreadStart(() => LNServerSocket.envioLogAsync(Evento, IpAddr, IdLog.ToString(), "36EF1469-05AF-425A-84B7-131BECABB6C5", User)));
                hilo.Start();
            }
            catch (Exception ex) {
                Error(ex.Message, true);
            }
            //  Task.Run(() => { pruebaTask2(); });



            //_loggerDebug.Debug(Evento);
            //var log = LNServerSocket.envioLogAsync(Evento, obtenerIP(), IdLog.ToString(), "36EF1469-05AF-425A-84B7-131BECABB6C5", User);
        }

        static string obtenerIP()
        {
            try
            {
                if (HttpContext.Current.Request is null)
                {
                    string h = "";
                }
                string userIP = HttpContext.Current.Request.UserHostAddress;

                return userIP;
            }
            catch (Exception ex)
            {
                //       Logueo.Error("[Logueo] [obtenerIP] "+ex.Message,"","");
                return "0.0.0.0";
            }
        }
    }
}
