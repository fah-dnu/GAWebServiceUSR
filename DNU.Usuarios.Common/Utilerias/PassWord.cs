using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DNU.Usuarios.Common.Utilerias
{
    [Obsolete("Not used any more, use Autenticacion instead", true)]
    public class PassWord_Deprecated
    {
        /// <summary>
        /// Realiza la comprobación de las claves hash del password
        /// </summary>
        /// <param name="enterPassword">Password tecleado</param>
        /// <param name="password">Clave contra la que se compara el password</param>
        /// <param name="salt">Valor del Password Salt</param>
        /// <param name="iterations">Número de iteraciones</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        /// <returns>TRUE si los password coinciden</returns>
        public static bool PasswordOK(string enterPassword, string password, string salt, int iterations,
            Logueo log, string user = "")
        {
            try
            {
                byte[] byteSalt = Convert.FromBase64String(salt);
                var ip = GetClientIp();
                var pbkdf2_EP = new Rfc2898DeriveBytes(enterPassword + user + ip, byteSalt, iterations);
                byte[] byte_EP = pbkdf2_EP.GetBytes(32);

                if (Convert.ToBase64String(byte_EP) == password)
                    return true;

                return false;
            }
            catch (Exception ex)
            {
                log.Error("[PasswordOK()] [" + ex.Message + "] [" + ex.StackTrace + "]");

                return false;
            }
        }


        /// <summary>
        /// Crea las claves PBKDF2 para el password del usuario
        /// </summary>
        /// <param name="password">Password tecleado</param>
        /// <param name="pwdKey">Referencia a la variable donde se establece la llave del password</param>
        /// <param name="pwdSalt">Referencia a la variable donde se establece el valor del Password Salt</param>
        /// <param name="iter">Referencia a la variable donde se establece el número de iteraciones</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void CreaPasswordUsuario(string password, ref string pwdKey, ref string pwdSalt, ref int iter,
            Logueo log, string user ="")
        {
            try
            {
                byte[] salt = GeneraSalt(log);
                int it = GeneraIteraciones(log);
                var ip = GetClientIp();
                var pbkdf2 = new Rfc2898DeriveBytes(password + user + ip, salt, it);
                byte[] key = pbkdf2.GetBytes(32);

                pwdKey = Convert.ToBase64String(key);
                pwdSalt = Convert.ToBase64String(salt);
                iter = it;
            }
            catch (Exception ex)
            {
                log.Error("[8011] [Falla al generar el valor hash] [" + ex.Message + "]");
            }
        }

        /// <summary>
        /// Genera un valor SALT criptográfico necesario para el PBKDF2
        /// </summary>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Valor SALT</returns>
        protected static byte[] GeneraSalt(Logueo log)
        {
            try
            {
                var csprng = new RNGCryptoServiceProvider();
                var saltBytes = new byte[16];

                csprng.GetBytes(saltBytes);

                return saltBytes;
            }
            catch (Exception ex)
            {
                log.Error("[GeneraSalt] [" + ex.Message + "]");
                throw ex;
            }
        }

        /// <summary>
        /// Genera un número aleatorio de iteraciones para el PBKDF2
        /// </summary>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Número de iteraciones</returns>
        protected static int GeneraIteraciones(Logueo log)
        {
            try
            {
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                byte[] buffer = new byte[5];

                rng.GetBytes(buffer);
                int result = BitConverter.ToInt32(buffer, 0);

                return new Random(result).Next(1000, 1010);
            }
            catch (Exception ex)
            {
                log.Error("[GeneraIteraciones] [" + ex.Message + "]");
                throw ex;
            }
        }

        private static string GetClientIp()
        {
            var responseIP = HttpContext.Current.Request.UserHostAddress;
            if (responseIP == "::1") return "127.0.0.1";
            return responseIP;
        }
    }
}
