using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Business.Implementations;
using Data.Interfaces;
using Entity.Context;
using Entity.Dto;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Business.Tests
{
    public class ProductBusinessTests
    {
        private static ApplicationDbContext CreateInMemoryContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;

            return new ApplicationDbContext(options);
        }

        private static IMapper CreateMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Product, ProductDto>().ReverseMap();
                cfg.CreateMap<ProductUnitPrice, ProductUnitPriceDto>().ReverseMap();
                cfg.CreateMap<UnitMeasure, UnitMeasureDto>().ReverseMap();
            });
            return config.CreateMapper();
        }

        [Fact]
        public async Task GetLowStockProducts_ReturnsProducts()
        {
            var dbName = nameof(GetLowStockProducts_ReturnsProducts);
            using var context = CreateInMemoryContext(dbName);
            var mapper = CreateMapper();

            // seed unit measure
            var um = new UnitMeasure { Name = "u" };
            context.unitMeasures.Add(um);
            context.products.Add(new Product { Name = "p1", StockOnHand = 1, ReorderPoint = 5, UnitMeasureId = 1, unitmeasure = um });
            context.products.Add(new Product { Name = "p2", StockOnHand = 10, ReorderPoint = 5, UnitMeasureId = 1, unitmeasure = um });
            context.SaveChanges();

            var mockData = new Mock<IProductData>();
            var logger = Mock.Of<ILogger<BaseBusiness<Product, ProductDto>>>();
            var sut = new ProductBusiness(mockData.Object, context, logger);

            var result = (await sut.GetLowStockProductsAsync()).ToList();

            Assert.Single(result);
            Assert.Equal("p1", result[0].Name);
        }

        [Fact]
        public async Task AdjustStock_DecreasesStock_WhenValid()
        {
            var dbName = nameof(AdjustStock_DecreasesStock_WhenValid);
            using var context = CreateInMemoryContext(dbName);

            var um = new UnitMeasure { Name = "u" };
            context.unitMeasures.Add(um);
            var product = new Product { Name = "p1", StockOnHand = 10, ReorderPoint = 5, UnitMeasureId = 1, unitmeasure = um };
            context.products.Add(product);
            context.SaveChanges();

            var mockData = new Mock<IProductData>();
            var logger = Mock.Of<ILogger<BaseBusiness<Product, ProductDto>>>();
            var sut = new ProductBusiness(mockData.Object, context, logger);

            await sut.AdjustStockAsync("p1", -5, "test");

            var updated = await context.products.FirstAsync(p => p.Name == "p1");
            Assert.Equal(5, updated.StockOnHand);
        }

        [Fact]
        public async Task AdjustStock_Throws_WhenResultNegative()
        {
            var dbName = nameof(AdjustStock_Throws_WhenResultNegative);
            using var context = CreateInMemoryContext(dbName);

            var um = new UnitMeasure { Name = "u" };
            context.unitMeasures.Add(um);
            var product = new Product { Name = "p1", StockOnHand = 2, ReorderPoint = 5, UnitMeasureId = 1, unitmeasure = um };
            context.products.Add(product);
            context.SaveChanges();

            var mockData = new Mock<IProductData>();
            var logger = Mock.Of<ILogger<BaseBusiness<Product, ProductDto>>>();
            var sut = new ProductBusiness(mockData.Object, context, logger);

            await Assert.ThrowsAsync<InvalidOperationException>(() => sut.AdjustStockAsync("p1", -5, "test"));
        }

        [Fact]
        public async Task GetStockByPresentations_ReturnsConvertedValues()
        {
            var dbName = nameof(GetStockByPresentations_ReturnsConvertedValues);
            using var context = CreateInMemoryContext(dbName);

            var unit = new UnitMeasure { Id = 1, Name = "Unidad" };
            context.unitMeasures.Add(unit);
            var product = new Product { Id = 1, Name = "p1", StockOnHand = 100, UnitMeasureId = 1, unitmeasure = unit };
            context.products.Add(product);
            context.productUnitPrices.Add(new ProductUnitPrice { ProductId = 1, UnitMeasureId = 1, ConversionFactor = 1, unitmeasure = unit });
            context.productUnitPrices.Add(new ProductUnitPrice { ProductId = 1, UnitMeasureId = 2, ConversionFactor = 10, unitmeasure = new UnitMeasure { Id = 2, Name = "Caja" } });
            context.SaveChanges();

            var mockData = new Mock<IProductData>();
            var logger = Mock.Of<ILogger<BaseBusiness<Product, ProductDto>>>();
            var sut = new ProductBusiness(mockData.Object, context, logger);

            var result = await sut.GetStockByPresentationsAsync("p1");

            Assert.True(result.ContainsKey("Unidad") || result.ContainsKey("Caja"));
        }
    }
}
