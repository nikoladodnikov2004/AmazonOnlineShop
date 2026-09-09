using System.Collections.Generic;
using AmazonShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AmazonShop.Infrastructure.Data
{
    public class AmazonShopDbContext : DbContext
    {
        public AmazonShopDbContext(DbContextOptions<AmazonShopDbContext> options) : base(options) { }

        public DbSet<Product> Products => Set<Product>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<User> Users => Set<User>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();
        public DbSet<CartItem> CartItems => Set<CartItem>();
        public DbSet<Category> Categories => Set<Category>();

        public DbSet<Review> Reviews => Set<Review>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Order>()
                .Property(o => o.TotalPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.UnitPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
