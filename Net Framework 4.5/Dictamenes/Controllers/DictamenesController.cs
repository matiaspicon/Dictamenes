using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Globalization;
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
    [Authorize]
    public class DictamenesController : Controller
    {
        private DictamenesDbContext db = new DictamenesDbContext();

        // GET: Dictamen
        public ActionResult Index()
        {
            var dictamenes = db.Dictamenes.Include(d => d.Asunto).Include(d => d.TipoDictamen);
            ViewData["IdAsunto"] = new SelectList(db.Asuntos.Where(m => m.EstaHabilitado), "Id", "Descripcion");
            ViewData["IdSujetoObligado"] = new SelectList(db.SujetosObligados.Where(m => m.RazonSocial != null), "Id", "RazonSocial");
            ViewData["IdTipoDictamen"] = new SelectList(db.TiposDictamen.Where(m => m.EstaHabilitado), "Id", "Descripcion");
            ViewData["TipoSujetoObligado"] = new SelectList(db.TiposSujetoObligado.Where(m => m.EstaHabilitado && m.Descripcion != "Denunciante"), "Id", "Descripcion");
            return View(dictamenes.ToList());
        }

        // GET: Dictamen/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dictamen dictamen = db.Dictamenes.Include(d => d.ArchivoPDF).Include(d => d.Asunto).Include(d => d.SujetoObligado).Include(d => d.TipoDictamen).FirstOrDefault(d => d.Id == id);
            if (dictamen == null)
            {
                dictamen = db.Dictamenes.Include(d => d.Asunto).Include(d => d.SujetoObligado).Include(d => d.TipoDictamen).FirstOrDefault(d => d.Id == id);
                if (dictamen == null)
                {
                    return HttpNotFound();
                }
            }

            ViewData["IdDenunciante"] = db.TiposSujetoObligado.FirstOrDefault(m => m.Descripcion == "Denunciante").Id;
            return View(dictamen);
        }


        // GET: Dictamenes/Create
        public ActionResult CargarFile()
        {
            if((string)Session["rol"] == "CARGAR" || (string)Session["rol"] == "EDITAR_CAMPOS")
            {
                return View();
            }
            return RedirectToAction("ErrorNoPermisos", "Login");
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
            var filePath = Path.Combine("~","Files", fileName + extension);
            
            bool basePathExists = System.IO.Directory.Exists(basePath);
            if (!basePathExists) Directory.CreateDirectory(basePath);
            
            if (!System.IO.File.Exists(Server.MapPath(filePath)))
            {
                file.SaveAs(Server.MapPath(filePath));
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
            ViewData["IdAsunto"] = new SelectList(db.Asuntos.Where(m => m.EstaHabilitado), "Id", "Descripcion");
            ViewData["IdSujetoObligado"] = new SelectList(db.SujetosObligados.Where(m => m.RazonSocial != null), "Id", "RazonSocial");
            ViewData["IdTipoDictamen"] = new SelectList(db.TiposDictamen.Where(m => m.EstaHabilitado), "Id", "Descripcion");
            ViewData["TipoSujetoObligado"] = new SelectList(db.TiposSujetoObligado.Where(m => m.EstaHabilitado), "Id", "Descripcion");
            return View("Create", dictamen);
        }


        // GET: Dictamen/Create
        public ActionResult Create()
        {
            ViewData["IdAsunto"] = new SelectList(db.Asuntos.Where(m => m.EstaHabilitado), "Id", "Descripcion");
            ViewData["IdSujetoObligado"] = new SelectList(db.SujetosObligados.Where(m => m.RazonSocial != null), "Id", "RazonSocial");
            ViewData["IdTipoDictamen"] = new SelectList(db.TiposDictamen.Where(m => m.EstaHabilitado), "Id", "Descripcion");
            ViewData["TipoSujetoObligado"] = new SelectList(db.TiposSujetoObligado.Where(m => m.EstaHabilitado), "Id", "Descripcion");
            return View();
        }


        // POST: Dictamen/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,NroGDE,NroExpediente,FechaCarga,Detalle,EsPublico,IdArchivoPDF,IdSujetoObligado,IdAsunto,IdTipoDictamen,Borrado,EstaActivo,FechaModificacion,IdUsuarioModificacion")] Dictamen dictamen, SujetoObligado sujetoObligado)
        {
            dictamen.NroGDE = dictamen.NroGDE.ToUpper();
            dictamen.NroExpediente = dictamen.NroExpediente.ToUpper();
            dictamen.Detalle = dictamen.Detalle != null ? dictamen.Detalle.ToUpper() : ".";

            if (sujetoObligado.CuilCuit > 0)
            {                
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


            ViewBag.IdAsunto = new SelectList(db.Asuntos.Where(m => m.EstaHabilitado), "Id", "Descripcion", dictamen.IdAsunto);
            ViewBag.IdSujetoObligado = new SelectList(db.SujetosObligados.Where(m => m.RazonSocial != null), "Id", "Razon Social", dictamen.IdSujetoObligado);
            ViewBag.IdTipoDictamen = new SelectList(db.TiposDictamen.Where(m => m.EstaHabilitado), "Id", "Descripcion", dictamen.IdTipoDictamen);
            return View(dictamen);
        }

        [HttpPost]
        public ActionResult Buscar( Busqueda busqueda)
        {
            var dictDB = db.Dictamenes.Include(d => d.SujetoObligado);

            var algo = db.Sp_FiltrarDictamenes(busqueda.NroGDE, busqueda.NroExp, busqueda.FechaCargaInicio, busqueda.FechaCargaInicio, busqueda.Detalle, busqueda.Contenido, busqueda.IdAsunto, busqueda.IdTipoDictamen, busqueda.IdSujetoObligado, busqueda.CuilCuit, busqueda.Nombre, busqueda.Apellido);

            //if (busqueda.NroGDE != null)
            //{
            //    dictDB = dictDB.Where(d => d.NroGDE.Contains(busqueda.NroGDE.ToUpper()));
            //}
            //if (busqueda.NroExp != null)
            //{
            //    dictDB = dictDB.Where(d => d.NroExpediente.Contains(busqueda.NroExp.ToUpper()));
            //}
            //if (busqueda.Detalle != null)
            //{
            //    dictDB = dictDB.Where(d => d.Detalle.Contains(busqueda.Detalle.ToUpper()));
            //}
            //if (busqueda.FechaCargaInicio != null)
            //{
            //    dictDB = dictDB.Where(d => d.FechaCarga >= busqueda.FechaCargaInicio);
            //}
            //if (busqueda.FechaCargaFinal != null)
            //{
            //    dictDB = dictDB.Where(d => d.FechaCarga <= busqueda.FechaCargaFinal);
            //}

            //if (busqueda.IdAsunto != null)
            //{
            //    dictDB = dictDB.Where(d => d.IdAsunto == busqueda.IdAsunto);
            //}
            //if (busqueda.IdTipoDictamen != null)
            //{
            //    dictDB = dictDB.Where(d => d.IdTipoDictamen == busqueda.IdTipoDictamen);
            //}
            //if (busqueda.Contenido != null)
            //{
            //    dictDB = dictDB.Include(d => d.ArchivoPDF).Where(d => d.ArchivoPDF.Contenido.Contains(busqueda.Contenido));
            //}

            //if (busqueda.CuilCuit > 0)
            //{
            //    dictDB = dictDB.Where(d => d.SujetoObligado.CuilCuit == busqueda.CuilCuit);
            //}
            //if (busqueda.EsDenunciante)
            //{

            //    dictDB = dictDB.Include(s => s.SujetoObligado.TipoSujetoObligado).Where(d => d.SujetoObligado.TipoSujetoObligado.Descripcion == "Denunciante");

            //    if (busqueda.Nombre != null)
            //    {
            //        dictDB = dictDB.Where(d => d.SujetoObligado.Nombre == busqueda.Nombre);
            //    }

            //    if (busqueda.Apellido != null)
            //    {
            //        dictDB = dictDB.Where(d => d.SujetoObligado.Apellido == busqueda.Apellido);
            //    }
            //}
            //else
            //{
            //    if (busqueda.IdTipoSujetoObligado != null)
            //    {
            //        dictDB = dictDB.Where(d => d.SujetoObligado.IdTipoSujetoObligado == busqueda.IdTipoSujetoObligado);
            //    }
            //    if (busqueda.IdSujetoObligado != null)
            //    {
            //        dictDB = dictDB.Where(d => d.IdSujetoObligado == busqueda.IdSujetoObligado);
            //    }
            //}

            ViewData["IdAsunto"] = new SelectList(db.Asuntos.Where(m => m.EstaHabilitado), "Id", "Descripcion");
            ViewData["IdSujetoObligado"] = new SelectList(db.SujetosObligados.Where(m => m.RazonSocial != null), "Id", "RazonSocial");
            ViewData["IdTipoDictamen"] = new SelectList(db.TiposDictamen.Where(m => m.EstaHabilitado), "Id", "Descripcion");
            ViewData["TipoSujetoObligado"] = new SelectList(db.TiposSujetoObligado.Where(m => m.EstaHabilitado && m.Descripcion != "Denunciante"), "Id", "Descripcion");
            ViewData["Busqueda"] = busqueda;
            return View("Index", algo.ToList());
        }


        // GET: Dictamen/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dictamen dictamen = db.Dictamenes.Include(d => d.SujetoObligado).FirstOrDefault(d => d.Id == id);
            if (dictamen == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdAsunto = new SelectList(db.Asuntos.Where(m => m.EstaHabilitado), "Id", "Descripcion", dictamen.IdAsunto);
            ViewBag.IdSujetoObligado = new SelectList(db.SujetosObligados.Where(m => m.RazonSocial != null), "Id", "RazonSocial", dictamen.IdSujetoObligado);
            ViewBag.IdTipoDictamen = new SelectList(db.TiposDictamen.Where(m => m.EstaHabilitado), "Id", "Descripcion", dictamen.IdTipoDictamen);

            return View(dictamen);
        }

        // POST: Dictamen/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,NroGDE,NroExpediente,FechaCarga,Detalle,EsPublico,IdArchivoPDF,IdSujetoObligado,IdAsunto,IdTipoDictamen,Borrado,EstaActivo,FechaModificacion,IdUsuarioModificacion,IdOriginal")] Dictamen dictamen, SujetoObligado sujetoObligado , HttpPostedFileBase file)
        {
            if (dictamen.IdSujetoObligado.HasValue)
            {
                sujetoObligado.Id = dictamen.IdSujetoObligado.Value;
                dictamen.SujetoObligado = sujetoObligado;

            }
            if (ModelState.IsValid)
            {
                Dictamen dictamenViejo = db.Dictamenes.Include(d => d.SujetoObligado).AsNoTracking().First(d => d.Id == dictamen.Id);

                DictamenLog dictamenLog = new DictamenLog
                {
                    IdOriginal = dictamenViejo.Id,
                    NroGDE = dictamenViejo.NroGDE,
                    NroExpediente = dictamenViejo.NroExpediente,
                    Detalle = dictamenViejo.Detalle,
                    EsPublico = dictamenViejo.EsPublico,
                    IdArchivoPDF = dictamenViejo.IdArchivoPDF,
                    IdSujetoObligado = dictamenViejo.IdSujetoObligado,
                    IdAsunto = dictamenViejo.IdAsunto,
                    IdTipoDictamen = dictamenViejo.IdTipoDictamen,
                    FechaCarga = dictamenViejo.FechaCarga,
                    FechaModificacion = DateTime.Now,
                    Borrado = false,
                    IdUsuarioModificacion = 3                    
                };

                dictamen.NroGDE = dictamen.NroGDE.ToUpper();
                dictamen.NroExpediente = dictamen.NroExpediente.ToUpper();
                dictamen.Detalle = dictamen.Detalle.ToUpper();

                if (file != null)
                    {

                    var fileName = Guid.NewGuid().ToString();
                    var extension = Path.GetExtension(file.FileName);
                    // compruebo directorio
                    var basePath = Server.MapPath("~/Files");
                    var filePath = Path.Combine("~", "Files", fileName + extension);

                    bool basePathExists = System.IO.Directory.Exists(basePath);
                    if (!basePathExists) Directory.CreateDirectory(basePath);

                    if (!System.IO.File.Exists(Server.MapPath(filePath)))
                    {
                        file.SaveAs(Server.MapPath(filePath));
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

                if ((sujetoObligado.CuilCuit > 0 && sujetoObligado.CuilCuit != dictamenViejo.SujetoObligado.CuilCuit) || sujetoObligado.Nombre != dictamenViejo.SujetoObligado.Nombre || sujetoObligado.Apellido != dictamenViejo.SujetoObligado.Apellido)

                {
                    SujetoObligado sujetoObligadoViejo = db.SujetosObligados.AsNoTracking().FirstOrDefault(d => d.Id == sujetoObligado.Id);
                    if (sujetoObligadoViejo != null)
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
                            FechaModificacion = DateTime.Now,
                            IdUsuarioModificacion = 3
                        };
                        sujetoObligado.IdTipoSujetoObligado = sujetoObligadoViejo.IdTipoSujetoObligado;
                        sujetoObligado.EstaHabilitado = sujetoObligadoViejo.EstaHabilitado;
                        db.Entry(sujetoObligado).State = EntityState.Modified;
                        db.SujetosObligadosLog.Add(sujetoObligadoLog);
                        db.SaveChanges();
                    }
                    else
                    {
                        sujetoObligado.IdTipoSujetoObligado = db.TiposSujetoObligado.FirstOrDefault(d => d.Descripcion == "Denunciante").Id;
                        db.SujetosObligados.Add(sujetoObligado);
                        db.SaveChanges();
                        dictamen.IdSujetoObligado = sujetoObligado.Id;
                    }
                }


                db.Entry(dictamen).State = EntityState.Modified;
                db.DictamenesLog.Add(dictamenLog);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            ViewBag.IdAsunto = new SelectList(db.Asuntos.Where(m => m.EstaHabilitado), "Id", "Descripcion", dictamen.IdAsunto);
            ViewBag.IdSujetoObligado = new SelectList(db.SujetosObligados.Where(m => m.RazonSocial != null), "Id", "RazonSocial", dictamen.IdSujetoObligado);
            ViewBag.IdTipoDictamen = new SelectList(db.TiposDictamen.Where(m => m.EstaHabilitado), "Id", "Descripcion", dictamen.IdTipoDictamen);
            return View(dictamen);
        }

        // GET: Dictamen/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dictamen dictamen = db.Dictamenes.Include(d => d.ArchivoPDF).Include(d => d.Asunto).Include(d => d.SujetoObligado).Include(d => d.TipoDictamen).FirstOrDefault(d => d.Id == id);            if (dictamen == null)
            {
                return HttpNotFound();
            }
            ViewData["IdDenunciante"] = db.TiposSujetoObligado.FirstOrDefault(m => m.Descripcion == "Denunciante").Id;
            return View(dictamen);
        }

        // POST: Dictamen/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var dictamen =  db.Dictamenes.Find(id);

            Dictamen dictamenViejo = db.Dictamenes.AsNoTracking().First(d => d.Id == id);

            db.Dictamenes.Remove(dictamen);

            DictamenLog dictamenLog = new DictamenLog
            {
                IdOriginal = dictamenViejo.Id,
                NroGDE = dictamenViejo.NroGDE,
                NroExpediente = dictamenViejo.NroExpediente,
                Detalle = dictamenViejo.Detalle,
                EsPublico = dictamenViejo.EsPublico,
                IdArchivoPDF = dictamenViejo.IdArchivoPDF,
                IdSujetoObligado = dictamenViejo.IdSujetoObligado,
                IdAsunto = dictamenViejo.IdAsunto,
                IdTipoDictamen = dictamenViejo.IdTipoDictamen,
                FechaCarga = dictamenViejo.FechaCarga,
                FechaModificacion = DateTime.Now,
                Borrado = true,
                IdUsuarioModificacion = 3
            };

            db.DictamenesLog.Add(dictamenLog);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult CargarDictamenes()
        {
            return View();
        }


        class DictamenData
        {
            [RegularExpression("[iI][fF]-[0-9]{4}-[0-9]+-[aA][pP][nN]-[A-Za-z]+#[A-Za-z]+",
            ErrorMessage = "El Numero de GDE ingresado no es valido.")]
            [MaxLength(30, ErrorMessage = "{0} admite un máximo de {1} caracteres")]
            public string NroGDE { get; set; }

            [Required]
            [RegularExpression("[eE][xX]-[0-9]{4}-[0-9]+-[aA][pP][nN]-[A-Za-z]+#[A-Za-z]+",
             ErrorMessage = "El Numero de Expediente ingresado no es valido.")]
            [MaxLength(30, ErrorMessage = "{0} admite un máximo de {1} caracteres")]
            public string NroExpediente { get; set; }

            public string FechaCarga { get; set; }

            public string Detalle { get; set; }

            public string Asunto { get; set; }
        }

        [HttpPost]
        public ActionResult CargarDictamenes(string JSONDictamenes, HttpPostedFileBase[] files)
        {
            var dictamenes = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DictamenData>>(JSONDictamenes);

            foreach (DictamenData dictamenData in dictamenes)
            {
                Dictamen dictamen = new Dictamen
                {
                    NroGDE = dictamenData.NroGDE,
                    NroExpediente = dictamenData.NroExpediente,
                    FechaCarga = DateTime.Parse(dictamenData.FechaCarga, CultureInfo.InvariantCulture),
                    Detalle = dictamenData.Detalle,
                    EsPublico = true,
                    Asunto = db.Asuntos.FirstOrDefault(a => a.Descripcion == dictamenData.Asunto),
                    ArchivoPDF = null
                };
                
                
                if (ModelState.IsValid)
                {
                    db.Dictamenes.Add(dictamen);
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
