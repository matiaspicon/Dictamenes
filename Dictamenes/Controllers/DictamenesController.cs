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
            var dictamenesDbContext = _context.Dictamenes.Include(d => d.Asunto).Include(d => d.SujetoObligado).Include(d => d.TipoDictamen);
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
            var fileName = Path.GetFileNameWithoutExtension(file.FileName);
            var extension = Path.GetExtension(file.FileName);
            
            // compruebo directorio
            var basePath = Path.Combine(Directory.GetCurrentDirectory() + "\\Files\\");
            bool basePathExists = System.IO.Directory.Exists(basePath);
            if (!basePathExists) Directory.CreateDirectory(basePath);

            var filePath = Path.Combine(basePath, file.FileName);
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
            ViewData["IdAsunto"] = new SelectList(_context.Asunto, "Id", "Descripcion");
            ViewData["IdSujetoObligado"] = new SelectList(_context.SujetoObligado.Where(m => m.RazonSocial != null), "id", "RazonSocial");
            ViewData["IdTipoDictamen"] = new SelectList(_context.TipoDictamen, "Id", "Descripcion");
            ViewData["TipoSujetoObligado"] = new SelectList(_context.TipoSujetoObligado, "Id", "Descripcion");
            ViewData["IdDenunciante"] = _context.TipoSujetoObligado.FirstOrDefault(m => m.Descripcion == "Denunciante").Id;
            return View("Create", dictamen);
        }


        // GET: Dictamenes/Create
        public IActionResult Create()
        {
            ViewData["IdAsunto"] = new SelectList(_context.Asunto, "Id", "Descripcion");
            ViewData["IdSujetoObligado"] = new SelectList(_context.SujetoObligado.Where(m => m.RazonSocial != null), "id", "RazonSocial");
            ViewData["IdTipoDictamen"] = new SelectList(_context.TipoDictamen, "Id", "Descripcion");
            ViewData["TipoSujetoObligado"] = new SelectList(_context.TipoSujetoObligado, "Id", "Descripcion");
            ViewData["IdDenunciante"] = _context.TipoSujetoObligado.FirstOrDefault(m => m.Descripcion == "Denunciante").Id;
            return View();
        }

        // POST: Dictamenes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,NroGDE,NroExpediente,FechaCarga,Detalle,EsPublico,IdArchivoPDF,IdSujetoObligado,IdAsunto,IdTipoDictamen,IdUsuario,EstaActivo,FechaModificacion,IdUsuarioModificacion")] Dictamen dictamen, [Bind("CuilCuit, Nombre, Apellido, IdTipoSujetoObligado")] SujetoObligado sujetoObligado)
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
                _context.Add(sujetoObligado);
                await _context.SaveChangesAsync();

                dictamen.IdSujetoObligado = sujetoObligado.id;

            }

            if (ModelState.IsValid)
            {
                _context.Add(dictamen);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdAsunto"] = new SelectList(_context.Asunto, "Id", "Descripcion");
            ViewData["IdSujetoObligado"] = new SelectList(_context.SujetoObligado.Where(m => m.RazonSocial != null), "id", "RazonSocial");
            ViewData["IdTipoDictamen"] = new SelectList(_context.TipoDictamen, "Id", "Descripcion");
            ViewData["TipoSujetoObligado"] = new SelectList(_context.TipoSujetoObligado, "Id", "Descripcion");
            ViewData["IdDenunciante"] = _context.TipoSujetoObligado.FirstOrDefault(m => m.Descripcion == "Denunciante").Id;
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
            ViewData["IdAsunto"] = new SelectList(_context.Asunto, "Id", "Descripcion");
            ViewData["IdSujetoObligado"] = new SelectList(_context.SujetoObligado.Where(m => m.RazonSocial != null), "id", "RazonSocial");
            ViewData["IdTipoDictamen"] = new SelectList(_context.TipoDictamen, "Id", "Descripcion");
            ViewData["TipoSujetoObligado"] = new SelectList(_context.TipoSujetoObligado, "Id", "Descripcion");
            ViewData["IdDenunciante"] = _context.TipoSujetoObligado.FirstOrDefault(m => m.Descripcion == "Denunciante").Id;
            return View(dictamen);
        }

        // POST: Dictamenes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,NroGDE,NroExpediente,FechaCarga,Detalle,EsPublico,IdArchivoPDF,IdSujetoObligado,IdAsunto,IdTipoDictamen,IdUsuario,EstaActivo,FechaModificacion,IdUsuarioModificacion")] Dictamen dictamen)
        {
            if (id != dictamen.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dictamen);
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
            ViewData["IdAsunto"] = new SelectList(_context.Asunto, "Id", "Descripcion");
            ViewData["IdSujetoObligado"] = new SelectList(_context.SujetoObligado.Where(m => m.RazonSocial != null), "id", "RazonSocial");
            ViewData["IdTipoDictamen"] = new SelectList(_context.TipoDictamen, "Id", "Descripcion");
            ViewData["TipoSujetoObligado"] = new SelectList(_context.TipoSujetoObligado, "Id", "Descripcion");
            ViewData["IdDenunciante"] = _context.TipoSujetoObligado.FirstOrDefault(m => m.Descripcion == "Denunciante").Id;
            return View(dictamen);
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
            Regex numeroGDE = new Regex("IF-[0-9]{4}-[0-9]{7,8}-APN-(DARH|DCTA|CGN|GAJ|GTYN)#(M[TJH]|SSN)", RegexOptions.IgnoreCase);
            // tpdo poner try catch para cuando no se encuentren resultados
            MatchCollection matches = numeroGDE.Matches(contenido);

            string nroGDE = matches[0].Value;

            Regex numeroExpediente = new Regex("[E][X] *-* *[0-9]{4} *- *[0-9]{7,9}? ?- ?-? ?APN *- *(DARH|DCTA|CGN|GA|GTYN|GAYR) *# *(M[TJH]|SSN)|[E][X] *-* *[0-9]{4} *- *[0-9]{7,9}", RegexOptions.IgnoreCase);

            matches = numeroExpediente.Matches(contenido);

            string nroExpediente = matches[0].Value.Replace(" ","").Replace("--", "-") ;



            Regex date = new Regex("[0-9]{4}.[0-9]{2}.[0-9]{2} [0-9]{2}:[0-9]{2}:[0-9]{2}", RegexOptions.IgnoreCase);

            matches = date.Matches(contenido);
            DateTime fechaCarga = DateTime.ParseExact(matches[0].Value, "yyyy.MM.dd HH:mm:ss", null);

      

            

            Dictamen dict = new Dictamen();

            dict.NroGDE = nroGDE;
            dict.NroExpediente = nroExpediente;
            dict.FechaCarga = fechaCarga;
            return dict;


        }

    }
}
