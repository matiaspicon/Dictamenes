using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Dictamenes.Database;
using Dictamenes.Models;

namespace Dictamenes.Controllers
{
    public class DictamenesController : Controller
    {
        private DictamenesDbContext db = new DictamenesDbContext();

        // GET: Dictamen
        public ActionResult Index()
        {
            var dictamenes = db.Dictamenes.Where(m => m.EstaActivo && !m.Borrado).Include(d => d.Asunto).Include(d => d.TipoDictamen);
            ViewData["IdAsunto"] = new SelectList(db.Asuntos.Where(m => m.EstaActivo && m.EstaHabilitado), "Id", "Descripcion");
            ViewData["IdSujetoObligado"] = new SelectList(db.SujetosObligados.Where(m => m.EstaActivo && m.RazonSocial != null), "Id", "RazonSocial");
            ViewData["IdTipoDictamen"] = new SelectList(db.TiposDictamen.Where(m => m.EstaActivo && m.EstaHabilitado), "Id", "Descripcion");
            ViewData["TipoSujetoObligado"] = new SelectList(db.TiposSujetoObligado.Where(m => m.EstaActivo && m.EstaHabilitado && m.Descripcion != "Denunciante"), "Id", "Descripcion");
            return View(dictamenes.ToList());
        }

        // GET: Dictamen/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dictamen dictamen = db.Dictamenes.Include(d => d.ArchivoPDF).Include(d => d.Asunto).Include(d => d.SujetoObligado).Include(d => d.TipoDictamen).Include(d => d.UsuarioModificacion).FirstOrDefault(d => d.Id == id);
            if (dictamen == null)
            {
                return HttpNotFound();
            }

            

            ViewData["IdDenunciante"] = db.TiposSujetoObligado.FirstOrDefault(m =>m.EstaActivo && m.Descripcion == "Denunciante").Id;
            return View(dictamen);
        }

