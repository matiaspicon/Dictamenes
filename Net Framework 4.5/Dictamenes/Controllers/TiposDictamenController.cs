using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Dictamenes.Database;
using Dictamenes.Models;

namespace Dictamenes.Controllers
{
    public class TiposDictamenController : Controller
    {
        private DictamenesDbContext db = new DictamenesDbContext();

        // GET: TiposDictamen
        public ActionResult Index()
        {
            var tipoDictamen = db.TiposDictamen.Where(d =>  d.EstaHabilitado);
            return View(tipoDictamen.ToList());
        }

        // GET: TiposDictamen/Details/5
        public ActionResult Details(int? id)
        {
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

        // GET: TiposDictamen/Create
        public ActionResult Create()
        {
                         return View();
        }

        // POST: TiposDictamen/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Descripcion,EstaHabilitado,EstaActivo,FechaModificacion,IdUsuarioModificacion")] TipoDictamen tipoDictamen)
        {
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
            if (ModelState.IsValid)
            {
                var tipoSujetoDictamenViejo = db.TiposDictamen.AsNoTracking().First(d => d.Id == tipoDictamen.Id);               
                TipoDictamenLog tipoDictamenLog = new TipoDictamenLog
                {
                    IdOriginal = tipoSujetoDictamenViejo.Id,
                    Descripcion = tipoSujetoDictamenViejo.Descripcion,
                    EstaHabilitado = tipoSujetoDictamenViejo.EstaHabilitado,
                    FechaModificacion = DateTime.Now,
                    IdUsuarioModificacion = 3
                };

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
