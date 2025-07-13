using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLCongViecMVC.Data;
using QLCongViecMVC.Models;
using QLCongViecMVC.Filters;
namespace QLCongViecMVC.Controllers
{
    [CheckLogin]
    public class TienDoCongViecController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TienDoCongViecController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var list = await _context.TienDoCongViecs
                .Include(t => t.CongViec)
                .ToListAsync();
            return View(list);
        }

        public IActionResult Create()
        {
            ViewBag.CongViecID = new SelectList(_context.CongViecs, "ID", "TieuDe");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TienDoCongViec model)
        {
            if (ModelState.IsValid)
            {
                model.ID = Guid.NewGuid().ToString("N")[..12];
                _context.TienDoCongViecs.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.CongViecID = new SelectList(_context.CongViecs, "ID", "TieuDe", model.CongViecID);
            return View(model);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var entity = await _context.TienDoCongViecs.FindAsync(id);
            if (entity == null) return NotFound();
            ViewBag.CongViecID = new SelectList(_context.CongViecs, "ID", "TieuDe", entity.CongViecID);
            return View(entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, TienDoCongViec model)
        {
            if (id != model.ID) return BadRequest();

            if (ModelState.IsValid)
            {
                _context.Update(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.CongViecID = new SelectList(_context.CongViecs, "ID", "TieuDe", model.CongViecID);
            return View(model);
        }

        public async Task<IActionResult> Delete(string id)
        {
            var entity = await _context.TienDoCongViecs
                .Include(t => t.CongViec)
                .FirstOrDefaultAsync(x => x.ID == id);
            return entity == null ? NotFound() : View(entity);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var entity = await _context.TienDoCongViecs.FindAsync(id);
            if (entity != null)
            {
                _context.TienDoCongViecs.Remove(entity);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
