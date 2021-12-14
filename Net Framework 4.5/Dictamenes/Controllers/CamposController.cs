using System.Web.Mvc;
using Dictamenes.Framework.Security;

namespace Dictamenes.Controllers
{
    public class CamposController : Controller
    {
        public ActionResult Menu()
        {
            if (Framework.Security.LoginService.Current.GrupoNombre != Models.Rol.CARGAR.ToString())
            {
                return RedirectToAction("ErrorNoPermisos", "Login");
            }

            return View();
        }
    }
}
