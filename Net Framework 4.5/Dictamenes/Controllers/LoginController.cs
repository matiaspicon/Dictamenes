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
            ViewBag.ReturnUrl = returnUrl;
            return View();           
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(string CuilCuit, string Idcomp, string Nombre, string Pass, string ReturnURL)
        {
            //if (CuilCuit != "" && Idcomp != "" && Nombre != "" && Pass != "")
            //{
            //    WCFUsuarioLogeado usuarioLogeado = loginService.LogeoUsuario(CuilCuit, Idcomp, Nombre, Pass, ConfigurationManager.AppSettings["IdApp"], false);
            //var algo = loginservice.obtengolistausuarioslogeados();
            //var algo1 = loginService.ObtengoPersonaByAplicativo(259);
            //var app = neguu.naplicaciones.obteneraplicacionbyid(259);
            //var usuariolog = loginservice.obtengolistausuarioslogeados();
            //WCFUsuarioLogeado usuarioLogeado = null;

            WCFUsuarioLogeado usuarioLogeado = new WCFUsuarioLogeado();
            if (usuarioLogeado != null)
                {
                    string userData = JsonConvert.SerializeObject(new UsuarioLogueado
                    {
                        ApellidoPersona = usuarioLogeado.ApellidoPersona,
                        CUIL_CUIT = usuarioLogeado.CUIL_CUIT,
                        //GrupoDescripcion = usuarioLogeado.Grupos.GrupoDescripcion == "CARGAR" ? Models.Rol.CARGAR.ToString() : Models.Rol.CONSULTAR.ToString(),
                        GrupoDescripcion = Nombre.ToUpper(),
                        IdGrupo = usuarioLogeado.Grupos.IdGrupo,
                        Id = 7777,
                        Mail = usuarioLogeado.Mail,
                        NombrePersona = usuarioLogeado.NombrePersona,
                        NombreUsuario = usuarioLogeado.NombreUsuario,
                        Telefono = usuarioLogeado.Telefono
                    });    

                    FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                        1,
                        //Nombre.ToUpper(),
                        "USER",
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
                    if (ReturnURL != null) return Redirect(ReturnURL);
                    return RedirectToAction("Index", "Dictamenes");
            }

            //}



            ViewBag.Error = "Los datos ingresados son incorrectos";
            return View();
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
