namespace QLCongViecMVC.Models
{
    public class CongViec
    {
        public string ID { get; set; } = Guid.NewGuid().ToString("N")[..12];
        public string TieuDe { get; set; }
        public string LoaiCongViec { get; set; } // 'CaNhan' hoặc 'Nhom'

        public string NguoiTaoID { get; set; }
        public NguoiDung? NguoiTao { get; set; }

        public string? NhomID { get; set; }
        public NhomCongViec? NhomCongViec { get; set; }

        public ChiTietCongViec? ChiTiet { get; set; }
        public ICollection<PhanCongCongViec>? PhanCong { get; set; }
        public TienDoCongViec? TienDo { get; set; }
        public ICollection<TienDoThanhVien>? TienDoThanhViens { get; set; }
    }
}
