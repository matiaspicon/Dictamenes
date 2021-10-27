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

namespace Dictamenes.Controllers
{
    public class LoginController : Controller
    {

        LogLoginService loginService = new LogLoginService();
        // GET: Login

        class UsuarioLogueado{
            
            public string NombreUsuario { get; set; }
            
            public int Id { get; set; }
            
            public string NombrePersona { get; set; }
            
            public string ApellidoPersona { get; set; }
            
            public string CUIL_CUIT { get; set; }
            
            public string Mail { get; set; }
            
            public string Telefono { get; set; }

            public string GrupoDescripcion { get; set; }

            public int IdGrupo { get; set; }

        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                if(Session["rol"] == null)
                {
                    var usuarioLogeado = JsonConvert.DeserializeObject<UsuarioLogueado>(((FormsIdentity)User.Identity).Ticket.UserData);
                    Session["rol"] = usuarioLogeado.GrupoDescripcion != "CARGAR" ? Models.Rol.CARGAR.ToString() : Models.Rol.CONSULTAR.ToString();
                    Session["rolID"] = usuarioLogeado.IdGrupo;
                }                  
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
            //WCFUsuarioLogeado usuarioLogeado = loginService.LogeoUsuario(CuilCuit, Idcomp, Nombre, Pass, ConfigurationManager.AppSettings["IdApp"], false);
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
                        GrupoDescripcion = usuarioLogeado.Grupos.GrupoDescripcion,
                        IdGrupo = usuarioLogeado.Grupos.IdGrupo,
                        Id = usuarioLogeado.Id,
                        Mail = usuarioLogeado.Mail,
                        NombrePersona = usuarioLogeado.NombrePersona,
                        NombreUsuario = usuarioLogeado.NombreUsuario,
                        Telefono = usuarioLogeado.Telefono
                    });
                    Session["rol"] = usuarioLogeado.Grupos.GrupoDescripcion != "CARGAR" ? Models.Rol.CARGAR.ToString() : Models.Rol.CONSULTAR.ToString();
                    Session["rolID"] = usuarioLogeado.Grupos.IdGrupo;

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
            Session["rol"] = null;
            Session["rolId"] = null;
            FormsAuthentication.SignOut();
            return RedirectToAction("Login","Login");
        }

        public ActionResult ErrorNoPermisos()
        {
            return View();
        }

    }
}
