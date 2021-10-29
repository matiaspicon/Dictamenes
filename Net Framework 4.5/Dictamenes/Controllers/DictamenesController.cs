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
            var dictamenes = db.Dictamenes.Where(d => d.FechaCarga.Year == DateTime.Today.Year).Include(d => d.Asunto).Include(d => d.TipoDictamen);
            ViewData["IdAsunto"] = new SelectList(db.Asuntos.Where(m => m.EstaHabilitado), "Id", "Descripcion");
            ViewData["IdSujetoObligado"] = new SelectList(db.SujetosObligados.Where(m => m.RazonSocial != null && m.EstaHabilitado), "Id", "RazonSocial");
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
            if ((string)Session["rol"] != Models.Rol.CARGAR.ToString())
            {
                return RedirectToAction("ErrorNoPermisos", "Login");
            }
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CargarFile(HttpPostedFileBase file)
        {
            if ((string)Session["rol"] != Models.Rol.CARGAR.ToString())
            {
                return RedirectToAction("ErrorNoPermisos", "Login");
            }

            Dictamen dictamen = new Dictamen();
            //separo informacion del archivo
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
            // extraigo la informacion del PDF del dictamen y creo el objeto con la misma
            dictamen = ExtratDictamenFromString(archivo.Contenido);
            dictamen.IdArchivoPDF = archivo.Id;
            dictamen.ArchivoPDF = archivo;
            // cargo la informacion para el formulario Create y devuelvo la VIEW del create con la informacion precargada
            // o sin la informacion precargada si no se pudo obtener nada del PDF
            ViewData["IdAsunto"] = new SelectList(db.Asuntos.Where(m => m.EstaHabilitado), "Id", "Descripcion");
            ViewData["IdSujetoObligado"] = new SelectList(db.SujetosObligados.Where(m => m.RazonSocial != null && m.EstaHabilitado), "Id", "RazonSocial");
            ViewData["IdTipoDictamen"] = new SelectList(db.TiposDictamen.Where(m => m.EstaHabilitado), "Id", "Descripcion");
            ViewData["TipoSujetoObligado"] = new SelectList(db.TiposSujetoObligado.Where(m => m.EstaHabilitado), "Id", "Descripcion");
            return View("Create", dictamen);
        }


        // GET: Dictamen/Create
        public ActionResult Create()
        {
            if ((string)Session["rol"] != Models.Rol.CARGAR.ToString())
            {
                return RedirectToAction("ErrorNoPermisos", "Login");
            }

            ViewData["IdAsunto"] = new SelectList(db.Asuntos.Where(m => m.EstaHabilitado), "Id", "Descripcion");
            ViewData["IdSujetoObligado"] = new SelectList(db.SujetosObligados.Where(m => m.RazonSocial != null && m.EstaHabilitado), "Id", "RazonSocial");
            ViewData["IdTipoDictamen"] = new SelectList(db.TiposDictamen.Where(m => m.EstaHabilitado), "Id", "Descripcion");
            ViewData["TipoSujetoObligado"] = new SelectList(db.TiposSujetoObligado.Where(m => m.EstaHabilitado), "Id", "Descripcion");
            return View();
        }


        // POST: Dictamen/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,NroGDE,NroExpediente,FechaCarga,Detalle,EsPublico,IdArchivoPDF,IdSujetoObligado,IdAsunto,IdTipoDictamen,Borrado,EstaActivo,FechaModificacion,IdUsuarioModificacion, SujetoObligado")] Dictamen dictamen)
        {
            if (db.Dictamenes.FirstOrDefault(d => d.NroGDE == dictamen.NroGDE) != null)
            {
                ModelState.AddModelError("NroGDE", "Ya existe un Dictamen con ese Número de GDE");
            }

            dictamen.FechaModificacion = DateTime.Now;
            dictamen.IdUsuarioModificacion = 1;
            dictamen.NroGDE = dictamen.NroGDE.ToUpper();
            dictamen.NroExpediente = dictamen.NroExpediente.ToUpper();
            dictamen.Detalle = dictamen.Detalle != null ? dictamen.Detalle.ToUpper() : ".";
            dictamen.EsPublico = false;

            if (dictamen.SujetoObligado.CuilCuit > 0)
            {
                SujetoObligado sujetoObligadoExistente = db.SujetosObligados.FirstOrDefault(s => s.CuilCuit == dictamen.SujetoObligado.CuilCuit);
                if (sujetoObligadoExistente != null)
                {
                    if (sujetoObligadoExistente.IdTipoSujetoObligado != db.TiposSujetoObligado.First(m => m.Descripcion == "Denunciante").Id)
                    {
                        ModelState.AddModelError("SujetosObligados.CuilCuit", "Ya existe un Sujeto Obligado con ese Número de Cuil");
                    }
                    else
                    {
                        dictamen.SujetoObligado = sujetoObligadoExistente;
                    }
                }
                else
                {
                    dictamen.SujetoObligado.IdTipoSujetoObligado = db.TiposSujetoObligado.First(m => m.Descripcion == "Denunciante").Id;
                    dictamen.SujetoObligado.IdUsuarioModificacion = 3;
                    dictamen.SujetoObligado.FechaModificacion = DateTime.Now;
                    db.SujetosObligados.Add(dictamen.SujetoObligado);
                    db.SaveChanges();
                    dictamen.IdSujetoObligado = dictamen.SujetoObligado.Id;
                }
            }
            else
            {
                ModelState.Remove("SujetoObligado.CuilCuit");
                dictamen.SujetoObligado = db.SujetosObligados.Find(dictamen.IdSujetoObligado);
            }

            if(dictamen.IdSujetoObligado == null)
            {
                dictamen.HaySujetoObligado = false;
            }
            else
            {
                dictamen.HaySujetoObligado = true;
            }

            if (ModelState.IsValid)
            {
                db.Dictamenes.Add(dictamen);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            if (dictamen.SujetoObligado != null && dictamen.SujetoObligado.Nombre == null) dictamen.SujetoObligado.CuilCuit = 0; // en caso de que haya un idSujetoObligado seleccionado (una empresa de la tabla) se resetea el cuilCuit
            ViewBag.IdAsunto = new SelectList(db.Asuntos.Where(m => m.EstaHabilitado), "Id", "Descripcion", dictamen.IdAsunto);
            ViewBag.IdSujetoObligado = new SelectList(db.SujetosObligados.Where(m => m.RazonSocial != null && m.EstaHabilitado), "Id", "RazonSocial", dictamen.IdSujetoObligado);
            ViewBag.IdTipoDictamen = new SelectList(db.TiposDictamen.Where(m => m.EstaHabilitado), "Id", "Descripcion", dictamen.IdTipoDictamen);
            return View(dictamen);
        }

        [HttpPost]
        public ActionResult Buscar(Busqueda busqueda)
        {
            var idTipoSujetoObligado = busqueda.EsDenunciante ? db.TiposSujetoObligado.First(d => d.Descripcion == "Denunciante").Id : busqueda.IdTipoSujetoObligado;

            var dictamenes = db.Sp_FiltrarDictamenes(busqueda.NroGDE, busqueda.NroExp, busqueda.FechaCargaInicio, busqueda.FechaCargaFinal, busqueda.Detalle, busqueda.Contenido, busqueda.IdAsunto, busqueda.IdTipoDictamen, busqueda.IdSujetoObligado, idTipoSujetoObligado, busqueda.CuilCuit, busqueda.Nombre, busqueda.Apellido);
            ViewData["IdAsunto"] = new SelectList(db.Asuntos.Where(m => m.EstaHabilitado), "Id", "Descripcion");
            ViewData["IdSujetoObligado"] = new SelectList(db.SujetosObligados.Where(m => m.RazonSocial != null && m.EstaHabilitado), "Id", "RazonSocial");
            ViewData["IdTipoDictamen"] = new SelectList(db.TiposDictamen.Where(m => m.EstaHabilitado), "Id", "Descripcion");
            ViewData["TipoSujetoObligado"] = new SelectList(db.TiposSujetoObligado.Where(m => m.EstaHabilitado && m.Descripcion != "Denunciante"), "Id", "Descripcion");
            ViewData["Busqueda"] = busqueda;
            return View("Index", dictamenes);
        }


        // GET: Dictamen/Edit/5
        public ActionResult Edit(int? id)
        {
            if ((string)Session["rol"] != Models.Rol.CARGAR.ToString())
            {
                return RedirectToAction("ErrorNoPermisos", "Login");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Dictamen dictamen = db.Dictamenes.Include(d => d.SujetoObligado).FirstOrDefault(d => d.Id == id);

            if (dictamen == null)
            {
                return HttpNotFound();
            }

            if (dictamen.SujetoObligado != null && dictamen.SujetoObligado.Nombre == null) dictamen.SujetoObligado.CuilCuit = 0; //en caso de que el sujetoObligado no sea denunciante, se elimina el valor de cuilCuit

            ViewBag.IdAsunto = new SelectList(db.Asuntos.Where(m => m.EstaHabilitado), "Id", "Descripcion", dictamen.IdAsunto);
            ViewBag.IdSujetoObligado = new SelectList(db.SujetosObligados.Where(m => m.RazonSocial != null && m.EstaHabilitado), "Id", "RazonSocial", dictamen.IdSujetoObligado);
            ViewBag.IdTipoDictamen = new SelectList(db.TiposDictamen.Where(m => m.EstaHabilitado), "Id", "Descripcion", dictamen.IdTipoDictamen);

            return View(dictamen);
        }

        // POST: Dictamen/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,NroGDE,NroExpediente,FechaCarga,Detalle,EsPublico,IdArchivoPDF,IdSujetoObligado,IdAsunto,IdTipoDictamen,Borrado,EstaActivo,FechaModificacion,IdUsuarioModificacion,IdOriginal, SujetoObligado")] Dictamen dictamen, bool EsDenunciante, bool BorrarArchivo, HttpPostedFileBase file)
        {
            if ((string)Session["rol"] != Models.Rol.CARGAR.ToString())
            {
                return RedirectToAction("ErrorNoPermisos", "Login");
            }


            Dictamen dictamenViejo = db.Dictamenes.Include(d => d.SujetoObligado).AsNoTracking().First(d => d.Id == dictamen.Id);
            if (dictamenViejo.NroGDE != dictamen.NroGDE && db.Dictamenes.FirstOrDefault(d => d.NroGDE == dictamen.NroGDE) != null)
            {
                ModelState.AddModelError("NroGDE", "Ya existe un Dictamen con ese Número de GDE");
            }
            
            if (!EsDenunciante)
            {
                ModelState.Remove("SujetoObligado.CuilCuit");
            }

            dictamen.EsPublico = false;

            if (ModelState.IsValid)
            {
                DictamenLog dictamenLog = new DictamenLog
                {
                    IdOriginal = dictamenViejo.Id,
                    NroGDE = dictamenViejo.NroGDE,
                    NroExpediente = dictamenViejo.NroExpediente,
                    Detalle = dictamenViejo.Detalle,
                    EsPublico = dictamenViejo.EsPublico,
                    HaySujetoObligado = dictamenViejo.IdSujetoObligado != null,
                    IdArchivoPDF = dictamenViejo.IdArchivoPDF,
                    IdSujetoObligado = dictamenViejo.IdSujetoObligado,
                    IdAsunto = dictamenViejo.IdAsunto,
                    IdTipoDictamen = dictamenViejo.IdTipoDictamen,
                    FechaCarga = dictamenViejo.FechaCarga,
                    FechaModificacion = dictamenViejo.FechaModificacion,
                    Borrado = false,
                    IdUsuarioModificacion = dictamenViejo.IdUsuarioModificacion
                };

                dictamen.FechaModificacion = DateTime.Now;
                dictamen.IdUsuarioModificacion = 3;
                dictamen.NroGDE = dictamen.NroGDE.ToUpper();
                dictamen.NroExpediente = dictamen.NroExpediente.ToUpper();
                dictamen.Detalle = dictamen.Detalle.ToUpper();
                if (BorrarArchivo)
                {
                    dictamen.ArchivoPDF = null;
                    dictamen.IdArchivoPDF = null;
                }
                else
                {
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
                }


                if (EsDenunciante) //es denuciante
                {
                    if (dictamenViejo.IdSujetoObligado.HasValue && dictamenViejo.SujetoObligado.RazonSocial == null)
                    {
                        if (dictamen.SujetoObligado.CuilCuit != dictamenViejo.SujetoObligado.CuilCuit || dictamen.SujetoObligado.Nombre != dictamenViejo.SujetoObligado.Nombre || dictamen.SujetoObligado.Apellido != dictamenViejo.SujetoObligado.Apellido)
                        {
                            SujetoObligado sujetoObligadoViejo = db.SujetosObligados.AsNoTracking().FirstOrDefault(d => d.Id == dictamen.IdSujetoObligado);
                            
                            SujetoObligadoLog sujetoObligadoLog = new SujetoObligadoLog
                            {
                                CuilCuit = sujetoObligadoViejo.CuilCuit,
                                Nombre = sujetoObligadoViejo.Nombre,
                                Apellido = sujetoObligadoViejo.Apellido,
                                RazonSocial = sujetoObligadoViejo.RazonSocial,
                                IdOriginal = sujetoObligadoViejo.Id,
                                EstaHabilitado = sujetoObligadoViejo.EstaHabilitado,
                                IdTipoSujetoObligado = db.TiposSujetoObligado.FirstOrDefault(d => d.Descripcion == "Denunciante").Id,
                                FechaModificacion = sujetoObligadoViejo.FechaModificacion,
                                IdUsuarioModificacion = sujetoObligadoViejo.IdUsuarioModificacion
                            };
                            sujetoObligadoViejo.CuilCuit = dictamen.SujetoObligado.CuilCuit;
                            sujetoObligadoViejo.Nombre = dictamen.SujetoObligado.Nombre;
                            sujetoObligadoViejo.Apellido = dictamen.SujetoObligado.Apellido;
                            sujetoObligadoViejo.IdUsuarioModificacion = 3;
                            sujetoObligadoViejo.FechaModificacion = DateTime.Now;
                            sujetoObligadoViejo.IdTipoSujetoObligado = sujetoObligadoViejo.IdTipoSujetoObligado;
                            sujetoObligadoViejo.EstaHabilitado = sujetoObligadoViejo.EstaHabilitado;

                            db.Entry(sujetoObligadoViejo).State = EntityState.Modified;

                            dictamen.SujetoObligado = sujetoObligadoViejo;

                            db.SujetosObligadosLog.Add(sujetoObligadoLog);
                            db.SaveChanges();
                            
                        }
                        else
                        {
                            dictamen.SujetoObligado.Id = dictamen.IdSujetoObligado.Value;
                        }
                    }
                    else
                    {
                        dictamen.SujetoObligado.Id = 0;
                        dictamen.SujetoObligado.FechaModificacion = DateTime.Now;
                        dictamen.SujetoObligado.IdTipoSujetoObligado = db.TiposSujetoObligado.FirstOrDefault(d => d.Descripcion == "Denunciante").Id;
                        dictamen.SujetoObligado.IdUsuarioModificacion = 1;
                        db.SujetosObligados.Add(dictamen.SujetoObligado);
                        db.SaveChanges();
                        dictamen.IdSujetoObligado = dictamen.SujetoObligado.Id;
                    }
                }
                else //no es denunciante
                {
                    if (dictamen.IdSujetoObligado.HasValue)
                    {
                        dictamen.SujetoObligado = db.SujetosObligados.Find(dictamen.IdSujetoObligado);
                    }
                    else
                    {
                        dictamen.IdSujetoObligado = null;
                        dictamen.SujetoObligado = null;
                    }
                }


                if (dictamen.IdSujetoObligado == null)
                {
                    dictamen.HaySujetoObligado = false;
                }
                else
                {
                    dictamen.HaySujetoObligado = true;
                }
                //-----------------------------------------------------------------------------

                db.Entry(dictamen).State = EntityState.Modified;
                db.DictamenesLog.Add(dictamenLog);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            ViewBag.IdAsunto = new SelectList(db.Asuntos.Where(m => m.EstaHabilitado), "Id", "Descripcion", dictamen.IdAsunto);
            ViewBag.IdSujetoObligado = new SelectList(db.SujetosObligados.Where(m => m.RazonSocial != null && m.EstaHabilitado), "Id", "RazonSocial", dictamen.IdSujetoObligado);
            ViewBag.IdTipoDictamen = new SelectList(db.TiposDictamen.Where(m => m.EstaHabilitado), "Id", "Descripcion", dictamen.IdTipoDictamen);
            return View(dictamen);
        }

        // GET: Dictamen/Delete/5
        public ActionResult Delete(int? id)
        {
            if ((string)Session["rol"] != Models.Rol.CARGAR.ToString())
            {
                return RedirectToAction("ErrorNoPermisos", "Login");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dictamen dictamen = db.Dictamenes.Include(d => d.ArchivoPDF).Include(d => d.Asunto).Include(d => d.SujetoObligado).Include(d => d.TipoDictamen).FirstOrDefault(d => d.Id == id); if (dictamen == null)
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
            if ((string)Session["rol"] != Models.Rol.CARGAR.ToString())
            {
                return RedirectToAction("ErrorNoPermisos", "Login");
            }

            var dictamen = db.Dictamenes.Find(id);

            Dictamen dictamenViejo = db.Dictamenes.AsNoTracking().First(d => d.Id == id);

            db.Dictamenes.Remove(dictamen);

            DictamenLog dictamenLog = new DictamenLog
            {
                IdOriginal = dictamenViejo.Id,
                NroGDE = dictamenViejo.NroGDE,
                NroExpediente = dictamenViejo.NroExpediente,
                Detalle = dictamenViejo.Detalle,
                EsPublico = dictamenViejo.EsPublico,
                HaySujetoObligado = dictamenViejo.IdSujetoObligado != null,
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
            ErrorMessage = "El Número de GDE ingresado no es valido.")]
            [MaxLength(30, ErrorMessage = "{0} admite un máximo de {1} caracteres")]
            public string NroGDE { get; set; }

            [Required]
            [RegularExpression("[eE][xX]-[0-9]{4}-[0-9]+-[aA][pP][nN]-[A-Za-z]+#[A-Za-z]+",
             ErrorMessage = "El Número de Expediente ingresado no es valido.")]
            [MaxLength(30, ErrorMessage = "{0} admite un máximo de {1} caracteres")]
            public string NroExpediente { get; set; }

            public string FechaCarga { get; set; }

            public string Detalle { get; set; }

            public string Asunto { get; set; }
        }


        private bool IsValid(Dictamen dictamen)
        {
            ModelState.Clear();
            if (!TryValidateModel(dictamen))
            {

            }
            return ModelState.IsValid;
        }

        [HttpPost]
        public ActionResult CargarDictamenes(string JSONDictamenes, HttpPostedFileBase[] files)
        {
            if ((string)Session["rol"] != Models.Rol.CARGAR.ToString())
            {
                return RedirectToAction("ErrorNoPermisos", "Login");
            }

            List<DictamenData> dictamenes;
            List<Dictamen> dictamenesError = new List<Dictamen>();
            List<ArchivoPDF> archivosPDFError = new List<ArchivoPDF>();

            try
            {
                dictamenes = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DictamenData>>(JSONDictamenes);
            }
            catch {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if(dictamenes != null)
            {
                foreach (DictamenData dictamenData in dictamenes)
                {

                    Asunto asunto = db.Asuntos.FirstOrDefault(a => a.Descripcion == dictamenData.Asunto);
                    if (asunto != null)
                    {
                        Dictamen dictamen = new Dictamen
                        {
                            NroGDE = dictamenData.NroGDE,
                            NroExpediente = dictamenData.NroExpediente,
                            FechaCarga = DateTime.ParseExact(dictamenData.FechaCarga, "dd-MM-yyyy HH:mm", CultureInfo.InvariantCulture),
                            Detalle = dictamenData.Detalle,
                            EsPublico = true,
                            HaySujetoObligado = false,
                            Asunto = asunto,
                            IdAsunto = asunto != null ? asunto.Id : 0,
                            ArchivoPDF = null,
                            FechaModificacion = DateTime.Now,
                            IdSujetoObligado = null,
                            IdUsuarioModificacion = 1,
                            IdTipoDictamen = db.TiposDictamen.First(m => m.Descripcion == "Sin valor").Id
                        };

                        if (IsValid(dictamen))
                        {
                            db.Dictamenes.Add(dictamen);
                            db.SaveChanges();
                        }
                        else
                        {
                            dictamenesError.Add(dictamen);
                        }
                    }

                }
            }

            if (files.FirstOrDefault() != null)
            {
                foreach (HttpPostedFileBase file in files)
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

                    string NroGDE;

                    Regex numeroGDE = new Regex("IF-[0-9]{4}-[0-9]+-APN-[A-Z]+#[A-Z]+", RegexOptions.IgnoreCase);

                    MatchCollection matches = numeroGDE.Matches(archivo.Contenido);
                    try
                    {
                        NroGDE = matches[0].Value;
                        Dictamen dict = db.Dictamenes.Where(m => m.NroGDE == NroGDE).ToArray().FirstOrDefault();
                        if (dict != null)
                        {
                            db.ArchivosPDF.Add(archivo);
                            db.SaveChanges();
                            dict.IdArchivoPDF = archivo.Id;
                            db.Entry(dict).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    catch
                    {
                        archivosPDFError.Add(archivo);
                    }
                }
            }           
            
            db.SaveChanges();
            return Json(new {archivosPDFError, dictamenesError});          
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
                DateTime fecha;
                DateTime.TryParseExact(matches[0].Value, "yyyy.MM.dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out fecha);
                dict.FechaCarga = fecha;
            }
            catch
            {
                dict.FechaCarga = DateTime.Now;
            }
            return dict;


        }
    }
}
