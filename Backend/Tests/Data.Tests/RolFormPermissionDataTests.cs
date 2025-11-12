using System.Linq;
using System.Threading.Tasks;
using Data.Implementations;
using Entity.Model;
using Xunit;

namespace Data.Tests
{
    public class RolFormPermissionDataTests
    {
        [Fact]
        public async Task CreateAndGetAll_RolFormPermission_Succeeds()
        {
            var dbName = nameof(CreateAndGetAll_RolFormPermission_Succeeds);
            using var context = TestUtilities.CreateInMemoryContext(dbName);
            var mapper = TestUtilities.CreateMapper();

            var sut = new RolFormPermissionData(context, mapper);

            var dto = new RolFormPermissionDto { RolId = 1, FormId = 1, PermissionId = 1 };

            var created = await sut.CreateAsync(dto);

            var all = (await sut.GetAllAsync()).ToList();

            Assert.NotEmpty(all);
        }

        [Fact]
        public async Task GetById_WhenNotFound_Throws()
        {
            var dbName = nameof(GetById_WhenNotFound_Throws);
            using var context = TestUtilities.CreateInMemoryContext(dbName);
            var mapper = TestUtilities.CreateMapper();

            var sut = new RolFormPermissionData(context, mapper);

            await Assert.ThrowsAsync<System.Collections.Generic.KeyNotFoundException>(async () => await sut.GetByIdAsync(999));
        }
    }
}
