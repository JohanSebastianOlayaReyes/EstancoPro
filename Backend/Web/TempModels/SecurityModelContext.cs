using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Web.TempModels;

public partial class SecurityModelContext : DbContext
{
    public SecurityModelContext()
    {
    }

    public SecurityModelContext(DbContextOptions<SecurityModelContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CashMovement> CashMovements { get; set; }

    public virtual DbSet<CashSession> CashSessions { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Form> Forms { get; set; }

    public virtual DbSet<FormModule> FormModules { get; set; }

    public virtual DbSet<Module> Modules { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<Person> Persons { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductUnitPrice> ProductUnitPrices { get; set; }

    public virtual DbSet<Purchase> Purchases { get; set; }

    public virtual DbSet<PurchaseProductDetail> PurchaseProductDetails { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<Rol> Rols { get; set; }

    public virtual DbSet<RolFormPermission> RolFormPermissions { get; set; }

    public virtual DbSet<Sale> Sales { get; set; }

    public virtual DbSet<SaleProductDetail> SaleProductDetails { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<UnitMeasure> UnitMeasures { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Userrol> Userrols { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CashMovement>(entity =>
        {
            entity.HasKey(e => new { e.CashSessionId, e.At });

            entity.ToTable("cashMovements");

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.CashSession).WithMany(p => p.CashMovements)
                .HasForeignKey(d => d.CashSessionId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<CashSession>(entity =>
        {
            entity.ToTable("cashSessions");

            entity.Property(e => e.ClosingAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.OpeningAmount).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("categories");
        });

        modelBuilder.Entity<Form>(entity =>
        {
            entity.ToTable("forms");
        });

        modelBuilder.Entity<FormModule>(entity =>
        {
            entity.HasKey(e => new { e.FormId, e.ModuleId });

            entity.ToTable("formModules");

            entity.HasIndex(e => e.ModuleId, "IX_formModules_ModuleId");

            entity.HasOne(d => d.Form).WithMany(p => p.FormModules)
                .HasForeignKey(d => d.FormId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Module).WithMany(p => p.FormModules)
                .HasForeignKey(d => d.ModuleId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Module>(entity =>
        {
            entity.ToTable("modules");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.ToTable("permissions");
        });

        modelBuilder.Entity<Person>(entity =>
        {
            entity.ToTable("persons");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("products");

            entity.HasIndex(e => e.CategoryId, "IX_products_CategoryId");

            entity.HasIndex(e => e.UnitMeasureId, "IX_products_UnitMeasureId");

            entity.Property(e => e.TaxRate).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UnitCost).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.UnitMeasure).WithMany(p => p.Products)
                .HasForeignKey(d => d.UnitMeasureId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ProductUnitPrice>(entity =>
        {
            entity.HasKey(e => new { e.ProductId, e.UnitMeasureId });

            entity.ToTable("productUnitPrices");

            entity.HasIndex(e => e.UnitMeasureId, "IX_productUnitPrices_UnitMeasureId");

            entity.Property(e => e.ConversionFactor).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UnitCost).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductUnitPrices)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.UnitMeasure).WithMany(p => p.ProductUnitPrices)
                .HasForeignKey(d => d.UnitMeasureId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Purchase>(entity =>
        {
            entity.ToTable("purchases");

            entity.HasIndex(e => e.SupplierId, "IX_purchases_SupplierId");

            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.TotalCost).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Supplier).WithMany(p => p.Purchases)
                .HasForeignKey(d => d.SupplierId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<PurchaseProductDetail>(entity =>
        {
            entity.ToTable("purchaseProductDetails");

            entity.HasIndex(e => e.ProductId, "IX_purchaseProductDetails_ProductId");

            entity.HasIndex(e => e.PurchaseId, "IX_purchaseProductDetails_PurchaseId");

            entity.HasIndex(e => e.UnitMeasureId, "IX_purchaseProductDetails_UnitMeasureId");

            entity.Property(e => e.LineTotal).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UnitCost).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Product).WithMany(p => p.PurchaseProductDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Purchase).WithMany(p => p.PurchaseProductDetails)
                .HasForeignKey(d => d.PurchaseId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.UnitMeasure).WithMany(p => p.PurchaseProductDetails)
                .HasForeignKey(d => d.UnitMeasureId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasIndex(e => e.UserId, "IX_RefreshTokens_UserId");

            entity.HasOne(d => d.User).WithMany(p => p.RefreshTokens).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.ToTable("rols");
        });

        modelBuilder.Entity<RolFormPermission>(entity =>
        {
            entity.HasKey(e => new { e.RolId, e.FormId, e.PermissionId });

            entity.ToTable("rolFormPermissions");

            entity.HasIndex(e => e.FormId, "IX_rolFormPermissions_FormId");

            entity.HasIndex(e => e.PermissionId, "IX_rolFormPermissions_PermissionId");

            entity.HasOne(d => d.Form).WithMany(p => p.RolFormPermissions)
                .HasForeignKey(d => d.FormId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Permission).WithMany(p => p.RolFormPermissions)
                .HasForeignKey(d => d.PermissionId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Rol).WithMany(p => p.RolFormPermissions)
                .HasForeignKey(d => d.RolId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Sale>(entity =>
        {
            entity.ToTable("sales");

            entity.HasIndex(e => e.CashSessionId, "IX_sales_CashSessionId");

            entity.Property(e => e.GrandTotal).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Subtotal).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TaxTotal).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.CashSession).WithMany(p => p.Sales)
                .HasForeignKey(d => d.CashSessionId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<SaleProductDetail>(entity =>
        {
            entity.HasKey(e => new { e.SaleId, e.ProductId, e.UnitMeasureId });

            entity.ToTable("saleProductDetails");

            entity.HasIndex(e => e.ProductId, "IX_saleProductDetails_ProductId");

            entity.HasIndex(e => e.UnitMeasureId, "IX_saleProductDetails_UnitMeasureId");

            entity.Property(e => e.LineSubtotal).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.LineTax).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.LineTotal).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TaxRate).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Product).WithMany(p => p.SaleProductDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Sale).WithMany(p => p.SaleProductDetails)
                .HasForeignKey(d => d.SaleId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.UnitMeasure).WithMany(p => p.SaleProductDetails)
                .HasForeignKey(d => d.UnitMeasureId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.ToTable("suppliers");
        });

        modelBuilder.Entity<UnitMeasure>(entity =>
        {
            entity.ToTable("unitMeasures");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");

            entity.HasIndex(e => e.PersonId, "IX_users_PersonId").IsUnique();

            entity.HasIndex(e => e.RolId, "IX_users_RolId");

            entity.HasOne(d => d.Person).WithOne(p => p.User).HasForeignKey<User>(d => d.PersonId);

            entity.HasOne(d => d.Rol).WithMany(p => p.Users).HasForeignKey(d => d.RolId);
        });

        modelBuilder.Entity<Userrol>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.RolId });

            entity.ToTable("userrols");

            entity.HasIndex(e => e.RolId, "IX_userrols_RolId");

            entity.HasOne(d => d.Rol).WithMany(p => p.Userrols)
                .HasForeignKey(d => d.RolId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.User).WithMany(p => p.Userrols)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
