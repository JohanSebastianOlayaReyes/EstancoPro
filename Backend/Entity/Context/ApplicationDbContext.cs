using Microsoft.EntityFrameworkCore;
using Entity.Model;

namespace Entity.Context
{
    public class ApplicationDbContext : DbContext
    {
        //costructor basio
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
            //entidades principales
            public DbSet<Form> forms { get; set; }
            public DbSet<FormModule> formModules { get; set; }
            public DbSet<Module> modules { get; set; }
            public DbSet<Permission> permissions { get; set; }
            public DbSet<Person> persons { get; set; }
            public DbSet<Rol> rols { get; set; }
            public DbSet<RolFormPermission> rolFormPermissions { get; set; }
            public DbSet<User> users { get; set; }
            public DbSet<UserRol> userrols { get; set; }
            public DbSet<RefreshToken> RefreshTokens { get; set; }

            //entidades de inventario y ventas
            public DbSet<Category> categories { get; set; }
            public DbSet<UnitMeasure> unitMeasures { get; set; }
            public DbSet<Product> products { get; set; }
            public DbSet<Supplier> suppliers { get; set; }
            public DbSet<Purchase> purchases { get; set; }
            public DbSet<PurchaseProductDetail> purchaseProductDetails { get; set; }
            public DbSet<Sale> sales { get; set; }
            public DbSet<SaleProductDetail> saleProductDetails { get; set; }
            public DbSet<CashSession> cashSessions { get; set; }
            public DbSet<CashMovement> cashMovements { get; set; }
            public DbSet<ProductUnitPrice> productUnitPrices { get; set; }

        //sobreescribiendo metodo de EntityFrameworkCore

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            /*relaciones entre entidades 
            Relacion de uno a uno */
            modelBuilder.Entity<User>()
            .HasOne(u => u.person)
            .WithOne(p => p.users)
            .HasForeignKey<User>(u => u.PersonId);//llave foranea 

            //Relaccion de mucho a muchos
            modelBuilder.Entity<UserRol>()
            .HasKey(ur => new { ur.UserId, ur.RolId }); // llave compuesta

            modelBuilder.Entity<UserRol>()
            .HasOne(ur => ur.user)
            .WithMany(U => U.userrols)
            .HasForeignKey(Ur => Ur.UserId)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserRol>()
            .HasOne(ur => ur.rol)
            .WithMany(r => r.userrols)
            .HasForeignKey(ur => ur.RolId)
            .OnDelete(DeleteBehavior.Restrict);

            //relcion de mucho a muchos (N-N-N)
            modelBuilder.Entity<RolFormPermission>()
            .HasKey(rfp => new { rfp.RolId, rfp.FormId, rfp.PermissionId });

            modelBuilder.Entity<RolFormPermission>()
            .HasOne(rfp => rfp.rol)
            .WithMany(r => r.rolFormPermissions)
            .HasForeignKey(rfp => rfp.RolId)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RolFormPermission>()
            .HasOne(rfp => rfp.form)
            .WithMany(f => f.rolFormPermissions)
            .HasForeignKey(rfp => rfp.FormId)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RolFormPermission>()
            .HasOne(rfp => rfp.permission)
            .WithMany(p => p.rolFormPermissions)
            .HasForeignKey(rfp => rfp.PermissionId)
            .OnDelete(DeleteBehavior.Restrict);

            //relacion de mucho a muchos
            modelBuilder.Entity<FormModule>()
            .HasKey(fm => new { fm.FormId, fm.ModuleId });

            modelBuilder.Entity<FormModule>()
            .HasOne(fm => fm.form)
            .WithMany(f => f.formModules)
            .HasForeignKey(fm => fm.FormId)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FormModule>()
            .HasOne(fm => fm.module)
            .WithMany(m => m.formModules)
            .HasForeignKey(fm => fm.ModuleId)
            .OnDelete(DeleteBehavior.Restrict);

            // Relación RefreshToken con User
            modelBuilder.Entity<RefreshToken>()
            .HasOne(rt => rt.User)
            .WithMany()
            .HasForeignKey(rt => rt.UserId);

            // Relaciones de inventario y ventas

