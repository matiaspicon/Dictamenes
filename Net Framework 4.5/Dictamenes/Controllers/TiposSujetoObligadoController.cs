﻿using System;
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
            var tipoSujetoObligado = db.TiposSujetoObligado.Where(d =>  d.EstaHabilitado);
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
                TipoSujetoObligado tipoSujetoObligadoViejo = db.TiposSujetoObligado.AsNoTracking().First(d => d.Id == tipoSujetoObligado.Id);

                TipoSujetoObligadoLog tipoSujetoObligadoLog = new TipoSujetoObligadoLog
                {
                    IdOriginal = tipoSujetoObligadoViejo.Id,
                    Descripcion = tipoSujetoObligadoViejo.Descripcion,
                    EstaHabilitado = tipoSujetoObligadoViejo.EstaHabilitado,
                    FechaModificacion = DateTime.Now,
                    IdUsuarioModificacion = 3
                };

                db.Entry(tipoSujetoObligado).State = EntityState.Modified;

                db.TiposSujetoObligadoLog.Add(tipoSujetoObligadoLog);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
        return View(tipoSujetoObligado);
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