        public async Task<ActionResult> DownloadFileFromFileSystem(int id)
        {

            var file = await db.ArchivosPDF.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (file == null) return null;
            var memory = new MemoryStream();
            using (var stream = new FileStream(file.Path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, file.TipoArchivo, file.Nombre + file.Extension);
        }


        // GET: Dictamenes/Create
        public ActionResult CargarFile()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CargarFile( HttpPostedFileBase file)
        {
            Dictamen dictamen = new Dictamen();
            //separo informacion del archivo
            var fileName = Guid.NewGuid().ToString();
            var extension = Path.GetExtension(file.FileName);
            // compruebo directorio
            var basePath = Server.MapPath("~/Files");
            var filePath = Path.Combine(basePath, fileName + extension);
            
            bool basePathExists = System.IO.Directory.Exists(basePath);
            if (!basePathExists) Directory.CreateDirectory(basePath);
            
            if (!System.IO.File.Exists(filePath))
            {
                file.SaveAs(filePath);
            }
            // creo el archivo para la base de datos
            var archivo = new ArchivoPDF
            {
                FechaCarga = DateTime.Now,
                TipoArchivo = file.ContentType,
                Extension = extension,
                Nombre = fileName,
                Path = filePath,
                Contenido = FileController.ExtractTextFromPdf(filePath),
            };
            db.ArchivosPDF.Add(archivo);
            db.SaveChanges();
            // extraigo la informacion del PDF del dictamen y creo el objeto con la misma
            dictamen = ExtratDictamenFromString(archivo.Contenido);
            dictamen.IdArchivoPDF = archivo.Id;
            dictamen.ArchivoPDF = archivo;

            // cargo la informacion para el formulario Create y devuelvo la VIEW del create con la informacion precargada
            // o sin la informacion precargada si no se pudo obtener nada del PDF
            ViewData["IdAsunto"] = new SelectList(db.Asuntos.Where(m => m.EstaActivo && m.EstaHabilitado), "Id", "Descripcion");
            ViewData["IdSujetoObligado"] = new SelectList(db.SujetosObligados.Where(m => m.EstaActivo && m.RazonSocial != null), "Id", "RazonSocial");
            ViewData["IdTipoDictamen"] = new SelectList(db.TiposDictamen.Where(m => m.EstaActivo && m.EstaHabilitado), "Id", "Descripcion");
            ViewData["TipoSujetoObligado"] = new SelectList(db.TiposSujetoObligado.Where(m => m.EstaActivo && m.EstaHabilitado), "Id", "Descripcion");
            return View("Create", dictamen);
        }


        // GET: Dictamen/Create
        public ActionResult Create()
        {
            ViewData["IdAsunto"] = new SelectList(db.Asuntos.Where(m => m.EstaActivo && m.EstaHabilitado), "Id", "Descripcion");
            ViewData["IdSujetoObligado"] = new SelectList(db.SujetosObligados.Where(m => m.EstaActivo && m.RazonSocial != null), "Id", "RazonSocial");
            ViewData["IdTipoDictamen"] = new SelectList(db.TiposDictamen.Where(m => m.EstaActivo && m.EstaHabilitado), "Id", "Descripcion");
            ViewData["TipoSujetoObligado"] = new SelectList(db.TiposSujetoObligado.Where(m => m.EstaActivo && m.EstaHabilitado), "Id", "Descripcion");
            return View();
        }


        // POST: Dictamen/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,NroGDE,NroExpediente,FechaCarga,Detalle,EsPublico,IdArchivoPDF,IdSujetoObligado,IdAsunto,IdTipoDictamen,Borrado,EstaActivo,FechaModificacion,IdUsuarioModificacion")] Dictamen dictamen, SujetoObligado sujetoObligado)
        {
            dictamen.IdUsuarioModificacion = 0;
            //dictamen.IdUsuarioModificacion = db.Usuario;
            dictamen.FechaModificacion = DateTime.Now;
            dictamen.EstaActivo = true;
            dictamen.Borrado = false;
            dictamen.NroGDE = dictamen.NroGDE.ToUpper();
            dictamen.NroExpediente = dictamen.NroExpediente.ToUpper();
            dictamen.Detalle = dictamen.Detalle != null ? dictamen.Detalle.ToUpper() : ".";

            if (sujetoObligado.CuilCuit > 0)
            {
                sujetoObligado.IdUsuarioModificacion = 0;
                sujetoObligado.FechaModificacion = DateTime.Now;
                sujetoObligado.EstaActivo = true;
                sujetoObligado.IdTipoSujetoObligado = db.TiposSujetoObligado.First(m => m.Descripcion == "Denunciante").Id;
                db.SujetosObligados.Add(sujetoObligado);
                db.SaveChanges();
                dictamen.IdSujetoObligado = sujetoObligado.Id;
            }

            if (ModelState.IsValid)
            {
                db.Dictamenes.Add(dictamen);
                db.SaveChanges();
                return RedirectToAction("Index");
            }


            ViewBag.IdAsunto = new SelectList(db.Asuntos.Where(m => m.EstaActivo && m.EstaHabilitado), "Id", "Descripcion", dictamen.IdAsunto);
            ViewBag.IdSujetoObligado = new SelectList(db.SujetosObligados.Where(m => m.EstaActivo && m.RazonSocial != null), "Id", "Razon Social", dictamen.IdSujetoObligado);
            ViewBag.IdTipoDictamen = new SelectList(db.TiposDictamen.Where(m => m.EstaActivo && m.EstaHabilitado), "Id", "Descripcion", dictamen.IdTipoDictamen);
            ViewBag.IdUsuarioModificacion = new SelectList(db.Usuarios, "Id", "Nombre", dictamen.IdUsuarioModificacion);
            return View(dictamen);
        }

        [HttpPost]
        public async Task<ActionResult> Buscar(Busqueda busqueda)
        {
            var dictDB = db.Dictamenes
                .Where(m => m.EstaActivo && !m.Borrado);

            if (busqueda.NroGDE != null)
            {
                dictDB = dictDB.Where(d => d.NroGDE.Contains(busqueda.NroGDE.ToUpper()));
            }
            if (busqueda.NroExp != null)
            {
                dictDB = dictDB.Where(d => d.NroExpediente.Contains(busqueda.NroExp.ToUpper()));
            }
            if (busqueda.Detalle != null)
            {
                dictDB = dictDB.Where(d => d.Detalle.Contains(busqueda.Detalle.ToUpper()));
            }
            if (busqueda.FechaCargaInicio != null)
            {
                dictDB = dictDB.Where(d => d.FechaCarga >= busqueda.FechaCargaInicio);
            }
            if (busqueda.FechaCargaFinal != null)
            {
                dictDB = dictDB.Where(d => d.FechaCarga <= busqueda.FechaCargaFinal);
            }

            if (busqueda.IdAsunto != null)
            {
                dictDB = dictDB.Where(d => d.IdAsunto == busqueda.IdAsunto);
            }
            if (busqueda.IdTipoDictamen != null)
            {
                dictDB = dictDB.Where(d => d.IdTipoDictamen == busqueda.IdTipoDictamen);
            }
            if (busqueda.Contenido != null)
            {
                dictDB = dictDB.Include(d => d.ArchivoPDF).Where(d => d.ArchivoPDF.Contenido.Contains(busqueda.Contenido));
            }

            if (busqueda.CuilCuit > 0)
            {
                dictDB = dictDB.Include(d => d.SujetoObligado).Where(d => d.SujetoObligado.CuilCuit == busqueda.CuilCuit);
            }
            if (busqueda.EsDenunciante)
            {

                dictDB = dictDB.Include(d => d.SujetoObligado).Include(s => s.SujetoObligado.TipoSujetoObligado).Where(d => d.SujetoObligado.TipoSujetoObligado.Descripcion == "Denunciante");

                if (busqueda.Nombre != null)
                {
                    dictDB = dictDB.Include(d => d.SujetoObligado).Where(d => d.SujetoObligado.Nombre == busqueda.Nombre);
                }

                if (busqueda.Apellido != null)
                {
                    dictDB = dictDB.Include(d => d.SujetoObligado).Where(d => d.SujetoObligado.Apellido == busqueda.Apellido);
                }
            }
            else
            {
                if (busqueda.IdTipoSujetoObligado != null)
                {
                    dictDB = dictDB.Include(d => d.SujetoObligado).Where(d => d.SujetoObligado.IdTipoSujetoObligado == busqueda.IdTipoSujetoObligado);
                }
                if (busqueda.IdSujetoObligado != null)
                {
                    dictDB = dictDB.Where(d => d.IdSujetoObligado == busqueda.IdSujetoObligado);
                }
            }


            ViewData["IdAsunto"] = new SelectList(db.Asuntos.Where(m => m.EstaActivo && m.EstaHabilitado), "Id", "Descripcion", "Seleccione uno");
            ViewData["IdSujetoObligado"] = new SelectList(db.SujetosObligados.Where(m => m.EstaActivo && m.RazonSocial != null), "Id", "RazonSocial");
            ViewData["IdTipoDictamen"] = new SelectList(db.TiposDictamen.Where(m => m.EstaActivo && m.EstaHabilitado), "Id", "Descripcion");
            ViewData["TipoSujetoObligado"] = new SelectList(db.TiposSujetoObligado.Where(m => m.EstaActivo && m.EstaHabilitado), "Id", "Descripcion");
            ViewData["Busqueda"] = busqueda;
            return View("Index", dictDB.ToList());
        }


        // GET: Dictamen/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dictamen dictamen = db.Dictamenes.Find(id);
            if (dictamen == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdArchivoPDF = new SelectList(db.ArchivosPDF, "Id", "Nombre", dictamen.IdArchivoPDF);
            ViewBag.IdAsunto = new SelectList(db.Asuntos, "Id", "Descripcion", dictamen.IdAsunto);
            ViewBag.IdSujetoObligado = new SelectList(db.SujetosObligados, "Id", "Nombre", dictamen.IdSujetoObligado);
            ViewBag.IdTipoDictamen = new SelectList(db.TiposDictamen, "Id", "Descripcion", dictamen.IdTipoDictamen);
            ViewBag.IdUsuarioModificacion = new SelectList(db.Usuarios, "Id", "Nombre", dictamen.IdUsuarioModificacion);
            return View(dictamen);
        }

        // POST: Dictamen/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,NroGDE,NroExpediente,FechaCarga,Detalle,EsPublico,IdArchivoPDF,IdSujetoObligado,IdAsunto,IdTipoDictamen,Borrado,EstaActivo,FechaModificacion,IdUsuarioModificacion")] Dictamen dictamen, SujetoObligado sujetoObligado , HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                    Dictamen dictamenViejo = db.Dictamenes.AsNoTracking().First(d => d.Id == dictamen.Id);

                    dictamen.IdUsuarioModificacion = 3;
                    //dictamen.IdUsuarioModificacion = _context.Usuario;
                    dictamen.EstaActivo = true;
                    dictamen.FechaModificacion = DateTime.Now;
                    dictamen.NroGDE = dictamen.NroGDE.ToUpper();
                    dictamen.NroExpediente = dictamen.NroExpediente.ToUpper();
                    dictamen.Detalle = dictamen.Detalle.ToUpper();
                    //if (borrarArchivo)
                    //{
                    //    dictamen.IdArchivoPDF = null;
                    //    dictamen.ArchivoPDF = null;
                    //}
                    //else
                    //{
                    if (file != null)
                    {

                        var fileName = Guid.NewGuid().ToString();
                        var extension = Path.GetExtension(file.FileName);
                        // compruebo directorio
                        var basePath = Server.MapPath("~/Files");
                        var filePath = Path.Combine(basePath, fileName + extension);

                        bool basePathExists = System.IO.Directory.Exists(basePath);
                        if (!basePathExists) Directory.CreateDirectory(basePath);

                        if (!System.IO.File.Exists(filePath))
                        {
                            file.SaveAs(filePath);
                        }
                        // creo el archivo para la base de datos
                        var archivo = new ArchivoPDF
                        {
                            FechaCarga = DateTime.Now,
                            TipoArchivo = file.ContentType,
                            Extension = extension,
                            Nombre = fileName,
                            Path = filePath,
                            Contenido = FileController.ExtractTextFromPdf(filePath),
                        };
                        db.ArchivosPDF.Add(archivo);
                        db.SaveChanges();
                        dictamen.IdArchivoPDF = archivo.Id;

                    } 
                    //}

                    db.Entry(dictamen).State = EntityState.Modified;

                    dictamenViejo.EstaActivo = false;
                    dictamenViejo.Id = 0;

                    db.Dictamenes.Add(dictamenViejo);
                    db.SaveChanges();
                    return RedirectToAction("Index");
            }
            ViewBag.IdArchivoPDF = new SelectList(db.ArchivosPDF, "Id", "Nombre", dictamen.IdArchivoPDF);
            ViewBag.IdAsunto = new SelectList(db.Asuntos, "Id", "Descripcion", dictamen.IdAsunto);
            ViewBag.IdSujetoObligado = new SelectList(db.SujetosObligados, "Id", "Nombre", dictamen.IdSujetoObligado);
            ViewBag.IdTipoDictamen = new SelectList(db.TiposDictamen, "Id", "Descripcion", dictamen.IdTipoDictamen);
            ViewBag.IdUsuarioModificacion = new SelectList(db.Usuarios, "Id", "Nombre", dictamen.IdUsuarioModificacion);
            return View(dictamen);
        }

        // GET: Dictamen/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dictamen dictamen = db.Dictamenes.Find(id);
            if (dictamen == null)
            {
                return HttpNotFound();
            }
            return View(dictamen);
        }

        // POST: Dictamen/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Dictamen dictamen = db.Dictamenes.Find(id);
            db.Dictamenes.Remove(dictamen);
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

        private static Dictamen ExtratDictamenFromString(string contenido)
        {
            Dictamen dict = new Dictamen();

            Regex numeroGDE = new Regex("IF-[0-9]{4}-[0-9]+-APN-[A-Z]+#[A-Z]+", RegexOptions.IgnoreCase);

            MatchCollection matches = numeroGDE.Matches(contenido);
            try
            {
                dict.NroGDE = matches[0].Value;
            }
            catch
            {

            }

            Regex numeroExpediente = new Regex("[E][X] *-* *[0-9]{4} *- *[0-9]+? ?- ?-? ?APN *- *[A-Z]+ *# *[A-Z]+|[E][X] *-* *[0-9]{4} *- *[0-9]+", RegexOptions.IgnoreCase);

            matches = numeroExpediente.Matches(contenido);

            try
            {
                dict.NroExpediente = matches[0].Value.Replace(" ", "").Replace("--", "-");
            }
            catch
            {

            }


            Regex date = new Regex("[0-9]{4}.[0-9]{2}.[0-9]{2} [0-9]{2}:[0-9]{2}:[0-9]{2}", RegexOptions.IgnoreCase);

            matches = date.Matches(contenido);
            try
            {
                dict.FechaCarga = DateTime.ParseExact(matches[0].Value, "yyyy.MM.dd HH:mm:ss", null);
            }
            catch
            {

            }
            return dict;


        }
    }
}
