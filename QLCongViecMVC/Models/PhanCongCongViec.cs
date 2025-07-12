namespace QLCongViecMVC.Models
{
    public class PhanCongCongViec
    {
        public string ID { get; set; }
        public string CongViecID { get; set; }
        public CongViec? CongViec { get; set; }

        public string NguoiDungID { get; set; }
        public NguoiDung? NguoiDung { get; set; }

        public DateTime NgayPhanCong { get; set; } = DateTime.Now;
    }
}
