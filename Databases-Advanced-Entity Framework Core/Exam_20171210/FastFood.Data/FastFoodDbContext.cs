using FastFood.Models;
using Microsoft.EntityFrameworkCore;

namespace FastFood.Data
{
	public class FastFoodDbContext : DbContext
	{
		public FastFoodDbContext()
		{
		}

		public FastFoodDbContext(DbContextOptions options)
			: base(options)
		{
		}

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
		{
			if (!builder.IsConfigured)
			{
				builder.UseSqlServer(Configuration.ConnectionString);
			}
		}

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Position>().HasAlternateKey(e => e.Name);

            builder.Entity<Item>().HasAlternateKey(e => e.Name);

            builder.Entity<OrderItem>().HasKey(e => new { e.OrderId, e.ItemId });

            builder.Entity<OrderItem>(entity =>
            {
                entity.HasOne(e => e.Item)
                .WithMany(i => i.OrderItems)
                .HasForeignKey(e => e.ItemId);

                entity.HasOne(e => e.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(e => e.OrderId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Order>().Ignore(e => e.TotalPrice);

            builder.Entity<Item>(entity =>
            {
                entity.HasOne(e => e.Category)
                .WithMany(c => c.Items)
                .HasForeignKey(e => e.CategoryId);
            });

        }
	}
}