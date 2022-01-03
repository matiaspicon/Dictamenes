using Dictamenes.Database;
using Dictamenes.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace Dictamenes.Controllers
{
    public class TiposDictamenController : Controller
    {
        private DictamenesDbContext db = new DictamenesDbContext();

        // GET: TiposDictamen
        public ActionResult Index()
        {
            if (!FrameworkMVC.Security.LoginService.IsAllowed(new[] { Models.Rol.CARGAR.ToString() }))
            {
                return RedirectToAction("ErrorNoPermisos", "Login");
            }
            var tipoDictamen = db.TiposDictamen;
            return View(tipoDictamen.ToList());
        }


        // GET: TiposDictamen/Create
        public ActionResult Create()
        {
            if (!FrameworkMVC.Security.LoginService.IsAllowed(new[] { Models.Rol.CARGAR.ToString() }))
            {
                return RedirectToAction("ErrorNoPermisos", "Login");
            }
            return View();
        }

        // POST: TiposDictamen/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Descripcion,EstaHabilitado,EstaActivo,FechaModificacion,IdUsuarioModificacion")] TipoDictamen tipoDictamen)
        {
            if (!FrameworkMVC.Security.LoginService.IsAllowed(new[] { Models.Rol.CARGAR.ToString() }))
            {
                return RedirectToAction("ErrorNoPermisos", "Login");
            }
            tipoDictamen.IdUsuarioModificacion =  FrameworkMVC.Security.LoginService.Current.UsuarioID;
            tipoDictamen.FechaModificacion = DateTime.Now;
            if (ModelState.IsValid)
            {
                db.TiposDictamen.Add(tipoDictamen);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tipoDictamen);
        }

        // GET: TiposDictamen/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!FrameworkMVC.Security.LoginService.IsAllowed(new[] { Models.Rol.CARGAR.ToString() }))
            {
                return RedirectToAction("ErrorNoPermisos", "Login");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoDictamen tipoDictamen = db.TiposDictamen.Find(id);
            if (tipoDictamen == null)
            {
                return HttpNotFound();
            }
            return View(tipoDictamen);
        }

        // POST: TiposDictamen/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Descripcion,EstaHabilitado,EstaActivo,FechaModificacion,IdUsuarioModificacion")] TipoDictamen tipoDictamen)
        {
            if (!FrameworkMVC.Security.LoginService.IsAllowed(new[] { Models.Rol.CARGAR.ToString() }))
            {
                return RedirectToAction("ErrorNoPermisos", "Login");
            }
            if (ModelState.IsValid)
            {
                var tipoSujetoDictamenViejo = db.TiposDictamen.AsNoTracking().First(d => d.Id == tipoDictamen.Id);
                TipoDictamenLog tipoDictamenLog = new TipoDictamenLog
                {
                    IdOriginal = tipoSujetoDictamenViejo.Id,
                    Descripcion = tipoSujetoDictamenViejo.Descripcion,
                    EstaHabilitado = tipoSujetoDictamenViejo.EstaHabilitado,
                    FechaModificacion = tipoSujetoDictamenViejo.FechaModificacion,
                    IdUsuarioModificacion = tipoSujetoDictamenViejo.IdUsuarioModificacion
                };

                tipoDictamen.IdUsuarioModificacion =  FrameworkMVC.Security.LoginService.Current.UsuarioID;
                tipoDictamen.FechaModificacion = DateTime.Now;
                db.Entry(tipoDictamen).State = EntityState.Modified;

                db.TiposDictamenLog.Add(tipoDictamenLog);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tipoDictamen);
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
