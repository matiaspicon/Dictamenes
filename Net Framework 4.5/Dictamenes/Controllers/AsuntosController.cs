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

            if (LoginController.GetUserRolIdentity(User.Identity) != Models.Rol.CARGAR.ToString())
            {
                return RedirectToAction("ErrorNoPermisos", "Login");
            }
            var asunto = db.Asuntos;
            return View(asunto.ToList());
        }

        // GET: Asuntos/Create
        [Authorize]
        public ActionResult Create()
        {
            if (LoginController.GetUserRolIdentity(User.Identity) != Models.Rol.CARGAR.ToString())
            {
                return RedirectToAction("ErrorNoPermisos", "Login");
            }
            return View();
        }

        // POST: Asuntos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Descripcion,EstaHabilitado,EstaActivo,FechaModificacion,IdUsuarioModificacion")] Asunto asunto)
        {
            if (LoginController.GetUserRolIdentity(User.Identity) != Models.Rol.CARGAR.ToString())
            {
                return RedirectToAction("ErrorNoPermisos", "Login");
            }
            asunto.FechaModificacion = DateTime.Now;
            asunto.IdUsuarioModificacion = LoginController.GetUserDataIdentity(User.Identity).Id;
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
            if (LoginController.GetUserRolIdentity(User.Identity) != Models.Rol.CARGAR.ToString())
            {
                return RedirectToAction("ErrorNoPermisos", "Login");
            }
            return View();
        }


        [HttpPost]
        public ActionResult CargarAsuntos(string JSONAsuntos)
        {

            if (LoginController.GetUserRolIdentity(User.Identity) != Models.Rol.CARGAR.ToString())
            {
                return RedirectToAction("ErrorNoPermisos", "Login");
            }

            var asuntos = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(JSONAsuntos);

            foreach (string a in asuntos) 
            {                
                Asunto asunto = new Asunto
                {
                    EstaHabilitado = true,
                    Descripcion = a,
                    FechaModificacion = DateTime.Now,
                    IdUsuarioModificacion = LoginController.GetUserDataIdentity(User.Identity).Id
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
            if (LoginController.GetUserRolIdentity(User.Identity) != Models.Rol.CARGAR.ToString())
            {
                return RedirectToAction("ErrorNoPermisos", "Login");
            }
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
            if (LoginController.GetUserRolIdentity(User.Identity) != Models.Rol.CARGAR.ToString())
            {
                return RedirectToAction("ErrorNoPermisos", "Login");
            }


            if (ModelState.IsValid)
            {
               Asunto asuntoViejo = db.Asuntos.AsNoTracking().First(d => d.Id == asunto.Id);
               AsuntoLog asuntoLog = new AsuntoLog
               {
                    IdOriginal = asuntoViejo.Id,
                    Descripcion = asuntoViejo.Descripcion,
                    EstaHabilitado = asuntoViejo.EstaHabilitado,
                    FechaModificacion = asuntoViejo.FechaModificacion,
                    IdUsuarioModificacion = asuntoViejo.IdUsuarioModificacion
               };

                asunto.IdUsuarioModificacion = LoginController.GetUserDataIdentity(User.Identity).Id;
                asunto.FechaModificacion = DateTime.Now;

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
