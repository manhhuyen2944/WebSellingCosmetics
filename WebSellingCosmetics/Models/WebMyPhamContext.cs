using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace WebSellingCosmetics.Models
{
    public partial class WebMyPhamContext : DbContext
    {
        public WebMyPhamContext()
        {
        }

        public WebMyPhamContext(DbContextOptions<WebMyPhamContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Accounts { get; set; } = null!;
        public virtual DbSet<AccountType> AccountTypes { get; set; } = null!;
        public virtual DbSet<Address> Addresses { get; set; } = null!;
        public virtual DbSet<Discount> Discounts { get; set; } = null!;
        public virtual DbSet<Message> Messages { get; set; } = null!;
        public virtual DbSet<Oder> Oders { get; set; } = null!;
        public virtual DbSet<OderItem> OderItems { get; set; } = null!;
        public virtual DbSet<Payment> Payments { get; set; } = null!;
        public virtual DbSet<PaymentMethod> PaymentMethods { get; set; } = null!;
        public virtual DbSet<Product> Products { get; set; } = null!;
        public virtual DbSet<ProductType> ProductTypes { get; set; } = null!;
        public virtual DbSet<ProductsInventory> ProductsInventorys { get; set; } = null!;
        public virtual DbSet<ReceiptProduct> ReceiptProducts { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<Shipment> Shipments { get; set; } = null!;
        public virtual DbSet<ShippingMethod> ShippingMethods { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=DESKTOP-VC92P42\\SQLEXPRESS;Database=WebMyPham;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasIndex(e => e.AccountTypeId, "IX_Accounts_AccountTypeId");

                entity.HasIndex(e => e.RoleId, "IX_Accounts_RoleId");

                entity.Property(e => e.Birthday).HasColumnType("datetime");

                entity.HasOne(d => d.AccountType)
                    .WithMany(p => p.Accounts)
                    .HasForeignKey(d => d.AccountTypeId)
                    .HasConstraintName("FK_Account_AccountType");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Accounts)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK_Account_Role");
            });

            modelBuilder.Entity<AccountType>(entity =>
            {
                entity.ToTable("AccountType");
            });

            modelBuilder.Entity<Address>(entity =>
            {
                entity.ToTable("Address");

                entity.HasIndex(e => e.AccountId, "IX_Address_AccountId");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Addresses)
                    .HasForeignKey(d => d.AccountId)
                    .HasConstraintName("FK_Address_Account");
            });

            modelBuilder.Entity<Discount>(entity =>
            {
                entity.Property(e => e.DiscountPercent)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("Discount_percent");
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasIndex(e => e.FromUserId, "IX_Messages_FromUserId");

                entity.HasOne(d => d.FromUser)
                    .WithMany(p => p.Messages)
                    .HasForeignKey(d => d.FromUserId);
            });

            modelBuilder.Entity<Oder>(entity =>
            {
                entity.HasKey(e => e.OdersId);

                entity.HasIndex(e => e.AccountId, "IX_Oders_AccountId");

                entity.HasIndex(e => e.DiscountId, "IX_Oders_DiscountId");

                entity.Property(e => e.CreateDay).HasColumnType("datetime");

                entity.Property(e => e.Total).HasColumnType("decimal(18, 0)");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Oders)
                    .HasForeignKey(d => d.AccountId)
                    .HasConstraintName("FK_Oders_Account");

                entity.HasOne(d => d.Discount)
                    .WithMany(p => p.Oders)
                    .HasForeignKey(d => d.DiscountId)
                    .HasConstraintName("FK_Oders_Discount");
            });

            modelBuilder.Entity<OderItem>(entity =>
            {
                entity.HasKey(e => new { e.OderId, e.ProductId });

                entity.ToTable("Oder_Items");

                entity.HasIndex(e => e.ProductId, "IX_Oder_Items_ProductId");

                entity.HasOne(d => d.Oder)
                    .WithMany(p => p.OderItems)
                    .HasForeignKey(d => d.OderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Oder_Items_Oders");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.OderItems)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Oder_Items_Product");
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(e => e.PaymentsId);

                entity.HasIndex(e => e.OdersId, "IX_Payments_OdersId");

                entity.HasIndex(e => e.PaymentMethodsId, "IX_Payments_PaymentMethodsId");

                entity.Property(e => e.Amount).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.PaymentDate).HasColumnType("datetime");

                entity.HasOne(d => d.Oders)
                    .WithMany(p => p.Payments)
                    .HasForeignKey(d => d.OdersId)
                    .HasConstraintName("FK_Payments_Oders");

                entity.HasOne(d => d.PaymentMethods)
                    .WithMany(p => p.Payments)
                    .HasForeignKey(d => d.PaymentMethodsId)
                    .HasConstraintName("FK_Payments_PaymentMethods");
            });

            modelBuilder.Entity<PaymentMethod>(entity =>
            {
                entity.HasKey(e => e.PaymentMethodsId);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasIndex(e => e.ProductTypeId, "IX_Products_ProductTypeId");

                entity.HasIndex(e => e.ProductInventoryId, "IX_Products_Product_InventoryId");

                entity.Property(e => e.ProductInventoryId).HasColumnName("Product_InventoryId");

                entity.HasOne(d => d.ProductInventory)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.ProductInventoryId)
                    .HasConstraintName("FK_Product_Product_Inventory");

                entity.HasOne(d => d.ProductType)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.ProductTypeId)
                    .HasConstraintName("FK_Product_ProductType");
            });

            modelBuilder.Entity<ProductsInventory>(entity =>
            {
                entity.HasKey(e => e.ProductInventoryId)
                    .HasName("PK_Product_Inventory");

                entity.ToTable("Products_Inventorys");

                entity.Property(e => e.ProductInventoryId).HasColumnName("Product_InventoryId");
            });

            modelBuilder.Entity<ReceiptProduct>(entity =>
            {
                entity.ToTable("Receipt_Products");

                entity.HasIndex(e => e.ProductId, "IX_Receipt_Products_ProductId");

                entity.Property(e => e.ReceiptProductId).HasColumnName("Receipt_ProductId");

                entity.Property(e => e.Price).HasColumnType("decimal(18, 0)");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ReceiptProducts)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK_Receipt_Product_Product");
            });

            modelBuilder.Entity<Shipment>(entity =>
            {
                entity.HasKey(e => e.ShipmentsId);

                entity.HasIndex(e => e.OdersId, "IX_Shipments_OdersId");

                entity.HasIndex(e => e.ShippingMethodsId, "IX_Shipments_ShippingMethodsId");

                entity.HasOne(d => d.Oders)
                    .WithMany(p => p.Shipments)
                    .HasForeignKey(d => d.OdersId)
                    .HasConstraintName("FK_Shipments_Oders");

                entity.HasOne(d => d.ShippingMethods)
                    .WithMany(p => p.Shipments)
                    .HasForeignKey(d => d.ShippingMethodsId)
                    .HasConstraintName("FK_Shipments_ShippingMethods");
            });

            modelBuilder.Entity<ShippingMethod>(entity =>
            {
                entity.HasKey(e => e.ShippingMethodsId);

                entity.Property(e => e.ShippingFee).HasColumnType("decimal(18, 0)");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
