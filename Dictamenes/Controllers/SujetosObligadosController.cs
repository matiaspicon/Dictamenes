using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Dictamenes.Database;

namespace Dictamenes.Models
{
    public class SujetosObligadosController : Controller
    {
        private readonly DictamenesDbContext _context;

        public SujetosObligadosController(DictamenesDbContext context)
        {
            _context = context;
        }

        // GET: SujetosObligados
        public async Task<IActionResult> Index()
        {
            var dictamenesDbContext = _context.SujetoObligado.Include(s => s.TipoSujetoObligado);
            return View(await dictamenesDbContext.ToListAsync());
        }

        // GET: SujetosObligados/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sujetoObligado = await _context.SujetoObligado
                .Include(s => s.TipoSujetoObligado)
                .FirstOrDefaultAsync(m => m.id == id);
            if (sujetoObligado == null)
            {
                return NotFound();
            }

            return View(sujetoObligado);
        }

        // GET: SujetosObligados/Create
        public IActionResult Create()
        {
            ViewData["IdTipoSujetoObligado"] = new SelectList(_context.TipoSujetoObligado, "Id", "Id");
            return View();
        }

        // POST: SujetosObligados/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,CuilCuit,Nombre,Apellido,RazonSocial,IdTipoSujetoObligado,EstaActivo,FechaModificacion,IdUsuarioModificacion")] SujetoObligado sujetoObligado)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sujetoObligado);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdTipoSujetoObligado"] = new SelectList(_context.TipoSujetoObligado, "Id", "Id", sujetoObligado.IdTipoSujetoObligado);
            return View(sujetoObligado);
        }

        // GET: SujetosObligados/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sujetoObligado = await _context.SujetoObligado.FindAsync(id);
            if (sujetoObligado == null)
            {
                return NotFound();
            }
            ViewData["IdTipoSujetoObligado"] = new SelectList(_context.TipoSujetoObligado, "Id", "Id", sujetoObligado.IdTipoSujetoObligado);
            return View(sujetoObligado);
        }

        // POST: SujetosObligados/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,CuilCuit,Nombre,Apellido,RazonSocial,IdTipoSujetoObligado,EstaActivo,FechaModificacion,IdUsuarioModificacion")] SujetoObligado sujetoObligado)
        {
            if (id != sujetoObligado.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sujetoObligado);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SujetoObligadoExists(sujetoObligado.id))
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
            ViewData["IdTipoSujetoObligado"] = new SelectList(_context.TipoSujetoObligado, "Id", "Id", sujetoObligado.IdTipoSujetoObligado);
            return View(sujetoObligado);
        }

        // GET: SujetosObligados/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sujetoObligado = await _context.SujetoObligado
                .Include(s => s.TipoSujetoObligado)
                .FirstOrDefaultAsync(m => m.id == id);
            if (sujetoObligado == null)
            {
                return NotFound();
            }

            return View(sujetoObligado);
        }

        // POST: SujetosObligados/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sujetoObligado = await _context.SujetoObligado.FindAsync(id);
            _context.SujetoObligado.Remove(sujetoObligado);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SujetoObligadoExists(int id)
        {
            return _context.SujetoObligado.Any(e => e.id == id);
        }
    }
}
