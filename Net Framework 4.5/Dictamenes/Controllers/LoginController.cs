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
        public ActionResult Login(string cuil, string idcomp, string Nombre, string Pass)
        {
            WCFUsuarioLogeado usuarioLogeado = loginService.LogeoUsuario(cuil, idcomp, Nombre, Pass, "1", false);
                        

            if (usuarioLogeado == null)
            {                
                string userData = JsonConvert.SerializeObject(new { cuil, idcomp, rol = Nombre });
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
