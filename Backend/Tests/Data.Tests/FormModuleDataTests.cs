using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Data.Implementations;
using Entity.Dto;
using Entity.Model;
using Xunit;

namespace Data.Tests
{
    public class FormModuleDataTests
    {
        [Fact]
        public async Task CreateAndGetAll_FormModule_Succeeds()
        {
            var dbName = nameof(CreateAndGetAll_FormModule_Succeeds);
            using var context = TestUtilities.CreateInMemoryContext(dbName);
            var mapper = TestUtilities.CreateMapper();

            // seed required related entities (use DbSet property names as defined in ApplicationDbContext)
            var form = new Form { Id = 1, Name = "F1", Description = "D1", Path = "/f1" };
            var module = new Module { Id = 1, Name = "M1", Description = "MD1" };
            context.forms.Add(form);
            context.modules.Add(module);
            await context.SaveChangesAsync();

            var sut = new FormModuleData(context, mapper);

            var dto = new FormModuleDto { FormId = form.Id, ModuleId = module.Id };

            var created = await sut.CreateAsync(dto);

            var all = (await sut.GetAllAsync()).ToList();

            Assert.Contains(all, x => x.FormId == form.Id && x.ModuleId == module.Id);
        }
    }
}
