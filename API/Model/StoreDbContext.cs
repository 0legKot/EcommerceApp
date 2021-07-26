using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Model {
    public class StoreDbContext : DbContext {
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductAmount> ProductAmounts { get; set; }
        public DbSet<OrderProduct> OrderProducts { get; set; }
        public StoreDbContext(DbContextOptions<StoreDbContext> options) : base(options) {
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<Order>()
            .HasMany(order => order.Products)
            .WithMany(product => product.Orders)
            .UsingEntity<OrderProduct>(
                j => j
                    .HasOne(orderProduct => orderProduct.Product)
                    .WithMany(product => product.OrderProducts)
                    .HasForeignKey(orderProduct => orderProduct.ProductId),
                j => j
                    .HasOne(orderProduct => orderProduct.Order)
                    .WithMany(order => order.OrderProducts)
                    .HasForeignKey(orderProduct => orderProduct.OrderId),
                j => {
                    j.ToTable("OrderProducts");
                });

            modelBuilder.Entity<ProductAmount>()
                .HasOne(productAmount => productAmount.Product)
                .WithOne(product => product.ProductAmount)
                .HasForeignKey<Product>(product => product.Id);
            modelBuilder.Entity<Product>()
                .HasOne(product => product.ProductAmount)
                .WithOne(productAmount => productAmount.Product)
                .HasForeignKey<ProductAmount>(productAmount => productAmount.ProductAmountId);

            modelBuilder.Entity<Product>()
                .HasOne(product => product.Category)
                .WithMany(category => category.Products);
        }
    }
}
