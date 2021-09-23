using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebApplication3.Database;
using WebApplication3.Models;

namespace Dictamenes.Models
{
    public class SujetosObligadosController : Controller
    {
        private DictamenesDbContext _context = new DictamenesDbContext();

        public SujetosObligadosController() { }

        public SujetosObligadosController(DictamenesDbContext context)
        {
            _context = context;
        }

        // GET: SujetosObligados
        public async Task<ActionResult> Index()
        {
            var dictamenesDbContext = _context.SujetoObligado.Where(s => s.EstaActivo).Include(s => s.TipoSujetoObligado).Where(s => s.TipoSujetoObligado.Descripcion != "Denunciante");

            return View(await dictamenesDbContext.ToListAsync());
        }

        // GET: SujetosObligados/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            var sujetoObligado = await _context.SujetoObligado
                .Include(s => s.TipoSujetoObligado)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sujetoObligado == null)
            {
                return HttpNotFound();
            }

            return View(sujetoObligado);
        }

        // GET: SujetosObligados/Create
        public ActionResult Create()
        {
            ViewData["IdTipoSujetoObligado"] = new SelectList(_context.TipoSujetoObligado.Where(m => m.EstaActivo && m.EstaHabilitado && m.Descripcion != "Denunciante"), "Id", "Descripcion");
            return View();
        }

        // POST: SujetosObligados/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(  SujetoObligado sujetoObligado)
        {
            sujetoObligado.IdUsuarioModificacion = 0;
            //dictamen.IdUsuarioModificacion = _context.Usuario;
            sujetoObligado.FechaModificacion = DateTime.Now;
            sujetoObligado.EstaActivo = true;

            if (ModelState.IsValid)
            {
                _context.SujetoObligado.Add(sujetoObligado);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdTipoSujetoObligado"] = new SelectList(_context.TipoSujetoObligado.Where(m => m.EstaActivo && m.EstaHabilitado && m.Descripcion != "Denunciante"), "Id", "Id", sujetoObligado.IdTipoSujetoObligado);
            return View(sujetoObligado);
        }

        // GET: SujetosObligados/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            var sujetoObligado = await _context.SujetoObligado.FindAsync(id);
            if (sujetoObligado == null)
            {
                return HttpNotFound();
            }
            ViewData["IdTipoSujetoObligado"] = new SelectList(_context.TipoSujetoObligado, "Id", "Id", sujetoObligado.IdTipoSujetoObligado);
            return View(sujetoObligado);
        }

        // POST: SujetosObligados/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, SujetoObligado sujetoObligado)
        {
            if (id != sujetoObligado.Id)
            {
                return HttpNotFound();
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
                    _context.Entry(sujetoObligado).State = EntityState.Modified;

                    sujetoObligadoViejo.EstaActivo = false;
                    sujetoObligadoViejo.Id = 0;

                    _context.SujetoObligado.Add(sujetoObligadoViejo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SujetoObligadoExists(sujetoObligado.Id))
                    {
                        return HttpNotFound();
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
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            var sujetoObligado = await _context.SujetoObligado
                .Include(s => s.TipoSujetoObligado)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sujetoObligado == null)
            {
                return HttpNotFound();
            }

            return View(sujetoObligado);
        }

        // POST: SujetosObligados/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
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
