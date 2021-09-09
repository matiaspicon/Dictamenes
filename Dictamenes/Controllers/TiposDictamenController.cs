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
    public class TiposDictamenController : Controller
    {
        private readonly DictamenesDbContext _context;

        public TiposDictamenController(DictamenesDbContext context)
        {
            _context = context;
        }

        // GET: TiposDictamen
        public async Task<IActionResult> Index()
        {
            return View(await _context.TipoDictamen.ToListAsync());
        }

        // GET: TiposDictamen/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoDictamen = await _context.TipoDictamen
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tipoDictamen == null)
            {
                return NotFound();
            }

            return View(tipoDictamen);
        }

        // GET: TiposDictamen/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TiposDictamen/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Descripcion,EstaHabilitado,EstaActivo,FechaModificacion,IdUsuarioModificacion")] TipoDictamen tipoDictamen)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tipoDictamen);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tipoDictamen);
        }

        // GET: TiposDictamen/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoDictamen = await _context.TipoDictamen.FindAsync(id);
            if (tipoDictamen == null)
            {
                return NotFound();
            }
            return View(tipoDictamen);
        }

        // POST: TiposDictamen/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Descripcion,EstaHabilitado,EstaActivo,FechaModificacion,IdUsuarioModificacion")] TipoDictamen tipoDictamen)
        {
            if (id != tipoDictamen.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tipoDictamen);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TipoDictamenExists(tipoDictamen.Id))
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
            return View(tipoDictamen);
        }

        // GET: TiposDictamen/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoDictamen = await _context.TipoDictamen
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tipoDictamen == null)
            {
                return NotFound();
            }

            return View(tipoDictamen);
        }

        // POST: TiposDictamen/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tipoDictamen = await _context.TipoDictamen.FindAsync(id);
            _context.TipoDictamen.Remove(tipoDictamen);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TipoDictamenExists(int id)
        {
            return _context.TipoDictamen.Any(e => e.Id == id);
        }
    }
}
