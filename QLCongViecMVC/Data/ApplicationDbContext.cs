using Microsoft.EntityFrameworkCore;
using QLCongViecMVC.Models;

namespace QLCongViecMVC.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        // DbSet cho các bảng
        public DbSet<NguoiDung> NguoiDungs { get; set; }
        public DbSet<VaiTro> VaiTros { get; set; }
        public DbSet<VaiTroNguoiDung> VaiTroNguoiDungs { get; set; }
        public DbSet<NhomCongViec> NhomCongViecs { get; set; }
        public DbSet<ThanhVienNhom> ThanhVienNhoms { get; set; }
        public DbSet<CongViec> CongViecs { get; set; }
        public DbSet<ChiTietCongViec> ChiTietCongViecs { get; set; }
        public DbSet<PhanCongCongViec> PhanCongCongViecs { get; set; }
        public DbSet<TienDoCongViec> TienDoCongViecs { get; set; }
        public DbSet<TienDoThanhVien> TienDoThanhViens { get; set; }
        public DbSet<HinhAnh> HinhAnhs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Khóa chính kép cho VaiTroNguoiDung
            modelBuilder.Entity<VaiTroNguoiDung>()
                .HasKey(vt => new { vt.NguoiDungID, vt.VaiTroID });

            // Khóa chính kép cho ThanhVienNhom
            modelBuilder.Entity<ThanhVienNhom>()
                .HasKey(tv => new { tv.NhomID, tv.NguoiDungID });

            // Khóa chính kép cho PhanCongCongViec
            modelBuilder.Entity<PhanCongCongViec>()
                .HasKey(pc => new { pc.CongViecID, pc.NguoiDungID });

            // Khóa chính kép cho TienDoThanhVien
            modelBuilder.Entity<TienDoThanhVien>()
                .HasKey(td => new { td.CongViecID, td.NguoiDungID });

            // 1-1: CongViec - ChiTietCongViec
            modelBuilder.Entity<ChiTietCongViec>()
                .HasOne(c => c.CongViec)
                .WithOne(c => c.ChiTiet)
                .HasForeignKey<ChiTietCongViec>(c => c.CongViecID)
                .OnDelete(DeleteBehavior.Cascade);

            // 1-1: CongViec - TienDoCongViec
            modelBuilder.Entity<TienDoCongViec>()
                .HasOne(td => td.CongViec)
                .WithOne(cv => cv.TienDo)
                .HasForeignKey<TienDoCongViec>(td => td.CongViecID)
                .OnDelete(DeleteBehavior.Cascade);

            // Giới hạn độ dài chuỗi (optionally)
            modelBuilder.Entity<NguoiDung>().Property(u => u.ID).HasMaxLength(12);
            modelBuilder.Entity<CongViec>().Property(c => c.ID).HasMaxLength(12);
            modelBuilder.Entity<NhomCongViec>().Property(n => n.ID).HasMaxLength(12);
            modelBuilder.Entity<HinhAnh>().Property(h => h.ID).HasMaxLength(12);

            modelBuilder.Entity<CongViec>()
            .HasOne(c => c.NhomCongViec)
            .WithMany()
            .HasForeignKey(c => c.NhomID);

             base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CongViec>()
                .HasOne(cv => cv.NhomCongViec)
                .WithMany(n => n.CongViecs)
                .HasForeignKey(cv => cv.NhomID)
                .HasConstraintName("FK_CongViec_NhomCongViec");
        }
    }
}
