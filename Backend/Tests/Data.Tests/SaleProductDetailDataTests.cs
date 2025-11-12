using System.Linq;
using System.Threading.Tasks;
using Data.Implementations;
using Entity.Dto;
using Xunit;

namespace Data.Tests
{
    public class SaleProductDetailDataTests
    {
        [Fact]
        public async Task CreateAndGetAll_SaleProductDetail_Succeeds()
        {
            var dbName = nameof(CreateAndGetAll_SaleProductDetail_Succeeds);
            using var context = TestUtilities.CreateInMemoryContext(dbName);
            var mapper = TestUtilities.CreateMapper();


            // Seed required related entities: UnitMeasure, Category, Product, CashSession, Sale
            context.unitMeasures.Add(new Entity.Model.UnitMeasure { Id = 1, Name = "u" });
            context.categories.Add(new Entity.Model.Category { Id = 1, Name = "c", Description = "d" });
            context.products.Add(new Entity.Model.Product { Id = 1, Name = "p", UnitCost = 1m, UnitPrice = 2m, CategoryId = 1, UnitMeasureId = 1, StockOnHand = 10, ReorderPoint = 1 });
            context.cashSessions.Add(new Entity.Model.CashSession { Id = 1, OpenedAt = System.DateTime.UtcNow, OpeningAmount = 0m, ClosingAmount = 0m });
            context.sales.Add(new Entity.Model.Sale { Id = 1, SoldAt = System.DateTime.UtcNow, Status = "NEW", Subtotal = 0m, TaxTotal = 0m, GrandTotal = 0m, CashSessionId = 1 });
            await context.SaveChangesAsync();

            var sut = new SaleProductDetailData(context, mapper);

            var dto = new SaleProductDetailDto { SaleId = 1, ProductId = 1, UnitMeasureId = 1, Quantity = 2, UnitPrice = 100 };

            var created = await sut.CreateAsync(dto);

            var all = (await sut.GetAllAsync()).ToList();

            Assert.NotEmpty(all);
        }

        [Fact]
        public async Task CreateAndGetById_SaleProductDetail_Succeeds()
        {
            var dbName = nameof(CreateAndGetById_SaleProductDetail_Succeeds);
            using var context = TestUtilities.CreateInMemoryContext(dbName);
            var mapper = TestUtilities.CreateMapper();

            var sut = new SaleProductDetailData(context, mapper);

            // Seed related entities for this test
            context.unitMeasures.Add(new Entity.Model.UnitMeasure { Id = 1, Name = "u" });
            context.categories.Add(new Entity.Model.Category { Id = 1, Name = "c", Description = "d" });
            context.products.Add(new Entity.Model.Product { Id = 1, Name = "p", UnitCost = 1m, UnitPrice = 2m, CategoryId = 1, UnitMeasureId = 1, StockOnHand = 10, ReorderPoint = 1 });
            context.cashSessions.Add(new Entity.Model.CashSession { Id = 1, OpenedAt = System.DateTime.UtcNow, OpeningAmount = 0m, ClosingAmount = 0m });
            context.sales.Add(new Entity.Model.Sale { Id = 1, SoldAt = System.DateTime.UtcNow, Status = "NEW", Subtotal = 0m, TaxTotal = 0m, GrandTotal = 0m, CashSessionId = 1 });
            await context.SaveChangesAsync();

            var dto = new SaleProductDetailDto { SaleId = 1, ProductId = 1, UnitMeasureId = 1, Quantity = 3, LineTotal = 30m };
            var created = await sut.CreateAsync(dto);

            var fetched = await sut.GetByIdAsync(created.SaleId, created.ProductId, created.UnitMeasureId);

            Assert.Equal(created.Quantity, fetched.Quantity);
            Assert.Equal(created.SaleId, fetched.SaleId);
        }

        [Fact]
        public async Task GetById_WhenNotFound_Throws()
        {
            var dbName = nameof(GetById_WhenNotFound_Throws);
            using var context = TestUtilities.CreateInMemoryContext(dbName);
            var mapper = TestUtilities.CreateMapper();

            var sut = new SaleProductDetailData(context, mapper);

            await Assert.ThrowsAsync<System.Collections.Generic.KeyNotFoundException>(async () => await sut.GetByIdAsync(999, 999, 999));
        }
    }
}
