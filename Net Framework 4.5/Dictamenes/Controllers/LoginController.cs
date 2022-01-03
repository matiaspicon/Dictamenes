using System.Web.Mvc;
using System.Web.Security;
using Dictamenes.Framework.Security;

namespace Dictamenes.Controllers
{
    public class LoginController : Controller
    {

        [AllowAnonymous]
        public ActionResult Login(string ID)
        {
            if (!User.Identity.IsAuthenticated)
            {
                if (!LoginService.LoginUniversalCallbackCookie(ID, menuNavBarBootstrap: true))
                {
                    return Redirect(LoginService.GetURLLoginUniversal());
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
