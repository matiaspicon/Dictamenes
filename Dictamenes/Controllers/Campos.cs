using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dictamenes.Controllers
{
    public class Campos : Controller
    {
        
        public IActionResult Menu()
        {
            return View();
        }
    }
}
