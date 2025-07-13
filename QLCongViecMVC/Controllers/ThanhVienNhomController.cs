using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLCongViecMVC.Data;
using QLCongViecMVC.Models;
using QLCongViecMVC.Filters;

namespace QLCongViecMVC.Controllers
{
    [CheckLogin]
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
                .Include(t => t.Nhom)
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
                .Include(t => t.Nhom)
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

        public IActionResult NhapMaNhom()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GuiYeuCauThamGia(string maNhom)
        {
            var nguoiDungID = HttpContext.Session.GetString("NguoiDungID");
            if (nguoiDungID == null) return RedirectToAction("DangNhap", "NguoiDung");

            var nhom = await _context.NhomCongViecs.FindAsync(maNhom);
            if (nhom == null)
            {
                TempData["Loi"] = "Không tìm thấy nhóm.";
                return RedirectToAction("NhapMaNhom");
            }

            var daTonTai = await _context.ThanhVienNhoms
                .AnyAsync(x => x.NhomID == nhom.ID && x.NguoiDungID == nguoiDungID);
            if (daTonTai)
            {
                TempData["Loi"] = "Bạn đã gửi yêu cầu hoặc là thành viên nhóm.";
                return RedirectToAction("NhapMaNhom");
            }

            var tv = new ThanhVienNhom
            {
                ID = Guid.NewGuid().ToString("N")[..12],
                NhomID = nhom.ID,
                NguoiDungID = nguoiDungID,
                NgayThamGia = DateTime.Now,
                TrangThai = "ChoDuyet"
            };
            _context.ThanhVienNhoms.Add(tv);
            await _context.SaveChangesAsync();

            TempData["ThongBao"] = "Đã gửi yêu cầu tham gia nhóm.";
            return RedirectToAction("Index", "Home");
        }

        [HttpGet("ThanhVienNhom/XacNhanThanhVien/{nhomId}")]
        public async Task<IActionResult> XacNhanThanhVien(string nhomId)
        {
            var nguoiDungID = HttpContext.Session.GetString("NguoiDungID");
            var nhom = await _context.NhomCongViecs.FirstOrDefaultAsync(n => n.ID == nhomId);

            if (nhom == null || nhom.NguoiTaoID != nguoiDungID)
                return Unauthorized();

            var danhSach = await _context.ThanhVienNhoms
                .Include(t => t.NguoiDung)
                .Where(t => t.NhomID == nhomId && t.TrangThai == "ChoDuyet")
                .ToListAsync();

            ViewBag.NhomID = nhomId;
            return View(danhSach);
        }

        [HttpPost]
        public async Task<IActionResult> XacNhanYeuCau(string nhomId, string nguoiDungId)
        {
            var tv = await _context.ThanhVienNhoms.FindAsync(nhomId, nguoiDungId);
            if (tv == null) return NotFound();

            tv.TrangThai = "DaDuyet";
            await _context.SaveChangesAsync();

            return RedirectToAction("XacNhanThanhVien", new { nhomId = nhomId });
        }

        [HttpPost]
        public async Task<IActionResult> TuChoiYeuCau(string nhomId, string nguoiDungId)
        {
            var thanhVien = await _context.ThanhVienNhoms.FindAsync(nhomId, nguoiDungId);
            if (thanhVien == null)
                return NotFound();

            _context.ThanhVienNhoms.Remove(thanhVien);
            await _context.SaveChangesAsync();

            return RedirectToAction("XacNhanThanhVien", new { nhomId = thanhVien.NhomID });
        }

        public async Task<IActionResult> LoiMoiThamGia()
        {
            var nguoiDungID = HttpContext.Session.GetString("NguoiDungID");
            if (nguoiDungID == null) return RedirectToAction("DangNhap", "NguoiDung");

            var loiMoi = await _context.ThanhVienNhoms
                .Include(tv => tv.Nhom)
                .Where(tv => tv.NguoiDungID == nguoiDungID && tv.TrangThai == "ChoThanhVienXacNhan")
                .ToListAsync();

            return View(loiMoi);
        }

        [HttpPost]
        public async Task<IActionResult> XacNhanLoiMoi(string nhomId)
        {
            var nguoiDungID = HttpContext.Session.GetString("NguoiDungID");
            if (nguoiDungID == null) return Unauthorized();

            var tv = await _context.ThanhVienNhoms.FindAsync(nhomId, nguoiDungID);
            if (tv == null || tv.TrangThai != "ChoThanhVienXacNhan") return NotFound();

            tv.TrangThai = "DaDuyet";
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> TuChoiLoiMoi(string nhomId)
        {
            var nguoiDungID = HttpContext.Session.GetString("NguoiDungID");
            if (nguoiDungID == null) return Unauthorized();

            var tv = await _context.ThanhVienNhoms.FindAsync(nhomId, nguoiDungID);
            if (tv == null || tv.TrangThai != "ChoThanhVienXacNhan") return NotFound();

            _context.ThanhVienNhoms.Remove(tv);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
