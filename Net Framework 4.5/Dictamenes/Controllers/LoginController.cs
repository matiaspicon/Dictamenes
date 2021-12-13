using Dictamenes.Models;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WCFLoginUniversal;
using Dictamenes.Security;
using NegUU;


namespace Dictamenes.Controllers
{
    public class LoginController : Controller
    {


        [AllowAnonymous]
        public void Login()
        {
            if (!User.Identity.IsAuthenticated)
            {
                Security.LoginService.LoginUniversalCallback(Request["ID"]);
            }
            else
            {
                HttpContext.Response.Redirect("/");
            }
        }        

        [Authorize]
        public void Logout()
        {
            FormsAuthentication.SignOut();
            Security.LoginService.RedirectToUserHome();
        }

        [AllowAnonymous]
        public ActionResult ErrorNoPermisos()
        {
            return View();
        }

    }
}
