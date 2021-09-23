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
    public class TiposDictamenController : Controller
    {
        private DictamenesDbContext _context = new DictamenesDbContext();

        public TiposDictamenController() { }

        public TiposDictamenController(DictamenesDbContext context)
        {
            _context = context;
        }

        // GET: TiposDictamen
        public async Task<ActionResult> Index()
        {
            return View(await _context.TipoDictamen.Where(s => s.EstaActivo).ToListAsync());
        }

        // GET: TiposDictamen/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            var tipoDictamen = await _context.TipoDictamen
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tipoDictamen == null)
            {
                return HttpNotFound();
            }

            return View(tipoDictamen);
        }

        // GET: TiposDictamen/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TiposDictamen/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(  TipoDictamen tipoDictamen)
        {
            tipoDictamen.IdUsuarioModificacion = 0;
            //tipoDictamen.IdUsuarioModificacion = _context.Usuario;
            tipoDictamen.FechaModificacion = DateTime.Now;
            tipoDictamen.EstaActivo = true;
            if (ModelState.IsValid)
            {
                _context.TipoDictamen.Add(tipoDictamen);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tipoDictamen);
        }

        // GET: TiposDictamen/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            var tipoDictamen = await _context.TipoDictamen.FindAsync(id);
            if (tipoDictamen == null)
            {
                return HttpNotFound();
            }
            return View(tipoDictamen);
        }

        // POST: TiposDictamen/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id,   TipoDictamen tipoDictamen)
        {
            if (id != tipoDictamen.Id)
            {
                return HttpNotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    TipoDictamen tipoDictamenViejo = _context.TipoDictamen.AsNoTracking().First(d => d.Id == id);
                    
                    tipoDictamen.EstaActivo = true;

                    tipoDictamen.IdUsuarioModificacion = 3;
                    //dictamen.IdUsuarioModificacion = _context.Usuario;
                    tipoDictamen.FechaModificacion = DateTime.Now;

                    _context.Entry(tipoDictamen).State = EntityState.Modified;

                    tipoDictamenViejo.EstaActivo = false;
                    tipoDictamenViejo.Id = 0;

                    _context.TipoDictamen.Add(tipoDictamenViejo);
                    await _context.SaveChangesAsync();


                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TipoDictamenExists(tipoDictamen.Id))
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
            return View(tipoDictamen);
        }

        // GET: TiposDictamen/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            var tipoDictamen = await _context.TipoDictamen
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tipoDictamen == null)
            {
                return HttpNotFound();
            }

            return View(tipoDictamen);
        }

        // POST: TiposDictamen/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
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
