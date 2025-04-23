using DNU.Usuarios.APIService.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace DNU.Usuarios.APIService.Negocio
{
    public class LNBloqueoUsuario
    {
        private static List<Usuario> usuariosBloqueados = new List<Usuario>();
        internal static bool BloquearUsuario(string nombreUsuario)
        {
            bool usuarioBloquedado = false;
            bool usuarioEncontrado = false;
            DateTime hoy = DateTime.Now;//DateTime.Today;
            int numeroDeIntentosPermitidos = Convert.ToInt32(ConfigurationManager.AppSettings["ReintentosLogueo"]);
            if (numeroDeIntentosPermitidos == 0)
            {
                return false;
            }

            if (usuariosBloqueados is null) {
                usuariosBloqueados = new List<Usuario>();
            }

            foreach (Usuario usuario in usuariosBloqueados)
            {
                if (usuario.NombreUsuario.ToUpper() == nombreUsuario.ToUpper())
                {
                    usuarioEncontrado = true;
                    // numeroReintentos = usuario.NumeroReintento;
                    usuario.NumeroReintento = usuario.NumeroReintento + 1;
                    if (usuario.NumeroReintento >= numeroDeIntentosPermitidos)
                    {
                        usuarioBloquedado = true;
                        usuario.FechaBloqueo = hoy;
                    }
                    break;
                }
            }

            if (!usuarioEncontrado)
            {
                usuariosBloqueados.Add(new Usuario { FechaBloqueo = hoy, NombreUsuario = nombreUsuario, NumeroReintento = 1 });
            }

            return usuarioBloquedado;

        }

        internal static bool ValidaUsuario(string nombreUsuario)
        {
            bool usuarioValido = true;

            int numeroDeMinutos = Convert.ToInt32(ConfigurationManager.AppSettings["MinutosEsperaReintentosLogueo"]);
            int numeroDeIntentosPermitidos = Convert.ToInt32(ConfigurationManager.AppSettings["ReintentosLogueo"]);

            //if (numeroDeMinutos == 0)
            //{
            //    return true;
            //}
            DateTime Hoy = DateTime.Now;//DateTime.Today;
            int numeroIteracion = 0;
            bool desbloquearUsuario = false;

            if (usuariosBloqueados is null)
            {
                usuariosBloqueados = new List<Usuario>();
            }
            for (int i = 0; i < usuariosBloqueados.Count; i++)
            {
                if (usuariosBloqueados[i].NombreUsuario.ToUpper() == nombreUsuario.ToUpper())
                {
                    if (usuariosBloqueados[i].NumeroReintento >= numeroDeIntentosPermitidos)
                    {
                        if ((Hoy - usuariosBloqueados[i].FechaBloqueo).Minutes >= numeroDeMinutos)
                        {
                            desbloquearUsuario = true;
                            numeroIteracion = i;
                        }
                        else
                        {
                            usuarioValido = false;
                        }
                    }
                    break;

                }

            }
            if (desbloquearUsuario)
            {
                usuariosBloqueados.RemoveAt(numeroIteracion);
            }

            return usuarioValido;

        }


        internal static void BorrarUsuarioListaNegra(string nombreUsuario)
        {
          
            int numeroIteracion = 0;
            bool desbloquearUsuario = false;
            if (usuariosBloqueados is null)
            {
                usuariosBloqueados = new List<Usuario>();
            }
            for (int i = 0; i < usuariosBloqueados.Count; i++)
            {
                if (usuariosBloqueados[i].NombreUsuario.ToUpper() == nombreUsuario.ToUpper())
                {
                    desbloquearUsuario = true;
                    numeroIteracion = i;
                    break;
                }

            }
            if (desbloquearUsuario)
            {
                usuariosBloqueados.RemoveAt(numeroIteracion);

            }

           

        }

    }
}