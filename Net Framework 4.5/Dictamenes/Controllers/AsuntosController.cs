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
            var asunto = db.Asuntos.Where(d =>d.EstaHabilitado);
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
                         return View();
        }

        // POST: Asuntos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Descripcion,EstaHabilitado,EstaActivo,FechaModificacion,IdUsuarioModificacion")] Asunto asunto)
        {
            if (ModelState.IsValid)
            {
                db.Asuntos.Add(asunto);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
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
               AsuntoLog asuntoLog = new AsuntoLog
                {
                    IdOriginal = asuntoViejo.Id,
                    Descripcion = asuntoViejo.Descripcion,
                    EstaHabilitado = asuntoViejo.EstaHabilitado,
                    FechaModificacion = DateTime.Now,
                    IdUsuarioModificacion = 3
                };

                db.Entry(asunto).State = EntityState.Modified;

                db.AsuntosLog.Add(asuntoLog);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
        return View(asunto);
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
