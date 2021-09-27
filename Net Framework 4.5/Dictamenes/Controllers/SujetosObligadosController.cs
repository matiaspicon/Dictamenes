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
    public class SujetosObligadosController : Controller
    {
        private DictamenesDbContext db = new DictamenesDbContext();

        // GET: SujetosObligados
        public ActionResult Index()
        {
            var sujetoObligado = db.SujetosObligados.Where(d => d.EstaActivo && d.RazonSocial != null).Include(s => s.TipoSujetoObligado).Include(s => s.UsuarioModificacion);
            return View(sujetoObligado.ToList());
        }

        // GET: SujetosObligados/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SujetoObligado sujetoObligado = db.SujetosObligados.Find(id);
            if (sujetoObligado == null)
            {
                return HttpNotFound();
            }
            return View(sujetoObligado);
        }

        // GET: SujetosObligados/Create
        public ActionResult Create()
        {
            ViewBag.IdTipoSujetoObligado = new SelectList(db.TiposSujetoObligado.Where(d => d.EstaActivo && d.EstaHabilitado && d.Descripcion != "Denunciante"), "Id", "Descripcion");
            return View();
        }

        // POST: SujetosObligados/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,CuilCuit,Nombre,Apellido,RazonSocial,IdTipoSujetoObligado,EstaHabilitado,EstaActivo,FechaModificacion,IdUsuarioModificacion")] SujetoObligado sujetoObligado)
        {
            sujetoObligado.EstaActivo = true;
            sujetoObligado.EstaHabilitado = true;
            sujetoObligado.FechaModificacion = DateTime.Now;
            sujetoObligado.IdUsuarioModificacion = 0;

            if (ModelState.IsValid)
            {
                db.SujetosObligados.Add(sujetoObligado);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdTipoSujetoObligado = new SelectList(db.TiposSujetoObligado.Where(d => d.EstaActivo && d.EstaHabilitado && d.Descripcion != "Denunciante"), "Id", "Descripcion", sujetoObligado.IdTipoSujetoObligado);
            return View(sujetoObligado);
        }

        // GET: SujetosObligados/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SujetoObligado sujetoObligado = db.SujetosObligados.Find(id);
            if (sujetoObligado == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdTipoSujetoObligado = new SelectList(db.TiposSujetoObligado.Where(d => d.EstaActivo && d.EstaHabilitado && d.Descripcion != "Denunciante"), "Id", "Descripcion", sujetoObligado.IdTipoSujetoObligado);
            return View(sujetoObligado);
        }

        // POST: SujetosObligados/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CuilCuit,Nombre,Apellido,RazonSocial,IdTipoSujetoObligado, EstaHabilitado,EstaActivo,FechaModificacion,IdUsuarioModificacion")] SujetoObligado sujetoObligado)
        {
            if (ModelState.IsValid)
            {
                SujetoObligado sujetoObligadoViejo = db.SujetosObligados.AsNoTracking().First(d => d.Id == sujetoObligado.Id);

                sujetoObligado.EstaActivo = true;
                sujetoObligado.IdUsuarioModificacion = 3;
                //dictamen.IdUsuarioModificacion = _context.Usuario;
                sujetoObligado.FechaModificacion = DateTime.Now;
                db.Entry(sujetoObligado).State = EntityState.Modified;

                sujetoObligadoViejo.EstaActivo = false;
                sujetoObligadoViejo.Id = 0;

                db.SujetosObligados.Add(sujetoObligadoViejo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdTipoSujetoObligado = new SelectList(db.TiposSujetoObligado.Where(d => d.EstaActivo && d.EstaHabilitado && d.Descripcion != "Denunciante"), "Id", "Descripcion", sujetoObligado.IdTipoSujetoObligado);
            return View(sujetoObligado);
        }

        // GET: SujetosObligados/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SujetoObligado sujetoObligado = db.SujetosObligados.Find(id);
            if (sujetoObligado == null)
            {
                return HttpNotFound();
            }
            return View(sujetoObligado);
        }

        // POST: SujetosObligados/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SujetoObligado sujetoObligado = db.SujetosObligados.Find(id);
            db.SujetosObligados.Remove(sujetoObligado);
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
