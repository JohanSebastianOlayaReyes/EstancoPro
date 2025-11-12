using System.Linq;
using System.Threading.Tasks;
using Data.Implementations;
using Entity.Dto;
using Entity.Model;
using Xunit;

namespace Data.Tests
{
    public class ModuleDataTests
    {
        [Fact]
        public async Task CreateAndGetAll_Module_Succeeds()
        {
            var dbName = nameof(CreateAndGetAll_Module_Succeeds);
            using var context = TestUtilities.CreateInMemoryContext(dbName);
            var mapper = TestUtilities.CreateMapper();

            var sut = new ModuleData(context, mapper);

            var dto = new ModuleDto { Name = "M1", Description = "desc" };

            var created = await sut.CreateAsync(dto);

            var all = (await sut.GetAllAsync()).ToList();

            Assert.Contains(all, x => x.Name == "M1" && x.Description == "desc");
        }

        [Fact]
        public async Task GetById_WhenNotFound_Throws()
        {
            var dbName = nameof(GetById_WhenNotFound_Throws);
            using var context = TestUtilities.CreateInMemoryContext(dbName);
            var mapper = TestUtilities.CreateMapper();

            var sut = new ModuleData(context, mapper);

            await Assert.ThrowsAsync<System.Collections.Generic.KeyNotFoundException>(async () => await sut.GetByIdAsync(999));
        }
    }
}
