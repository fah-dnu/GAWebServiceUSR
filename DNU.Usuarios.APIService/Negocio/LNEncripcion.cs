using DNU.Usuarios.Common.Utilerias;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace DALCentralAplicaciones.LogicaNegocio
{
    public class LNEncripcion
    {
        public static RSACryptoServiceProvider rsa;
        const string CONTAINER_NAME = "ContenedorRS55A";
        private Logueo log;

        public LNEncripcion(Logueo log)
        {
            this.log = log;
            CspParameters cspParams;
            cspParams = new CspParameters(1); 
            cspParams.Flags = CspProviderFlags.UseMachineKeyStore;
            cspParams.KeyContainerName = CONTAINER_NAME;

            // Instanciar el algoritmo de cifrado RSA

            rsa = new RSACryptoServiceProvider(cspParams);

            cargarLlavePrivada();
        }

        private void cargarLlavePrivada()
        {
          //  log.Evento("Path llave privada: " + HttpContext.Current.Server.MapPath("~") + "\\Keys\\llave_privada.xml");
            StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~") + "\\Keys\\llave_privada.xml");
            //StreamReader reader = new StreamReader(@"C:\inetpub\wwwroot\DNU.Usuarios.Inco\Keys\llave_privada.xml");
            string publicPrivateKeyXML = reader.ReadToEnd();
            rsa.FromXmlString(publicPrivateKeyXML);
            reader.Close();
        }

        public string descifrar(string textoCifrado)
        {
            byte[] textoCifradoBytes = Convert.FromBase64String(textoCifrado);
            byte[] textoPlanoBytes = rsa.Decrypt(textoCifradoBytes, false);

            return Encoding.UTF8.GetString(textoPlanoBytes);
        }
    }
}
