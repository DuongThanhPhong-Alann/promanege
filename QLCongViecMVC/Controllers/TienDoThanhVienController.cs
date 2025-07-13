using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLCongViecMVC.Data;
using QLCongViecMVC.Models;
using QLCongViecMVC.Filters;
namespace QLCongViecMVC.Controllers
{
    [CheckLogin]
    public class TienDoThanhVienController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TienDoThanhVienController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var list = await _context.TienDoThanhViens
                .Include(t => t.CongViec)
                .Include(t => t.NguoiDung)
                .ToListAsync();
            return View(list);
        }

        public IActionResult Create()
        {
            ViewBag.CongViecID = new SelectList(_context.CongViecs, "ID", "TieuDe");
            ViewBag.NguoiDungID = new SelectList(_context.NguoiDungs, "ID", "TenDangNhap");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TienDoThanhVien model)
        {
            if (ModelState.IsValid)
            {
                model.ID = Guid.NewGuid().ToString("N")[..12];
                _context.TienDoThanhViens.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.CongViecID = new SelectList(_context.CongViecs, "ID", "TieuDe", model.CongViecID);
            ViewBag.NguoiDungID = new SelectList(_context.NguoiDungs, "ID", "TenDangNhap", model.NguoiDungID);
            return View(model);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var entity = await _context.TienDoThanhViens.FindAsync(id);
            if (entity == null) return NotFound();
            ViewBag.CongViecID = new SelectList(_context.CongViecs, "ID", "TieuDe", entity.CongViecID);
            ViewBag.NguoiDungID = new SelectList(_context.NguoiDungs, "ID", "TenDangNhap", entity.NguoiDungID);
            return View(entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, TienDoThanhVien model)
        {
            if (id != model.ID) return BadRequest();

            if (ModelState.IsValid)
            {
                _context.Update(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.CongViecID = new SelectList(_context.CongViecs, "ID", "TieuDe", model.CongViecID);
            ViewBag.NguoiDungID = new SelectList(_context.NguoiDungs, "ID", "TenDangNhap", model.NguoiDungID);
            return View(model);
        }

        public async Task<IActionResult> Delete(string id)
        {
            var entity = await _context.TienDoThanhViens
                .Include(t => t.CongViec)
                .Include(t => t.NguoiDung)
                .FirstOrDefaultAsync(x => x.ID == id);
            return entity == null ? NotFound() : View(entity);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var entity = await _context.TienDoThanhViens.FindAsync(id);
            if (entity != null)
            {
                _context.TienDoThanhViens.Remove(entity);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
