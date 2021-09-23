using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebApplication3.Database;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    public class AsuntosController : Controller
    {
        private DictamenesDbContext _context = new DictamenesDbContext();

        public AsuntosController() { }

        public AsuntosController(DictamenesDbContext context)
        {
            _context = context;
        }

        // GET: Asuntos
        public async Task<ActionResult> Index()
        {
            return View(await _context.Asunto.Where(s => s.EstaActivo).ToListAsync());
        }

        // GET: Asuntos/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            var asunto = await _context.Asunto
                .FirstOrDefaultAsync(m => m.Id == id);
            if (asunto == null)
            {
                return HttpNotFound();
            }

            return View(asunto);
        }

        // GET: Asuntos/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Asuntos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Asunto asunto)
        {
            //asunto.IdUsuarioModificacion = 0;
            //asunto.IdUsuarioModificacion = _context.Usuario;
            asunto.FechaModificacion = DateTime.Now;
            asunto.EstaActivo = true;
            if (ModelState.IsValid)
            {
                _context.Asunto.Add(asunto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(asunto);
        }

        // GET: Asuntos/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            var asunto = await _context.Asunto.FindAsync(id);
            if (asunto == null)
            {
                return HttpNotFound();
            }
            return View(asunto);
        }

        // POST: Asuntos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Descripcion,EstaHabilitado,EstaActivo,FechaModificacion,IdUsuarioModificacion")] Asunto asunto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Asunto asuntoViejo = _context.Asunto.AsNoTracking().First(d => d.Id == asunto.Id);

                    //asunto.IdUsuarioModificacion = 3;
                    //dictamen.IdUsuarioModificacion = _context.Usuario;
                    asunto.EstaActivo = true;
                    asunto.FechaModificacion = DateTime.Now;
                    _context.Entry(asunto).State = EntityState.Modified;

                    asuntoViejo.EstaActivo = false;
                    asuntoViejo.Id = 0;                    

                    _context.Asunto.Add(asuntoViejo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AsuntoExists(asunto.Id))
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
            return View(asunto);
        }

        // GET: Asuntos/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            var asunto = await _context.Asunto
                .FirstOrDefaultAsync(m => m.Id == id);
            if (asunto == null)
            {
                return HttpNotFound();
            }

            return View(asunto);
        }

        // POST: Asuntos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
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
