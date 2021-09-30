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
            string[] asuntos = {"ADHESION PLAN/RAMO",
                         "ALTA / INSCRIPCIÓN REGISTRO PAS",
                         "ALTA REASEGURADORA EXTRANJERA ADMITIDA",
                         "ALTA REGISTRO AGENTES INSTITORIOS",
                         "ALTA REGISTRO DE AUDITORES",
                         "ALTA/ INSCRIPCION REGISTRO DE ACTUARIOS",
                         "ALTA/INSCRIPCION REGISTRO LIQUIDADORES DE SINIESTROS Y AVERIAS",
                         "AMPLIACION DE CUPO EVALUACIÓN",
                         "ASIGNACION DE FUNCIONES",
                         "ASIGNACION UNIDADES RETRIBUTIVAS",
                         "AUMENTO DE CAPITAL",
                         "AUTORIZACION DE PLAN/ RAMO",
                         "AUTORIZACION PARA OPERAR",
                         "BAJA  AUTORIZACION PARA OPERAR",
                         "BAJA  INSCRIPCION REGISTRO LIQUIDADORES DE SINIESTROS Y AVERIAS",
                         "BAJA FUNCIONARIO",
                         "BAJA PLAN/RAMO",
                         "BAJA REGISTRO DE AGENTES INSTITORIOS",
                         "BAJA REGISTRO DE AUDITORES",
                         "BAJA REGISTRO PAS",
                         "BONIFICACION DESEMPEÑO DESTACADO",
                         "CAMBIO DE AGRUPAMIENTO",
                         "CAMBIO DE DENOMINACION",
                         "CAMBIO DE GRADO",
                         "CESION DE CARTERA",
                         "COMPENSACION TRANSITORIA",
                         "CONSTITUCION - ESTATUTOS",
                         "CONTRATACION",
                         "CONTRATACION DIRECTA",
                         "DELEGACION DE FIRMA",
                         "DESIGNACION FUNCIONARIO",
                         "FONDO DE RESERVA",
                         "FONDO ROTATORIO",
                         "FUCION",
                         "GERENCIAMIENTO",
                         "INCUMPLIMIENTO  PAS",
                         "INCUMPLIMIENTO AL RGAA",
                         "INHABILITACION PAS",
                         "INSCRIPCION Registro de Entidades Especializadas en Cobranzas",
                         "INSCRIPCION Registro de Sociedades y Asociaciones de Graduados en Ciencias Económicas",
                         "INSCRIPCION Registro Especial Seguro Colectivo de Vida Obligatorio para Trabajadores Rurales",
                         "LEVANTAMIENTO ",
                         "LEVANTAMIENTO INHABILITACION PAS",
                         "LEVANTAMIENTO PARCIAL",
                         "LICITACION PRIVADA",
                         "LICITACION PUBLICA",
                         "MEDIDAS CAUTELARES",
                         "MODIFICACION CLAUSULA PLAN/RAMO",
                         "MODIFICACION RESOLUCION",
                         "MODIFICACION RGAA ",
                         "OBSERVACIONES- AJUSTES A EECC",
                         "OPERAR SIN AUTORIZACION",
                         "PLAN DE RECONVERSION",
                         "PLAN DE REGULARIZACIÓN ",
                         "PRESENTACION EECC",
                         "PROCESO EVALUACION DE DESEMPEÑO",
                         "PROMOCION DE GRADO",
                         "PROMOCION DE TRAMO",
                         "RECATEGORIZACION",
                         "RECURSO DE APELACION",
                         "RECURSO DE RECONSIDERACION",
                         "REFORMA DE ESTATUTO",
                         "REFORMA NORMATIVA INTERNA",
                         "REGIMEN DE ALICUOTAS",
                         "REGIMEN DE INVERSIONES",
                         "REHABILITACIÓN MATRÍCULA PAS",
                         "RENOVACION CONTRATACION EMPLEO PUBLICO",
                         "RENUNCIA FUNCIONARIO",
                         "RESERVA DE SINIESTROS PENDIENTES",
                         "REVOCATORIA PARA OPERAR",
                         "SERVICIOS EXTRAORDINARIOS",
                         "SUPLEMENTO FUNCION ESPECIFICA",
                         "VERIFICACION DE EECC",
                         "VIAJE OFICIAL" };

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
                    db.SaveChanges();
                }
            }

            return new HttpStatusCodeResult(HttpStatusCode.OK);
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
