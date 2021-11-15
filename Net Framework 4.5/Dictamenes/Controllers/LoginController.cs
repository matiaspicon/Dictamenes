using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WCFLoginUniversal;
using System.Web.Security;
using Newtonsoft.Json;
using System.Configuration;
using BEUU;
using System.Security.Claims;
using Microsoft.Owin.Security.Cookies;
using System.Net;
using Dictamenes.Models;

namespace Dictamenes.Controllers
{
    public class LoginController : Controller
    {

        LogLoginService loginService = new LogLoginService();
        // GET: Login

        

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {                
                if (returnUrl != null) return Redirect(returnUrl);
                return RedirectToAction("Index", "Dictamenes");                              
            }
            return View();           
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(string CuilCuit, string Idcomp, string Nombre, string Pass, string ReturnURL)
        {
            if (CuilCuit != "" && Idcomp != "" && Nombre != "" && Pass != "")
            {
                //hago el logeo del usuario con el modulo provisto
                WCFUsuarioLogeado usuarioLogeado = loginService.LogeoUsuario(CuilCuit, Idcomp, Nombre, Pass, ConfigurationManager.AppSettings["IdApp"], true);
                //WCFUsuarioLogeado usuarioLogeado = new WCFUsuarioLogeado();
                if (usuarioLogeado != null)
                {
                    //a partir del usuario logeado obtengo el rol del mismo
                    string rol = SetRol(usuarioLogeado.Grupos.IdGrupo.ToString());

                    string menuBootstrap = usuarioLogeado.Menu
                        .Replace("<li><a href=\"#\">Dictamenes</a><ul>", "") //Borro el menu padre
                        .Replace("sf-menu", "navbar-nav mr-auto") //Cambio la clase para bootstrap
                        .Replace("<a ", "<a class=\"nav-link\""); //Le agrego la clase para los elementos

                    string userData = JsonConvert.SerializeObject(new UsuarioLogueado
                    {
                        ApellidoPersona = usuarioLogeado.ApellidoPersona,
                        CUIL_CUIT = usuarioLogeado.CUIL_CUIT,
                        GrupoDescripcion = rol,
                        IdGrupo = usuarioLogeado.Grupos.IdGrupo,
                        Id = usuarioLogeado.Id,
                        Mail = usuarioLogeado.Mail,
                        NombrePersona = usuarioLogeado.NombrePersona,
                        NombreUsuario = usuarioLogeado.NombreUsuario,
                        Telefono = usuarioLogeado.Telefono,
                        Menu = menuBootstrap
                    });
                    FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                        1,
                        Nombre.ToUpper(),
                        DateTime.Now,
                        DateTime.Now.AddMinutes(30),
                        false,
                        userData,
                        FormsAuthentication.FormsCookiePath);

                    // Encripta el ticket.
                    string encTicket = FormsAuthentication.Encrypt(ticket);

                    // Crea la cookie.
                    Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));   
                    
                    //en caso de que no tenga alguno de los dos roles habilitados, no se le permitira navegar
                    if (rol == null) return RedirectToAction("ErrorNoPermisos");

                    //en caso de que haya se haya pasado una URL
                    if (ReturnURL != null)
                    {
                        try
                        {
                            var algo = ReturnURL.Split('/').Last();
                            if (algo == "index") return Redirect(ReturnURL);
                        }
                        catch { }
                    }        

                    //redirrecionamiento al listado de todos los Dictamenes
                    return RedirectToAction("Index", "Dictamenes");
                }
            }
            //si el logeo fallo por alguna razon, se devuelve este error generico
            ViewBag.Error = "Los datos ingresados son incorrectos";
            return View();
        }

        private string SetRol(string idGrupo)
        {
            //si pertenece a alguno de los dos grupos de la aplicacion se le otorga el rol correspondiente
            //sino por defecto devuelve null

            string rol = null;
                        
            if (idGrupo == ConfigurationManager.AppSettings["IdGrupoConsultas"])
            {
                rol = Rol.CONSULTAR.ToString();
            }else if(idGrupo == ConfigurationManager.AppSettings["IdGrupoCarga"])
            {
                rol = Rol.CARGAR.ToString();
            }
            return rol;

        }

        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index","Dictamenes");
        }

        static public UsuarioLogueado GetUserDataIdentity(object identityUser)
        {
            var identity = identityUser as FormsIdentity;
            if (identity == null) return null;
            return JsonConvert.DeserializeObject<UsuarioLogueado>(identity.Ticket.UserData);
        }

        static public string GetUserRolIdentity(object identityUser)
        {
            UsuarioLogueado user = GetUserDataIdentity(identityUser);
            if (user == null) return "";
            return user.GrupoDescripcion;
        }

        [AllowAnonymous]
        public ActionResult ErrorNoPermisos()
        {
            return View();
        }

    }
}
