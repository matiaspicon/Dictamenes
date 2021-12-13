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
using Dictamenes.Security;

namespace Dictamenes.Controllers
{
    [Authorize]
    public class DictamenesController : Controller
    {
        private DictamenesDbContext db = new DictamenesDbContext();

        // GET: Dictamen
        public ActionResult Index()
        {
            //El usuario que no sea tenga rol de Cargar o Consulta no podra acceder a esta opcion
            if (Security.LoginService.Current.GrupoNombre != Models.Rol.CARGAR.ToString() && Security.LoginService.Current.GrupoNombre != Models.Rol.CONSULTAR.ToString())
            {
                return RedirectToAction("ErrorNoPermisos", "Login");
            }

            //Trae todos los dictamenes que sean del anio actual
            var dictamenes = db.Dictamenes.Where(d => d.FechaCarga.Year == DateTime.Today.Year).Include(d => d.Asunto).Include(d => d.TipoDictamen);
            ViewData["IdAsunto"] = new SelectList(db.Asuntos.Where(m => m.EstaHabilitado).OrderBy(m => m.Descripcion).OrderBy(m => m.Descripcion), "Id", "Descripcion");
            ViewData["IdSujetoObligado"] = new SelectList(db.SujetosObligados.Where(m => m.RazonSocial != null && m.EstaHabilitado).OrderBy(m => m.RazonSocial).OrderBy(m => m.RazonSocial), "Id", "RazonSocial");
            ViewData["IdTipoDictamen"] = new SelectList(db.TiposDictamen.Where(m => m.EstaHabilitado).OrderBy(m => m.Descripcion), "Id", "Descripcion");
            ViewData["TipoSujetoObligado"] = new SelectList(db.TiposSujetoObligado.Where(m => m.EstaHabilitado && m.Descripcion != "Denunciante").OrderBy(m => m.Descripcion), "Id", "Descripcion");
            return View(dictamenes.ToList());
        }

        // GET: Dictamen/Details/5
        public ActionResult Details(int? id)
        {
            //El usuario que no sea tenga rol de Cargar o Consulta no podra acceder a esta opcion
            if (Security.LoginService.Current.GrupoNombre != Models.Rol.CARGAR.ToString() && Security.LoginService.Current.GrupoNombre != Models.Rol.CONSULTAR.ToString())
            {
                return RedirectToAction("ErrorNoPermisos", "Login");
            }

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
            return View(dictamen);
        }


