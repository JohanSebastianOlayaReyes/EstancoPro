using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Implementations;
using Entity.Dto;
using Entity.Model;
using Xunit;

namespace Data.Tests
{
    public class RolUserDataTests
    {
        [Fact]
        public async Task CreateAsync_UserRol_Succeeds()
        {
            var dbName = nameof(CreateAsync_UserRol_Succeeds);
            var context = TestUtilities.CreateInMemoryContext(dbName);
            var mapper = TestUtilities.CreateMapper();

            // Seed User and Rol (required FKs)
            var user = new User { Email = "test@test.com", Password = "pwd123" };
            var rol = new Rol { TypeRol = "Admin", Description = "Admin role" };
            context.users.Add(user);
            context.rols.Add(rol);
            await context.SaveChangesAsync();

            var sut = new RolUserData(context, mapper);

            var dto = new UserRolDto 
            { 
                UserId = user.Id, 
                RolId = rol.Id
            };
            var created = await sut.CreateAsync(dto);

            Assert.NotNull(created);
            Assert.Equal(user.Id, created.UserId);
            Assert.Equal(rol.Id, created.RolId);
        }

        [Fact]
        public async Task GetAll_UserRol_ReturnsEmptyList()
        {
            var dbName = nameof(GetAll_UserRol_ReturnsEmptyList);
            var context = TestUtilities.CreateInMemoryContext(dbName);
            var mapper = TestUtilities.CreateMapper();

            var sut = new RolUserData(context, mapper);

            var result = await sut.GetAllAsync();

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAll_UserRol_ReturnsCreatedRecords()
        {
            var dbName = nameof(GetAll_UserRol_ReturnsCreatedRecords);
            var context = TestUtilities.CreateInMemoryContext(dbName);
            var mapper = TestUtilities.CreateMapper();

            // Seed User and Rol
            var user1 = new User { Email = "user1@test.com", Password = "pwd123" };
            var user2 = new User { Email = "user2@test.com", Password = "pwd123" };
            var rol1 = new Rol { TypeRol = "Admin", Description = "Admin role" };
            var rol2 = new Rol { TypeRol = "User", Description = "User role" };
            context.users.AddRange(user1, user2);
            context.rols.AddRange(rol1, rol2);
            await context.SaveChangesAsync();

            var sut = new RolUserData(context, mapper);

            // Create two UserRol records
            var dto1 = new UserRolDto { UserId = user1.Id, RolId = rol1.Id };
            var dto2 = new UserRolDto { UserId = user2.Id, RolId = rol2.Id };
            await sut.CreateAsync(dto1);
            await sut.CreateAsync(dto2);

            var result = await sut.GetAllAsync();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task CreateAsync_DuplicateUserRol_ThrowsException()
        {
            var dbName = nameof(CreateAsync_DuplicateUserRol_ThrowsException);
            var context = TestUtilities.CreateInMemoryContext(dbName);
            var mapper = TestUtilities.CreateMapper();

            // Seed User and Rol
            var user = new User { Email = "test@test.com", Password = "pwd123" };
            var rol = new Rol { TypeRol = "Admin", Description = "Admin role" };
            context.users.Add(user);
            context.rols.Add(rol);
            await context.SaveChangesAsync();

            var sut = new RolUserData(context, mapper);

            // Create first UserRol
            var dto = new UserRolDto { UserId = user.Id, RolId = rol.Id };
            await sut.CreateAsync(dto);

            // Attempt to create duplicate (should throw due to composite key constraint)
            await Assert.ThrowsAsync<InvalidOperationException>(() => sut.CreateAsync(dto));
        }
    }
}
