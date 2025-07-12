namespace QLCongViecMVC.Models
{
    public class ThanhVienNhom
    {
        public string ID { get; set; }
        public string NhomID { get; set; }
        public NhomCongViec? NhomCongViec { get; set; }

        public string NguoiDungID { get; set; }
        public NguoiDung? NguoiDung { get; set; }
    }
}
