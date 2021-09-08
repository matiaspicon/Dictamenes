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
    public class TiposSujetoObligadoController : Controller
    {
        private readonly DictamenesDbContext _context;

        public TiposSujetoObligadoController(DictamenesDbContext context)
        {
            _context = context;
        }

        // GET: TiposSujetoObligado
        public async Task<IActionResult> Index()
        {
            return View(await _context.Empleados.ToListAsync());
        }

        // GET: TiposSujetoObligado/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoSujetoObligado = await _context.Empleados
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tipoSujetoObligado == null)
            {
                return NotFound();
            }

            return View(tipoSujetoObligado);
        }

        // GET: TiposSujetoObligado/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TiposSujetoObligado/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Descripcion,EstaHabilitado,EstaActivo,FechaModificacion,IdUsuarioModificacion")] TipoSujetoObligado tipoSujetoObligado)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tipoSujetoObligado);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tipoSujetoObligado);
        }

        // GET: TiposSujetoObligado/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoSujetoObligado = await _context.Empleados.FindAsync(id);
            if (tipoSujetoObligado == null)
            {
                return NotFound();
            }
            return View(tipoSujetoObligado);
        }

        // POST: TiposSujetoObligado/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Descripcion,EstaHabilitado,EstaActivo,FechaModificacion,IdUsuarioModificacion")] TipoSujetoObligado tipoSujetoObligado)
        {
            if (id != tipoSujetoObligado.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tipoSujetoObligado);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TipoSujetoObligadoExists(tipoSujetoObligado.Id))
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
            return View(tipoSujetoObligado);
        }

        // GET: TiposSujetoObligado/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoSujetoObligado = await _context.Empleados
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tipoSujetoObligado == null)
            {
                return NotFound();
            }

            return View(tipoSujetoObligado);
        }

        // POST: TiposSujetoObligado/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tipoSujetoObligado = await _context.Empleados.FindAsync(id);
            _context.Empleados.Remove(tipoSujetoObligado);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TipoSujetoObligadoExists(int id)
        {
            return _context.Empleados.Any(e => e.Id == id);
        }
    }
}
