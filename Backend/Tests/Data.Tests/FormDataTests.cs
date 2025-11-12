using System.Linq;
using System.Threading.Tasks;
using Data.Implementations;
using Entity.Context;
using Entity.Model;
using Xunit;

namespace Data.Tests
{
    public class FormDataTests
    {
        [Fact]
        public async Task CreateAsync_PersistsFormAndReturnsDto()
        {
            var ctx = TestUtilities.CreateInMemoryContext("form_create");
            var mapper = TestUtilities.CreateMapper();

            var sut = new FormData(ctx, mapper);

            var dto = new Entity.Dto.FormDto { Name = "F1", Description = "d", Path = "/p" };
            var created = await sut.CreateAsync(dto);

            Assert.NotNull(created);
            Assert.True(created.Id > 0);

            var db = ctx.forms.FirstOrDefault(f => f.Id == created.Id);
            Assert.NotNull(db);
            Assert.Equal("F1", db.Name);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsDto_WhenExists()
        {
            var ctx = TestUtilities.CreateInMemoryContext("form_getbyid");
            var mapper = TestUtilities.CreateMapper();
            ctx.forms.Add(new Form { Name = "x", Description = "d", Path = "/p" });
            await ctx.SaveChangesAsync();

            var sut = new FormData(ctx, mapper);
            var first = ctx.forms.First();

            var dto = await sut.GetByIdAsync(first.Id);

            Assert.NotNull(dto);
            Assert.Equal(first.Name, dto.Name);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAll()
        {
            var ctx = TestUtilities.CreateInMemoryContext("form_getall");
            var mapper = TestUtilities.CreateMapper();
            ctx.forms.AddRange(new Form { Name = "a", Description = "d", Path = "/a" }, new Form { Name = "b", Description = "d2", Path = "/b" });
            await ctx.SaveChangesAsync();

            var sut = new FormData(ctx, mapper);
            var list = await sut.GetAllAsync();

            Assert.Equal(2, list.Count());
        }
    }
}
