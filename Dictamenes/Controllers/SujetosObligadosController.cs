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
            var dictamenesDbContext = _context.Clientes.Include(s => s.TipoSujetoObligado);
            return View(await dictamenesDbContext.ToListAsync());
        }

        // GET: SujetosObligados/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sujetoObligado = await _context.Clientes
                .Include(s => s.TipoSujetoObligado)
                .FirstOrDefaultAsync(m => m.CuilCuit == id);
            if (sujetoObligado == null)
            {
                return NotFound();
            }

            return View(sujetoObligado);
        }

        // GET: SujetosObligados/Create
        public IActionResult Create()
        {
            ViewData["IdTipoSujetoObligado"] = new SelectList(_context.Empleados, "Id", "Id");
            return View();
        }

        // POST: SujetosObligados/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CuilCuit,Nombre,Apellido,RazonSocial,IdTipoSujetoObligado,EstaActivo,FechaModificacion,IdUsuarioModificacion")] SujetoObligado sujetoObligado)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sujetoObligado);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdTipoSujetoObligado"] = new SelectList(_context.Empleados, "Id", "Id", sujetoObligado.IdTipoSujetoObligado);
            return View(sujetoObligado);
        }

        // GET: SujetosObligados/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sujetoObligado = await _context.Clientes.FindAsync(id);
            if (sujetoObligado == null)
            {
                return NotFound();
            }
            ViewData["IdTipoSujetoObligado"] = new SelectList(_context.Empleados, "Id", "Id", sujetoObligado.IdTipoSujetoObligado);
            return View(sujetoObligado);
        }

        // POST: SujetosObligados/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CuilCuit,Nombre,Apellido,RazonSocial,IdTipoSujetoObligado,EstaActivo,FechaModificacion,IdUsuarioModificacion")] SujetoObligado sujetoObligado)
        {
            if (id != sujetoObligado.CuilCuit)
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
                    if (!SujetoObligadoExists(sujetoObligado.CuilCuit))
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
            ViewData["IdTipoSujetoObligado"] = new SelectList(_context.Empleados, "Id", "Id", sujetoObligado.IdTipoSujetoObligado);
            return View(sujetoObligado);
        }

        // GET: SujetosObligados/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sujetoObligado = await _context.Clientes
                .Include(s => s.TipoSujetoObligado)
                .FirstOrDefaultAsync(m => m.CuilCuit == id);
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
            var sujetoObligado = await _context.Clientes.FindAsync(id);
            _context.Clientes.Remove(sujetoObligado);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SujetoObligadoExists(int id)
        {
            return _context.Clientes.Any(e => e.CuilCuit == id);
        }
    }
}
