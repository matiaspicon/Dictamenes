using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WCFLoginUniversal;
using System.Web.Security;
using Newtonsoft.Json;
using BEUU;
using System.Security.Claims;
using Microsoft.Owin.Security.Cookies;
using System.Net;

namespace Dictamenes.Controllers
{
    public class LoginController : Controller
    {

        LogLoginService loginService = new LogLoginService();
        // GET: Login
        [AllowAnonymous]
        public ActionResult Login()
        {
            if (User.Identity.IsAuthenticated && Session["rol"] == null)
            {
                try
                {
                    Session["rol"] = (string)JsonConvert.DeserializeObject<dynamic>(((FormsIdentity)User.Identity).Ticket.UserData).rol;                    
                    return RedirectToAction("Index", "Dictamenes");
                }
                catch { }                
            }

            return View();           
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(string CuilCuit, string idcomp, string Nombre, string Pass)
        {
            WCFUsuarioLogeado usuarioLogeado = loginService.LogeoUsuario(CuilCuit, idcomp, Nombre, Pass, "259", false);
            var algo = loginService.ObtengoListaUsuariosLogeados();
            var algo1 = loginService.ObtengoPersonaByAplicativo(259);
            var usuarioLog = loginService.ObtengoListaUsuariosLogeados();
           //WCFUsuarioLogeado usuarioLogeado = null;

            if (usuarioLogeado == null)
            {                
                string userData = JsonConvert.SerializeObject(new { CuilCuit, idcomp, rol = "CARGAR" });
                Session["rol"] = Nombre;
                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1,
                Nombre,
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
                return RedirectToAction("Index", "Dictamenes");
            }

            ViewBag.Error = "Los datos ingresados son incorrectos";
            return View();
        }

        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        public ActionResult ErrorNoPermisos()
        {
            return View();
        }

    }
}
