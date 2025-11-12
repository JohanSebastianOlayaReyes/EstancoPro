using System.Linq;
using System.Threading.Tasks;
using Data.Implementations;
using Entity.Dto;
using Entity.Model;
using Xunit;

namespace Data.Tests
{
    public class PersonDataTests
    {
        [Fact]
        public async Task CreateAndGetAll_Person_Succeeds()
        {
            var dbName = nameof(CreateAndGetAll_Person_Succeeds);
            using var context = TestUtilities.CreateInMemoryContext(dbName);
            var mapper = TestUtilities.CreateMapper();

            var sut = new PersonData(context, mapper);

            var dto = new PersonDto { FullName = "Juan Perez", FirstName = "Juan", FirstLastName = "Perez", PhoneNumber = 123456, NumberIdentification = 98765 };

            var created = await sut.CreateAsync(dto);

            var all = (await sut.GetAllAsync()).ToList();

            Assert.Contains(all, x => x.FullName == "Juan Perez");
        }

        [Fact]
        public async Task GetById_WhenNotFound_Throws()
        {
            var dbName = nameof(GetById_WhenNotFound_Throws);
            using var context = TestUtilities.CreateInMemoryContext(dbName);
            var mapper = TestUtilities.CreateMapper();

            var sut = new PersonData(context, mapper);

            await Assert.ThrowsAsync<System.Collections.Generic.KeyNotFoundException>(async () => await sut.GetByIdAsync(999));
        }
    }
}
