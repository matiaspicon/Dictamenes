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
    public class AsuntosController : Controller
    {
        private readonly DictamenesDbContext _context;

        public AsuntosController(DictamenesDbContext context)
        {
            _context = context;
        }

        // GET: Asuntos
        public async Task<IActionResult> Index()
        {
            return View(await _context.Asunto.Where(s => s.EstaActivo).ToListAsync());
        }

        // GET: Asuntos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var asunto = await _context.Asunto
                .FirstOrDefaultAsync(m => m.Id == id);
            if (asunto == null)
            {
                return NotFound();
            }

            return View(asunto);
        }

        // GET: Asuntos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Asuntos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Descripcion,EstaHabilitado,EstaActivo,FechaModificacion,IdUsuarioModificacion")] Asunto asunto)
        {
            asunto.IdUsuarioModificacion = 0;
            //asunto.IdUsuarioModificacion = _context.Usuario;
            asunto.FechaModificacion = DateTime.Now;
            asunto.EstaActivo = true;
            if (ModelState.IsValid)
            {
                _context.Add(asunto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(asunto);
        }

        // GET: Asuntos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var asunto = await _context.Asunto.FindAsync(id);
            if (asunto == null)
            {
                return NotFound();
            }
            return View(asunto);
        }

        // POST: Asuntos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Descripcion,EstaHabilitado,EstaActivo,FechaModificacion,IdUsuarioModificacion")] Asunto asunto)
        {
            if (id != asunto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Asunto asuntoViejo = _context.Asunto.AsNoTracking().First(d => d.Id == id);

                    asunto.IdUsuarioModificacion = 3;
                    //dictamen.IdUsuarioModificacion = _context.Usuario;
                    asunto.EstaActivo = true;
                    asunto.FechaModificacion = DateTime.Now;                    
                    _context.Update(asunto);

                    asuntoViejo.EstaActivo = false;
                    asuntoViejo.Id = 0;                    

                    _context.Asunto.Add(asuntoViejo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AsuntoExists(asunto.Id))
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
            return View(asunto);
        }

        // GET: Asuntos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var asunto = await _context.Asunto
                .FirstOrDefaultAsync(m => m.Id == id);
            if (asunto == null)
            {
                return NotFound();
            }

            return View(asunto);
        }

        // POST: Asuntos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var asunto = await _context.Asunto.FindAsync(id);
            _context.Asunto.Remove(asunto);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AsuntoExists(int id)
        {
            return _context.Asunto.Any(e => e.Id == id);
        }
    }
}
