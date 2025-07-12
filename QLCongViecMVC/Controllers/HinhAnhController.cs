using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLCongViecMVC.Data;
using QLCongViecMVC.Models;

namespace QLCongViecMVC.Controllers
{
    public class HinhAnhController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HinhAnhController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var list = await _context.HinhAnhs.ToListAsync();
            return View(list);
        }

        public IActionResult Create()
        {
            ViewBag.LoaiDoiTuong = new SelectList(new[] { "NguoiDung", "NhomCongViec", "CongViec" });
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HinhAnh model, IFormFile? fileUpload)
        {
            if (ModelState.IsValid)
            {
                model.ID = Guid.NewGuid().ToString("N")[..12];

                if (fileUpload != null && fileUpload.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                    var fileName = Guid.NewGuid().ToString("N")[..8] + Path.GetExtension(fileUpload.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                        await fileUpload.CopyToAsync(stream);

                    model.DuongDan = fileName;
                }

                _context.HinhAnhs.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.LoaiDoiTuong = new SelectList(new[] { "NguoiDung", "NhomCongViec", "CongViec" }, model.LoaiDoiTuong);
            return View(model);
        }


        public async Task<IActionResult> Edit(string id)
        {
            var entity = await _context.HinhAnhs.FindAsync(id);
            if (entity == null) return NotFound();
            ViewBag.LoaiDoiTuong = new SelectList(new[] { "NguoiDung", "NhomCongViec", "CongViec" }, entity.LoaiDoiTuong);
            return View(entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, HinhAnh model)
        {
            if (id != model.ID) return BadRequest();

            if (ModelState.IsValid)
            {
                _context.Update(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.LoaiDoiTuong = new SelectList(new[] { "NguoiDung", "NhomCongViec", "CongViec" }, model.LoaiDoiTuong);
            return View(model);
        }

        public async Task<IActionResult> Delete(string id)
        {
            var entity = await _context.HinhAnhs.FirstOrDefaultAsync(x => x.ID == id);
            return entity == null ? NotFound() : View(entity);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var entity = await _context.HinhAnhs.FindAsync(id);
            if (entity != null)
            {
                _context.HinhAnhs.Remove(entity);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}