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
                .FirstOrDefaultAsync(m => m.id == id);
            if (dictamen == null)
            {
                return NotFound();
            }

            return View(dictamen);
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
            var fileName = Path.GetFileNameWithoutExtension(file.FileName);
            var extension = Path.GetExtension(file.FileName);
            var fileModel1 = new FileOnDatabaseModel
            {
                CreatedOn = DateTime.UtcNow,
                FileType = file.ContentType,
                Extension = extension,
                Name = fileName
            };
            using (var dataStream = new MemoryStream())
            {
                await file.CopyToAsync(dataStream);
                fileModel1.Data = dataStream.ToArray();
            }
            _context.FilesOnDatabase.Add(fileModel1);

            var basePath = Path.Combine(Directory.GetCurrentDirectory() + "\\Files\\");
            bool basePathExists = System.IO.Directory.Exists(basePath);
            if (!basePathExists) Directory.CreateDirectory(basePath);
            var filePath = Path.Combine(basePath, file.FileName);
            if (!System.IO.File.Exists(filePath))
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                var fileModel2 = new FileOnFileSystemModel
                {
                    CreatedOn = DateTime.Now,
                    FileType = file.ContentType,
                    Extension = extension,
                    Name = fileName,
                    FilePath = filePath
                };
                _context.FilesOnFileSystem.Add(fileModel2);
                _context.SaveChanges();
            }

            var contenido = FileController.ExtractTextFromPdf(filePath);


            var dictamen = ExtratDictamenFromString(contenido);

            ViewData["IdAsunto"] = new SelectList(_context.Asunto, "Id", "Id", dictamen.IdAsunto);
            ViewData["IdSujetoObligado"] = new SelectList(_context.SujetoObligado, "id", "id", dictamen.IdSujetoObligado);
            ViewData["IdTipoDictamen"] = new SelectList(_context.TipoDictamen, "Id", "Id", dictamen.IdTipoDictamen);
            return RedirectToAction("Create", dictamen);
        }


        // GET: Dictamenes/Create
        public IActionResult Create()
        {
            ViewData["IdAsunto"] = new SelectList(_context.Asunto, "Id", "Id");
            ViewData["IdSujetoObligado"] = new SelectList(_context.SujetoObligado, "id", "id");
            ViewData["IdTipoDictamen"] = new SelectList(_context.TipoDictamen, "Id", "Id");
            return View();
        }

        // POST: Dictamenes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,NroGDE,NroExpediente,FechaCarga,Detalle,EsPublico,IdArchivoLigado,IdSujetoObligado,IdAsunto,IdTipoDictamen,IdUsuarioGenerador,EstaActivo,FechaModificacion,IdUsuarioModificacion")] Dictamen dictamen)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dictamen);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdAsunto"] = new SelectList(_context.Asunto, "Id", "Id", dictamen.IdAsunto);
            ViewData["IdSujetoObligado"] = new SelectList(_context.SujetoObligado, "id", "id", dictamen.IdSujetoObligado);
            ViewData["IdTipoDictamen"] = new SelectList(_context.TipoDictamen, "Id", "Id", dictamen.IdTipoDictamen);
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
            ViewData["IdAsunto"] = new SelectList(_context.Asunto, "Id", "Id", dictamen.IdAsunto);
            ViewData["IdSujetoObligado"] = new SelectList(_context.SujetoObligado, "id", "id", dictamen.IdSujetoObligado);
            ViewData["IdTipoDictamen"] = new SelectList(_context.TipoDictamen, "Id", "Id", dictamen.IdTipoDictamen);
            return View(dictamen);
        }

        // POST: Dictamenes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,NroGDE,NroExpediente,FechaCarga,Detalle,EsPublico,IdArchivoLigado,IdSujetoObligado,IdAsunto,IdTipoDictamen,IdUsuarioGenerador,EstaActivo,FechaModificacion,IdUsuarioModificacion")] Dictamen dictamen)
        {
            if (id != dictamen.id)
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
                    if (!DictamenExists(dictamen.id))
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
            ViewData["IdAsunto"] = new SelectList(_context.Asunto, "Id", "Id", dictamen.IdAsunto);
            ViewData["IdSujetoObligado"] = new SelectList(_context.SujetoObligado, "id", "id", dictamen.IdSujetoObligado);
            ViewData["IdTipoDictamen"] = new SelectList(_context.TipoDictamen, "Id", "Id", dictamen.IdTipoDictamen);
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
                .FirstOrDefaultAsync(m => m.id == id);
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
            return _context.Dictamenes.Any(e => e.id == id);
        }

        private static Dictamen ExtratDictamenFromString(string contenido)
        {
            Regex numeroGDE = new Regex("IF-[0-9]{4}-[0-9]{7,8}-APN-(DARH|DCTA|CGN|GAJ|GTYN)#(M[TJH]|SSN)", RegexOptions.IgnoreCase);

            MatchCollection matches = numeroGDE.Matches(contenido);

            string nroGDE = matches[0].Value;

            Regex numeroExpediente = new Regex("EX-[0-9]{4}-[0-9]{7,8}-APN-(DARH|DCTA|CGN|GA|GTYN)#(M[TJH]|SSN)", RegexOptions.IgnoreCase);

            matches = numeroExpediente.Matches(contenido);

            string nroExpediente = matches[0].Value;

            Regex date = new Regex("[0-9]{4}.[0-9]{2}.[0-9]{2} [0-9]{2}:[0-9]{2}:[0-9]{2}", RegexOptions.IgnoreCase);

            matches = date.Matches(contenido);
            DateTime fechaCarga = DateTime.ParseExact(matches[0].Value, "yyyy.MM.dd HH:mm:ss", null);

      

            //matches = dateUsuarioGenerador.Matches(contenido);
            //Console.WriteLine(matches.Count);
            //string usuarioGenerador = matches[0].Value;
            //Console.WriteLine(usuarioGenerador);

            Dictamen dict = new Dictamen();

            dict.NroGDE = nroGDE;
            dict.NroExpediente = nroExpediente;
            dict.FechaCarga = fechaCarga;
            return dict;


        }

    }
}
