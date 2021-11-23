using System.Web.Mvc;

namespace Dictamenes.Controllers
{
    public class CamposController : Controller
    {
        public ActionResult Menu()
        {
            if (LoginController.GetUserRolIdentity(User.Identity) != Models.Rol.CARGAR.ToString())
            {
                return RedirectToAction("ErrorNoPermisos", "Login");
            }

            return View();
        }
    }
}
