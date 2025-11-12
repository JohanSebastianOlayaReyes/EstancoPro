using System.Collections.Generic;
using System.Threading.Tasks;
using Business.Implementations;
using Data.Interfaces;
using Entity.Dto;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Business.Tests
{
    public class PersonBusinessTests
    {
        [Fact]
        public async Task GetAll_DelegatesToData()
        {
            var mock = new Mock<IPersonData>();
            mock.Setup(m => m.GetAllAsync()).ReturnsAsync(new List<PersonDto>());

            var loggerMock = new Mock<ILogger<BaseBusiness<Person, PersonDto>>>();
            var sut = new PersonBusiness(mock.Object, loggerMock.Object);

            var res = await sut.GetAllAsync();

            mock.Verify(m => m.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task Create_DelegatesToData()
        {
            var mock = new Mock<IPersonData>();
            var dto = new PersonDto { FullName = "Ana Lopez", FirstName = "Ana", FirstLastName = "Lopez", PhoneNumber = 111222, NumberIdentification = 33344 };
            mock.Setup(m => m.CreateAsync(dto)).ReturnsAsync(dto);

            var loggerMock = new Mock<ILogger<BaseBusiness<Person, PersonDto>>>();
            var sut = new PersonBusiness(mock.Object, loggerMock.Object);

            var res = await sut.CreateAsync(dto);

            mock.Verify(m => m.CreateAsync(dto), Times.Once);
        }
    }
}
