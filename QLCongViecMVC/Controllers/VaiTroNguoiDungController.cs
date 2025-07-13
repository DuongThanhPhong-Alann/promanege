using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLCongViecMVC.Data;
using QLCongViecMVC.Models;
using QLCongViecMVC.Filters;
namespace QLCongViecMVC.Controllers
{
    [CheckLogin]
    public class VaiTroNguoiDungController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VaiTroNguoiDungController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var list = await _context.VaiTroNguoiDungs
                .Include(v => v.NguoiDung)
                .Include(v => v.VaiTro)
                .ToListAsync();
            return View(list);
        }

        public IActionResult Create()
        {
            ViewBag.NguoiDungID = new SelectList(_context.NguoiDungs, "ID", "TenDangNhap");
            ViewBag.VaiTroID = new SelectList(_context.VaiTros, "ID", "TenVaiTro");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VaiTroNguoiDung model)
        {
            if (ModelState.IsValid)
            {
                model.ID = Guid.NewGuid().ToString("N")[..12];
                _context.VaiTroNguoiDungs.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.NguoiDungID = new SelectList(_context.NguoiDungs, "ID", "TenDangNhap", model.NguoiDungID);
            ViewBag.VaiTroID = new SelectList(_context.VaiTros, "ID", "TenVaiTro", model.VaiTroID);
            return View(model);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var entity = await _context.VaiTroNguoiDungs.FindAsync(id);
            if (entity == null) return NotFound();
            ViewBag.NguoiDungID = new SelectList(_context.NguoiDungs, "ID", "TenDangNhap", entity.NguoiDungID);
            ViewBag.VaiTroID = new SelectList(_context.VaiTros, "ID", "TenVaiTro", entity.VaiTroID);
            return View(entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, VaiTroNguoiDung model)
        {
            if (id != model.ID) return BadRequest();

            if (ModelState.IsValid)
            {
                _context.Update(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.NguoiDungID = new SelectList(_context.NguoiDungs, "ID", "TenDangNhap", model.NguoiDungID);
            ViewBag.VaiTroID = new SelectList(_context.VaiTros, "ID", "TenVaiTro", model.VaiTroID);
            return View(model);
        }

        public async Task<IActionResult> Delete(string id)
        {
            var entity = await _context.VaiTroNguoiDungs
                .Include(v => v.NguoiDung)
                .Include(v => v.VaiTro)
                .FirstOrDefaultAsync(x => x.ID == id);
            return entity == null ? NotFound() : View(entity);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var entity = await _context.VaiTroNguoiDungs.FindAsync(id);
            if (entity != null)
            {
                _context.VaiTroNguoiDungs.Remove(entity);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}