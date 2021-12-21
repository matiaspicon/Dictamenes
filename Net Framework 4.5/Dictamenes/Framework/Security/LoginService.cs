using Dictamenes.Framework.Login;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security;
using System.ServiceModel;
using System.Web;
using System.Web.Security;

namespace Dictamenes.Framework.Security
{
    public static class LoginService
    {
        /// <summary>
        /// Este metodo recupera los datos del login universal y setea la cookie de Autenticacion.
        /// </summary>
        /// <param name="sessionId">El ID de sesion encriptado que provee el Login Universal luego de loguearse.</param>
        /// <param name="menuNavBarBootstrap">Booleano para parsear el Menu obtenido a formato bootstrap.</param>
        /// <returns>Devuelve true si el logueo se realizo correctamente y se creo la cookie de autenticación. Devuelve false si el ID de sesion es null o si hubo un error en el proceso.</returns>
        public static bool LoginUniversalCallbackCookie(string sessionId, bool menuNavBarBootstrap = false, int minsCookieTimeout = 30)
        {
            WCFUsuarioLogeado datosLogin = LoginUniversalCallback(sessionId);
            if (datosLogin == null) return false;

            //a partir del usuario logeado obtengo el rol del mismo
            string menu;
            if (menuNavBarBootstrap)
            {
                menu = datosLogin.Menu
                .Replace("<li><a href=\"#\">Dictamenes</a><ul>", "") //Borro el menu padre
                .Replace("sf-menu", "navbar-nav mr-auto") //Cambio la clase para bootstrap
                .Replace("<a ", "<a class=\"nav-link\"") //Le agrego la clase para los elementos
                .Replace("<ul></ul>", ""); //Borro etiquetas sin usar
            }
            else
            {
                menu = datosLogin.Menu;
            }

            SessionToken token = new SessionToken
            {
                SessionID = sessionId,
                GrupoID = datosLogin.Grupos.IdGrupo,
                GrupoNombre = SetRol(datosLogin.Grupos.IdGrupo.ToString()),
                UsuarioID = datosLogin.Id,
                UsuarioNombre = datosLogin.NombreUsuario,
                UsuarioApellido = datosLogin.ApellidoPersona,
                CiaID = datosLogin.Ent.Codigo,
                Cuil = datosLogin.CUIL_CUIT,
                Gerencia = datosLogin.Gerencias,
                OperacionID = "",
                Menu = menu,
                Dictionary = new Dictionary<string, object>()
            };

            string userData = JsonConvert.SerializeObject(token);
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                1,
                datosLogin.NombreUsuario.ToUpper(),
                DateTime.Now,
                DateTime.Now.AddMinutes(minsCookieTimeout),
                false,
                userData,
                FormsAuthentication.FormsCookiePath);

            // Encripta el ticket.
            string encTicket = FormsAuthentication.Encrypt(ticket);

            // Crea la cookie.
            HttpContext.Current.Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));

            return true;

        }

        /// <summary>
        /// Este metodo recupera los datos del logueo en el login universal.
        /// </summary>
        /// <param name="sessionId">El ID de sesion encriptado que provee el Login Universal luego de loguearse.</param>
        /// <returns>Devuelve el usuario loguedo o null en caso de no poder obtener los datos.</returns>
        public static WCFUsuarioLogeado LoginUniversalCallback(string sessionId)
        {
            if (string.IsNullOrEmpty(sessionId))
            {
                return null;
            }
            LogLoginClient loginUniversalClient = LoginUniversalClient;
            string str = loginUniversalClient.Desencriptar(sessionId);
            WCFUsuarioLogeado datosLogin = loginUniversalClient.GetDatosLogin(Convert.ToInt32(str.Substring(0, str.IndexOf('-'))), false);
            return datosLogin;
        }

        /// <summary>
        /// Este metodo se encarga de generar la URL a la que redirigir a el usuario para realizar el logueo en el Login Universal.
        /// </summary>
        /// <returns>Devuelve en string la URL a la que se debe redigir al usuario.</returns>
        public static String GetURLLoginUniversal()
        {
            string ipString = HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
            string str2 = LoginUniversalClient.Encriptar(App.Config.AppID.Value.ToString());
            return string.Format(App.Config.LoginUrl.Value, str2, (((ipString.Split(new char[] { '.' }).Length != 4) || !IPAddress.TryParse(ipString, out IPAddress address)) || ipString.Contains("127.0")) ? "1" : "0");
        }

        public static LogLoginClient LoginUniversalClient
        {
            get
            {
                if ((App.Config.WebReferences["LoginUniversal"] == null) || string.IsNullOrEmpty(App.Config.WebReferences["LoginUniversal"].Url))
                {
                    throw new SecurityException("LoginUniversal WebReference not set in Web.Config");
                }
                return new LogLoginClient(new BasicHttpBinding(), new EndpointAddress(App.Config.WebReferences["LoginUniversal"].Url));
            }
        }

        /// <summary>
        /// Este metodo se encarga de buscar el ID de grupo en las framework.config.
        /// </summary>
        /// <param name="idGrupo">ID del grupo buscado.</param>
        /// <returns>El nombre del grupo al que pertenece el usuario o null si no se encontro.</returns>
        private static string SetRol(string idGrupo)
        {
            //si pertenece a alguno de los dos grupos de la aplicacion se le otorga el rol correspondiente
            //sino por defecto devuelve null

            string rolString = null;

            foreach (var rol in App.Config.Roles)
            {
                if (rol.Id == idGrupo)
                {
                    rolString = rol.Name;
                }
            }
            return rolString;

        }

        /// <summary>
        /// Obtiene los datos del usuario de las Cookies.
        /// </summary>
        public static SessionToken Current
        {
            get
            {
                var cookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
                if (cookie == null) return new SessionToken();
                var cookieValue = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName].Value;
                var cookieDecrypt = FormsAuthentication.Decrypt(cookieValue);
                return JsonConvert.DeserializeObject<SessionToken>(cookieDecrypt.UserData);
            }
        }

        public static bool IsAllowed(string[] roles)
        {
            var rolesEnum = (IEnumerable<string>)roles;
            var rol = LoginService.Current.GrupoNombre;
            var allowed = rolesEnum.Contains<string>(rol);

            return allowed;
        }
    }
}
