using System.Threading.Tasks;
using AutoMapper;
using Data.Implementations;
using Entity.Context;
using Entity.Dto;
using Xunit;

namespace Data.Tests
{
    public class ProductUnitPriceDataTests
    {
        [Fact]
        public async Task CreateAndGetById_ProductUnitPrice_Succeeds()
        {
            var dbName = nameof(CreateAndGetById_ProductUnitPrice_Succeeds);
            using var context = TestUtilities.CreateInMemoryContext(dbName);
            var mapper = TestUtilities.CreateMapper();

            var sut = new ProductUnitPriceData(context, mapper);

            var dto = new ProductUnitPriceDto { ProductId = 1, UnitMeasureId = 1, UnitPrice = 9.99m, ConversionFactor = 1m };
            var created = await sut.CreateAsync(dto);

            var fetched = await sut.GetByIdAsync(1, 1);

            Assert.Equal(created.UnitPrice, fetched.UnitPrice);
            Assert.Equal(1, fetched.ProductId);
        }
    }
}
