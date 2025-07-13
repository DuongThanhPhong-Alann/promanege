using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLCongViecMVC.Data;
using QLCongViecMVC.Models;
using QLCongViecMVC.Filters;

namespace QLCongViecMVC.Controllers
{
    [CheckLogin]
    public class NhomCongViecController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public NhomCongViecController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // Danh sách nhóm mà người dùng đã tham gia (trạng thái: DaDuyet)
        public async Task<IActionResult> Index()
        {
            var nguoiDungID = HttpContext.Session.GetString("NguoiDungID");

            var danhSachNhom = await _context.ThanhVienNhoms
                .Where(tv => tv.NguoiDungID == nguoiDungID && tv.TrangThai == "DaDuyet")
                .Include(tv => tv.Nhom)
                .ThenInclude(n => n.NguoiTao)
                .Select(tv => tv.Nhom!)
                .ToListAsync();

            return View(danhSachNhom);
        }

        // GET: Tạo nhóm mới
        public IActionResult Create()
        {
            ViewBag.ThanhVienList = new MultiSelectList(_context.NguoiDungs, "ID", "TenDangNhap");
            return View();
        }

        // POST: Tạo nhóm mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NhomCongViec model, List<string> selectedMembers, IFormFile? AnhDaiDienUpload)
        {
            if (ModelState.IsValid)
            {
                model.ID = Guid.NewGuid().ToString("N")[..10];
                model.NguoiTaoID = HttpContext.Session.GetString("NguoiDungID");
                model.NgayTao = DateTime.Now;

                _context.NhomCongViecs.Add(model);

                // Thêm người tạo nhóm là thành viên đã duyệt
                var tvTao = new ThanhVienNhom
                {
                    ID = Guid.NewGuid().ToString("N")[..12],
                    NhomID = model.ID,
                    NguoiDungID = model.NguoiTaoID!,
                    TrangThai = "DaDuyet",
                    NgayThamGia = DateTime.Now
                };
                _context.ThanhVienNhoms.Add(tvTao);

                // Thêm các thành viên được chọn (trạng thái: ChoThanhVienXacNhan)
                foreach (var thanhVienId in selectedMembers)
                {
                    if (thanhVienId != model.NguoiTaoID)
                    {
                        var tv = new ThanhVienNhom
                        {
                            ID = Guid.NewGuid().ToString("N")[..12],
                            NhomID = model.ID,
                            NguoiDungID = thanhVienId,
                            TrangThai = "ChoThanhVienXacNhan",
                            NgayThamGia = DateTime.Now
                        };
                        _context.ThanhVienNhoms.Add(tv);
                    }
                }

                // Lưu ảnh nếu có
                if (AnhDaiDienUpload != null && AnhDaiDienUpload.Length > 0)
                {
                    var fileName = $"{Guid.NewGuid().ToString("N")[..10]}_{AnhDaiDienUpload.FileName}";
                    var filePath = Path.Combine(_env.WebRootPath, "uploads", fileName);
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
                    using var stream = new FileStream(filePath, FileMode.Create);
                    await AnhDaiDienUpload.CopyToAsync(stream);

                    var hinhAnh = new HinhAnh
                    {
                        ID = Guid.NewGuid().ToString("N")[..12],
                        DuongDan = "/uploads/" + fileName,
                        LoaiDoiTuong = "Nhom",
                        DoiTuongID = model.ID
                    };
                    _context.HinhAnhs.Add(hinhAnh);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.ThanhVienList = new MultiSelectList(_context.NguoiDungs, "ID", "TenDangNhap", selectedMembers);
            return View(model);
        }

        // Xem chi tiết nhóm
       public async Task<IActionResult> Details(string id)
        {
            var nhom = await _context.NhomCongViecs
                .Include(n => n.NguoiTao)
                .Include(n => n.ThanhVienNhoms!)
                    .ThenInclude(tv => tv.NguoiDung)
                .FirstOrDefaultAsync(n => n.ID == id);

            if (nhom == null) return NotFound();

            var nguoiDungID = HttpContext.Session.GetString("NguoiDungID");
            var isThanhVien = await _context.ThanhVienNhoms
                .AnyAsync(t => t.NhomID == id && t.NguoiDungID == nguoiDungID && t.TrangThai == "DaDuyet");

            if (!isThanhVien) return Unauthorized();

            // ✅ Lọc thành viên có trạng thái đã duyệt
            nhom.ThanhVienNhoms = nhom.ThanhVienNhoms?
                .Where(tv => tv.TrangThai == "DaDuyet")
                .ToList();

            return View(nhom);
        }

        // Sửa nhóm
        public async Task<IActionResult> Edit(string id)
        {
            var nhom = await _context.NhomCongViecs.FindAsync(id);
            if (nhom == null) return NotFound();

            var nguoiDungID = HttpContext.Session.GetString("NguoiDungID");
            if (nhom.NguoiTaoID != nguoiDungID) return Unauthorized();

            return View(nhom);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, NhomCongViec model)
        {
            if (id != model.ID) return BadRequest();

            var nhom = await _context.NhomCongViecs.FindAsync(id);
            if (nhom == null) return NotFound();

            var nguoiDungID = HttpContext.Session.GetString("NguoiDungID");
            if (nhom.NguoiTaoID != nguoiDungID) return Unauthorized();

            nhom.TenNhom = model.TenNhom;
            nhom.MoTa = model.MoTa;
            nhom.MucTieu = model.MucTieu;
            nhom.NgayBatDau = model.NgayBatDau;
            nhom.NgayKetThuc = model.NgayKetThuc;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Xóa nhóm
        public async Task<IActionResult> Delete(string id)
        {
            var nhom = await _context.NhomCongViecs.FindAsync(id);
            if (nhom == null) return NotFound();

            var nguoiDungID = HttpContext.Session.GetString("NguoiDungID");
            if (nhom.NguoiTaoID != nguoiDungID) return Unauthorized();

            return View(nhom);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var nhom = await _context.NhomCongViecs.FindAsync(id);
            if (nhom != null)
            {
                // XÓA CÁC THÀNH VIÊN thuộc nhóm đó trước
                var thanhVienTrongNhom = _context.ThanhVienNhoms.Where(tv => tv.NhomID == id);
                _context.ThanhVienNhoms.RemoveRange(thanhVienTrongNhom);

                // Sau đó mới xóa nhóm
                _context.NhomCongViecs.Remove(nhom);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

    }
}
