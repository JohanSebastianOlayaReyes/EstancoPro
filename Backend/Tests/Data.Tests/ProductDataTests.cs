using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Data.Implementations;
using Entity.Context;
using Entity.Dto;
using Entity.Model;
using Xunit;

namespace Data.Tests
{
    public class ProductDataTests
    {
        [Fact]
        public async Task CreateAndGetAll_Product_Succeeds()
        {
            var dbName = nameof(CreateAndGetAll_Product_Succeeds);
            using var context = TestUtilities.CreateInMemoryContext(dbName);
            var mapper = TestUtilities.CreateMapper();

            var sut = new ProductData(context, mapper);

            var dto = new ProductDto { Name = "Test", UnitCost = 1m, UnitPrice = 2m, CategoryId = 1, UnitMeasureId = 1 };
            var created = await sut.CreateAsync(dto);

            var all = (await sut.GetAllAsync()).ToList();

            Assert.Single(all);
            Assert.Equal("Test", all[0].Name);
            Assert.True(created.Id != 0);
        }
    }
}
