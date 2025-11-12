using System.Linq;
using System.Threading.Tasks;
using Data.Implementations;
using Entity.Context;
using Entity.Dto;
using Xunit;

namespace Data.Tests
{
    public class PurchaseProductDetailDataTests
    {
        [Fact]
        public async Task CreateAndGetAll_PurchaseProductDetail_Succeeds()
        {
            var dbName = nameof(CreateAndGetAll_PurchaseProductDetail_Succeeds);
            using var context = TestUtilities.CreateInMemoryContext(dbName);
            var mapper = TestUtilities.CreateMapper();

            var sut = new PurchaseProductDetailData(context, mapper);

            var dto = new PurchaseProductDetailDto { Quantity = 2, UnitCost = 5m, LineTotal = 10m, PurchaseId = 1, ProductId = 1, UnitMeasureId = 1 };
            var created = await sut.CreateAsync(dto);

            var all = (await sut.GetAllAsync()).ToList();

            Assert.Single(all);
            Assert.Equal(2, all[0].Quantity);
        }
    }
}
