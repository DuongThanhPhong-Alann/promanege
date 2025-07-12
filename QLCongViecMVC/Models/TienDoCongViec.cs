namespace QLCongViecMVC.Models
{
    public class TienDoCongViec
    {
        public string ID { get; set; }
        public string CongViecID { get; set; }
        public CongViec? CongViec { get; set; }

        public int PhanTramHoanThanh { get; set; }
        public int SoNgayDaLam { get; set; }
    }
}
