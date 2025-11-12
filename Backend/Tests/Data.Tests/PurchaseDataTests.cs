using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Implementations;
using Entity.Dto;
using Xunit;

namespace Data.Tests
{
    public class PurchaseDataTests
    {
        [Fact]
        public async Task CreateAndGetById_Purchase_Succeeds()
        {
            var dbName = nameof(CreateAndGetById_Purchase_Succeeds);
            var context = TestUtilities.CreateInMemoryContext(dbName);
            var mapper = TestUtilities.CreateMapper();

            // Seed Supplier first (required FK)
            var supplier = new Entity.Model.Supplier { Name = "Test Supplier", Phone = "1234567890" };
            context.suppliers.Add(supplier);
            await context.SaveChangesAsync();

            var sut = new PurchaseData(context, mapper);

            var dto = new PurchaseDto 
            { 
                SupplierId = supplier.Id, 
                OrderedAt = DateTime.UtcNow,
                TotalCost = 50000m,
                Status = true
            };
            var created = await sut.CreateAsync(dto);

            Assert.NotNull(created);
            Assert.Equal(50000m, created.TotalCost);

            var retrieved = await sut.GetByIdAsync(created.Id);

            Assert.NotNull(retrieved);
            Assert.Equal(created.Id, retrieved.Id);
            Assert.Equal(50000m, retrieved.TotalCost);
        }

        [Fact]
        public async Task GetAll_Purchase_ReturnsEmptyList()
        {
            var dbName = nameof(GetAll_Purchase_ReturnsEmptyList);
            var context = TestUtilities.CreateInMemoryContext(dbName);
            var mapper = TestUtilities.CreateMapper();

            var sut = new PurchaseData(context, mapper);

            var result = await sut.GetAllAsync();

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task Update_Purchase_Succeeds()
        {
            var dbName = nameof(Update_Purchase_Succeeds);
            var context = TestUtilities.CreateInMemoryContext(dbName);
            var mapper = TestUtilities.CreateMapper();

            // Seed Supplier
            var supplier = new Entity.Model.Supplier { Name = "Test Supplier", Phone = "1234567890" };
            context.suppliers.Add(supplier);
            await context.SaveChangesAsync();

            var sut = new PurchaseData(context, mapper);

            var dto = new PurchaseDto 
            { 
                SupplierId = supplier.Id, 
                OrderedAt = DateTime.UtcNow,
                TotalCost = 50000m,
                Status = true
            };
            var created = await sut.CreateAsync(dto);

            var updated = new PurchaseDto 
            { 
                Id = created.Id, 
                SupplierId = supplier.Id,
                TotalCost = 75000m,
                Status = false,
                ReceivedAt = DateTime.UtcNow
            };
            await sut.UpdateAsync(created.Id, updated);

            var retrieved = await sut.GetByIdAsync(created.Id);

            Assert.Equal(75000m, retrieved.TotalCost);
        }

        [Fact]
        public async Task Delete_Purchase_Succeeds()
        {
            var dbName = nameof(Delete_Purchase_Succeeds);
            var context = TestUtilities.CreateInMemoryContext(dbName);
            var mapper = TestUtilities.CreateMapper();

            // Seed Supplier
            var supplier = new Entity.Model.Supplier { Name = "Test Supplier", Phone = "1234567890" };
            context.suppliers.Add(supplier);
            await context.SaveChangesAsync();

            var sut = new PurchaseData(context, mapper);

            var dto = new PurchaseDto 
            { 
                SupplierId = supplier.Id, 
                OrderedAt = DateTime.UtcNow,
                TotalCost = 50000m,
                Status = true
            };
            var created = await sut.CreateAsync(dto);

            await sut.DeleteLogicAsync(created.Id);

            // After soft delete, GetByIdAsync should throw KeyNotFoundException
            await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.GetByIdAsync(created.Id));
        }
    }
}
