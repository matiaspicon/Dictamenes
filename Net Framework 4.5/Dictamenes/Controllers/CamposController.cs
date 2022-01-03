using System.Web.Mvc;

namespace Dictamenes.Controllers
{
    public class CamposController : Controller
    {
        public ActionResult Menu()
        {
            if (!Framework.Security.LoginService.IsAllowed(new[] { Models.Rol.CARGAR.ToString() }))
            {
                return RedirectToAction("ErrorNoPermisos", "Login");
            }

            return View();
        }
    }
}
