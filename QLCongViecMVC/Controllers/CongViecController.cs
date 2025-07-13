using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLCongViecMVC.Data;
using QLCongViecMVC.Models;
using QLCongViecMVC.Filters;

namespace QLCongViecMVC.Controllers
{
    [CheckLogin]
    public class CongViecController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CongViecController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var nguoiDungID = HttpContext.Session.GetString("NguoiDungID");

            var list = await _context.CongViecs
                .Include(c => c.NguoiTao)
                .Include(c => c.NhomCongViec)
                .Where(c => c.NguoiTaoID == nguoiDungID ||
                    (c.LoaiCongViec == "Nhom" && c.NhomID != null && _context.ThanhVienNhoms.Any(tv => tv.NhomID == c.NhomID && tv.NguoiDungID == nguoiDungID && tv.TrangThai == "DaDuyet")))
                .ToListAsync();

            return View(list);
        }

        public IActionResult Create()
        {
            ViewBag.NguoiTaoID = new SelectList(_context.NguoiDungs, "ID", "TenDangNhap");
            ViewBag.NhomID = new SelectList(_context.NhomCongViecs, "ID", "TenNhom");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CongViec model)
        {
            var nguoiDungID = HttpContext.Session.GetString("NguoiDungID");
            if (string.IsNullOrEmpty(nguoiDungID))
                return RedirectToAction("DangNhap", "NguoiDung");

            if (ModelState.IsValid)
            {
                model.ID = Guid.NewGuid().ToString("N")[..12];
                model.NguoiTaoID = nguoiDungID; // luôn lấy từ session

                _context.CongViecs.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.NhomID = new SelectList(_context.NhomCongViecs, "ID", "TenNhom", model.NhomID);
            return View(model);
        }



        public async Task<IActionResult> Edit(string id)
        {
            var entity = await _context.CongViecs.FindAsync(id);
            if (entity == null) return NotFound();
            ViewBag.NguoiTaoID = new SelectList(_context.NguoiDungs, "ID", "TenDangNhap", entity.NguoiTaoID);
            ViewBag.NhomID = new SelectList(_context.NhomCongViecs, "ID", "TenNhom", entity.NhomID);
            return View(entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, CongViec model)
        {
            if (id != model.ID) return BadRequest();

            if (ModelState.IsValid)
            {
                _context.Update(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.NguoiTaoID = new SelectList(_context.NguoiDungs, "ID", "TenDangNhap", model.NguoiTaoID);
            ViewBag.NhomID = new SelectList(_context.NhomCongViecs, "ID", "TenNhom", model.NhomID);
            return View(model);
        }

        public async Task<IActionResult> Delete(string id)
        {
            var entity = await _context.CongViecs
                .Include(c => c.NguoiTao)
                .Include(c => c.NhomCongViec)
                .FirstOrDefaultAsync(x => x.ID == id);
            return entity == null ? NotFound() : View(entity);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var entity = await _context.CongViecs.FindAsync(id);
            if (entity != null)
            {
                _context.CongViecs.Remove(entity);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
