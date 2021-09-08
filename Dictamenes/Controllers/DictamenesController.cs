using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Dictamenes.Database;
using Dictamenes.Models;

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
            var dictamenesDbContext = _context.Categorias.Include(d => d.Asunto).Include(d => d.SujetoObligado).Include(d => d.TipoDictamen);
            return View(await dictamenesDbContext.ToListAsync());
        }

        // GET: Dictamenes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dictamen = await _context.Categorias
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
            ViewData["IdAsunto"] = new SelectList(_context.Asunto, "Id", "Id");
            ViewData["IdSujetoObligado"] = new SelectList(_context.Clientes, "CuilCuit", "CuilCuit");
            ViewData["IdTipoDictamen"] = new SelectList(_context.Compras, "Id", "Id");
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
            ViewData["IdSujetoObligado"] = new SelectList(_context.Clientes, "CuilCuit", "CuilCuit", dictamen.IdSujetoObligado);
            ViewData["IdTipoDictamen"] = new SelectList(_context.Compras, "Id", "Id", dictamen.IdTipoDictamen);
            return View(dictamen);
        }

        // GET: Dictamenes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dictamen = await _context.Categorias.FindAsync(id);
            if (dictamen == null)
            {
                return NotFound();
            }
            ViewData["IdAsunto"] = new SelectList(_context.Asunto, "Id", "Id", dictamen.IdAsunto);
            ViewData["IdSujetoObligado"] = new SelectList(_context.Clientes, "CuilCuit", "CuilCuit", dictamen.IdSujetoObligado);
            ViewData["IdTipoDictamen"] = new SelectList(_context.Compras, "Id", "Id", dictamen.IdTipoDictamen);
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
            ViewData["IdSujetoObligado"] = new SelectList(_context.Clientes, "CuilCuit", "CuilCuit", dictamen.IdSujetoObligado);
            ViewData["IdTipoDictamen"] = new SelectList(_context.Compras, "Id", "Id", dictamen.IdTipoDictamen);
            return View(dictamen);
        }

        // GET: Dictamenes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dictamen = await _context.Categorias
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
            var dictamen = await _context.Categorias.FindAsync(id);
            _context.Categorias.Remove(dictamen);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DictamenExists(int id)
        {
            return _context.Categorias.Any(e => e.id == id);
        }
    }
}
