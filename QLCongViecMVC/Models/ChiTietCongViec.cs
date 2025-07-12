namespace QLCongViecMVC.Models
{
    public class ChiTietCongViec
    {
        public string ID { get; set; }
        public string CongViecID { get; set; }
        public CongViec? CongViec { get; set; }

        public string? MoTa { get; set; }
        public DateTime? NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }
    }
}
