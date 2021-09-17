using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Dictamenes.Database;
using Dictamenes.Models;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;

namespace Dictamenes.Controllers
{
    public class DictamenesController : Controller
    {
        private readonly DictamenesDbContext _context;

        public DictamenesController(DictamenesDbContext context)
        {
            _context = context;
        }

        // GET: Dictamenes
        public async Task<IActionResult> Index()
        {
            var dictamenesDbContext = _context.Dictamenes.Where(m => m.EstaActivo).Include(d => d.Asunto).Include(d => d.SujetoObligado).Include(d => d.TipoDictamen);
            ViewData["IdAsunto"] = new SelectList(_context.Asunto.Where(m => m.EstaActivo && m.EstaHabilitado), "Id", "Descripcion");
            ViewData["IdSujetoObligado"] = new SelectList(_context.SujetoObligado.Where(m => m.EstaActivo && m.RazonSocial != null), "Id", "RazonSocial");
            ViewData["IdTipoDictamen"] = new SelectList(_context.TipoDictamen.Where(m => m.EstaActivo && m.EstaHabilitado), "Id", "Descripcion");
            ViewData["TipoSujetoObligado"] = new SelectList(_context.TipoSujetoObligado.Where(m => m.EstaActivo && m.EstaHabilitado), "Id", "Descripcion");
            return View(await dictamenesDbContext.ToListAsync());
        }

