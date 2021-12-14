using Dictamenes.Models;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using NegUU;
using Dictamenes.Framework.Security;

namespace Dictamenes.Controllers
{
    public class LoginController : Controller
    {


        [AllowAnonymous]
        public ActionResult Login()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Redirect(LoginService.LoginUniversalCallback(Request["ID"]));
            }
            else
            {
                return Redirect("/");
            }
        }        

        [Authorize]
        public void Logout()
        {
            FormsAuthentication.SignOut();
            LoginService.RedirectToUserHome();
        }

        [AllowAnonymous]
        public ActionResult ErrorNoPermisos()
        {
            return View();
        }

    }
}
