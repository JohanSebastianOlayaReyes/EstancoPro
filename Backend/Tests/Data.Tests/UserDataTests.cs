using System.Threading.Tasks;
using Data.Implementations;
using Entity.Context;
using Entity.Dto;
using Xunit;

namespace Data.Tests
{
    public class UserDataTests
    {
        [Fact]
        public async Task CreateAndGetById_User_Succeeds()
        {
            var dbName = nameof(CreateAndGetById_User_Succeeds);
            using var context = TestUtilities.CreateInMemoryContext(dbName);
            var mapper = TestUtilities.CreateMapper();

            var sut = new UserData(context, mapper);

            var dto = new Entity.Model.UserDto { FullName = "testuser", Password = "pwd", PersonId = 0, Email = "t@test.local" };
            var created = await sut.CreateAsync(dto);

            var fetched = await sut.GetByIdAsync(created.Id);

            Assert.Equal("t@test.local", fetched.Email);
            Assert.True(created.Id != 0);
        }
    }
}