        // GET: Dictamenes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dictamen = await _context.Dictamenes
                .Include(d => d.Asunto)
                .Include(d => d.SujetoObligado)
                .Include(d => d.TipoDictamen)
                .Include(d => d.ArchivoPDF)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dictamen == null)
            {
                return NotFound();
            }
            ViewData["IdDenunciante"] = _context.TipoSujetoObligado.FirstOrDefault(m => m.Descripcion == "Denunciante").Id;
            return View(dictamen);
        }
        public async Task<IActionResult> DownloadFileFromFileSystem(int id)
        {

            var file = await _context.ArchivoPDF.Where(x => x.Id == id).FirstOrDefaultAsync();
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
        public IActionResult CargarFile()
        {
            return View();
        }

        // POST: Dictamenes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CargarFile(IFormFile file)
        {
            Dictamen dictamen = new Dictamen();
            //separo informacion del archivo
            var fileName = Guid.NewGuid().ToString();
            var extension = Path.GetExtension(file.FileName);
            
            // compruebo directorio
            var basePath = Path.Combine(Directory.GetCurrentDirectory() + "\\Files\\");
            bool basePathExists = System.IO.Directory.Exists(basePath);
            if (!basePathExists) Directory.CreateDirectory(basePath);

            var filePath = Path.Combine(basePath, fileName + extension);
            if (!System.IO.File.Exists(filePath))
            {
                // si no existe en el directorio, lo copio
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }               
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
            _context.ArchivoPDF.Add(archivo);
            _context.SaveChanges();    
            // extraigo la informacion del PDF del dictamen y creo el objeto con la misma
            dictamen = ExtratDictamenFromString(archivo.Contenido);
            dictamen.IdArchivoPDF = archivo.Id;
            dictamen.ArchivoPDF = archivo;

            // cargo la informacion para el formulario Create y devuelvo la VIEW del create con la informacion precargada
            // o sin la informacion precargada si no se pudo obtener nada del PDF
            ViewData["IdAsunto"] = new SelectList(_context.Asunto.Where(m => m.EstaActivo && m.EstaHabilitado), "Id", "Descripcion");
            ViewData["IdSujetoObligado"] = new SelectList(_context.SujetoObligado.Where(m => m.EstaActivo && m.RazonSocial != null), "Id", "RazonSocial");
            ViewData["IdTipoDictamen"] = new SelectList(_context.TipoDictamen.Where(m => m.EstaActivo && m.EstaHabilitado), "Id", "Descripcion");
            ViewData["TipoSujetoObligado"] = new SelectList(_context.TipoSujetoObligado.Where(m => m.EstaActivo && m.EstaHabilitado), "Id", "Descripcion");
            return View("Create", dictamen);
        }


        // GET: Dictamenes/Create
        public IActionResult Create()
        {
            ViewData["IdAsunto"] = new SelectList(_context.Asunto.Where(m => m.EstaActivo), "Id", "Descripcion");
            ViewData["IdSujetoObligado"] = new SelectList(_context.SujetoObligado.Where(m => m.EstaActivo && m.RazonSocial != null), "Id", "RazonSocial");
            ViewData["IdTipoDictamen"] = new SelectList(_context.TipoDictamen.Where(m => m.EstaActivo), "Id", "Descripcion");
            ViewData["TipoSujetoObligado"] = new SelectList(_context.TipoSujetoObligado.Where(m => m.EstaActivo), "Id", "Descripcion");
            return View();
        }

        // POST: Dictamenes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,NroGDE,NroExpediente,FechaCarga,Detalle,EsPublico,IdArchivoPDF,IdSujetoObligado,IdAsunto,IdTipoDictamen,IdUsuario,EstaActivo,FechaModificacion,IdUsuarioModificacion")] Dictamen dictamen, [Bind("CuilCuit, Nombre, Apellido")] SujetoObligado sujetoObligado)
        {

            dictamen.IdUsuarioModificacion = 0;
            //dictamen.IdUsuarioModificacion = _context.Usuario;
            dictamen.FechaModificacion = DateTime.Now;
            dictamen.EstaActivo = true;

            if (sujetoObligado.CuilCuit > 0)
            {
                sujetoObligado.IdUsuarioModificacion = 0;
                sujetoObligado.FechaModificacion = DateTime.Now;
                sujetoObligado.EstaActivo = true;
                sujetoObligado.IdTipoSujetoObligado = _context.TipoSujetoObligado.First(m => m.Descripcion == "Denunciante").Id;
                _context.Add(sujetoObligado);
                await _context.SaveChangesAsync();

                dictamen.IdSujetoObligado = sujetoObligado.Id;

            }

            if (ModelState.IsValid)
            {
                _context.Add(dictamen);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdAsunto"] = new SelectList(_context.Asunto.Where(m => m.EstaActivo && m.EstaHabilitado), "Id", "Descripcion");
            ViewData["IdSujetoObligado"] = new SelectList(_context.SujetoObligado.Where(m => m.EstaActivo && m.RazonSocial != null), "Id", "RazonSocial");
            ViewData["IdTipoDictamen"] = new SelectList(_context.TipoDictamen.Where(m => m.EstaActivo && m.EstaHabilitado), "Id", "Descripcion");
            ViewData["TipoSujetoObligado"] = new SelectList(_context.TipoSujetoObligado.Where(m => m.EstaActivo && m.EstaHabilitado), "Id", "Descripcion");
            return View(dictamen);
        }

        // GET: Dictamenes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dictamen = await _context.Dictamenes.FindAsync(id);
            if (dictamen == null)
            {
                return NotFound();
            }
            ViewData["IdAsunto"] = new SelectList(_context.Asunto.Where(m => m.EstaActivo && m.EstaHabilitado), "Id", "Descripcion");
            ViewData["IdSujetoObligado"] = new SelectList(_context.SujetoObligado.Where(m => m.EstaActivo && m.RazonSocial != null), "Id", "RazonSocial");
            ViewData["IdTipoDictamen"] = new SelectList(_context.TipoDictamen.Where(m => m.EstaActivo && m.EstaHabilitado), "Id", "Descripcion");
            ViewData["TipoSujetoObligado"] = new SelectList(_context.TipoSujetoObligado.Where(m => m.EstaActivo && m.EstaHabilitado), "Id", "Descripcion");
            return View(dictamen);
        }

        // POST: Dictamenes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NroGDE,NroExpediente,FechaCarga,Detalle,EsPublico,IdArchivoPDF,IdSujetoObligado,IdAsunto,IdTipoDictamen,IdUsuario,EstaActivo,FechaModificacion,IdUsuarioModificacion")] Dictamen dictamen, IFormFile file)
        {
            if (id != dictamen.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {                    
                    Dictamen dictamenViejo = _context.Dictamenes.AsNoTracking().First(d => d.Id == id);

                    dictamen.IdUsuarioModificacion = 3;
                    //dictamen.IdUsuarioModificacion = _context.Usuario;
                    dictamen.EstaActivo = true;
                    dictamen.FechaModificacion = DateTime.Now;

                    if (file != null)
                    {

                        var fileName = Guid.NewGuid().ToString();
                        var extension = Path.GetExtension(file.FileName);

                        // compruebo directorio
                        var basePath = Path.Combine(Directory.GetCurrentDirectory() + "\\Files\\");
                        bool basePathExists = System.IO.Directory.Exists(basePath);
                        if (!basePathExists) Directory.CreateDirectory(basePath);

                        var filePath = Path.Combine(basePath, fileName + extension);
                        if (!System.IO.File.Exists(filePath))
                        {
                            // si no existe en el directorio, lo copio
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }
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
                        _context.ArchivoPDF.Add(archivo);
                        await _context.SaveChangesAsync();
                        dictamen.IdArchivoPDF = archivo.Id;

                    }

                    _context.Update(dictamen);

                    dictamenViejo.EstaActivo = false;
                    dictamenViejo.Id = 0;

                    _context.Dictamenes.Add(dictamenViejo);
                    await _context.SaveChangesAsync();



                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DictamenExists(dictamen.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdAsunto"] = new SelectList(_context.Asunto.Where(m => m.EstaActivo && m.EstaHabilitado), "Id", "Descripcion");
            ViewData["IdSujetoObligado"] = new SelectList(_context.SujetoObligado.Where(m => m.EstaActivo && m.RazonSocial != null), "Id", "RazonSocial");
            ViewData["IdTipoDictamen"] = new SelectList(_context.TipoDictamen.Where(m => m.EstaActivo && m.EstaHabilitado), "Id", "Descripcion");
            ViewData["TipoSujetoObligado"] = new SelectList(_context.TipoSujetoObligado.Where(m => m.EstaActivo && m.EstaHabilitado), "Id", "Descripcion");
            return View(dictamen);
        }

        [HttpPost]
        public async Task<IActionResult> Buscar([Bind("NroGDE", " NroExp", "FechaCargaInicio", "FechaCargaFinal", "Contenido", "Detalle", "IdAsunto", "IdTipoDictamen", "IdTipoSujetoObligado", "EsDenunciante","IdSujetoObligado", "CuilCuit", "Nombre", "Apellido")]Busqueda busqueda)
        {
            var dictDB = _context.Dictamenes
                .Where(m => m.EstaActivo);

            if(busqueda.NroGDE != null)
            {
                dictDB = dictDB.Where(d => d.NroGDE.Contains(busqueda.NroGDE));
            }
            if (busqueda.NroExp != null)
            {
                dictDB = dictDB.Where(d => d.NroExpediente.Contains(busqueda.NroExp));
            }
            if (busqueda.Detalle != null)
            {
                dictDB = dictDB.Where(d => d.Detalle.Contains(busqueda.Detalle));
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
            if (busqueda.IdSujetoObligado != null)
            {
                dictDB = dictDB.Where(d => d.IdSujetoObligado == busqueda.IdSujetoObligado);
            }
            if (busqueda.IdTipoDictamen != null)
            {
                dictDB = dictDB.Where(d => d.IdTipoDictamen == busqueda.IdTipoDictamen);
            }
            if (busqueda.Contenido != null)
            {
                dictDB = dictDB.Include(d => d.ArchivoPDF).Where(d => d.ArchivoPDF.Contenido.Contains(busqueda.Contenido));
            }
            if (busqueda.IdTipoSujetoObligado != null)
            {
                dictDB = dictDB.Include(d => d.SujetoObligado).Where(d => d.SujetoObligado.IdTipoSujetoObligado == busqueda.IdTipoSujetoObligado);
            }
            if (busqueda.CuilCuit > 0)
            {
                dictDB = dictDB.Include(d => d.SujetoObligado).Where(d => d.SujetoObligado.CuilCuit == busqueda.CuilCuit);
            }

            if (busqueda.Nombre != null)
            {
                dictDB = dictDB.Include(d => d.SujetoObligado).Where(d => d.SujetoObligado.Nombre == busqueda.Nombre);
            }

            if (busqueda.Apellido != null)
            {
                dictDB = dictDB.Include(d => d.SujetoObligado).Where(d => d.SujetoObligado.Apellido == busqueda.Apellido);
            }


            ViewData["IdAsunto"] = new SelectList(_context.Asunto.Where(m => m.EstaActivo && m.EstaHabilitado), "Id", "Descripcion","Seleccione uno");
            ViewData["IdSujetoObligado"] = new SelectList(_context.SujetoObligado.Where(m => m.EstaActivo && m.RazonSocial != null), "Id", "RazonSocial");
            ViewData["IdTipoDictamen"] = new SelectList(_context.TipoDictamen.Where(m => m.EstaActivo && m.EstaHabilitado), "Id", "Descripcion");
            ViewData["TipoSujetoObligado"] = new SelectList(_context.TipoSujetoObligado.Where(m => m.EstaActivo && m.EstaHabilitado), "Id", "Descripcion");
            ViewData["Busqueda"] = busqueda;
            return View("Index",await dictDB.ToListAsync());
        }

        // GET: Dictamenes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dictamen = await _context.Dictamenes
                .Include(d => d.Asunto)
                .Include(d => d.SujetoObligado)
                .Include(d => d.TipoDictamen)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dictamen == null)
            {
                return NotFound();
            }

            return View(dictamen);
        }

        // POST: Dictamenes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dictamen = await _context.Dictamenes.FindAsync(id);
            _context.Dictamenes.Remove(dictamen);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DictamenExists(int id)
        {
            return _context.Dictamenes.Any(e => e.Id == id);
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
