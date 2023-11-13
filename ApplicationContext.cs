using Brest_Pekar.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Brest_Pekar
{
    class ApplicationContext : DbContext
    {
        public DbSet<ProductType> Product_Types { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Store> Stores { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<RealesedOrder> Realesed_Orders { get; set; } = null!;
        public DbSet<Supply> Supplies { get; set; } = null!;
        public DbSet<Bank> Banks { get; set; } = null!;
        public DbSet<BankAccount> BankAccounts { get; set; } = null!;

        public static ApplicationContext Instance = null!;

        public ApplicationContext()
        {
            Database.EnsureCreated();
            Instance = this;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(
                "server=localhost;user=root;password=root;database=db_oao_pekar2;",
                new MySqlServerVersion(new Version(8, 0, 11))
            );
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            optionsBuilder.EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductType>()
             .HasMany(e => e.Products)
             .WithOne(e => e.product_type)
             .HasForeignKey(e => e.product_typeid)
             .IsRequired();

            modelBuilder.Entity<ProductType>()
             .HasMany(e => e.Orders)
             .WithOne(e => e.product_type)
             .HasForeignKey(e => e.product_typeid)
             .IsRequired();

            modelBuilder.Entity<Store>()
             .HasMany(e => e.Orders)
             .WithOne(e => e.store)
             .HasForeignKey(e => e.storeid)
            .IsRequired();

            modelBuilder.Entity<ProductType>()
             .HasMany(e => e.RealesedOrders)
             .WithOne(e => e.product_type)
             .HasForeignKey(e => e.product_typeid)
            .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Store>()
             .HasMany(e => e.RealesedOrders)
             .WithOne(e => e.store)
             .HasForeignKey(e => e.storeid)
            .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<BankAccount>()
             .HasMany(e => e.Stores)
             .WithOne(e => e.iban)
             .HasForeignKey(e => e.bankaccountid)
            .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Bank>()
             .HasMany(e => e.BankAccounts)
             .WithOne(e => e.bank)
             .HasForeignKey(e => e.bankid)
             .IsRequired();

            modelBuilder
                .Entity<Order>()
                .HasMany(c => c.products)
                .WithMany(s => s.orders)
                .UsingEntity<Supply>(
                    j => j
                    .HasOne(pt => pt.Product)
                    .WithMany(t => t.supplies)
                    .HasForeignKey(pt => pt.ProductId),
                    j => j
                    .HasOne(pt => pt.Order)
                    .WithMany(p => p.supplies)
                    .HasForeignKey(pt => pt.OrderId), 
                    j => 
                    {
                        j.HasIndex(pt => pt.ProductId).IsUnique(true);
                        j.HasKey(t => new { t.OrderId, t.ProductId });
                        j.ToTable("supplies");
                    }
               );
        }
    }
}
