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
                WCFUsuarioLogeado usuarioLogeado = loginService.LogeoUsuario(CuilCuit, Idcomp, Nombre, Pass, ConfigurationManager.AppSettings["IdApp"], true);
                //WCFUsuarioLogeado usuarioLogeado = new WCFUsuarioLogeado();
                if (usuarioLogeado != null)
                {
                    string rol = setRol(usuarioLogeado.Grupos.IdGrupo.ToString());

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
                        Menu = usuarioLogeado.Menu
                        
                    });
                    FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                        1,
                        Nombre.ToUpper(),
                        DateTime.Now,
                        DateTime.Now.AddMinutes(30),
                        false,
                        userData,
                        FormsAuthentication.FormsCookiePath);

                    // Encrypt the ticket.
                    string encTicket = FormsAuthentication.Encrypt(ticket);

                    // Create the cookie.
                    Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));

                    // Redirect back to original URL.
                    if (rol == null) return RedirectToAction("ErrorNoPermisos");
                    if (ReturnURL != null) return Redirect(ReturnURL);
                    return RedirectToAction("Index", "Dictamenes");
            }

            }



            ViewBag.Error = "Los datos ingresados son incorrectos";
            return View();
        }

        private string setRol(string idGrupo)
        {
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
            return RedirectToAction("Login","Login");
        }


        static public UsuarioLogueado GetUserData(object usuario)
        {
            var identity = usuario as FormsIdentity;
            if (identity == null) return null;
            return JsonConvert.DeserializeObject<UsuarioLogueado>(identity.Ticket.UserData);
        }

        static public string GetUserRol(object usuario)
        {
            UsuarioLogueado user = GetUserData(usuario);
            if (user == null) return "";
            return user.GrupoDescripcion;
        }

        public ActionResult ErrorNoPermisos()
        {
            return View();
        }

    }
}
