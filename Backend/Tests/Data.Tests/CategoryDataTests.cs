using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Data.Implementations;
using Entity.Context;
using Entity.Model;
using Xunit;

namespace Data.Tests
{
    public class CategoryDataTests
    {
        [Fact]
        public async Task CreateAsync_PersistsCategoryAndReturnsDto()
        {
            var ctx = TestUtilities.CreateInMemoryContext("cat_create");
            var mapper = TestUtilities.CreateMapper();

            var sut = new CategoryData(ctx, mapper);

            var dto = new Entity.Dto.CategoryDto { Name = "C1", Description = "desc" };
            var created = await sut.CreateAsync(dto);

            Assert.NotNull(created);
            Assert.True(created.Id > 0);

            var db = ctx.categories.FirstOrDefault(c => c.Id == created.Id);
            Assert.NotNull(db);
            Assert.Equal("C1", db.Name);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsDto_WhenExists()
        {
            var ctx = TestUtilities.CreateInMemoryContext("cat_getbyid");
            var mapper = TestUtilities.CreateMapper();
            ctx.categories.Add(new Category { Name = "x", Description = "d" });
            await ctx.SaveChangesAsync();

            var sut = new CategoryData(ctx, mapper);
            var first = ctx.categories.First();

            var dto = await sut.GetByIdAsync(first.Id);

            Assert.NotNull(dto);
            Assert.Equal(first.Name, dto.Name);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAll()
        {
            var ctx = TestUtilities.CreateInMemoryContext("cat_getall");
            var mapper = TestUtilities.CreateMapper();
            ctx.categories.AddRange(new Category { Name = "a", Description = "d1" }, new Category { Name = "b", Description = "d2" });
            await ctx.SaveChangesAsync();

            var sut = new CategoryData(ctx, mapper);
            var list = await sut.GetAllAsync();

            Assert.Equal(2, list.Count());
        }
    }
}
