using DNU.Usuarios.Common.Utilerias;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DNU.Usuarios.APIService.Negocio.Authorization
{
    public class TableToken
    {
        private static readonly TableToken _instancia = new TableToken();
        private static List<string> tokens = new List<string>();

        private TableToken() { }

        public static TableToken GetTableToken()
        {
            return _instancia;
        }

        public static bool getToken(string token, Logueo log)
        {
            bool existe = false;

            try
            {
                var output = (from t in tokens
                              where t == token
                              select t).Single();

                existe = true;
            }
            catch (Exception ex)
            {
                log.Error("[DNU.Usuarios] [getToken] " + ex.Message);
                existe = false;
            }

            return existe;
        }

        public static void setToken(string token, Logueo log)
        {
            try
            {
                tokens.Add(token);
            }
            catch (Exception ex) {
                log.Error("[DNU.Usuarios] [setToken] " + ex.Message);
                tokens = new List<string>();
                tokens.Clear();
               
            }

}

        public static void deleteToken(string token, Logueo log)
        {
            try
            {
                tokens.Remove(token);
            }
            catch (Exception ex) {
                log.Error("[DNU.Usuarios] [deleteToken] " + ex.Message);
                tokens = new List<string>();
                tokens.Clear();
                
            }
        }
    }
}