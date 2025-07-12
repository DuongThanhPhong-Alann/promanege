using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLCongViecMVC.Data;
using QLCongViecMVC.Models;

namespace QLCongViecMVC.Controllers
{
    public class NguoiDungController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NguoiDungController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var list = await _context.NguoiDungs.ToListAsync();
            return View(list);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(NguoiDung model)
        {
            if (ModelState.IsValid)
            {
                model.ID = Guid.NewGuid().ToString("N")[..12];
                _context.NguoiDungs.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var user = await _context.NguoiDungs.FindAsync(id);
            return user == null ? NotFound() : View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, NguoiDung model)
        {
            if (id != model.ID) return BadRequest();

            if (ModelState.IsValid)
            {
                _context.Update(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> Delete(string id)
        {
            var user = await _context.NguoiDungs.FindAsync(id);
            return user == null ? NotFound() : View(user);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _context.NguoiDungs.FindAsync(id);
            if (user != null)
            {
                _context.NguoiDungs.Remove(user);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // [GET] Đăng nhập
        public IActionResult DangNhap()
        {
            return View();
        }

        // [POST] Đăng nhập
      
        [HttpPost]
        public async Task<IActionResult> DangNhap(string tenDangNhap, string matKhau)
        {
            var user = await _context.NguoiDungs.FirstOrDefaultAsync(x =>
                (x.TenDangNhap == tenDangNhap || x.Email == tenDangNhap || x.SoDienThoai == tenDangNhap)
                && x.MatKhau == matKhau);

            if (user != null)
            {
                HttpContext.Session.SetString("NguoiDungID", user.ID);
                HttpContext.Session.SetString("TenDangNhap", user.TenDangNhap);
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Loi = "Sai thông tin đăng nhập hoặc mật khẩu!";
            return View();
        }


        // [GET] Đăng ký
        public IActionResult DangKy()
        {
            return View();
        }

        // [POST] Đăng ký
        [HttpPost]
        public async Task<IActionResult> DangKy(NguoiDung model, IFormFile? AnhDaiDienUpload)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Kiểm tra trùng tên đăng nhập / email
            if (_context.NguoiDungs.Any(x => x.TenDangNhap == model.TenDangNhap))
            {
                ModelState.AddModelError("TenDangNhap", "Tên đăng nhập đã tồn tại.");
                return View(model);
            }
            if (_context.NguoiDungs.Any(x => x.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Email đã tồn tại.");
                return View(model);
            }

            model.ID = Guid.NewGuid().ToString("N")[..12];

            // Thêm người dùng vào CSDL trước
            _context.NguoiDungs.Add(model);
            await _context.SaveChangesAsync();

            // Xử lý upload ảnh đại diện → lưu vào bảng HinhAnh
            if (AnhDaiDienUpload != null && AnhDaiDienUpload.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid().ToString("N")[..8] + Path.GetExtension(AnhDaiDienUpload.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                    await AnhDaiDienUpload.CopyToAsync(stream);

                var hinhAnh = new HinhAnh
                {
                    ID = Guid.NewGuid().ToString("N")[..12],
                    DuongDan = fileName,
                    LoaiDoiTuong = "NguoiDung",
                    DoiTuongID = model.ID
                };

                _context.HinhAnhs.Add(hinhAnh);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("DangNhap");
        }


        // Đăng xuất
        public IActionResult DangXuat()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

    }
}
