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
    public class AsuntosController : Controller
    {
        private DictamenesDbContext db = new DictamenesDbContext();

        // GET: Asuntos
        [Authorize]
        public ActionResult Index()
        {
            var asunto = db.Asuntos.Where(d => d.EstaActivo && d.EstaHabilitado).Include(a => a.Usuario);
            return View(asunto.ToList());
        }

        // GET: Asuntos/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Asunto asunto = db.Asuntos.Find(id);
            if (asunto == null)
            {
                return HttpNotFound();
            }
            return View(asunto);
        }

        // GET: Asuntos/Create
        [Authorize]
        public ActionResult Create()
        {
            ViewBag.IdUsuarioModificacion = new SelectList(db.Usuarios, "Id", "Nombre");
            return View();
        }

        // POST: Asuntos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Descripcion,EstaHabilitado,EstaActivo,FechaModificacion,IdUsuarioModificacion")] Asunto asunto)
        {
            asunto.EstaActivo = true;
            asunto.FechaModificacion = DateTime.Now;
            asunto.IdUsuarioModificacion = 0;


            if (ModelState.IsValid)
            {
                db.Asuntos.Add(asunto);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdUsuarioModificacion = new SelectList(db.Usuarios, "Id", "Nombre", asunto.IdUsuarioModificacion);
            return View(asunto);
        }

        public ActionResult CargarAsuntos()
        {
            return View();
        }


        [HttpPost]
        public ActionResult CargarAsuntos(string JSONAsuntos)
        {
            var asuntos = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(JSONAsuntos);

            foreach (string a in asuntos) 
            {                
                Asunto asunto = new Asunto
                {
                    EstaHabilitado = true,
                    EstaActivo = true,
                    FechaModificacion = DateTime.Now,
                    IdUsuarioModificacion = 0,
                    Descripcion = a
                };

                if (ModelState.IsValid)
                {
                        db.Asuntos.Add(asunto);                    
                }
            }
            try
            {
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }            
        }

        // GET: Asuntos/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Asunto asunto = db.Asuntos.Find(id);
            if (asunto == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdUsuarioModificacion = new SelectList(db.Usuarios, "Id", "Nombre", asunto.IdUsuarioModificacion);
            return View(asunto);
        }

        // POST: Asuntos/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit([Bind(Include = "Id,Descripcion,EstaHabilitado,EstaActivo,FechaModificacion,IdUsuarioModificacion")] Asunto asunto)
        {
            if (ModelState.IsValid)
            {
                Asunto asuntoViejo = db.Asuntos.AsNoTracking().First(d => d.Id == asunto.Id);

                asunto.IdUsuarioModificacion = 3;
                //dictamen.IdUsuarioModificacion = db.Usuario;
                asunto.EstaActivo = true;
                asunto.FechaModificacion = DateTime.Now;
                db.Entry(asunto).State = EntityState.Modified;

                asuntoViejo.EstaActivo = false;
                asuntoViejo.Id = 0;

                db.Asuntos.Add(asuntoViejo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdUsuarioModificacion = new SelectList(db.Usuarios, "Id", "Nombre", asunto.IdUsuarioModificacion);
            return View(asunto);
        }

        // GET: Asuntos/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Asunto asunto = db.Asuntos.Find(id);
            if (asunto == null)
            {
                return HttpNotFound();
            }
            return View(asunto);
        }

        // POST: Asuntos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult DeleteConfirmed(int id)
        {
            Asunto asunto = db.Asuntos.Find(id);
            db.Asuntos.Remove(asunto);
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
