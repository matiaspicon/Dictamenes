using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WCFLoginUniversal;

namespace Dictamenes.Controllers
{
    public class CamposController : Controller
    {
        public ActionResult Menu()
        {
            //var logIn = new WCFLoginUniversal.LogLoginService();
            //WCFUsuarioLogeado wCFUsuarioLogeado = logIn.LogeoUsuario("123123","1","NASDAS","ASDASD","1123123",false);            
            //logIn.ObtenerAplicacionById(1);
            ////logIn.GetDatosLogin(2);

            return View();
        }
    }
}
