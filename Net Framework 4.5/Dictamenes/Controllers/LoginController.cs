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
using System.Collections.Generic;

namespace Dictamenes.Controllers
{
    public class LoginController : Controller
    {


        [AllowAnonymous]
        public ActionResult Login(string ID)
        {
            if (!User.Identity.IsAuthenticated)
            { 
                if (!LoginService.LoginUniversalCallback(ID))
                {
                    return Redirect(LoginService.RedirectToLogin());
                }
            }
            return RedirectToAction("Index", "Dictamenes");
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
