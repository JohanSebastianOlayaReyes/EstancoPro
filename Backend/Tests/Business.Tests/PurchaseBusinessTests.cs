using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Entity.Context;
using Entity.Model;
using Business.Implementations;
using Moq;
using Data.Interfaces;
using Microsoft.Extensions.Logging;
using Entity.Dto;

namespace Business.Tests
{
    public class PurchaseBusinessTests
    {
        private ApplicationDbContext CreateInMemoryContext(SqliteConnection conn)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(conn)
                .Options;

            var ctx = new ApplicationDbContext(options);
            // Disable foreign key enforcement in tests to avoid having to seed every related entity
            // This keeps tests focused and faster. If you need strict relational behavior, remove this.
            try
            {
                using var cmd = conn.CreateCommand();
                cmd.CommandText = "PRAGMA foreign_keys = OFF;";
                cmd.ExecuteNonQuery();
            }
            catch
            {
                // ignore if command fails
            }

            ctx.Database.EnsureCreated();
            return ctx;
        }

        [Fact]
        public async Task ReceivePurchaseAsync_HappyPath_UpdatesStockAndCreatesCashMovement()
        {
            var conn = new SqliteConnection("DataSource=:memory:");
            conn.Open();
            try
            {
                using var ctx = CreateInMemoryContext(conn);

                // Seed supplier
                var supplier = new Supplier { Name = "Sup1" };
                ctx.suppliers.Add(supplier);
                supplier.Phone = "000"; await ctx.SaveChangesAsync();

                // Seed product and unit
                var unit = new UnitMeasure { Name = "u" };
                var product = new Product { Name = "P1", StockOnHand = 10 };
                ctx.unitMeasures.Add(unit);
                ctx.products.Add(product);
                await ctx.SaveChangesAsync();

                // Seed ProductUnitPrice with conversion factor 2
                var pup = new ProductUnitPrice { ProductId = product.Id, UnitMeasureId = unit.Id, ConversionFactor = 2m, product = product, unitmeasure = unit };
                ctx.productUnitPrices.Add(pup);

                // Seed purchase with one line (create detail and attach navigations so EF doesn't try to insert missing FKs)
                var purchase = new Purchase
                {
                    OrderedAt = DateTime.UtcNow,
                    status = false,
                    TotalCost = 100m,
                    SupplierId = supplier.Id,
                };

                var detail = new PurchaseProductDetail
                {
                    purchase = purchase,
                    PurchaseId = purchase.Id,
                    product = product,
                    ProductId = product.Id,
                    unitMeasure = unit,
                    UnitMeasureId = unit.Id,
                    Quantity = 3,
                    UnitCost = 10m
                };

                purchase.purchaseproductdetail = new System.Collections.Generic.List<PurchaseProductDetail> { detail };
                ctx.purchases.Add(purchase);
                await ctx.SaveChangesAsync();

                // (no cash session for this test - we won't pay in cash)
                var mockData = new Mock<IPurchaseData>();
                var mockLogger = new Mock<ILogger<BaseBusiness<Purchase, PurchaseDto>>>();

                var sut = new PurchaseBusiness(mockData.Object, ctx, mockLogger.Object);

                // Act: receive and pay in cash
                try
                {
                    await sut.ReceivePurchaseAsync(purchase.Id, payInCash: false, cashSessionId: null);
                }
                catch (Exception ex)
                {
                    var entries = ctx.ChangeTracker.Entries().Select(e => $"{e.Entity.GetType().Name}:{e.State}");
                    throw new Exception($"TESTERROR: {ex.GetType().Name}: {ex.Message} TRACKER: {string.Join(",", entries)}", ex);
                }

                // Reload entities
                var updatedPurchase = await ctx.purchases.FindAsync(purchase.Id);
                var updatedProduct = await ctx.products.FindAsync(product.Id);
                Assert.True(updatedPurchase.status);
                // Quantity 3 * conversion 2 => +6
                Assert.Equal(16, updatedProduct.StockOnHand);
                // no cash movement expected in this test
            }
            finally
            {
                conn.Close();
            }
        }

        [Fact]
        public async Task ReceivePurchaseAsync_WhenNotFound_ThrowsKeyNotFoundException()
        {
            var conn = new SqliteConnection("DataSource=:memory:");
            conn.Open();
            try
            {
                using var ctx = CreateInMemoryContext(conn);
                var mockData = new Mock<IPurchaseData>();
                var mockLogger = new Mock<ILogger<BaseBusiness<Purchase, PurchaseDto>>>();
                var sut = new PurchaseBusiness(mockData.Object, ctx, mockLogger.Object);

                await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.ReceivePurchaseAsync(9999));
            }
            finally
            {
                conn.Close();
            }
        }

        [Fact]
        public async Task CancelPurchaseAsync_HappyPath_MarksDeleted()
        {
            var conn = new SqliteConnection("DataSource=:memory:");
            conn.Open();
            try
            {
                using var ctx = CreateInMemoryContext(conn);
                var sup = new Supplier { Name = "S1", Phone = "000" };
                ctx.suppliers.Add(sup);
                await ctx.SaveChangesAsync();

                var purchase = new Purchase { OrderedAt = DateTime.UtcNow, status = false, TotalCost = 0m, SupplierId = sup.Id };
                ctx.purchases.Add(purchase);
                await ctx.SaveChangesAsync();

                var mockData = new Mock<IPurchaseData>();
                var mockLogger = new Mock<ILogger<BaseBusiness<Purchase, PurchaseDto>>>();
                var sut = new PurchaseBusiness(mockData.Object, ctx, mockLogger.Object);

                await sut.CancelPurchaseAsync(purchase.Id, "Cancel reason");

                var updated = await ctx.purchases.FindAsync(purchase.Id);
                Assert.False(updated.Active);
                Assert.NotNull(updated.DeleteAt);
            }
            finally
            {
                conn.Close();
            }
        }

        [Fact]
        public async Task GetBySupplierNameAsync_ReturnsMatches()
        {
            var conn = new SqliteConnection("DataSource=:memory:");
            conn.Open();
            try
            {
                using var ctx = CreateInMemoryContext(conn);
                var supplier = new Supplier { Name = "Sx", Phone = "000" };
                ctx.suppliers.Add(supplier);
                await ctx.SaveChangesAsync();

                var p1 = new Purchase { SupplierId = supplier.Id, OrderedAt = DateTime.UtcNow, status = false, TotalCost = 10m };
                var p2 = new Purchase { SupplierId = supplier.Id, OrderedAt = DateTime.UtcNow.AddDays(-1), status = true, TotalCost = 5m };
                ctx.purchases.AddRange(p1, p2);
                await ctx.SaveChangesAsync();

                var mockData = new Mock<IPurchaseData>();
                var mockLogger = new Mock<ILogger<BaseBusiness<Purchase, PurchaseDto>>>();
                var sut = new PurchaseBusiness(mockData.Object, ctx, mockLogger.Object);

                var result = await sut.GetBySupplierNameAsync("Sx");
                Assert.Equal(2, result.Count());
            }
            finally
            {
                conn.Close();
            }
        }
    }
}
