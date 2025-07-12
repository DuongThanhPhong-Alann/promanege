namespace QLCongViecMVC.Models
{
    public class VaiTro
    {
        public string ID { get; set; } = Guid.NewGuid().ToString("N")[..12];
        public string TenVaiTro { get; set; }

        public ICollection<VaiTroNguoiDung>? VaiTroNguoiDungs { get; set; }
    }
}
