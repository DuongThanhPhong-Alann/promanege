namespace QLCongViecMVC.Models
{
    public class NguoiDung
    {
        public string ID { get; set; } = Guid.NewGuid().ToString("N")[..12];
        public string TenDangNhap { get; set; }
        public string Email { get; set; }
        public string SoDienThoai { get; set; }
        public string MatKhau { get; set; }
    }
}
