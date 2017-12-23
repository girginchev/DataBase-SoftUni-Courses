namespace ProductsShop.Data
{
    using Microsoft.EntityFrameworkCore;
    using ProductsShop.Models;
    using System;
    using System.Linq;

    public class ProductsShopContext : DbContext
    {
        public ProductsShopContext()
        { }

        public ProductsShopContext(DbContextOptions options)
            : base(options)
        { }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CategoryProduct> CategoryProducts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Config.ConnectinString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity => 
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.LastName).IsRequired();
                
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name).IsRequired();

                entity.HasOne(e => e.Buyer)
                .WithMany(u => u.BoughtProducts)
                .HasForeignKey(e => e.BuyerId);

                entity.HasOne(e => e.Seller)
                .WithMany(u => u.SoldProducts)
                .HasForeignKey(e => e.SellerId);
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name).IsRequired();
            });

            modelBuilder.Entity<CategoryProduct>(entity =>
            {
                entity.HasKey(e => new { e.ProductId, e.CategoryId });

                entity.HasOne(e => e.Product)
                .WithMany(p => p.CategoryProducts)
                .HasForeignKey(e => e.ProductId);

                entity.HasOne(e => e.Category)
                .WithMany(c => c.CategoryProducts)
                .HasForeignKey(e => e.CategoryId);
            });
        }


    }
}
