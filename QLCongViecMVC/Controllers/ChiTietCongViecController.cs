using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLCongViecMVC.Data;
using QLCongViecMVC.Models;

namespace QLCongViecMVC.Controllers
{
    public class ChiTietCongViecController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ChiTietCongViecController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var list = await _context.ChiTietCongViecs
                .Include(c => c.CongViec)
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
        public async Task<IActionResult> Create(ChiTietCongViec model)
        {
            if (ModelState.IsValid)
            {
                model.ID = Guid.NewGuid().ToString("N")[..12];
                _context.ChiTietCongViecs.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.CongViecID = new SelectList(_context.CongViecs, "ID", "TieuDe", model.CongViecID);
            return View(model);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var entity = await _context.ChiTietCongViecs.FindAsync(id);
            if (entity == null) return NotFound();
            ViewBag.CongViecID = new SelectList(_context.CongViecs, "ID", "TieuDe", entity.CongViecID);
            return View(entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, ChiTietCongViec model)
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
            var entity = await _context.ChiTietCongViecs
                .Include(c => c.CongViec)
                .FirstOrDefaultAsync(x => x.ID == id);
            return entity == null ? NotFound() : View(entity);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var entity = await _context.ChiTietCongViecs.FindAsync(id);
            if (entity != null)
            {
                _context.ChiTietCongViecs.Remove(entity);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
