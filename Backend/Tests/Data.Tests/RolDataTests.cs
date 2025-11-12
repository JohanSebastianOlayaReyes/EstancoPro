using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Data.Implementations;
using Entity.Context;
using Entity.Model;
using Xunit;

namespace Data.Tests
{
    public class RolDataTests
    {
        [Fact]
        public async Task CreateAsync_PersistsRolAndReturnsDto()
        {
            var ctx = TestUtilities.CreateInMemoryContext("rol_create");
            var mapper = TestUtilities.CreateMapper();

            var sut = new RolData(ctx, mapper);

            var dto = new Entity.Dto.RolDto { TypeRol = "Admin", Description = "administrador" };
            var created = await sut.CreateAsync(dto);

            Assert.NotNull(created);
            Assert.True(created.Id > 0);

            var db = ctx.rols.FirstOrDefault(r => r.Id == created.Id);
            Assert.NotNull(db);
            Assert.Equal("Admin", db.TypeRol);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsDto_WhenExists()
        {
            var ctx = TestUtilities.CreateInMemoryContext("rol_getbyid");
            var mapper = TestUtilities.CreateMapper();
            ctx.rols.Add(new Rol { TypeRol = "x", Description = "d" });
            await ctx.SaveChangesAsync();

            var sut = new RolData(ctx, mapper);
            var first = ctx.rols.First();

            var dto = await sut.GetByIdAsync(first.Id);

            Assert.NotNull(dto);
            Assert.Equal(first.TypeRol, dto.TypeRol);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAll()
        {
            var ctx = TestUtilities.CreateInMemoryContext("rol_getall");
            var mapper = TestUtilities.CreateMapper();
            ctx.rols.AddRange(new Rol { TypeRol = "a", Description = "d1" }, new Rol { TypeRol = "b", Description = "d2" });
            await ctx.SaveChangesAsync();

            var sut = new RolData(ctx, mapper);
            var list = await sut.GetAllAsync();

            Assert.Equal(2, list.Count());
        }
    }
}
