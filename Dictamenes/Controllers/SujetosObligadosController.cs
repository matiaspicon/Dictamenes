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
            var dictamenesDbContext = _context.SujetoObligado.Where(s => s.EstaActivo).Include(s => s.TipoSujetoObligado).Where(s => s.TipoSujetoObligado.Descripcion != "Denunciante");

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
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sujetoObligado == null)
            {
                return NotFound();
            }

            return View(sujetoObligado);
        }

        // GET: SujetosObligados/Create
        public IActionResult Create()
        {
            ViewData["IdTipoSujetoObligado"] = new SelectList(_context.TipoSujetoObligado.Where(m => m.EstaActivo && m.EstaHabilitado && m.Descripcion != "Denunciante"), "Id", "Descripcion");
            return View();
        }

        // POST: SujetosObligados/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,CuilCuit,RazonSocial,IdTipoSujetoObligado,EstaActivo,FechaModificacion,IdUsuarioModificacion")] SujetoObligado sujetoObligado)
        {
            sujetoObligado.IdUsuarioModificacion = 0;
            //dictamen.IdUsuarioModificacion = _context.Usuario;
            sujetoObligado.FechaModificacion = DateTime.Now;
            sujetoObligado.EstaActivo = true;

            if (ModelState.IsValid)
            {
                _context.Add(sujetoObligado);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdTipoSujetoObligado"] = new SelectList(_context.TipoSujetoObligado.Where(m => m.EstaActivo && m.EstaHabilitado && m.Descripcion != "Denunciante"), "Id", "Id", sujetoObligado.IdTipoSujetoObligado);
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
            if (id != sujetoObligado.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    SujetoObligado sujetoObligadoViejo = _context.SujetoObligado.AsNoTracking().First(d => d.Id == id);
                    sujetoObligadoViejo.EstaActivo = true;

                    sujetoObligado.IdUsuarioModificacion = 3;
                    //dictamen.IdUsuarioModificacion = _context.Usuario;
                    sujetoObligado.FechaModificacion = DateTime.Now;
                    _context.Update(sujetoObligado);

                    sujetoObligadoViejo.EstaActivo = false;
                    sujetoObligadoViejo.Id = 0;

                    _context.SujetoObligado.Add(sujetoObligadoViejo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SujetoObligadoExists(sujetoObligado.Id))
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
                .FirstOrDefaultAsync(m => m.Id == id);
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
            return _context.SujetoObligado.Any(e => e.Id == id);
        }
    }
}
