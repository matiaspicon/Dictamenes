using Dictamenes.Database;
using Dictamenes.Models;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace Dictamenes.Controllers
{
    public class DenunciantesController : Controller
    {
        private DictamenesDbContext db = new DictamenesDbContext();

        // GET: SujetosControlados
        public ActionResult Index()
        {
            if (!FrameworkMVC.Security.LoginService.IsAllowed(new[] { Models.Rol.CARGAR.ToString() }))
            {
                return RedirectToAction("ErrorNoPermisos", "Login");
            }

            var SujetoControlado = db.SujetosControlados.Where(d => d.RazonSocial == null);
            return View(SujetoControlado.ToList());
        }

        // GET: SujetosControlados/Edit/5
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
            SujetoControlado SujetoControlado = db.SujetosControlados.Find(id);
            if (SujetoControlado == null)
            {
                return HttpNotFound();
            }
            return View(SujetoControlado);
        }

        // POST: SujetosControlados/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CuilCuit,Nombre,Apellido,RazonSocial,IdTipoSujetoControlado, EstaHabilitado,EstaActivo,FechaModificacion,IdUsuarioModificacion, IdOriginal")] SujetoControlado SujetoControlado)
        {
            if (!FrameworkMVC.Security.LoginService.IsAllowed(new[] { Models.Rol.CARGAR.ToString() }))
            {
                return RedirectToAction("ErrorNoPermisos", "Login");
            }

            SujetoControlado SujetoControladoViejo = db.SujetosControlados.AsNoTracking().First(d => d.Id == SujetoControlado.Id);
            if (SujetoControladoViejo.CuilCuit != SujetoControlado.CuilCuit && db.SujetosControlados.FirstOrDefault(s => s.CuilCuit == SujetoControlado.CuilCuit) != null)
            {
                ModelState.AddModelError("CuilCuit", "Ya existe un Sujeto Controlado con ese Cuil/Cuit");
            }

            if (ModelState.IsValid)
            {
                SujetoControladoLog SujetoControladoLog = new SujetoControladoLog
                {
                    CuilCuit = SujetoControladoViejo.CuilCuit,
                    Nombre = SujetoControladoViejo.Nombre,
                    Apellido = SujetoControladoViejo.Apellido,
                    RazonSocial = SujetoControladoViejo.RazonSocial,
                    IdOriginal = SujetoControladoViejo.Id,
                    EstaHabilitado = SujetoControladoViejo.EstaHabilitado,
                    IdTipoSujetoControlado = SujetoControladoViejo.IdTipoSujetoControlado,
                    FechaModificacion = SujetoControladoViejo.FechaModificacion,
                    IdUsuarioModificacion = SujetoControladoViejo.IdUsuarioModificacion
                };
                SujetoControlado.IdUsuarioModificacion =  FrameworkMVC.Security.LoginService.Current.UsuarioID;
                SujetoControlado.FechaModificacion = DateTime.Now;

                db.Entry(SujetoControlado).State = EntityState.Modified;

                db.SujetosControladosLog.Add(SujetoControladoLog);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(SujetoControlado);
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
