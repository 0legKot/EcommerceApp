using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Model
{
    public class StoreDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductAmount> ProductAmounts { get; set; }
        public DbSet<OrderProduct> OrderProducts { get; set; }
        public StoreDbContext(DbContextOptions<StoreDbContext> options) :base(options)
        {
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderProduct>().HasKey(orderProduct => new
            {
                orderProduct.ProductId,
                orderProduct.OrderId
            });
            modelBuilder.Entity<OrderProduct>()
                .HasOne(orderProduct => orderProduct.Order)
                .WithMany(order => order.OrderProducts)
                .HasForeignKey(orderProduct => orderProduct.ProductId);
            modelBuilder.Entity<ProductAmount>()
                .HasOne(productAmount => productAmount.Product)
                .WithOne(product => product.ProductAmount)
                .HasForeignKey<Product>(product => product.Id);
        }
    }
}
