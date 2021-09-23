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
            var tipoDictamen = db.TiposDictamen.Include(t => t.Usuario);
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
            ViewBag.IdUsuarioModificacion = new SelectList(db.Usuarios, "Id", "Nombre");
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

            ViewBag.IdUsuarioModificacion = new SelectList(db.Usuarios, "Id", "Nombre", tipoDictamen.IdUsuarioModificacion);
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
            ViewBag.IdUsuarioModificacion = new SelectList(db.Usuarios, "Id", "Nombre", tipoDictamen.IdUsuarioModificacion);
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
                db.Entry(tipoDictamen).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdUsuarioModificacion = new SelectList(db.Usuarios, "Id", "Nombre", tipoDictamen.IdUsuarioModificacion);
            return View(tipoDictamen);
        }

        // GET: TiposDictamen/Delete/5
        public ActionResult Delete(int? id)
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

        // POST: TiposDictamen/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TipoDictamen tipoDictamen = db.TiposDictamen.Find(id);
            db.TiposDictamen.Remove(tipoDictamen);
            db.SaveChanges();
            return RedirectToAction("Index");
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
