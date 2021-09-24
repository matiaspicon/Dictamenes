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
    public class TiposSujetoObligadoController : Controller
    {
        private DictamenesDbContext db = new DictamenesDbContext();

        // GET: TiposSujetoObligado
        public ActionResult Index()
        {
            var tipoSujetoObligado = db.TiposSujetoObligado.Where(d => d.EstaActivo && d.EstaHabilitado).Include(t => t.Usuario);
            return View(tipoSujetoObligado.ToList());
        }

        // GET: TiposSujetoObligado/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoSujetoObligado tipoSujetoObligado = db.TiposSujetoObligado.Find(id);
            if (tipoSujetoObligado == null)
            {
                return HttpNotFound();
            }
            return View(tipoSujetoObligado);
        }

        // GET: TiposSujetoObligado/Create
        public ActionResult Create()
        {
            ViewBag.IdUsuarioModificacion = new SelectList(db.Usuarios, "Id", "Nombre");
            return View();
        }

        // POST: TiposSujetoObligado/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Descripcion,EstaHabilitado,EstaActivo,FechaModificacion,IdUsuarioModificacion")] TipoSujetoObligado tipoSujetoObligado)
        {
            if (ModelState.IsValid)
            {
                db.TiposSujetoObligado.Add(tipoSujetoObligado);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdUsuarioModificacion = new SelectList(db.Usuarios, "Id", "Nombre", tipoSujetoObligado.IdUsuarioModificacion);
            return View(tipoSujetoObligado);
        }

        // GET: TiposSujetoObligado/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoSujetoObligado tipoSujetoObligado = db.TiposSujetoObligado.Find(id);
            if (tipoSujetoObligado == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdUsuarioModificacion = new SelectList(db.Usuarios, "Id", "Nombre", tipoSujetoObligado.IdUsuarioModificacion);
            return View(tipoSujetoObligado);
        }

        // POST: TiposSujetoObligado/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Descripcion,EstaHabilitado,EstaActivo,FechaModificacion,IdUsuarioModificacion")] TipoSujetoObligado tipoSujetoObligado)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tipoSujetoObligado).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdUsuarioModificacion = new SelectList(db.Usuarios, "Id", "Nombre", tipoSujetoObligado.IdUsuarioModificacion);
            return View(tipoSujetoObligado);
        }

        // GET: TiposSujetoObligado/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoSujetoObligado tipoSujetoObligado = db.TiposSujetoObligado.Find(id);
            if (tipoSujetoObligado == null)
            {
                return HttpNotFound();
            }
            return View(tipoSujetoObligado);
        }

        // POST: TiposSujetoObligado/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TipoSujetoObligado tipoSujetoObligado = db.TiposSujetoObligado.Find(id);
            db.TiposSujetoObligado.Remove(tipoSujetoObligado);
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
