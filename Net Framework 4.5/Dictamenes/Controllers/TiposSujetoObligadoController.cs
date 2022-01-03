using Dictamenes.Database;
using Dictamenes.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace Dictamenes.Controllers
{
    public class TiposSujetoControladoController : Controller
    {
        private DictamenesDbContext db = new DictamenesDbContext();

        // GET: TiposSujetoControlado
        public ActionResult Index()
        {
            if (!Framework.Security.LoginService.IsAllowed(new[] { Models.Rol.CARGAR.ToString() }))
            {
                return RedirectToAction("ErrorNoPermisos", "Login");
            }
            var tipoSujetoControlado = db.TiposSujetoControlado;
            return View(tipoSujetoControlado.ToList());
        }

        // GET: TiposSujetoControlado/Create
        public ActionResult Create()
        {
            if (!Framework.Security.LoginService.IsAllowed(new[] { Models.Rol.CARGAR.ToString() }))
            {
                return RedirectToAction("ErrorNoPermisos", "Login");
            }
            return View();
        }

        // POST: TiposSujetoControlado/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Descripcion,EstaHabilitado,EstaActivo,FechaModificacion,IdUsuarioModificacion")] TipoSujetoControlado tipoSujetoControlado)
        {
            if (!Framework.Security.LoginService.IsAllowed(new[] { Models.Rol.CARGAR.ToString() }))
            {
                return RedirectToAction("ErrorNoPermisos", "Login");
            }
            tipoSujetoControlado.FechaModificacion = DateTime.Now;
            tipoSujetoControlado.IdUsuarioModificacion =  FrameworkMVC.Security.LoginService.Current.UsuarioID;
            if (ModelState.IsValid)
            {
                db.TiposSujetoControlado.Add(tipoSujetoControlado);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tipoSujetoControlado);
        }

        // GET: TiposSujetoControlado/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!Framework.Security.LoginService.IsAllowed(new[] { Models.Rol.CARGAR.ToString() }))
            {
                return RedirectToAction("ErrorNoPermisos", "Login");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoSujetoControlado tipoSujetoControlado = db.TiposSujetoControlado.Find(id);
            if (tipoSujetoControlado == null)
            {
                return HttpNotFound();
            }
            return View(tipoSujetoControlado);
        }

        // POST: TiposSujetoControlado/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Descripcion,EstaHabilitado,EstaActivo,FechaModificacion,IdUsuarioModificacion")] TipoSujetoControlado tipoSujetoControlado)
        {
            if (!Framework.Security.LoginService.IsAllowed(new[] { Models.Rol.CARGAR.ToString() }))
            {
                return RedirectToAction("ErrorNoPermisos", "Login");
            }
            if (ModelState.IsValid)
            {
                TipoSujetoControlado tipoSujetoControladoViejo = db.TiposSujetoControlado.AsNoTracking().First(d => d.Id == tipoSujetoControlado.Id);

                TipoSujetoControladoLog tipoSujetoControladoLog = new TipoSujetoControladoLog
                {
                    IdOriginal = tipoSujetoControladoViejo.Id,
                    Descripcion = tipoSujetoControladoViejo.Descripcion,
                    EstaHabilitado = tipoSujetoControladoViejo.EstaHabilitado,
                    FechaModificacion = tipoSujetoControladoViejo.FechaModificacion,
                    IdUsuarioModificacion = tipoSujetoControladoViejo.IdUsuarioModificacion
                };

                tipoSujetoControlado.IdUsuarioModificacion =  FrameworkMVC.Security.LoginService.Current.UsuarioID;
                tipoSujetoControlado.FechaModificacion = DateTime.Now;

                db.Entry(tipoSujetoControlado).State = EntityState.Modified;

                db.TiposSujetoControladoLog.Add(tipoSujetoControladoLog);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tipoSujetoControlado);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
