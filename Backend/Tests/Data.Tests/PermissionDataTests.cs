using System.Linq;
using System.Threading.Tasks;
using Data.Implementations;
using Entity.Dto;
using Xunit;

namespace Data.Tests
{
    public class PermissionDataTests
    {
        [Fact]
        public async Task CreateAndGetAll_Permission_Succeeds()
        {
            var dbName = nameof(CreateAndGetAll_Permission_Succeeds);
            using var context = TestUtilities.CreateInMemoryContext(dbName);
            var mapper = TestUtilities.CreateMapper();

            var sut = new PemissionData(context, mapper);

            var dto = new PermissionDto { TypePermission = "T1", Description = "D1" };

            var created = await sut.CreateAsync(dto);

            var all = (await sut.GetAllAsync()).ToList();

            Assert.Contains(all, x => x.TypePermission == "T1" && x.Description == "D1");
        }

        [Fact]
        public async Task GetById_WhenNotFound_Throws()
        {
            var dbName = nameof(GetById_WhenNotFound_Throws);
            using var context = TestUtilities.CreateInMemoryContext(dbName);
            var mapper = TestUtilities.CreateMapper();

            var sut = new PemissionData(context, mapper);

            await Assert.ThrowsAsync<System.Collections.Generic.KeyNotFoundException>(async () => await sut.GetByIdAsync(999));
        }
    }
}
