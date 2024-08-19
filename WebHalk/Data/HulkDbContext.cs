using Microsoft.EntityFrameworkCore;
using WebHalk.Data.Entities;
using WebHalk.Models.Products;

namespace WebHalk.Data
{
    public class HulkDbContext : DbContext
    {
        public HulkDbContext(DbContextOptions<HulkDbContext> options)
            : base(options)
        {
        }

        public DbSet<CategoryEntity> Categories { get; set; }
        public DbSet<ProductEntity> Products { get; set; }
        public DbSet<ProductPhotoEntity> ProductPhotos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CategoryEntity>()
                .HasMany(c => c.Products)
                .WithOne(p => p.Category)
                .HasForeignKey(p => p.CategoryId);

            modelBuilder.Entity<ProductEntity>()
                .HasMany(p => p.ProductPhotos)
                .WithOne(pp => pp.Product)
                .HasForeignKey(pp => pp.ProductId);
        }
    }
}
