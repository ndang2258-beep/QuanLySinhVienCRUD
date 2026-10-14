
using Microsoft.EntityFrameworkCore;

namespace QuanLySinhVienCRUD
{
    public class AppDbContext : DbContext
    {
        public DbSet<Student> Students { get; set; }

        protected override void OnConfiguring(
            DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
                    @"Server=localhost\SQLEXPRESS01;Database=QuanLySinhVienDB;Trusted_Connection=True;TrustServerCertificate=True");
            }
        }
    }
}
