using Dictamenes.Models;
using Dictamenes.Framework;
using Dictamenes.Framework.Configuration;
using Dictamenes.Framework.Login;
using Dictamenes.Framework.Security;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.ServiceModel;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;

namespace Dictamenes.Framework.Security
{
    public static class LoginService
    {        
        public static bool LoginUniversalCallback(string sessionId)
        {
            if (string.IsNullOrEmpty(sessionId))
            {
                return false;
            }
            LogLoginClient loginUniversalClient = LoginUniversalClient;
            string str = loginUniversalClient.Desencriptar(sessionId);
            WCFUsuarioLogeado datosLogin = loginUniversalClient.GetDatosLogin(Convert.ToInt32(str.Substring(0, str.IndexOf('-'))), false);
            if (datosLogin == null) return false;

            //a partir del usuario logeado obtengo el rol del mismo

            string menuBootstrap = datosLogin.Menu
                .Replace("<li><a href=\"#\">Dictamenes</a><ul>", "") //Borro el menu padre
                .Replace("sf-menu", "navbar-nav mr-auto") //Cambio la clase para bootstrap
                .Replace("<a ", "<a class=\"nav-link\""); //Le agrego la clase para los elementos

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
                Menu = menuBootstrap,
                Dictionary = new Dictionary<string, object>()
            };

            string userData = JsonConvert.SerializeObject(token);
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                1,
                datosLogin.NombreUsuario.ToUpper(),
                DateTime.Now,
                DateTime.Now.AddMinutes(30),
                false,
                userData,
                FormsAuthentication.FormsCookiePath);

            // Encripta el ticket.
            string encTicket = FormsAuthentication.Encrypt(ticket);

            // Crea la cookie.
            HttpContext.Current.Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));

            return true;

        }

        public static String RedirectToLogin()
        {
            IPAddress address;
            string ipString = HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
            string str2 = LoginUniversalClient.Encriptar(App.Config.AppID.Value.ToString());
            return string.Format(App.Config.LoginUrl.Value, str2, (((ipString.Split(new char[] { '.' }).Length != 4) || !IPAddress.TryParse(ipString, out address)) || ipString.Contains("127.0")) ? "1" : "0");
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



        public static SessionToken Current
        {
            get => GetCurrent();
        }

        public static bool IsAllowed(string[] roles)
        {
            var rolesEnum = (IEnumerable<string>)roles;
            var rol = LoginService.Current.GrupoNombre;
            var allowed = rolesEnum.Contains<string>(rol);

            return allowed;
        }

        public static SessionToken GetCurrent()
        {
            var cookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (cookie == null) return null;
            var cookieValue = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName].Value;
            var cookieDecrypt = FormsAuthentication.Decrypt(cookieValue);
            return JsonConvert.DeserializeObject<SessionToken>(cookieDecrypt.UserData);
        }

    }
}
