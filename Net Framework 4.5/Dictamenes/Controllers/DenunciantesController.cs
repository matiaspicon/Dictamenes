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
    public class DenunciantesController : Controller
    {
        private DictamenesDbContext db = new DictamenesDbContext();

        // GET: SujetosObligados
        public ActionResult Index()
        {
            var sujetoObligado = db.SujetosObligados.Where(d => d.RazonSocial == null);
            return View(sujetoObligado.ToList());
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
            return View(sujetoObligado);
        }

        // POST: SujetosObligados/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CuilCuit,Nombre,Apellido,RazonSocial,IdTipoSujetoObligado, EstaHabilitado,EstaActivo,FechaModificacion,IdUsuarioModificacion, IdOriginal")] SujetoObligado sujetoObligado)
        {
            if (ModelState.IsValid)
            {
                SujetoObligado sujetoObligadoViejo = db.SujetosObligados.AsNoTracking().First(d => d.Id == sujetoObligado.Id);
                SujetoObligadoLog sujetoObligadoLog = new SujetoObligadoLog 
                {                    
                    CuilCuit = sujetoObligadoViejo.CuilCuit,
                    Nombre = sujetoObligadoViejo.Nombre,
                    Apellido = sujetoObligadoViejo.Apellido,
                    RazonSocial = sujetoObligadoViejo.RazonSocial,
                    IdOriginal = sujetoObligadoViejo.Id,
                    EstaHabilitado = sujetoObligadoViejo.EstaHabilitado,
                    IdTipoSujetoObligado = sujetoObligadoViejo.IdTipoSujetoObligado,
                    FechaModificacion = sujetoObligadoViejo.FechaModificacion,
                    IdUsuarioModificacion = sujetoObligadoViejo.IdUsuarioModificacion
                };
                sujetoObligado.IdUsuarioModificacion = 3;
                sujetoObligado.FechaModificacion = DateTime.Now;

                db.Entry(sujetoObligado).State = EntityState.Modified;

                db.SujetosObligadosLog.Add(sujetoObligadoLog);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(sujetoObligado);
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
