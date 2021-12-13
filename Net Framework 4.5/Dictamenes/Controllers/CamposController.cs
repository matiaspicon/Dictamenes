using System.Web.Mvc;

namespace Dictamenes.Controllers
{
    public class CamposController : Controller
    {
        public ActionResult Menu()
        {
            if (Dictamenes.Framework.Security.LoginService.Current.GrupoNombre != Models.Rol.CARGAR.ToString())
            {
                return RedirectToAction("ErrorNoPermisos", "Login");
            }

            return View();
        }
    }
}
