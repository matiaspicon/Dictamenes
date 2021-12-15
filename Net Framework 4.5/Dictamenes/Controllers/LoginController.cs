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
                var returnURL = LoginService.LoginUniversalCallback(Request["ID"]);
                if (returnURL != "/") return Redirect(returnURL);
                else return RedirectToAction("Index", "Dictamenes");
            }
            else
            {
                return RedirectToAction("Index","Dictamenes");
            }
        }        

        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Dictamenes");
        }

        [AllowAnonymous]
        public ActionResult ErrorNoPermisos()
        {
            return View();
        }

    }
}
