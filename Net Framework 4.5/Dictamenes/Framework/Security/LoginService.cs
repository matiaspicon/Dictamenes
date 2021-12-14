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
        public static bool IsAllowed(string[] roles) =>
            IsLogged() ? roles.Contains<string>(Current.GrupoNombre) : false;

        public static bool IsLogged() =>
            !ReferenceEquals(Current, null);


        public static String LoginUniversalCallback(string sessionId, string forceUrl = null)
        {
            if (string.IsNullOrEmpty(sessionId))
            {
                return RedirectToUserHome();
            }
            LogLoginClient loginUniversalClient = LoginUniversalClient;
            string str = loginUniversalClient.Desencriptar(sessionId);
            WCFUsuarioLogeado datosLogin = loginUniversalClient.GetDatosLogin(Convert.ToInt32(str.Substring(0, str.IndexOf('-'))), false);
            if (datosLogin == null)
            {
                return RedirectToUserHome ();
            }
            else
            {
                //a partir del usuario logeado obtengo el rol del mismo
                string rol = SetRol(datosLogin.Grupos.IdGrupo.ToString());

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

                
                return RedirectToUserHome();
                
            }
        }

        private static string SetRol(string idGrupo)
        {
            //si pertenece a alguno de los dos grupos de la aplicacion se le otorga el rol correspondiente
            //sino por defecto devuelve null

            string rol = null;

            if (idGrupo == ConfigurationManager.AppSettings["IdGrupoConsultas"])
            {
                rol = Rol.CONSULTAR.ToString();
            }
            else if (idGrupo == ConfigurationManager.AppSettings["IdGrupoCarga"])
            {
                rol = Rol.CARGAR.ToString();
            }
            return rol;

        }


        public static String RedirectToLogin()
        {
            IPAddress address;
            string ipString = HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
            string str2 = LoginUniversalClient.Encriptar(App.Config.AppID.Value.ToString());
            return string.Format(App.Config.LoginUrl.Value, str2, (((ipString.Split(new char[] { '.' }).Length != 4) || !IPAddress.TryParse(ipString, out address)) || ipString.Contains("127.0")) ? "1" : "0");
        }

        public static String RedirectToUserHome()
        {
            if (!IsLogged())
            {
                return RedirectToLogin();
            }           
            return "/";
            
        }

        //public static bool StaticLogin(int idUsuario)
        //{
        //    LogLoginClient loginUniversalClient = LoginUniversalClient;
        //    Usuarios usuarios = loginUniversalClient.ObtengoPersonaByAplicativoIdUser(App.Config.AppID.Value, idUsuario);
        //    if (usuarios == null)
        //    {
        //        return false;
        //    }
        //    SessionToken token = new SessionToken
        //    {
        //        GrupoID = usuarios.Grupos.First<Grupos>().IdGrupo,
        //        GrupoNombre = usuarios.Grupos.First<Grupos>().GrupoDescripcion,
        //        UsuarioID = usuarios.Id,
        //        UsuarioNombre = usuarios.NombreUsuario,
        //        UsuarioApellido = usuarios.ApellidoPersona,
        //        CiaID = usuarios.Ent.Codigo,
        //        Cuil = usuarios.CUIL_CUIT,
        //        Gerencia = usuarios.Gerencias,
        //        OperacionID = "",
        //        Menu = BuildMenu(loginUniversalClient.ObtengoMenuXML(idUsuario, App.Config.AppID.Value, usuarios.Grupos.First<Grupos>().IdGrupo)),
        //        Dictionary = new Dictionary<string, object>()
        //    };
        //    return true;
        //}

        public static SessionToken Current
        {
            get => GetCurrent();
        }

        public static SessionToken GetCurrent()
        {
            var cookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (cookie == null) return null;
            var cookieValue = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName].Value;
            var cookieDecrypt = FormsAuthentication.Decrypt(cookieValue);
            return JsonConvert.DeserializeObject<SessionToken>(cookieDecrypt.UserData);
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

        public static string BuildMenu(string aXmlString)
        {
            #region Obtener el XML del String del Menu
            Encoding lEncoding = Encoding.GetEncoding("ISO-8859-1");
            byte[] lEncodedString = lEncoding.GetBytes(depurarXMLMenu(aXmlString));
            // Put the byte array into a stream and rewind it to the beginning
            var lMemStream = new MemoryStream(lEncodedString);
            lMemStream.Flush();
            lMemStream.Position = 0;

            // Build the XmlDocument from the MemorySteam of UTF-8 encoded bytes
            var lXmlDoc = new XmlDocument();
            lXmlDoc.Load(lMemStream);
            #endregion
            #region Obtener el XSL del String de la URL que pasen.
            const string lUrlFromMenu = "http://seguro3.ssn.gov.ar/UsuarioUniversalWS/xslt/menu.xsl";
            var lXSLReader = new XmlTextReader(lUrlFromMenu);
            var lXSLDoc = new XmlDocument();
            lXSLDoc.Load(lXSLReader);
            #endregion
            #region Hago la transformacion
            var lXslt = new XslTransform();
            lXslt.Load(lXSLDoc);
            var lWritter = new StringWriter();
            lXslt.Transform(lXmlDoc.CreateNavigator(), null, lWritter);
            #endregion
            return lWritter.ToString();


        }
        private static String depurarXMLMenu(String aXmlMenu)
        {
            return aXmlMenu.Replace("\n", "").Replace("\\\"", "");
        }


    }
}
