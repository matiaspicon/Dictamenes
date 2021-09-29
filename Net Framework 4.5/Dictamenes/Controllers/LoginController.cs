using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WCFLoginUniversal;
using System.Web.Security;
using Newtonsoft.Json;

namespace WebApplication3.Controllers
{
    public class LoginController : Controller
    {

        LogLoginService loginService = new LogLoginService();
        // GET: Login
        public ActionResult Login()
        {            
            return View();
        }

        [HttpPost]
        public ActionResult Login(string cuil, string idcomp, string Nombre, string Pass)
        {
            WCFUsuarioLogeado usuarioLogeado = loginService.LogeoUsuario(cuil, idcomp, Nombre, Pass, "1", false);

            if(usuarioLogeado == null)
            {

                string userData = JsonConvert.SerializeObject(new { cuil, idcomp });

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

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        // GET: Login/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Login/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Login/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Login/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Login/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Login/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Login/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
