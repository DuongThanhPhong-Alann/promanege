using System.ComponentModel.DataAnnotations.Schema;
namespace QLCongViecMVC.Models
{
    public class ThanhVienNhom
    {
        public string ID { get; set; }

        public string NhomID { get; set; }
        public NhomCongViec? Nhom { get; set; }   // ✅ Đúng với Database First: NhomID → Nhom

        public string NguoiDungID { get; set; }
        public NguoiDung? NguoiDung { get; set; } // ✅ Đúng với Database First: NguoiDungID → NguoiDung

        public string TrangThai { get; set; } = "ChoDuyet";

        public DateTime? NgayThamGia { get; set; } = DateTime.Now;
    }
}

