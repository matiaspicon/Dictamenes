using Dictamenes.Database;
using Dictamenes.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Dictamenes.Controllers
{
    [Authorize]
    public class DictamenesController : Controller
    {
        private DictamenesDbContext db = new DictamenesDbContext();

        // GET: Dictamen
        public ActionResult Index()
        {
            //El usuario que no este en el rol de Cargar o Consulta no podra acceder a esta opcion
            if (!FrameworkMVC.Security.LoginService.IsAllowed(new[] { Models.Rol.CARGAR.ToString(), Models.Rol.CONSULTAR.ToString() }))
            {
                return RedirectToAction("ErrorNoPermisos", "Login");
            }
            //Trae todos los dictamenes que sean del anio actual
            var dictamenes = db.Dictamenes.Where(d => d.FechaCarga.Year == DateTime.Today.Year).Include(d => d.Asunto).Include(d => d.TipoDictamen);
            ViewData["IdAsunto"] = new SelectList(db.Asuntos.Where(m => m.EstaHabilitado).OrderBy(m => m.Descripcion).OrderBy(m => m.Descripcion), "Id", "Descripcion");
            ViewData["IdSujetoControlado"] = new SelectList(db.SujetosControlados.Where(m => m.RazonSocial != null && m.EstaHabilitado).OrderBy(m => m.RazonSocial).OrderBy(m => m.RazonSocial), "Id", "RazonSocial");
            ViewData["IdTipoDictamen"] = new SelectList(db.TiposDictamen.Where(m => m.EstaHabilitado).OrderBy(m => m.Descripcion), "Id", "Descripcion");
            ViewData["TipoSujetoControlado"] = new SelectList(db.TiposSujetoControlado.Where(m => m.EstaHabilitado && m.Descripcion != "Denunciante").OrderBy(m => m.Descripcion), "Id", "Descripcion");
            return View(dictamenes.ToList());
        }

        // GET: Dictamen/Details/5
        public ActionResult Details(int? id)
        {
            //El usuario que no este en el rol de Cargar o Consulta no podra acceder a esta opcion
            if (!FrameworkMVC.Security.LoginService.IsAllowed(new[] { Models.Rol.CARGAR.ToString(), Models.Rol.CONSULTAR.ToString() }))
            {
                return RedirectToAction("ErrorNoPermisos", "Login");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dictamen dictamen = db.Dictamenes.Include(d => d.ArchivoPDF).Include(d => d.Asunto).Include(d => d.SujetoControlado).Include(d => d.TipoDictamen).FirstOrDefault(d => d.Id == id);
            if (dictamen == null)
            {
                dictamen = db.Dictamenes.Include(d => d.Asunto).Include(d => d.SujetoControlado).Include(d => d.TipoDictamen).FirstOrDefault(d => d.Id == id);
                if (dictamen == null)
                {
                    return HttpNotFound();
                }
            }
            return View(dictamen);
        }


        // GET: Dictamenes/Create
        public ActionResult CargarFile()
        {

            //El usuario que no este en el rol de Cargar no podra acceder a esta opcion
            if (!FrameworkMVC.Security.LoginService.IsAllowed(new[] { Models.Rol.CARGAR.ToString() }))
            {
                return RedirectToAction("ErrorNoPermisos", "Login");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CargarFile(HttpPostedFileBase file)
        {
            //El usuario que no este en el rol de Cargar no podra acceder a esta opcion
            if (!FrameworkMVC.Security.LoginService.IsAllowed(new[] { Models.Rol.CARGAR.ToString() }))
            {
                return RedirectToAction("ErrorNoPermisos", "Login");
            }

            Dictamen dictamen = new Dictamen();

            //separo informacion del archivo
            var fileName = Guid.NewGuid().ToString();
            var extension = Path.GetExtension(file.FileName);

            //si el archivo no es de tipo PDF, no se carga se crea el archivo en la base de datos ni se almacena el archivo
            if (extension != ".pdf")
            {
                //seteo los valores del dictamen a null
                dictamen.IdArchivoPDF = null;
                dictamen.ArchivoPDF = null;
                //seteo la fecha de Carga a la actual para facilidad de carga
                dictamen.FechaCarga = DateTime.Now;

                ViewData["IdAsunto"] = new SelectList(db.Asuntos.Where(m => m.EstaHabilitado).OrderBy(m => m.Descripcion), "Id", "Descripcion");
                ViewData["IdSujetoControlado"] = new SelectList(db.SujetosControlados.Where(m => m.RazonSocial != null && m.EstaHabilitado).OrderBy(m => m.RazonSocial), "Id", "RazonSocial");
                ViewData["IdTipoDictamen"] = new SelectList(db.TiposDictamen.Where(m => m.EstaHabilitado).OrderBy(m => m.Descripcion), "Id", "Descripcion");
                ViewData["TipoSujetoControlado"] = new SelectList(db.TiposSujetoControlado.Where(m => m.EstaHabilitado).OrderBy(m => m.Descripcion), "Id", "Descripcion");

                //cargo la informacion para el formulario Create y devuelvo la VIEW del create con un dictamen sin archivo
                //que esta dentro de la variable dictamen
                return View("Create", dictamen);
            }

            // compruebo directorio
            var basePath = Server.MapPath("~/Archivos");
            var filePath = Path.Combine("~", "Archivos", fileName + extension);

            bool basePathExists = System.IO.Directory.Exists(basePath);
            if (!basePathExists) Directory.CreateDirectory(basePath);

            if (!System.IO.File.Exists(Server.MapPath(filePath)))
            {
                file.SaveAs(Server.MapPath(filePath));
            }

            // creo el objeto archivo para agregar al dictamen
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
            dictamen.NroGDE = Path.GetFileNameWithoutExtension(file.FileName).Replace("%","#");

            ViewData["IdAsunto"] = new SelectList(db.Asuntos.Where(m => m.EstaHabilitado).OrderBy(m => m.Descripcion), "Id", "Descripcion");
            ViewData["IdSujetoControlado"] = new SelectList(db.SujetosControlados.Where(m => m.RazonSocial != null && m.EstaHabilitado).OrderBy(m => m.RazonSocial), "Id", "RazonSocial");
            ViewData["IdTipoDictamen"] = new SelectList(db.TiposDictamen.Where(m => m.EstaHabilitado).OrderBy(m => m.Descripcion), "Id", "Descripcion");
            ViewData["TipoSujetoControlado"] = new SelectList(db.TiposSujetoControlado.Where(m => m.EstaHabilitado).OrderBy(m => m.Descripcion), "Id", "Descripcion");

            // cargo la informacion para el formulario Create y devuelvo la VIEW del create con la informacion extraida del Archivo
            //que esta dentro de la variable dictamen
            return View("Create", dictamen);
        }


        // GET: Dictamen/Create
        public ActionResult Create()
        {
            //El usuario que no este en el rol de Cargar no podra acceder a esta opcion
            if (!FrameworkMVC.Security.LoginService.IsAllowed(new[] { Models.Rol.CARGAR.ToString() }))
            {
                return RedirectToAction("ErrorNoPermisos", "Login");
            }

            ViewData["IdAsunto"] = new SelectList(db.Asuntos.Where(m => m.EstaHabilitado).OrderBy(m => m.Descripcion), "Id", "Descripcion");
            ViewData["IdSujetoControlado"] = new SelectList(db.SujetosControlados.Where(m => m.RazonSocial != null && m.EstaHabilitado).OrderBy(m => m.RazonSocial), "Id", "RazonSocial");
            ViewData["IdTipoDictamen"] = new SelectList(db.TiposDictamen.Where(m => m.EstaHabilitado).OrderBy(m => m.Descripcion), "Id", "Descripcion");
            ViewData["TipoSujetoControlado"] = new SelectList(db.TiposSujetoControlado.Where(m => m.EstaHabilitado).OrderBy(m => m.Descripcion), "Id", "Descripcion");
            return View(new Dictamen());
        }


        // POST: Dictamen/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ArchivoPDF, Id,NroGDE,NroExpediente,FechaCarga,Detalle,EsPublico,IdArchivoPDF,IdSujetoControlado,IdAsunto,IdTipoDictamen,Borrado,EstaActivo,FechaModificacion,IdUsuarioModificacion, SujetoControlado")] Dictamen dictamen)
        {
            //El usuario que no este en el rol de Cargar no podra acceder a esta opcion
            if (!FrameworkMVC.Security.LoginService.IsAllowed(new[] { Models.Rol.CARGAR.ToString() }))
            {
                return RedirectToAction("ErrorNoPermisos", "Login");
            }

            //en caso de que exista un dictamen con el mismo numero de GDE
            if (db.Dictamenes.FirstOrDefault(d => d.NroGDE == dictamen.NroGDE) != null)
            {
                //se agrega un error en ese campo y se vuelve a solicitar la carga
                ModelState.AddModelError("NroGDE", "Ya existe un Dictamen con ese Número de GDE");
            }

            //limpio y estandarizo la informacion
            dictamen.FechaModificacion = DateTime.Now;
            dictamen.IdUsuarioModificacion =  FrameworkMVC.Security.LoginService.Current.UsuarioID;
            dictamen.NroGDE = dictamen.NroGDE.ToUpper();
            dictamen.NroExpediente = dictamen.NroExpediente.ToUpper();
            dictamen.Detalle = dictamen.Detalle.ToUpper();
            dictamen.EsPublico = false;
            dictamen.ArchivoPDF = db.ArchivosPDF.Find(dictamen.IdArchivoPDF);


            //compruebo si la data del formulario del denunciante es distinto al valor por defecto
            if (dictamen.SujetoControlado.CuilCuit > 0)
            {
                //busco si existe otro Sujeto Controlado con el mismo cuilCuit
                SujetoControlado SujetoControladoExistente = db.SujetosControlados.FirstOrDefault(s => s.CuilCuit == dictamen.SujetoControlado.CuilCuit);
                if (SujetoControladoExistente != null)
                {
                    //chequeo si el SujetoControlado existente no es denunciante
                    if (SujetoControladoExistente.IdTipoSujetoControlado != db.TiposSujetoControlado.First(m => m.Descripcion == "Denunciante").Id)
                    {
                        //devuelvo un error ya que ese cuilCuit corresponde a una empresa
                        ModelState.AddModelError("SujetoControlado.CuilCuit", "Ya existe un Sujeto Controlado con ese Número de Cuil");
                    }
                    else
                    {
                        //en caso de ser un denunciante, simplemente uso el SujetoControlado denunciante existente
                        dictamen.IdSujetoControlado = SujetoControladoExistente.Id;
                        dictamen.SujetoControlado = SujetoControladoExistente;
                    }
                }
                //en caso de no existir un SujetoControlado con el mismo cuilCuit, lo creo
                else
                {
                    dictamen.SujetoControlado.IdTipoSujetoControlado = db.TiposSujetoControlado.First(m => m.Descripcion == "Denunciante").Id;
                    dictamen.SujetoControlado.IdUsuarioModificacion =  FrameworkMVC.Security.LoginService.Current.UsuarioID;
                    dictamen.SujetoControlado.FechaModificacion = DateTime.Now;
                    db.SujetosControlados.Add(dictamen.SujetoControlado);
                }
            }
            //en caso de no haber un valor en el formulario de Denunciante
            else
            {
                //remuevo la comprobacion del campo cuilCuit
                ModelState.Remove("SujetoControlado.CuilCuit");
                //seteo el SujetoControlado para que corresponda con el IdSujetoControlado
                //en caso que el IdSujetoControlado sea null Find() devuelve null tambien
                dictamen.SujetoControlado = db.SujetosControlados.Find(dictamen.IdSujetoControlado);
            }

            //seteo el valor del HaySujetoControlado basado en el valor de IdSujetoControlado
            dictamen.HaySujetoControlado = dictamen.IdSujetoControlado != null;
            
            if (ModelState.IsValid)
            {
                db.Dictamenes.Add(dictamen);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            if (dictamen.SujetoControlado != null && dictamen.SujetoControlado.Nombre == null) dictamen.SujetoControlado.CuilCuit = 0; // en caso de que haya un idSujetoControlado seleccionado (una empresa de la tabla) se resetea el cuilCuit
            ViewBag.IdAsunto = new SelectList(db.Asuntos.Where(m => m.EstaHabilitado).OrderBy(m => m.Descripcion), "Id", "Descripcion", dictamen.IdAsunto);
            ViewBag.IdSujetoControlado = new SelectList(db.SujetosControlados.Where(m => m.RazonSocial != null && m.EstaHabilitado).OrderBy(m => m.RazonSocial), "Id", "RazonSocial", dictamen.IdSujetoControlado);
            ViewBag.IdTipoDictamen = new SelectList(db.TiposDictamen.Where(m => m.EstaHabilitado).OrderBy(m => m.Descripcion), "Id", "Descripcion", dictamen.IdTipoDictamen);
            return View(dictamen);
        }


        public ActionResult Buscar()
        {
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Buscar(Busqueda busqueda)
        {
            //seteo el valor para la busqueda ya que viene el formato bool y necesito un int de id
            var idTipoSujetoControlado = busqueda.EsDenunciante ? db.TiposSujetoControlado.First(d => d.Descripcion == "Denunciante").Id : busqueda.IdTipoSujetoControlado;

            //ejecuto el StoreProcedure "sp_FiltrarDictamenes" de la base de datos con los valores pasado por parametro
            var dictamenes = db.Sp_FiltrarDictamenes(busqueda.NroGDE, busqueda.NroExp, busqueda.FechaCargaInicio, busqueda.FechaCargaFinal,
                busqueda.Detalle, busqueda.Contenido, busqueda.IdAsunto, busqueda.IdTipoDictamen, busqueda.IdSujetoControlado,
                idTipoSujetoControlado, busqueda.CuilCuit, busqueda.Nombre, busqueda.Apellido);

            //cargo las listas desplegables
            ViewData["IdAsunto"] = new SelectList(db.Asuntos.Where(m => m.EstaHabilitado).OrderBy(m => m.Descripcion), "Id", "Descripcion");
            ViewData["IdSujetoControlado"] = new SelectList(db.SujetosControlados.Where(m => m.RazonSocial != null && m.EstaHabilitado).OrderBy(m => m.RazonSocial), "Id", "RazonSocial");
            ViewData["IdTipoDictamen"] = new SelectList(db.TiposDictamen.Where(m => m.EstaHabilitado).OrderBy(m => m.Descripcion), "Id", "Descripcion");
            ViewData["TipoSujetoControlado"] = new SelectList(db.TiposSujetoControlado.Where(m => m.EstaHabilitado && m.Descripcion != "Denunciante").OrderBy(m => m.Descripcion), "Id", "Descripcion");

            //envio los parametros actuales de busqueda para que puedan ser mostrado en la vista
            ViewData["Busqueda"] = busqueda;

            //envio los dictamenes filtrados a la vista
            return View("Index", dictamenes);
        }


        // GET: Dictamen/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!FrameworkMVC.Security.LoginService.IsAllowed(new[] { Models.Rol.CARGAR.ToString() }))
            {
                return View("ErrorNoPermisos", "Login");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Dictamen dictamen = db.Dictamenes.Include(d => d.SujetoControlado).Include(d => d.ArchivoPDF).FirstOrDefault(d => d.Id == id);

            if (dictamen == null)
            {
                return HttpNotFound();
            }

            if (dictamen.SujetoControlado != null && dictamen.SujetoControlado.Nombre == null) dictamen.SujetoControlado.CuilCuit = 0; //en caso de que el SujetoControlado no sea denunciante, se elimina el valor de cuilCuit

            ViewBag.IdAsunto = new SelectList(db.Asuntos.OrderBy(m => m.Descripcion), "Id", "Descripcion", dictamen.IdAsunto);
            ViewBag.IdSujetoControlado = new SelectList(db.SujetosControlados.Where(m => m.RazonSocial != null).OrderBy(m => m.RazonSocial), "Id", "RazonSocial", dictamen.IdSujetoControlado);
            ViewBag.IdTipoDictamen = new SelectList(db.TiposDictamen.OrderBy(m => m.Descripcion), "Id", "Descripcion", dictamen.IdTipoDictamen);

            return View(dictamen);
        }

        // POST: Dictamen/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,NroGDE,NroExpediente,FechaCarga,Detalle,EsPublico,IdArchivoPDF,IdSujetoControlado,IdAsunto,IdTipoDictamen,Borrado,EstaActivo,FechaModificacion,IdUsuarioModificacion,IdOriginal, SujetoControlado, Archivo")] Dictamen dictamen, bool EsDenunciante, bool BorrarArchivo, HttpPostedFileBase file)
        {
            //El usuario que no este en el rol de Cargar no podra acceder a esta opcion
            if (!FrameworkMVC.Security.LoginService.IsAllowed(new[] { Models.Rol.CARGAR.ToString() }))
            {
                return RedirectToAction("ErrorNoPermisos", "Login");
            }

            //obtengo una copia del dictamen anterior
            Dictamen dictamenViejo = db.Dictamenes.Include(d => d.SujetoControlado).AsNoTracking().First(d => d.Id == dictamen.Id);

            //en caso de que se haya modificado el numero de GDE y que el numero de GDE se encuentre presente en otro dictamen
            if (dictamenViejo.NroGDE != dictamen.NroGDE && db.Dictamenes.FirstOrDefault(d => d.NroGDE == dictamen.NroGDE) != null)
            {
                //se agrega un error en ese campo y se vuelve a solicitar la carga
                ModelState.AddModelError("NroGDE", "Ya existe un Dictamen con ese Número de GDE");
            }


            if (!EsDenunciante)
            {
                //remuevo la comprobacion del campo cuilCuit
                ModelState.Remove("SujetoControlado.CuilCuit");
            }

            dictamen.EsPublico = false;

            if (ModelState.IsValid)
            {

                //creo un dictamenLog con todos los datos del Dictamen anterior
                DictamenLog dictamenLog = new DictamenLog
                {
                    IdOriginal = dictamenViejo.Id,
                    NroGDE = dictamenViejo.NroGDE,
                    NroExpediente = dictamenViejo.NroExpediente,
                    Detalle = dictamenViejo.Detalle,
                    EsPublico = dictamenViejo.EsPublico,
                    HaySujetoControlado = dictamenViejo.IdSujetoControlado != null,
                    IdArchivoPDF = dictamenViejo.IdArchivoPDF,
                    IdSujetoControlado = dictamenViejo.IdSujetoControlado,
                    IdAsunto = dictamenViejo.IdAsunto,
                    IdTipoDictamen = dictamenViejo.IdTipoDictamen,
                    FechaCarga = dictamenViejo.FechaCarga,
                    FechaModificacion = dictamenViejo.FechaModificacion,
                    Borrado = false,
                    IdUsuarioModificacion = dictamenViejo.IdUsuarioModificacion
                };

                //limpio y estandarizo la informacion
                dictamen.FechaModificacion = DateTime.Now;
                dictamen.IdUsuarioModificacion =  FrameworkMVC.Security.LoginService.Current.UsuarioID;
                dictamen.NroGDE = dictamen.NroGDE.ToUpper();
                dictamen.NroExpediente = dictamen.NroExpediente.ToUpper();
                dictamen.Detalle = dictamen.Detalle.ToUpper();
                dictamen.ArchivoPDF = db.ArchivosPDF.Find(dictamen.IdArchivoPDF);

                if (BorrarArchivo) 
                {
                    //borro el archivo
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
                        var basePath = Server.MapPath("~/Archivos");
                        var filePath = Path.Combine("~", "Archivos", fileName + extension);

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


                if (EsDenunciante)
                {
                    bool EsElMismo = false;
                    //reviso que el sujeto no sea null
                    if (dictamen.SujetoControlado != null && dictamenViejo.SujetoControlado != null)
                    {
                        if (dictamen.SujetoControlado.CuilCuit == dictamenViejo.SujetoControlado.CuilCuit)
                        {
                            EsElMismo = true;
                            if (dictamen.SujetoControlado.Nombre != dictamenViejo.SujetoControlado.Nombre || dictamen.SujetoControlado.Apellido != dictamenViejo.SujetoControlado.Apellido)
                            {
                                //hago la modificacion del SujetoControlado

                                SujetoControlado SujetoControladoViejo = db.SujetosControlados.AsNoTracking().FirstOrDefault(d => d.Id == dictamen.IdSujetoControlado);

                                SujetoControladoLog SujetoControladoLog = new SujetoControladoLog
                                {
                                    CuilCuit = SujetoControladoViejo.CuilCuit,
                                    Nombre = SujetoControladoViejo.Nombre,
                                    Apellido = SujetoControladoViejo.Apellido,
                                    RazonSocial = SujetoControladoViejo.RazonSocial,
                                    IdOriginal = SujetoControladoViejo.Id,
                                    EstaHabilitado = SujetoControladoViejo.EstaHabilitado,
                                    IdTipoSujetoControlado = db.TiposSujetoControlado.FirstOrDefault(d => d.Descripcion == "Denunciante").Id,
                                    FechaModificacion = SujetoControladoViejo.FechaModificacion,
                                    IdUsuarioModificacion = SujetoControladoViejo.IdUsuarioModificacion
                                };
                                SujetoControladoViejo.CuilCuit = dictamen.SujetoControlado.CuilCuit;
                                SujetoControladoViejo.Nombre = dictamen.SujetoControlado.Nombre;
                                SujetoControladoViejo.Apellido = dictamen.SujetoControlado.Apellido;
                                SujetoControladoViejo.IdUsuarioModificacion =  FrameworkMVC.Security.LoginService.Current.UsuarioID;
                                SujetoControladoViejo.FechaModificacion = DateTime.Now;
                                SujetoControladoViejo.IdTipoSujetoControlado = SujetoControladoViejo.IdTipoSujetoControlado;
                                SujetoControladoViejo.EstaHabilitado = SujetoControladoViejo.EstaHabilitado;

                                db.Entry(SujetoControladoViejo).State = EntityState.Modified;

                                //guardo al nuevo SujetoControlado dentro del dictamen
                                dictamen.SujetoControlado = SujetoControladoViejo;

                                //guardo el log del SujetoControlado que se modifico
                                db.SujetosControladosLog.Add(SujetoControladoLog);
                                db.SaveChanges();

                            }
                            else
                            {
                                //completo el id del Sujeto Controlado en caso de que no se hayan realizado modificaciones
                                dictamen.SujetoControlado.Id = dictamen.IdSujetoControlado.Value;
                            }
                        }
                    }
                    if (!EsElMismo)
                    {
                        //busco si existe otro Sujeto Controlado con el mismo cuilCuit
                        SujetoControlado SujetoControladoExistente = db.SujetosControlados.FirstOrDefault(s => s.CuilCuit == dictamen.SujetoControlado.CuilCuit);
                        if (SujetoControladoExistente != null)
                        {
                            //chequeo si el SujetoControlado existente no es denunciante
                            if (SujetoControladoExistente.IdTipoSujetoControlado != db.TiposSujetoControlado.First(m => m.Descripcion == "Denunciante").Id)
                            {
                                //devuelvo un error ya que ese cuilCuit corresponde a una empresa
                                ModelState.AddModelError("SujetoControlado.CuilCuit", "Ya existe un Sujeto Controlado con ese Número de Cuil");
                            }
                            else
                            {
                                //en caso de ser un denunciante, simplemente uso el SujetoControlado denunciante existente
                                dictamen.SujetoControlado = SujetoControladoExistente;
                                dictamen.IdSujetoControlado = SujetoControladoExistente.Id;
                            }
                        }
                        //en caso de no existir un SujetoControlado con el mismo cuilCuit, lo creo
                        else
                        {
                            //se crea el nuevo Sujeto Controlado
                            dictamen.SujetoControlado.Id = 0;
                            dictamen.SujetoControlado.FechaModificacion = DateTime.Now;
                            dictamen.SujetoControlado.IdTipoSujetoControlado = db.TiposSujetoControlado.FirstOrDefault(d => d.Descripcion == "Denunciante").Id;
                            dictamen.SujetoControlado.IdUsuarioModificacion =  FrameworkMVC.Security.LoginService.Current.UsuarioID;
                            db.SujetosControlados.Add(dictamen.SujetoControlado);
                            db.SaveChanges();
                            //guardo al nuevo SujetoControlado dentro del dictamen
                            dictamen.IdSujetoControlado = dictamen.SujetoControlado.Id;
                        }
                    }
                }
                else //no es denunciante
                {
                    if (dictamen.IdSujetoControlado.HasValue)
                    {
                        //busco el Sujeto Controlado completo para agregarlo al dictamen
                        dictamen.SujetoControlado = db.SujetosControlados.Find(dictamen.IdSujetoControlado);
                    }
                    else
                    {
                        //si no tiene valor, es porque fue seleccionada la opcion de "Sin Sujeto Controlado"
                        dictamen.IdSujetoControlado = null;
                        dictamen.SujetoControlado = null;
                    }
                }


                //seteo el valor del HaySujetoControlado basado en el valor de IdSujetoControlado
                dictamen.HaySujetoControlado = dictamen.IdSujetoControlado != null;

                //-----------------------------------------------------------------------------
                if (ModelState.IsValid)
                {
                    //modifico el dictamen
                    db.Entry(dictamen).State = EntityState.Modified;

                    //agrego el log del dictamen
                    db.DictamenesLog.Add(dictamenLog);                    
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                
                
            }

            //en caso de haber un error con la modificacion se devuelve el dictamen con los errores correspondientes
            ViewBag.IdAsunto = new SelectList(db.Asuntos.Where(m => m.EstaHabilitado).OrderBy(m => m.Descripcion), "Id", "Descripcion", dictamen.IdAsunto);
            ViewBag.IdSujetoControlado = new SelectList(db.SujetosControlados.Where(m => m.RazonSocial != null && m.EstaHabilitado).OrderBy(m => m.RazonSocial), "Id", "RazonSocial", dictamen.IdSujetoControlado);
            ViewBag.IdTipoDictamen = new SelectList(db.TiposDictamen.Where(m => m.EstaHabilitado).OrderBy(m => m.Descripcion), "Id", "Descripcion", dictamen.IdTipoDictamen);
            return View(dictamen);
        }

        // GET: Dictamen/Delete/5
        public ActionResult Delete(int? id)
        {
            if (!FrameworkMVC.Security.LoginService.IsAllowed(new[] { Models.Rol.CARGAR.ToString() }))
            {
                return RedirectToAction("ErrorNoPermisos", "Login");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dictamen dictamen = db.Dictamenes.Include(d => d.ArchivoPDF).Include(d => d.Asunto).Include(d => d.SujetoControlado).Include(d => d.TipoDictamen).FirstOrDefault(d => d.Id == id); if (dictamen == null)
            {
                return HttpNotFound();
            }
            ViewData["IdDenunciante"] = db.TiposSujetoControlado.FirstOrDefault(m => m.Descripcion == "Denunciante").Id;
            return View(dictamen);
        }

        // POST: Dictamen/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!FrameworkMVC.Security.LoginService.IsAllowed(new[] { Models.Rol.CARGAR.ToString() }))
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
                HaySujetoControlado = dictamenViejo.IdSujetoControlado != null,
                IdArchivoPDF = dictamenViejo.IdArchivoPDF,
                IdSujetoControlado = dictamenViejo.IdSujetoControlado,
                IdAsunto = dictamenViejo.IdAsunto,
                IdTipoDictamen = dictamenViejo.IdTipoDictamen,
                FechaCarga = dictamenViejo.FechaCarga,
                FechaModificacion = DateTime.Now,
                //valor para indicar que el dictamen fue borrado y no modificado
                Borrado = true,
                IdUsuarioModificacion =  FrameworkMVC.Security.LoginService.Current.UsuarioID
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
            if (!FrameworkMVC.Security.LoginService.IsAllowed(new[] { Models.Rol.CARGAR.ToString() }))
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
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (dictamenes != null)
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
                            HaySujetoControlado = false,
                            Asunto = asunto,
                            IdAsunto = asunto != null ? asunto.Id : 0,
                            ArchivoPDF = null,
                            FechaModificacion = DateTime.Now,
                            IdSujetoControlado = null,
                            IdUsuarioModificacion =  FrameworkMVC.Security.LoginService.Current.UsuarioID,
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
                    var basePath = Server.MapPath("~/Archivos");
                    var filePath = Path.Combine("~", "Archivos", fileName + extension);

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
            return Json(new { archivosPDFError, dictamenesError });
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
            MatchCollection matches;

            //Regex numeroGDE = new Regex("IF-[0-9]{4}-[0-9]+-APN-[A-Z]+#[A-Z]+", RegexOptions.IgnoreCase);
            //matches = numeroGDE.Matches(contenido);
            //try
            //{
            //    //obtiene el primer resultado
            //    dict.NroGDE = matches[0].Value;
            //}
            ////en caso de que no encuentre ninguna, no hace nada y deja el valor por defecto
            //catch { }

            Regex numeroExpediente = new Regex("[E][X] *-* *[0-9]{4} *- *[0-9]+? ?- ?-? ?APN *- *[A-Z]+ *# *[A-Z]+|[E][X] *-* *[0-9]{4} *- *[0-9]+", RegexOptions.IgnoreCase);
            matches = numeroExpediente.Matches(contenido);
            try
            {
                //obtiene el primer resultado y normaliza el campo
                //ya que el numero de expediente es extraido directamente de la descripcion del dictamen
                dict.NroExpediente = matches[0].Value.Replace(" ", "").Replace("--", "-");
            }
            catch { }

            Regex date = new Regex("[0-9]{4}.[0-9]{2}.[0-9]{2} [0-9]{2}:[0-9]{2}:[0-9]{2}", RegexOptions.IgnoreCase);
            matches = date.Matches(contenido);
            try
            {
                DateTime fecha;
                //trata de parsear la fecha con ese formato
                DateTime.TryParseExact(matches[0].Value, "yyyy.MM.dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out fecha);
                //si la instruccion anterior no eleva una excepcion, guarda la fecha en el dictamen
                dict.FechaCarga = fecha;
            }
            catch
            {
                //en caso de no lograrlo o no haber resultado de la busqueda, devuelve la fecha actual
                dict.FechaCarga = DateTime.Now;
            }
            return dict;
        }
    }
}
