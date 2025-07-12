namespace QLCongViecMVC.Models
{
    public class VaiTroNguoiDung
    {
        public string ID { get; set; }
        public string NguoiDungID { get; set; }
        public NguoiDung? NguoiDung { get; set; }

        public string VaiTroID { get; set; }
        public VaiTro? VaiTro { get; set; }
    }
}
