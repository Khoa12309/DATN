using APPDATA.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace APPDATA.DB
{
    public class ShoppingDB: DbContext
    {
        public ShoppingDB()
        {
        }
        public ShoppingDB(DbContextOptions<ShoppingDB> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)

        {
            optionsBuilder.UseSqlServer(@"Data Source=LAPTOP-N6FK027M\SQLEXPRESS;Initial Catalog=DATN_THI;Integrated Security=True");



        }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Address> Address { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartDetail> CartDetails { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<BillDetail> BillDetails { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductDetail> ProductDetails { get; set; }
        public DbSet<PaymentMethodDetail> PaymentMethodDetails { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Voucher> Vouchers { get; set; } 
        public DbSet<Material> Materials { get; set; }
        public DbSet<Size> Sizes { get; set; }
        public DbSet<BillHistory> BillHistories { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<VoucherForAcc> VoucherForAccs { get; set; }
    }
}
