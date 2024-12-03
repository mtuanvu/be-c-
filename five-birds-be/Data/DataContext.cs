using five_birds_be.Models;
using Microsoft.EntityFrameworkCore;

namespace five_birds_be.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
        
        }

         public DbSet<AboutUs> AboutUs { get; set; }
        public DbSet<Footer> Footers { get; set; } // Quản lý bảng Footer

        public DbSet<FooterImage> FooterImages { get; set; } // Thêm quản lý bảng FooterImage


    }

}