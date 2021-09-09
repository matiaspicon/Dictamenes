using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Dictamenes.Database;
using Dictamenes.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

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
        public IActionResult Create()
        {
            ViewData["IdAsunto"] = new SelectList(_context.Asunto, "Id", "Descripcion");
            ViewData["IdSujetoObligado"] = new SelectList(_context.SujetoObligado.Where(m => m.RazonSocial != null), "id", "RazonSocial");
            ViewData["IdTipoDictamen"] = new SelectList(_context.TipoDictamen, "Id", "Descripcion");
            ViewData["TipoSujetoObligado"] = new SelectList(_context.TipoSujetoObligado, "Id", "Descripcion");
            return View();
        }

        // POST: Dictamenes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,NroGDE,NroExpediente,FechaCarga,Detalle,EsPublico,IdArchivoLigado,IdSujetoObligado,IdAsunto,IdTipoDictamen,IdUsuarioGenerador,EstaActivo")] Dictamen dictamen, [Bind("CuilCuit, Nombre, Apellido, IdTipoSujetoObligado")] SujetoObligado sujetoObligado, [Bind("file")] List<IFormFile> file)
        {
            dictamen.IdUsuarioModificacion = 0;
            //dictamen.IdUsuarioModificacion = _context.Usuario;
            dictamen.FechaModificacion = DateTime.Now;
            dictamen.EstaActivo = true;

            //dictamen.IdUsuarioGenerador = _context.Usuario


            //if(sujetoObligado.CuilCuit != 0)
            //{
            //    sujetoObligado.IdUsuarioModificacion = 0;
            //    sujetoObligado.FechaModificacion = DateTime.Now;
            //    sujetoObligado.EstaActivo = true;
            //    _context.Add(sujetoObligado);
            //}


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
            ViewData["IdSujetoObligado"] = new SelectList(_context.SujetoObligado, "CuilCuit", "CuilCuit", dictamen.IdSujetoObligado);
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
            ViewData["IdAsunto"] = new SelectList(_context.Asunto, "Id", "Descripcion", dictamen.IdAsunto);
            ViewData["IdSujetoObligado"] = new SelectList(_context.SujetoObligado, "CuilCuit", "RazonSocial", dictamen.IdSujetoObligado);
            ViewData["IdTipoDictamen"] = new SelectList(_context.TipoDictamen, "Id", "Descripcion", dictamen.IdTipoDictamen);
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
    }
}
