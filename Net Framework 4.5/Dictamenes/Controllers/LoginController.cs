using System.Web.Mvc;
using System.Web.Security;


namespace Dictamenes.Controllers
{
    public class LoginController : Controller
    {


        [AllowAnonymous]
        public void Login()
        {
            if (!User.Identity.IsAuthenticated)
            {
                Dictamenes.Framework.Security.LoginService.LoginUniversalCallback(Request["ID"]);
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
            Dictamenes.Framework.Security.LoginService.RedirectToUserHome();
        }

        [AllowAnonymous]
        public ActionResult ErrorNoPermisos()
        {
            return View();
        }

    }
}
