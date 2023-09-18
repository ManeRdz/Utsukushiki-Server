using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace server.Models;

public partial class EcommerceDbContext : DbContext
{
    public EcommerceDbContext()
    {
    }

    public EcommerceDbContext(DbContextOptions<EcommerceDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=tcp:utsukushiki-db.database.windows.net,1433;Initial Catalog=ecommerce_db;Persist Security Info=False;User ID=adminUtsu;Password=Password;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.AdminId).HasName("PK__admins__43AA414149331E3C");

            entity.ToTable("admins");

            entity.Property(e => e.AdminId).HasColumnName("admin_id");
            entity.Property(e => e.AdminPassword)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("admin_password");
            entity.Property(e => e.AdminUsername)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("admin_username");
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.CartId).HasName("PK__cart__2EF52A2715E34DDC");

            entity.ToTable("cart");

            entity.Property(e => e.CartId).HasColumnName("cart_id");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Product).WithMany(p => p.Carts)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_product_id");

            entity.HasOne(d => d.User).WithMany(p => p.Carts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_user_id");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__products__47027DF58ECABA7C");

            entity.ToTable("products");

            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.ProductCategory)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("product_category");
            entity.Property(e => e.ProductDescription)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("product_description");
            entity.Property(e => e.ProductDir)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("product_dir");
            entity.Property(e => e.ProductName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("product_name");
            entity.Property(e => e.ProductPrice).HasColumnName("product_price");
            entity.Property(e => e.ProductStock).HasColumnName("product_stock");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__users__B9BE370FC9BC6ADC");

            entity.ToTable("users");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.UserEmail)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("user_email");
            entity.Property(e => e.UserPassword)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("user_password");
            entity.Property(e => e.UserUsername)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("user_username");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
