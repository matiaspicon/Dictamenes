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

        // GET: SujetosObligados
        public ActionResult Index()
        {
            if (LoginController.GetUserRolIdentity(User.Identity) != Models.Rol.CARGAR.ToString())
            {
                return RedirectToAction("ErrorNoPermisos", "Login");
            }

            var sujetoObligado = db.SujetosObligados.Where(d => d.RazonSocial == null);
            return View(sujetoObligado.ToList());
        }

        // GET: SujetosObligados/Edit/5
        public ActionResult Edit(int? id)
        {
            if (LoginController.GetUserRolIdentity(User.Identity) != Models.Rol.CARGAR.ToString())
            {
                return RedirectToAction("ErrorNoPermisos", "Login");
            }
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
            if (LoginController.GetUserRolIdentity(User.Identity) != Models.Rol.CARGAR.ToString())
            {
                return RedirectToAction("ErrorNoPermisos", "Login");
            }

            SujetoObligado sujetoObligadoViejo = db.SujetosObligados.AsNoTracking().First(d => d.Id == sujetoObligado.Id);
            if (sujetoObligadoViejo.CuilCuit != sujetoObligado.CuilCuit && db.SujetosObligados.FirstOrDefault(s => s.CuilCuit == sujetoObligado.CuilCuit) != null)
            {
                ModelState.AddModelError("CuilCuit", "Ya existe un Sujeto Obligado con ese Cuil/Cuit");
            }

            if (ModelState.IsValid)
            {
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
                sujetoObligado.IdUsuarioModificacion = LoginController.GetUserDataIdentity(User.Identity).Id;
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
