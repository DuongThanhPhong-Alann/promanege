using System.ComponentModel.DataAnnotations;

namespace QLCongViecMVC.Models
{
    public class HinhAnh
    {
        [Key]
        public string ID { get; set; } = Guid.NewGuid().ToString("N")[..12];

        [Required(ErrorMessage = "Vui lòng chọn ảnh")]
        [Display(Name = "Đường dẫn ảnh")]
        public string DuongDan { get; set; }

        [Required]
        [Display(Name = "Loại đối tượng")]
        public string LoaiDoiTuong { get; set; } // 'NguoiDung', 'CongViec', 'Nhom'

        [Required]
        [Display(Name = "ID đối tượng")]
        public string DoiTuongID { get; set; }

        // Gợi ý thêm nếu sau này muốn phân loại ảnh (chính/phụ)
        // public string? VaiTroAnh { get; set; } // ví dụ: "DaiDien", "HinhMoTa", v.v.
    }
}
