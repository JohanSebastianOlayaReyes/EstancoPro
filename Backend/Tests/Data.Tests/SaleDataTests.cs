using System.Linq;
using System.Threading.Tasks;
using Data.Implementations;
using Entity.Context;
using Entity.Dto;
using Xunit;

namespace Data.Tests
{
    public class SaleDataTests
    {
        [Fact]
        public async Task CreateAndGetAll_Sale_Succeeds()
        {
            var dbName = nameof(CreateAndGetAll_Sale_Succeeds);
            using var context = TestUtilities.CreateInMemoryContext(dbName);
            var mapper = TestUtilities.CreateMapper();

            var sut = new SaleData(context, mapper);

            var dto = new SaleDto { CashSessionId = 1, Status = "OPEN", SoldAt = System.DateTime.UtcNow, Subtotal = 0m, TaxTotal = 0m, GrandTotal = 0m };
            var created = await sut.CreateAsync(dto);

            var all = (await sut.GetAllAsync()).ToList();

            Assert.Single(all);
            Assert.Equal(1, all[0].CashSessionId);
        }
    }
}
