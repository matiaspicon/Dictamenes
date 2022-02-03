using System.Web.Mvc;
using System.Web.Security;
using FrameworkMVC.Security;

namespace Dictamenes.Controllers
{
    public class LoginController : Controller
    {

        [AllowAnonymous]
        public ActionResult Login(string ID)
        {
            //comprueba si el usuario esta logueado o si tiene cookie en su sistema
            if (!User.Identity.IsAuthenticated)
            {
                //si no esta logueado se envia el ID al metodo para loguearnos
                if (!LoginService.LoginUniversalCallbackCookie(ID, menuNavBarBootstrap: true))
                {
                    //en caso de que el ID sea nulo o haya fallado el logueo, se redirige a la URL del login universal
                    return Redirect(LoginService.GetURLLoginUniversal());
                }
            }
            //si ya estaba logueado o se completo correctamente en logueo, se redirige al index de Dictamenes
            return RedirectToAction("Index", "Dictamenes");
        }

        [Authorize]
        public ActionResult Logout()
        {
            //borra la cookie en el usuario
            FormsAuthentication.SignOut();
            //redirige al index de Dictamenes
            return RedirectToAction("Index", "Dictamenes");
        }

        [AllowAnonymous]
        public ActionResult ErrorNoPermisos()
        {
            return View();
        }

    }
}
