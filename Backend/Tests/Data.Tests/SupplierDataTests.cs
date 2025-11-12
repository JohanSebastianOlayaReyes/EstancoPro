using System.Linq;
using System.Threading.Tasks;
using Data.Implementations;
using Entity.Dto;
using Xunit;

namespace Data.Tests
{
    public class SupplierDataTests
    {
        [Fact]
        public async Task CreateAndGetAll_Supplier_Succeeds()
        {
            var dbName = nameof(CreateAndGetAll_Supplier_Succeeds);
            using var context = TestUtilities.CreateInMemoryContext(dbName);
            var mapper = TestUtilities.CreateMapper();

            var sut = new SupplierData(context, mapper);

            var dto = new SupplierDto { Name = "Proveedor A", Phone = "12345" };

            var created = await sut.CreateAsync(dto);

            var all = (await sut.GetAllAsync()).ToList();

            Assert.Contains(all, x => x.Name == "Proveedor A" && x.Phone == "12345");
        }

        [Fact]
        public async Task GetById_WhenNotFound_Throws()
        {
            var dbName = nameof(GetById_WhenNotFound_Throws);
            using var context = TestUtilities.CreateInMemoryContext(dbName);
            var mapper = TestUtilities.CreateMapper();

            var sut = new SupplierData(context, mapper);

            await Assert.ThrowsAsync<System.Collections.Generic.KeyNotFoundException>(async () => await sut.GetByIdAsync(999));
        }
    }
}
