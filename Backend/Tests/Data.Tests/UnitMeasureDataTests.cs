using System.Linq;
using System.Threading.Tasks;
using Data.Implementations;
using Entity.Context;
using Entity.Dto;
using Xunit;

namespace Data.Tests
{
    public class UnitMeasureDataTests
    {
        [Fact]
        public async Task CreateAndGetAll_UnitMeasure_Succeeds()
        {
            var dbName = nameof(CreateAndGetAll_UnitMeasure_Succeeds);
            using var context = TestUtilities.CreateInMemoryContext(dbName);
            var mapper = TestUtilities.CreateMapper();

            var sut = new UnitMeasureData(context, mapper);

            var dto = new UnitMeasureDto { Name = "u1" };
            var created = await sut.CreateAsync(dto);

            var all = (await sut.GetAllAsync()).ToList();

            Assert.Single(all);
            Assert.Equal("u1", all[0].Name);
        }
    }
}