            // Relación Category - Product (uno a muchos)
            modelBuilder.Entity<Product>()
            .HasOne(p => p.category)
            .WithMany(c => c.products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

            // Relación UnitMeasure - Product (uno a muchos)
            modelBuilder.Entity<Product>()
            .HasOne(p => p.unitmeasure)
            .WithMany(u => u.products)
            .HasForeignKey(p => p.UnitMeasureId)
            .OnDelete(DeleteBehavior.Restrict);

            // Relación Supplier - Purchase (uno a muchos)
            modelBuilder.Entity<Purchase>()
            .HasOne(p => p.supplier)
            .WithMany(s => s.purchases)
            .HasForeignKey(p => p.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);

            // Relación Purchase - PurchaseProductDetail (uno a muchos)
            modelBuilder.Entity<PurchaseProductDetail>()
            .HasOne(ppd => ppd.purchase)
            .WithMany(p => p.purchaseproductdetail)
            .HasForeignKey(ppd => ppd.PurchaseId)
            .OnDelete(DeleteBehavior.Restrict);

            // Relación Product - PurchaseProductDetail (uno a muchos)
            modelBuilder.Entity<PurchaseProductDetail>()
            .HasOne(ppd => ppd.product)
            .WithMany()
            .HasForeignKey(ppd => ppd.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

            // Relación UnitMeasure - PurchaseProductDetail (uno a muchos)
            modelBuilder.Entity<PurchaseProductDetail>()
            .HasOne(ppd => ppd.unitMeasure)
            .WithMany(u => u.purchaseproductdetails)
            .HasForeignKey(ppd => ppd.UnitMeasureId)
            .OnDelete(DeleteBehavior.Restrict);

            // Relación CashSession - Sale (uno a muchos)
            modelBuilder.Entity<Sale>()
            .HasOne(s => s.cashSession)
            .WithMany(cs => cs.sele)
            .HasForeignKey(s => s.CashSessionId)
            .OnDelete(DeleteBehavior.Restrict);

            // Configuración de llave compuesta para SaleProductDetail
            modelBuilder.Entity<SaleProductDetail>()
            .HasKey(spd => new { spd.SaleId, spd.ProductId, spd.UnitMeasureId });

            // Relación Sale - SaleProductDetail (uno a muchos)
            modelBuilder.Entity<SaleProductDetail>()
            .HasOne(spd => spd.sale)
            .WithMany(s => s.saleproductdetail)
            .HasForeignKey(spd => spd.SaleId)
            .OnDelete(DeleteBehavior.Restrict);

            // Relación Product - SaleProductDetail (uno a muchos)
            modelBuilder.Entity<SaleProductDetail>()
            .HasOne(spd => spd.product)
            .WithMany()
            .HasForeignKey(spd => spd.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

            // Relación UnitMeasure - SaleProductDetail (uno a muchos)
            modelBuilder.Entity<SaleProductDetail>()
            .HasOne(spd => spd.unitMeasure)
            .WithMany(u => u.saleproductdetail)
            .HasForeignKey(spd => spd.UnitMeasureId)
            .OnDelete(DeleteBehavior.Restrict);

            // Configuración de llave compuesta para CashMovement
            modelBuilder.Entity<CashMovement>()
            .HasKey(cm => new { cm.CashSessionId, cm.At });

            // Relación CashSession - CashMovement (uno a muchos)
            modelBuilder.Entity<CashMovement>()
            .HasOne(cm => cm.cashsession)
            .WithMany(cs => cs.cashmovement)
            .HasForeignKey(cm => cm.CashSessionId)
            .OnDelete(DeleteBehavior.Restrict);

            // Configuración de llave compuesta para ProductUnitPrice
            modelBuilder.Entity<ProductUnitPrice>()
            .HasKey(pup => new { pup.ProductId, pup.UnitMeasureId });

            // Relación Product - ProductUnitPrice (uno a muchos)
            modelBuilder.Entity<ProductUnitPrice>()
            .HasOne(pup => pup.product)
            .WithMany()
            .HasForeignKey(pup => pup.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

            // Relación UnitMeasure - ProductUnitPrice (uno a muchos)
            modelBuilder.Entity<ProductUnitPrice>()
            .HasOne(pup => pup.unitmeasure)
            .WithMany(u => u.productunitprices)
            .HasForeignKey(pup => pup.UnitMeasureId)
            .OnDelete(DeleteBehavior.Restrict);
        }
    }
}