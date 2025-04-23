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
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace DNU.Usuarios.APIService.Negocio
{
    internal class TokenValidationHandler : DelegatingHandler
    {
        private static bool TryRetrieveToken(HttpRequestMessage request, out string token)
        {
            token = null;
            IEnumerable<string> authzHeaders;
            if (!request.Headers.TryGetValues("Authorization", out authzHeaders) || authzHeaders.Count() > 1)
            {
                return false;
            }
            var bearerToken = authzHeaders.ElementAt(0);
            token = bearerToken.StartsWith("Bearer ") ? bearerToken.Substring(7) : bearerToken;
            return true;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Logueo log = new Logueo("");
            HttpStatusCode statusCode;
            string token;

            // determine whether a jwt exists or not
            if (!TryRetrieveToken(request, out token))
            {
                statusCode = HttpStatusCode.Unauthorized;
                return base.SendAsync(request, cancellationToken);
            }

            if (!validateToken(token, log))
            {
                statusCode = HttpStatusCode.Unauthorized;
                return base.SendAsync(request, cancellationToken);
            }


            try
            {
                var secretKey = AzureExtensions.ObtenerValorSecretoAzure(ConfigurationManager.AppSettings["JWT_SECRET_KEY"].ToString(), "", "", log);//ConfigurationManager.AppSettings["JWT_SECRET_KEY"];
                var audienceToken = ConfigurationManager.AppSettings["JWT_AUDIENCE_TOKEN"];
                var issuerToken = ConfigurationManager.AppSettings["JWT_ISSUER_TOKEN"];
                var securityKey = new SymmetricSecurityKey(System.Text.Encoding.Default.GetBytes(secretKey));

                SecurityToken securityToken;

                var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                TokenValidationParameters validationParameters = new TokenValidationParameters()
                {
                    ValidAudience = audienceToken,
                    ValidIssuer = issuerToken,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    LifetimeValidator = this.LifetimeValidator,
                    IssuerSigningKey = securityKey
                };

                // Extract and assign Current Principal and user
                Thread.CurrentPrincipal = tokenHandler.ValidateToken(token, validationParameters, out securityToken);
                HttpContext.Current.User = tokenHandler.ValidateToken(token, validationParameters, out securityToken);

                return base.SendAsync(request, cancellationToken);
            }
            catch (SecurityTokenValidationException ex)
            {
                statusCode = HttpStatusCode.Unauthorized;
                log.Error("[SendAsync] " + "[" + ex.Message + "]" + ex.StackTrace);
            }
            catch (Exception ex)
            {
                log.Error("[SendAsync] " + "[" + ex.Message + "]" + ex.StackTrace);
                statusCode = HttpStatusCode.InternalServerError;
            }

            return Task<HttpResponseMessage>.Factory.StartNew(() => new HttpResponseMessage(statusCode) { });
        }

        public bool LifetimeValidator(DateTime? notBefore, DateTime? expires, SecurityToken securityToken, TokenValidationParameters validationParameters)
        {
            if (expires != null)
            {
                if (DateTime.UtcNow < expires) return true;
            }
            return false;
        }


        public bool validateToken(string pToken, Logueo log)
        {
            Hashtable ht = new Hashtable();
            bool existeToken = false;
            ht.Add("@token", pToken);

            var validaToken = new ResponseGral()
            {
                CodigoRespuesta = 99,
                Mensaje = "Error inesperado. Consulte al administrador"
            };

            bool activarTemporal = false;
            try {
                activarTemporal= Convert.ToBoolean(ConfigurationManager.AppSettings["enableTokenTemporal"].ToString());
            } catch (Exception Token) {
                activarTemporal = false;
            }

            //if (TableToken.getToken(pToken))
            //{
            //    // log.Evento("[validateToken] [" + pToken + "]");

            //    return existeToken = true;
            //}
            if (activarTemporal && TableToken.getToken(pToken,log))//false)//TableToken.getToken(pToken))
            {
                // log.Evento("[validateToken] [" + pToken + "]");

                return existeToken = true;
            }

            try
            {
                validaToken = SPs.executeSP("wsR_ValidateToken_reportLogIn", ht, log);
                if (validaToken.CodigoRespuesta == 0)
                {
                    if (activarTemporal)
                    {
                        TableToken.setToken(pToken, log);
                    }
                    return existeToken = true;
                }
            }
            catch (Exception ex)
            {
                log.Error("[validateTokenBD] " + "[" + ex.Message + "]" + ex.StackTrace);
            }

            //else
            //{
            //    ////hardkode wirebit
            //    //if (pToken == "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6InVzcldzV2lyZWJpdCIsIm5hbWVpZCI6IjEwMGJkM2U5LWQ1OTEtNGI4YS04ZmZjLTY2N2QwYjQ0Yjg3MCIsIm5iZiI6MTY5MDczODEwMywiZXhwIjoxNzIyMjc0MTAzLCJpYXQiOjE2OTA3MzgxMDMsImlzcyI6Imh0dHA6Ly9sb2NhbGhvc3QiLCJhdWQiOiJodHRwOi8vbG9jYWxob3N0In0.ajqfQvejwTyz0yMHI9hHUnn-xrODIVvsef5CBbtIirY") {
            //    //    TableToken.setToken(pToken);
            //    //    if (TableToken.getToken(pToken))
            //    //    {
            //    //        // log.Evento("[validateToken] [" + pToken + "]");
            //    //        return existeToken = true;
            //    //    }
            //    //}
            //    ////fin hardkode


            //    //esta va afuera
            //    try
            //    {
            //        validaToken = SPs.executeSP("wsR_ValidateToken_reportLogIn", ht, log);
            //        if (validaToken.CodigoRespuesta == 0)
            //        {
            //            TableToken.setToken(pToken);

            //            return existeToken = true;
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        log.Error("[validateTokenBD] " + "[" + ex.Message + "]" + ex.StackTrace);
            //    }
            //}

            return existeToken;
        }
    }
}