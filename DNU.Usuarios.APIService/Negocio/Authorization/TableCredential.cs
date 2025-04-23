using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DNU.Usuarios.APIService.Negocio.Authorization
{
    public class TableCredential
    {
        private static readonly TableCredential _instancia = new TableCredential();
        private static List<string> credentials = new List<string>();

        private TableCredential() { }

        public static TableCredential GetTableCredential()
        {
            return _instancia;
        }

        public static bool getCredential(string credential)
        {
            bool existe = false;

            try
            {
                var output = (from t in credentials
                              where t == credential
                              select t).Single();

                existe = true;
            }
            catch (Exception ex)
            {
                existe = false;
            }

            return existe;
        }

        public static void setCredential(string credential)
        {
            credentials.Add(credential);
        }

        public static void deleteCredential(string credential)
        {
            credentials.Remove(credential);
        }
    }
}