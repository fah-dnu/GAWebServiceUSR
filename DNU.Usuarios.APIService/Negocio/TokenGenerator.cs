using DNU.Usuarios.APIService.Negocio.Authorization;
using DNU.Usuarios.Common;
using DNU.Usuarios.Common.Extensiones;
using DNU.Usuarios.Common.Utilerias;
using DNU.Usuarios.DataContract.BD;
using DNU.Usuarios.DataContract.Response;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace DNU.Usuarios.APIService.Negocio
{
    public class TokenGenerator
    {
        public static string GenerateTokenJwt(string username,Logueo log, string userId,string appID=null)
        {
            // appsetting for Token JWT
            var secretKey = AzureExtensions.ObtenerValorSecretoAzure(ConfigurationManager.AppSettings["JWT_SECRET_KEY"].ToString(), "", "",log);// ConfigurationManager.AppSettings["JWT_SECRET_KEY"];
            var audienceToken = ConfigurationManager.AppSettings["JWT_AUDIENCE_TOKEN"];
            var issuerToken = ConfigurationManager.AppSettings["JWT_ISSUER_TOKEN"];
            var expireTime = ConfigurationManager.AppSettings["JWT_EXPIRE_MINUTES"];

            var securityKey = new SymmetricSecurityKey(System.Text.Encoding.Default.GetBytes(secretKey));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            // create a claimsIdentity
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(new[] { 
                new Claim(ClaimTypes.Name, username) ,
                new Claim(ClaimTypes.NameIdentifier, userId)
            });
            var caducaToken = ConfigurationManager.AppSettings["CaducaToken"];
            // create token to the user
            var tokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwtSecurityToken;
            if (caducaToken == "1")
            {
                jwtSecurityToken = tokenHandler.CreateJwtSecurityToken(
                    audience: audienceToken,
                    issuer: issuerToken,
                    subject: claimsIdentity,
                    notBefore: DateTime.UtcNow,
                    expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(expireTime)),
                    signingCredentials: signingCredentials);
            }
            else
            {
                jwtSecurityToken = tokenHandler.CreateJwtSecurityToken(
                    audience: audienceToken,
                    issuer: issuerToken,
                    subject: claimsIdentity,
                    notBefore: DateTime.UtcNow,
                    expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(525600)),
                    signingCredentials: signingCredentials);
            }

            var jwtTokenString = tokenHandler.WriteToken(jwtSecurityToken);

            //ResponseGral respInsertToken = InsertToken(jwtTokenString, username, log);
            ResponseGral respInsertToken = InsertToken(jwtTokenString, username,appID,log);

            if (!respInsertToken.CodigoRespuesta.Equals("99"))
            {
                log.EntradaSalida("[POST: api/LogIn/] " + "[Borrando Token anterior]", "DBU.Usuarios", true);
                TableToken.deleteToken(respInsertToken.Mensaje,log);
                log.EntradaSalida("[POST: api/LogIn/] " + "[Token anterior eliminado]", "DBU.Usuarios", false);

                log.EntradaSalida("[POST: api/LogIn/] " + "[Agregando nuevo token]", "DBU.Usuarios", true);
                TableToken.setToken(jwtTokenString,log);
                log.EntradaSalida("[POST: api/LogIn/] " + "[Nuevo token agregado]", "DBU.Usuarios", false);

            }
            else
            {
                log.Error("[DNU.Usuarios] [GenerateTokenJwt] [" + respInsertToken + "]");
            }

            return jwtTokenString;
        }


        public static ResponseGral InsertToken(string pToken, string pUser, Logueo log)
        {
            Hashtable ht = new Hashtable();

            ht.Add("@token", pToken);
            ht.Add("@usuario", pUser);

            var updateToken = new ResponseGral()
            {
                CodigoRespuesta = 99,
                Mensaje = "Error inesperado. Consulte al administrador"
            };

            try
            {
                updateToken = SPs.executeSP("wsU_UpdateToken_updateLogIn", ht, log);
            }
            catch (Exception ex)
            {
                log.Error("[DNU.Usuarios] [InsertToken] " + ex.Message);
            }

          //  log.Evento("[DNU.Usuarios] [InsertToken] " + JsonSerializer.Serialize(updateToken));

            return updateToken;
        }

        public static ResponseGral InsertToken(string pToken, string pUser,string appID ,Logueo log)
        {
            Hashtable ht = new Hashtable();

            ht.Add("@token", pToken);
            ht.Add("@usuario", pUser);

            string versiondock = ConfigurationManager.AppSettings["versionWsDock"]?.ToString();
            if ((!string.IsNullOrEmpty(versiondock)) && versiondock == "1")
            {
                 ht.Add("@appID", appID);
            }

            var updateToken = new ResponseGral()
            {
                CodigoRespuesta = 99,
                Mensaje = "Error inesperado. Consulte al administrador"
            };

            try
            {
                updateToken = SPs.executeSP("wsU_UpdateToken_updateLogIn", ht, log);
            }
            catch (Exception ex)
            {
                log.Error("[DNU.Usuarios] [InsertToken] " + ex.Message);
            }

            //  log.Evento("[DNU.Usuarios] [InsertToken] " + JsonSerializer.Serialize(updateToken));

            return updateToken;
        }
    }
}