        // GET: Dictamenes/Create
        public ActionResult CargarFile()
        {

            //El usuario que no sea tenga rol de Cargar no podra acceder a esta opcion
            if (Security.LoginService.Current.GrupoNombre != Models.Rol.CARGAR.ToString())
            {
                return RedirectToAction("ErrorNoPermisos", "Login");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CargarFile(HttpPostedFileBase file)
        {
            //El usuario que no sea tenga rol de Cargar no podra acceder a esta opcion
            if (Security.LoginService.Current.GrupoNombre != Models.Rol.CARGAR.ToString())
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
                ViewData["IdSujetoObligado"] = new SelectList(db.SujetosObligados.Where(m => m.RazonSocial != null && m.EstaHabilitado).OrderBy(m => m.RazonSocial), "Id", "RazonSocial");
                ViewData["IdTipoDictamen"] = new SelectList(db.TiposDictamen.Where(m => m.EstaHabilitado).OrderBy(m => m.Descripcion), "Id", "Descripcion");
                ViewData["TipoSujetoObligado"] = new SelectList(db.TiposSujetoObligado.Where(m => m.EstaHabilitado).OrderBy(m => m.Descripcion), "Id", "Descripcion");

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

            ViewData["IdAsunto"] = new SelectList(db.Asuntos.Where(m => m.EstaHabilitado).OrderBy(m => m.Descripcion), "Id", "Descripcion");
            ViewData["IdSujetoObligado"] = new SelectList(db.SujetosObligados.Where(m => m.RazonSocial != null && m.EstaHabilitado).OrderBy(m => m.RazonSocial), "Id", "RazonSocial");
            ViewData["IdTipoDictamen"] = new SelectList(db.TiposDictamen.Where(m => m.EstaHabilitado).OrderBy(m => m.Descripcion), "Id", "Descripcion");
            ViewData["TipoSujetoObligado"] = new SelectList(db.TiposSujetoObligado.Where(m => m.EstaHabilitado).OrderBy(m => m.Descripcion), "Id", "Descripcion");

            // cargo la informacion para el formulario Create y devuelvo la VIEW del create con la informacion extraida del Archivo
            //que esta dentro de la variable dictamen
            return View("Create", dictamen);
        }


        // GET: Dictamen/Create
        public ActionResult Create()
        {
            //El usuario que no sea tenga rol de Cargar no podra acceder a esta opcion
            if (Security.LoginService.Current.GrupoNombre != Models.Rol.CARGAR.ToString())
            {
                return RedirectToAction("ErrorNoPermisos", "Login");
            }

            ViewData["IdAsunto"] = new SelectList(db.Asuntos.Where(m => m.EstaHabilitado).OrderBy(m => m.Descripcion), "Id", "Descripcion");
            ViewData["IdSujetoObligado"] = new SelectList(db.SujetosObligados.Where(m => m.RazonSocial != null && m.EstaHabilitado).OrderBy(m => m.RazonSocial), "Id", "RazonSocial");
            ViewData["IdTipoDictamen"] = new SelectList(db.TiposDictamen.Where(m => m.EstaHabilitado).OrderBy(m => m.Descripcion), "Id", "Descripcion");
            ViewData["TipoSujetoObligado"] = new SelectList(db.TiposSujetoObligado.Where(m => m.EstaHabilitado).OrderBy(m => m.Descripcion), "Id", "Descripcion");
            return View();
        }


        // POST: Dictamen/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,NroGDE,NroExpediente,FechaCarga,Detalle,EsPublico,IdArchivoPDF,IdSujetoObligado,IdAsunto,IdTipoDictamen,Borrado,EstaActivo,FechaModificacion,IdUsuarioModificacion, SujetoObligado")] Dictamen dictamen)
        {
            //El usuario que no sea tenga rol de Cargar no podra acceder a esta opcion
            if (Security.LoginService.Current.GrupoNombre != Models.Rol.CARGAR.ToString())
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
            dictamen.IdUsuarioModificacion =  Security.LoginService.Current.UsuarioID;
            dictamen.NroGDE = dictamen.NroGDE.ToUpper();
            dictamen.NroExpediente = dictamen.NroExpediente.ToUpper();
            dictamen.Detalle = dictamen.Detalle.ToUpper();
            dictamen.EsPublico = false;


            //compruebo si la data del formulario del denunciante es distinto al valor por defecto
            if (dictamen.SujetoObligado.CuilCuit > 0)
            {
                //busco si existe otro sujeto obligado con el mismo cuilCuit
                SujetoObligado sujetoObligadoExistente = db.SujetosObligados.FirstOrDefault(s => s.CuilCuit == dictamen.SujetoObligado.CuilCuit);
                if (sujetoObligadoExistente != null)
                {
                    //chequeo si el sujetoObligado existente no es denunciante
                    if (sujetoObligadoExistente.IdTipoSujetoObligado != db.TiposSujetoObligado.First(m => m.Descripcion == "Denunciante").Id)
                    {
                        //devuelvo un error ya que ese cuilCuit corresponde a una empresa
                        ModelState.AddModelError("SujetoObligado.CuilCuit", "Ya existe un Sujeto Obligado con ese Número de Cuil");
                    }
                    else
                    {
                        //en caso de ser un denunciante, simplemente uso el sujetoObligado denunciante existente
                        dictamen.IdSujetoObligado = sujetoObligadoExistente.Id;
                        dictamen.SujetoObligado = sujetoObligadoExistente;
                    }
                }
                //en caso de no existir un sujetoObligado con el mismo cuilCuit, lo creo
                else
                {
                    dictamen.SujetoObligado.IdTipoSujetoObligado = db.TiposSujetoObligado.First(m => m.Descripcion == "Denunciante").Id;
                    dictamen.SujetoObligado.IdUsuarioModificacion =  Security.LoginService.Current.UsuarioID;
                    dictamen.SujetoObligado.FechaModificacion = DateTime.Now;
                    db.SujetosObligados.Add(dictamen.SujetoObligado);
                }
            }
            //en caso de no haber un valor en el formulario de Denunciante
            else
            {
                //remuevo la comprobacion del campo cuilCuit
                ModelState.Remove("SujetoObligado.CuilCuit");
                //seteo el SujetoObligado para que corresponda con el IdSujetoObligado
                //en caso que el IdSujetoObligado sea null Find() devuelve null tambien
                dictamen.SujetoObligado = db.SujetosObligados.Find(dictamen.IdSujetoObligado);
            }

            //seteo el valor del HaySujetoObligado basado en el valor de IdSujetoObligado
            dictamen.HaySujetoObligado = dictamen.IdSujetoObligado != null;

            if (ModelState.IsValid)
            {
                db.Dictamenes.Add(dictamen);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            if (dictamen.SujetoObligado != null && dictamen.SujetoObligado.Nombre == null) dictamen.SujetoObligado.CuilCuit = 0; // en caso de que haya un idSujetoObligado seleccionado (una empresa de la tabla) se resetea el cuilCuit
            ViewBag.IdAsunto = new SelectList(db.Asuntos.Where(m => m.EstaHabilitado).OrderBy(m => m.Descripcion), "Id", "Descripcion", dictamen.IdAsunto);
            ViewBag.IdSujetoObligado = new SelectList(db.SujetosObligados.Where(m => m.RazonSocial != null && m.EstaHabilitado).OrderBy(m => m.RazonSocial), "Id", "RazonSocial", dictamen.IdSujetoObligado);
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
            var idTipoSujetoObligado = busqueda.EsDenunciante ? db.TiposSujetoObligado.First(d => d.Descripcion == "Denunciante").Id : busqueda.IdTipoSujetoObligado;

            //ejecuto el StoreProcedure "sp_FiltrarDictamenes" de la base de datos con los valores pasado por parametro
            var dictamenes = db.Sp_FiltrarDictamenes(busqueda.NroGDE, busqueda.NroExp, busqueda.FechaCargaInicio, busqueda.FechaCargaFinal,
                busqueda.Detalle, busqueda.Contenido, busqueda.IdAsunto, busqueda.IdTipoDictamen, busqueda.IdSujetoObligado,
                idTipoSujetoObligado, busqueda.CuilCuit, busqueda.Nombre, busqueda.Apellido);

            //cargo las listas desplegables
            ViewData["IdAsunto"] = new SelectList(db.Asuntos.Where(m => m.EstaHabilitado).OrderBy(m => m.Descripcion), "Id", "Descripcion");
            ViewData["IdSujetoObligado"] = new SelectList(db.SujetosObligados.Where(m => m.RazonSocial != null && m.EstaHabilitado).OrderBy(m => m.RazonSocial), "Id", "RazonSocial");
            ViewData["IdTipoDictamen"] = new SelectList(db.TiposDictamen.Where(m => m.EstaHabilitado).OrderBy(m => m.Descripcion), "Id", "Descripcion");
            ViewData["TipoSujetoObligado"] = new SelectList(db.TiposSujetoObligado.Where(m => m.EstaHabilitado && m.Descripcion != "Denunciante").OrderBy(m => m.Descripcion), "Id", "Descripcion");

            //envio los parametros actuales de busqueda para que puedan ser mostrado en la vista
            ViewData["Busqueda"] = busqueda;

            //envio los dictamenes filtrados a la vista
            return View("Index", dictamenes);
        }


        // GET: Dictamen/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Security.LoginService.Current.GrupoNombre != Models.Rol.CARGAR.ToString())
            {
                return View("ErrorNoPermisos", "Login");
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

            ViewBag.IdAsunto = new SelectList(db.Asuntos.OrderBy(m => m.Descripcion), "Id", "Descripcion", dictamen.IdAsunto);
            ViewBag.IdSujetoObligado = new SelectList(db.SujetosObligados.Where(m => m.RazonSocial != null).OrderBy(m => m.RazonSocial), "Id", "RazonSocial", dictamen.IdSujetoObligado);
            ViewBag.IdTipoDictamen = new SelectList(db.TiposDictamen.OrderBy(m => m.Descripcion), "Id", "Descripcion", dictamen.IdTipoDictamen);

            return View(dictamen);
        }

        // POST: Dictamen/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,NroGDE,NroExpediente,FechaCarga,Detalle,EsPublico,IdArchivoPDF,IdSujetoObligado,IdAsunto,IdTipoDictamen,Borrado,EstaActivo,FechaModificacion,IdUsuarioModificacion,IdOriginal, SujetoObligado")] Dictamen dictamen, bool EsDenunciante, bool BorrarArchivo, HttpPostedFileBase file)
        {
            //El usuario que no sea tenga rol de Cargar no podra acceder a esta opcion
            if (Security.LoginService.Current.GrupoNombre != Models.Rol.CARGAR.ToString())
            {
                return RedirectToAction("ErrorNoPermisos", "Login");
            }

            //obtengo una copia del dictamen anterior
            Dictamen dictamenViejo = db.Dictamenes.Include(d => d.SujetoObligado).AsNoTracking().First(d => d.Id == dictamen.Id);

            //en caso de que se haya modificado el numero de GDE y que el numero de GDE se encuentre presente en otro dictamen
            if (dictamenViejo.NroGDE != dictamen.NroGDE && db.Dictamenes.FirstOrDefault(d => d.NroGDE == dictamen.NroGDE) != null)
            {
                //se agrega un error en ese campo y se vuelve a solicitar la carga
                ModelState.AddModelError("NroGDE", "Ya existe un Dictamen con ese Número de GDE");
            }


            if (!EsDenunciante)
            {
                //remuevo la comprobacion del campo cuilCuit
                ModelState.Remove("SujetoObligado.CuilCuit");
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

                //limpio y estandarizo la informacion
                dictamen.FechaModificacion = DateTime.Now;
                dictamen.IdUsuarioModificacion =  Security.LoginService.Current.UsuarioID;
                dictamen.NroGDE = dictamen.NroGDE.ToUpper();
                dictamen.NroExpediente = dictamen.NroExpediente.ToUpper();
                dictamen.Detalle = dictamen.Detalle.ToUpper();


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
                    if (dictamen.SujetoObligado != null && dictamenViejo.SujetoObligado != null)
                    {
                        if (dictamen.SujetoObligado.CuilCuit == dictamenViejo.SujetoObligado.CuilCuit)
                        {
                            EsElMismo = true;
                            if (dictamen.SujetoObligado.Nombre != dictamenViejo.SujetoObligado.Nombre || dictamen.SujetoObligado.Apellido != dictamenViejo.SujetoObligado.Apellido)
                            {
                                //hago la modificacion del sujetoObligado

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
                                sujetoObligadoViejo.IdUsuarioModificacion =  Security.LoginService.Current.UsuarioID;
                                sujetoObligadoViejo.FechaModificacion = DateTime.Now;
                                sujetoObligadoViejo.IdTipoSujetoObligado = sujetoObligadoViejo.IdTipoSujetoObligado;
                                sujetoObligadoViejo.EstaHabilitado = sujetoObligadoViejo.EstaHabilitado;

                                db.Entry(sujetoObligadoViejo).State = EntityState.Modified;

                                //guardo al nuevo sujetoObligado dentro del dictamen
                                dictamen.SujetoObligado = sujetoObligadoViejo;

                                //guardo el log del sujetoObligado que se modifico
                                db.SujetosObligadosLog.Add(sujetoObligadoLog);
                                db.SaveChanges();

                            }
                            else
                            {
                                //completo el id del sujeto obligado en caso de que no se hayan realizado modificaciones
                                dictamen.SujetoObligado.Id = dictamen.IdSujetoObligado.Value;
                            }
                        }
                    }
                    if (!EsElMismo)
                    {
                        //busco si existe otro sujeto obligado con el mismo cuilCuit
                        SujetoObligado sujetoObligadoExistente = db.SujetosObligados.FirstOrDefault(s => s.CuilCuit == dictamen.SujetoObligado.CuilCuit);
                        if (sujetoObligadoExistente != null)
                        {
                            //chequeo si el sujetoObligado existente no es denunciante
                            if (sujetoObligadoExistente.IdTipoSujetoObligado != db.TiposSujetoObligado.First(m => m.Descripcion == "Denunciante").Id)
                            {
                                //devuelvo un error ya que ese cuilCuit corresponde a una empresa
                                ModelState.AddModelError("SujetoObligado.CuilCuit", "Ya existe un Sujeto Obligado con ese Número de Cuil");
                            }
                            else
                            {
                                //en caso de ser un denunciante, simplemente uso el sujetoObligado denunciante existente
                                dictamen.SujetoObligado = sujetoObligadoExistente;
                                dictamen.IdSujetoObligado = sujetoObligadoExistente.Id;
                            }
                        }
                        //en caso de no existir un sujetoObligado con el mismo cuilCuit, lo creo
                        else
                        {
                            //se crea el nuevo sujeto obligado
                            dictamen.SujetoObligado.Id = 0;
                            dictamen.SujetoObligado.FechaModificacion = DateTime.Now;
                            dictamen.SujetoObligado.IdTipoSujetoObligado = db.TiposSujetoObligado.FirstOrDefault(d => d.Descripcion == "Denunciante").Id;
                            dictamen.SujetoObligado.IdUsuarioModificacion =  Security.LoginService.Current.UsuarioID;
                            db.SujetosObligados.Add(dictamen.SujetoObligado);
                            db.SaveChanges();
                            //guardo al nuevo sujetoObligado dentro del dictamen
                            dictamen.IdSujetoObligado = dictamen.SujetoObligado.Id;
                        }
                    }
                }
                else //no es denunciante
                {
                    if (dictamen.IdSujetoObligado.HasValue)
                    {
                        //busco el sujeto obligado completo para agregarlo al dictamen
                        dictamen.SujetoObligado = db.SujetosObligados.Find(dictamen.IdSujetoObligado);
                    }
                    else
                    {
                        //si no tiene valor, es porque fue seleccionada la opcion de "Sin sujeto obligado"
                        dictamen.IdSujetoObligado = null;
                        dictamen.SujetoObligado = null;
                    }
                }


                //seteo el valor del HaySujetoObligado basado en el valor de IdSujetoObligado
                dictamen.HaySujetoObligado = dictamen.IdSujetoObligado != null;

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
            ViewBag.IdSujetoObligado = new SelectList(db.SujetosObligados.Where(m => m.RazonSocial != null && m.EstaHabilitado).OrderBy(m => m.RazonSocial), "Id", "RazonSocial", dictamen.IdSujetoObligado);
            ViewBag.IdTipoDictamen = new SelectList(db.TiposDictamen.Where(m => m.EstaHabilitado).OrderBy(m => m.Descripcion), "Id", "Descripcion", dictamen.IdTipoDictamen);
            return View(dictamen);
        }

        // GET: Dictamen/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Security.LoginService.Current.GrupoNombre != Models.Rol.CARGAR.ToString())
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
            if (Security.LoginService.Current.GrupoNombre != Models.Rol.CARGAR.ToString())
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
                //valor para indicar que el dictamen fue borrado y no modificado
                Borrado = true,
                IdUsuarioModificacion =  Security.LoginService.Current.UsuarioID
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
            if (Security.LoginService.Current.GrupoNombre != Models.Rol.CARGAR.ToString())
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
                            HaySujetoObligado = false,
                            Asunto = asunto,
                            IdAsunto = asunto != null ? asunto.Id : 0,
                            ArchivoPDF = null,
                            FechaModificacion = DateTime.Now,
                            IdSujetoObligado = null,
                            IdUsuarioModificacion =  Security.LoginService.Current.UsuarioID,
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

            Regex numeroGDE = new Regex("IF-[0-9]{4}-[0-9]+-APN-[A-Z]+#[A-Z]+", RegexOptions.IgnoreCase);
            MatchCollection matches = numeroGDE.Matches(contenido);
            try
            {
                //obtiene el primer resultado
                dict.NroGDE = matches[0].Value;
            }
            //en caso de que no encuentre ninguna, no hace nada y deja el valor por defecto
            catch { }

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
