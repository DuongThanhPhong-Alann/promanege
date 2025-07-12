namespace QLCongViecMVC.Models
{
    public class TienDoThanhVien
    {
        public string ID { get; set; }
        public string CongViecID { get; set; }
        public CongViec? CongViec { get; set; }

        public string NguoiDungID { get; set; }
        public NguoiDung? NguoiDung { get; set; }

        public int PhanTramHoanThanh { get; set; }
        public int SoNgayDaLam { get; set; }
    }
}
