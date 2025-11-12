using System;
using System.Threading.Tasks;
using Data.Implementations;
using Entity.Context;
using Entity.Dto;
using Xunit;

namespace Data.Tests
{
    public class CashMovementDataTests
    {
        [Fact]
        public async Task CreateAndGetById_CashMovement_Succeeds()
        {
            var dbName = nameof(CreateAndGetById_CashMovement_Succeeds);
            using var context = TestUtilities.CreateInMemoryContext(dbName);
            var mapper = TestUtilities.CreateMapper();

            var sut = new CashMovementData(context, mapper);

            var at = DateTime.UtcNow;
            var dto = new CashMovementDto { CashSessionId = 1, At = at, Amount = 100m, Type = "IN" };
            var created = await sut.CreateAsync(dto);

            var fetched = await sut.GetByIdAsync(1, at);

            Assert.Equal(created.Amount, fetched.Amount);
            Assert.Equal(1, fetched.CashSessionId);
        }
    }
}
