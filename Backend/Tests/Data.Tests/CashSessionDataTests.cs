using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Implementations;
using Entity.Dto;
using Xunit;

namespace Data.Tests
{
    public class CashSessionDataTests
    {
        [Fact]
        public async Task CreateAndGetById_CashSession_Succeeds()
        {
            var dbName = nameof(CreateAndGetById_CashSession_Succeeds);
            var context = TestUtilities.CreateInMemoryContext(dbName);
            var mapper = TestUtilities.CreateMapper();

            var sut = new CashSessionData(context, mapper);

            var dto = new CashSessionDto { OpeningAmount = 100m, OpenedAt = DateTime.UtcNow };
            var created = await sut.CreateAsync(dto);

            Assert.NotNull(created);
            Assert.Equal(100m, created.OpeningAmount);

            var retrieved = await sut.GetByIdAsync(created.Id);

            Assert.NotNull(retrieved);
            Assert.Equal(created.Id, retrieved.Id);
            Assert.Equal(100m, retrieved.OpeningAmount);
        }

        [Fact]
        public async Task GetAll_CashSession_ReturnsEmptyList()
        {
            var dbName = nameof(GetAll_CashSession_ReturnsEmptyList);
            var context = TestUtilities.CreateInMemoryContext(dbName);
            var mapper = TestUtilities.CreateMapper();

            var sut = new CashSessionData(context, mapper);

            var result = await sut.GetAllAsync();

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task Update_CashSession_Succeeds()
        {
            var dbName = nameof(Update_CashSession_Succeeds);
            var context = TestUtilities.CreateInMemoryContext(dbName);
            var mapper = TestUtilities.CreateMapper();

            var sut = new CashSessionData(context, mapper);

            var dto = new CashSessionDto { OpeningAmount = 100m, OpenedAt = DateTime.UtcNow };
            var created = await sut.CreateAsync(dto);

            var updated = new CashSessionDto { Id = created.Id, OpeningAmount = 150m, OpenedAt = created.OpenedAt };
            await sut.UpdateAsync(created.Id, updated);

            var retrieved = await sut.GetByIdAsync(created.Id);

            Assert.Equal(150m, retrieved.OpeningAmount);
        }

        [Fact]
        public async Task Delete_CashSession_Succeeds()
        {
            var dbName = nameof(Delete_CashSession_Succeeds);
            var context = TestUtilities.CreateInMemoryContext(dbName);
            var mapper = TestUtilities.CreateMapper();

            var sut = new CashSessionData(context, mapper);

            var dto = new CashSessionDto { OpeningAmount = 100m, OpenedAt = DateTime.UtcNow };
            var created = await sut.CreateAsync(dto);

            await sut.DeleteLogicAsync(created.Id);

            // After soft delete, GetByIdAsync should throw KeyNotFoundException
            await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.GetByIdAsync(created.Id));
        }
    }
}
