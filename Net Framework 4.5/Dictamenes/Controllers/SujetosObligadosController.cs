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
    public class SujetosObligadosController : Controller
    {
        private DictamenesDbContext db = new DictamenesDbContext();

        // GET: SujetosObligados
        public ActionResult Index()
        {
            if (Security.LoginService.Current.GrupoNombre != Models.Rol.CARGAR.ToString())
            {
                return RedirectToAction("ErrorNoPermisos", "Login");
            }

            var sujetoObligado = db.SujetosObligados.Where(d => d.RazonSocial != null).Include(s => s.TipoSujetoObligado);
            return View(sujetoObligado.ToList());
        }

        // GET: SujetosObligados/Create
        public ActionResult Create()
        {
            if (Security.LoginService.Current.GrupoNombre != Models.Rol.CARGAR.ToString())
            {
                return RedirectToAction("ErrorNoPermisos", "Login");
            }

            ViewBag.IdTipoSujetoObligado = new SelectList(db.TiposSujetoObligado.Where(d => d.EstaHabilitado && d.Descripcion != "Denunciante").OrderBy(m => m.Descripcion), "Id", "Descripcion");
            return View();
        }

        // POST: SujetosObligados/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,CuilCuit,Nombre,Apellido,RazonSocial,IdTipoSujetoObligado,EstaHabilitado,EstaActivo,FechaModificacion,IdUsuarioModificacion")] SujetoObligado sujetoObligado)
        {
            if (Security.LoginService.Current.GrupoNombre != Models.Rol.CARGAR.ToString())
            {
                return RedirectToAction("ErrorNoPermisos", "Login");
            }

            if (db.SujetosObligados.FirstOrDefault(s => s.CuilCuit == sujetoObligado.CuilCuit) != null)
            {
                ModelState.AddModelError("CuilCuit", "Ya existe un Sujeto Obligado con ese Cuil/Cuit");
            }

            sujetoObligado.IdUsuarioModificacion = Security.LoginService.Current.UsuarioID;
            sujetoObligado.FechaModificacion = DateTime.Now;
            if (ModelState.IsValid)
            {
                db.SujetosObligados.Add(sujetoObligado);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdTipoSujetoObligado = new SelectList(db.TiposSujetoObligado.Where(d => d.EstaHabilitado && d.Descripcion != "Denunciante").OrderBy(m => m.Descripcion), "Id", "Descripcion", sujetoObligado.IdTipoSujetoObligado);
            return View(sujetoObligado);
        }

        // GET: SujetosObligados/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Security.LoginService.Current.GrupoNombre != Models.Rol.CARGAR.ToString())
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
            ViewBag.IdTipoSujetoObligado = new SelectList(db.TiposSujetoObligado.Where(d => d.EstaHabilitado && d.Descripcion != "Denunciante").OrderBy(m => m.Descripcion), "Id", "Descripcion", sujetoObligado.IdTipoSujetoObligado);
            return View(sujetoObligado);
        }

        // POST: SujetosObligados/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CuilCuit,Nombre,Apellido,RazonSocial,IdTipoSujetoObligado, EstaHabilitado,EstaActivo,FechaModificacion,IdUsuarioModificacion, IdOriginal")] SujetoObligado sujetoObligado)
        {
            if (Security.LoginService.Current.GrupoNombre != Models.Rol.CARGAR.ToString())
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
                sujetoObligado.IdUsuarioModificacion =  Security.LoginService.Current.UsuarioID;
                sujetoObligado.FechaModificacion = DateTime.Now;

                db.Entry(sujetoObligado).State = EntityState.Modified;

                db.SujetosObligadosLog.Add(sujetoObligadoLog);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdTipoSujetoObligado = new SelectList(db.TiposSujetoObligado.Where(d => d.EstaHabilitado && d.Descripcion != "Denunciante").OrderBy(m => m.Descripcion), "Id", "Descripcion", sujetoObligado.IdTipoSujetoObligado);
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
