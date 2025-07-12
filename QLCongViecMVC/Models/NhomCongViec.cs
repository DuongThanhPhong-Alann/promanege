namespace QLCongViecMVC.Models
{
    public class NhomCongViec
    {
        public string ID { get; set; } = Guid.NewGuid().ToString("N")[..12];
        public string TenNhom { get; set; }
        public string? MoTa { get; set; }
        public string? MucTieu { get; set; }
        public DateTime? NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }
        public string? NguoiTaoID { get; set; }
        public NguoiDung? NguoiTao { get; set; }
        public DateTime NgayTao { get; set; } = DateTime.Now;

        public ICollection<ThanhVienNhom>? ThanhVienNhoms { get; set; }
        public ICollection<CongViec>? CongViecs { get; set; }
    }
}
