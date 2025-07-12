using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLCongViecMVC.Data;
using QLCongViecMVC.Models;

namespace QLCongViecMVC.Controllers
{
    public class ThanhVienNhomController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ThanhVienNhomController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var list = await _context.ThanhVienNhoms
                .Include(t => t.NhomCongViec)
                .Include(t => t.NguoiDung)
                .ToListAsync();
            return View(list);
        }

        public IActionResult Create()
        {
            ViewBag.NhomID = new SelectList(_context.NhomCongViecs, "ID", "TenNhom");
            ViewBag.NguoiDungID = new SelectList(_context.NguoiDungs, "ID", "TenDangNhap");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ThanhVienNhom model)
        {
            if (ModelState.IsValid)
            {
                model.ID = Guid.NewGuid().ToString("N")[..12];
                _context.ThanhVienNhoms.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.NhomID = new SelectList(_context.NhomCongViecs, "ID", "TenNhom", model.NhomID);
            ViewBag.NguoiDungID = new SelectList(_context.NguoiDungs, "ID", "TenDangNhap", model.NguoiDungID);
            return View(model);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var entity = await _context.ThanhVienNhoms.FindAsync(id);
            if (entity == null) return NotFound();
            ViewBag.NhomID = new SelectList(_context.NhomCongViecs, "ID", "TenNhom", entity.NhomID);
            ViewBag.NguoiDungID = new SelectList(_context.NguoiDungs, "ID", "TenDangNhap", entity.NguoiDungID);
            return View(entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, ThanhVienNhom model)
        {
            if (id != model.ID) return BadRequest();

            if (ModelState.IsValid)
            {
                _context.Update(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.NhomID = new SelectList(_context.NhomCongViecs, "ID", "TenNhom", model.NhomID);
            ViewBag.NguoiDungID = new SelectList(_context.NguoiDungs, "ID", "TenDangNhap", model.NguoiDungID);
            return View(model);
        }

        public async Task<IActionResult> Delete(string id)
        {
            var entity = await _context.ThanhVienNhoms
                .Include(t => t.NhomCongViec)
                .Include(t => t.NguoiDung)
                .FirstOrDefaultAsync(x => x.ID == id);
            return entity == null ? NotFound() : View(entity);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var entity = await _context.ThanhVienNhoms.FindAsync(id);
            if (entity != null)
            {
                _context.ThanhVienNhoms.Remove(entity);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}