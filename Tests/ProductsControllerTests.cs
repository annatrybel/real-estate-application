using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using WebApp.Controllers;
using WebApp.Data;
using WebApp.Models;
using WebApp.Models.ViewModels;


namespace Tests
{
    public class ProductsControllerTests
    {
        [Fact]
        public async Task Upsert_Get_ReturnsViewWithProductVM()
        {
            var options = new DbContextOptionsBuilder<WebAppContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var context = new WebAppContext(options);

            context.Category.Add(new Category { Id = 1, Name = "TestCategory", DisplayOrder = 1 });
            context.ListingsType.Add(new WebApp.Models.ListingsType { Id = 1, Name = "TestListingsType" });
            context.SaveChanges();


            var mockWebHostEnvironment = new Mock<IWebHostEnvironment>();
            var mockLogger = new Mock<ILogger<ProductsController>>();

            var controller = new ProductsController(context, mockWebHostEnvironment.Object, mockLogger.Object);


            var result = await controller.Upsert(1) as ViewResult;
            Assert.NotNull(result);
            Assert.IsType<ProductVM>(result.Model);

            var model = result.Model as ProductVM;
            Assert.NotNull(model.Product);
            Assert.Equal(1, model.Product.Id);
        }

        public async Task Upsert_Post_ValidModel_RedirectsToIndex()
        {
            var options = new DbContextOptionsBuilder<WebAppContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var context = new WebAppContext(options);

           
            var mockWebHostEnvironment = new Mock<IWebHostEnvironment>();
            var mockLogger = new Mock<ILogger<ProductsController>>();

            var controller = new ProductsController(context, mockWebHostEnvironment.Object, mockLogger.Object);

            var productVM = new ProductVM
            {
                Product = new Product { Name = "TestProduct", Description = "TestDescription" }
            };

            var result = await controller.Upsert(productVM);

       
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);

      
            var product = await context.Product.FirstOrDefaultAsync(p => p.Name == "TestProduct");
            Assert.NotNull(product);
        }
    }
}
